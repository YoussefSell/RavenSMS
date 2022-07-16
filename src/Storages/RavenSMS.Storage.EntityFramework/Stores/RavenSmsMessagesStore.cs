﻿namespace RavenSMS.Stores.EntityFramework;

/// <summary>
/// the store implementation for <see cref="IRavenSmsMessagesStore"/>
/// </summary>
public partial class RavenSmsMessagesStore : IRavenSmsMessagesStore
{
    /// <inheritdoc/>
    public async Task<(long totalSent, long totalFailed, long totalInQueue)> GetCountsAsync(CancellationToken cancellationToken = default)
    {
        var result = await _messages.GroupBy(e => e.Status)
            .Select(grouping => new
            {
                grouping.Key,
                Count = grouping.Count()
            })
            .ToDictionaryAsync(e => e.Key, e => e.Count, cancellationToken: cancellationToken);

        return (
            result.TryGetValue(RavenSmsMessageStatus.Sent, out var totalSent) ? totalSent : 0,
            result.TryGetValue(RavenSmsMessageStatus.Failed, out var totalFailed) ? totalFailed : 0,
            result.TryGetValue(RavenSmsMessageStatus.Queued, out var totalQueued) ? totalQueued : 0
        );
    }

    /// <inheritdoc/>
    public Task<RavenSmsMessage[]> GetAllAsync(CancellationToken cancellationToken = default)
        => _messages.ToArrayAsync(cancellationToken: cancellationToken);

    /// <inheritdoc/>
    public async Task<(RavenSmsMessage[] data, int rowsCount)> GetAllAsync(RavenSmsMessageFilter filter, CancellationToken cancellationToken = default)
    {
        var query = _messages.Include(e => e.Client).AsQueryable();

        // apply the filter & the orderBy
        query = SetFilter(query, filter);
        query = query.DynamicOrderBy(filter.OrderBy, filter.SortDirection);

        var rowsCount = 0;

        if (!filter.IgnorePagination)
        {
            rowsCount = await query.Select(e => e.Id)
                .Distinct()
                .CountAsync(cancellationToken: cancellationToken);

            query = query.Skip((filter.PageIndex - 1) * filter.PageSize)
                .Take(filter.PageSize);
        }

        var data = await query.ToArrayAsync(cancellationToken: cancellationToken);

        rowsCount = filter.IgnorePagination
            ? data.Length
            : rowsCount;

        return (data, rowsCount);
    }

    /// <inheritdoc/>
    public Task<bool> AnyAsync(string messageId, CancellationToken cancellationToken = default)
        => _messages.AsNoTracking().AnyAsync(message => message.Id == messageId, cancellationToken: cancellationToken);

    /// <inheritdoc/>
    public Task<RavenSmsMessage?> FindByIdAsync(string messageId, CancellationToken cancellationToken = default)
        => _messages.Include(e => e.Client)
            .Include(m => m.SendAttempts)
            .FirstOrDefaultAsync(message => message.Id == messageId, cancellationToken: cancellationToken);

    /// <inheritdoc/>
    public async Task<Result<RavenSmsMessage>> CreateAsync(RavenSmsMessage message, CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = _messages.Add(message);
            await _context.SaveChangesAsync(cancellationToken);
            return entity.Entity;
        }
        catch (Exception ex)
        {
            return Result.Failure<RavenSmsMessage>()
                .WithMessage("Failed to save the message, an exception has been accrued")
                .WithErrors(ex);
        }
    }

    /// <inheritdoc/>
    public async Task<Result<RavenSmsMessage>> UpdateAsync(RavenSmsMessage message, CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = _messages.Update(message);
            await _context.SaveChangesAsync(cancellationToken);
            return entity.Entity;
        }
        catch (Exception ex)
        {
            return Result.Failure<RavenSmsMessage>()
                .WithMessage("Failed to update the message, an exception has been accrued")
                .WithErrors(ex);
        }
    }

    /// <inheritdoc/>
    public async Task<Result> DeleteAsync(RavenSmsMessage message, CancellationToken cancellationToken = default)
    {
        try
        {
            _ = _messages.Remove(message);
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure()
                .WithMessage("Failed to update the message, an exception has been accrued")
                .WithErrors(ex);
        }
    }
}

/// <summary>
/// partial part for <see cref="RavenSmsClientsStore"/>
/// </summary>
public partial class RavenSmsMessagesStore
{
    const string _dateFormat = "yyyy-MM-ddTHH:mm:sszzz";

    private readonly IRavenSmsDbContext _context;
    private readonly DbSet<RavenSmsMessage> _messages;

    public RavenSmsMessagesStore(IRavenSmsDbContext context)
    {
        _context = context;
        _messages = _context.Set<RavenSmsMessage>();
    }

    private static IQueryable<RavenSmsMessage> SetFilter(IQueryable<RavenSmsMessage> query, RavenSmsMessageFilter filter)
    {
        if (!string.IsNullOrEmpty(filter.SearchQuery))
            query = query.Where(e =>
                EF.Functions.Like(e.Body, $"%{filter.SearchQuery}%") ||
                EF.Functions.Like(e.Client.PhoneNumber, $"%{filter.SearchQuery}%")
            );

        if (!string.IsNullOrEmpty(filter.StartDate) && DateTimeOffset.TryParseExact(filter.StartDate, _dateFormat, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var startDate))
            query = query.Where(e => e.CreateOn.Date >= startDate.Date);

        if (!string.IsNullOrEmpty(filter.EndDate) && DateTimeOffset.TryParseExact(filter.EndDate, _dateFormat, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var endDate))
            query = query.Where(e => e.CreateOn.Date <= endDate.Date);

        if (filter.Priority.HasValue)
            query = query.Where(e => e.Priority == filter.Priority);

        if (filter.Status != RavenSmsMessageStatus.None)
            query = query.Where(e => filter.Status == e.Status);

        if (filter.ExcludeStatus.Any())
            query = query.Where(e => !filter.ExcludeStatus.Contains(e.Status));

        if (filter.To is not null && filter.To.Any())
            query = query.Where(e => filter.To.Contains((string)e.To));

        if (filter.Clients is not null && filter.Clients.Any())
            query = query.Where(e => filter.Clients.Contains(e.ClientId));

        return query;
    }
}

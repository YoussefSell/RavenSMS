﻿namespace RavenSMS.Stores.InMemory;

/// <summary>
/// the default implementation for <see cref="IRavenSmsMessagesStore"/> with an in memory store
/// </summary>
public partial class RavenSmsMessagesInMemoryStore : IRavenSmsMessagesStore
{
    /// <inheritdoc/>
    public Task<(long totalSent, long totalFailed, long totalInQueue)> GetCountsAsync(CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
            cancellationToken.ThrowIfCancellationRequested();

        var result = _messages.GroupBy(e => e.Status)
            .Select(grouping => new
            {
                grouping.Key,
                Count = grouping.LongCount()
            })
            .ToDictionary(e => e.Key, e => e.Count);

        return Task.FromResult((
            result.TryGetValue(RavenSmsMessageStatus.Sent, out var totalSent) ? totalSent : 0,
            result.TryGetValue(RavenSmsMessageStatus.Failed, out var totalFailed) ? totalFailed : 0,
            result.TryGetValue(RavenSmsMessageStatus.Queued, out var totalQueued) ? totalQueued : 0
        ));
    }

    /// <inheritdoc/>
    public Task<RavenSmsMessage[]> GetAllAsync(CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
            cancellationToken.ThrowIfCancellationRequested();

        return Task.FromResult(_messages.ToArray());
    }

    /// <inheritdoc/>
    public Task<(RavenSmsMessage[] data, int rowsCount)> GetAllAsync(RavenSmsMessageFilter filter, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
            cancellationToken.ThrowIfCancellationRequested();

        return GetAllAsync(filter);
    }

    /// <inheritdoc/>
    public Task<bool> AnyAsync(string messageId, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
            cancellationToken.ThrowIfCancellationRequested();

        return Task.FromResult(_messages.Any(message => message.Id == messageId));
    }

    /// <inheritdoc/>
    public async Task<RavenSmsMessage?> FindByIdAsync(string messageId, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
            cancellationToken.ThrowIfCancellationRequested();

        var message = _messages.FirstOrDefault(message => message.Id == messageId);
        if (message is not null)
        {
            // we need to set the client info
            message.Client = await _clientsStore.FindByIdAsync(message.ClientId, cancellationToken);
        }

        return message;
    }

    /// <inheritdoc/>
    public async Task<Result<RavenSmsMessage>> CreateAsync(RavenSmsMessage message, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
            cancellationToken.ThrowIfCancellationRequested();

        message.Client = await _clientsStore.FindByIdAsync(message.ClientId, cancellationToken);
        _messages.Add(message);

        return Result.Success(message);
    }

    /// <inheritdoc/>
    public async Task<Result<RavenSmsMessage>> UpdateAsync(RavenSmsMessage message, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
            cancellationToken.ThrowIfCancellationRequested();

        var messageToUpdate = _messages.FirstOrDefault(c => c.Id == message.Id);
        if (messageToUpdate == null)
        {
            return Result.Failure<RavenSmsMessage>()
                .WithMessage("Failed to update the message, not found")
                .WithCode("message_not_found");
        }

        message.Client = await _clientsStore.FindByIdAsync(message.ClientId, cancellationToken);
        return Result.Success(messageToUpdate);
    }

    /// <inheritdoc/>
    public Task<Result> DeleteAsync(RavenSmsMessage message, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
            cancellationToken.ThrowIfCancellationRequested();

        var messageToUpdate = _messages.FirstOrDefault(c => c.Id == message.Id);
        if (messageToUpdate == null)
        {
            return Task.FromResult(Result.Failure()
                .WithMessage("Failed to delete the message, not found")
                .WithCode("message_not_found"));
        }

        _messages.Remove(messageToUpdate);

        return Task.FromResult(Result.Success());
    }
}

/// <summary>
/// partial part for <see cref="RavenSmsMessagesInMemoryStore"/>
/// </summary>
public partial class RavenSmsMessagesInMemoryStore
{
    const string _dateFormat = "yyyy-MM-ddTHH:mm:sszzz";
    private readonly IRavenSmsClientsStore _clientsStore;
    private readonly ICollection<RavenSmsMessage> _messages;

    public RavenSmsMessagesInMemoryStore(IRavenSmsClientsStore clientsStore)
    {
        _messages = new List<RavenSmsMessage>();
        _clientsStore = clientsStore;
    }

    private Task<(RavenSmsMessage[] data, int rowsCount)> GetAllAsync(RavenSmsMessageFilter filter)
    {
        // apply the filter & the orderBy
        var query = SetFilter(_messages, filter);

        var rowsCount = 0;

        if (!filter.IgnorePagination)
        {
            rowsCount = query.Select(e => e.Id)
                .Distinct()
                .Count();

            query = query.Skip((filter.PageIndex - 1) * filter.PageSize)
                .Take(filter.PageSize);
        }

        var data = query.ToArray();

        rowsCount = filter.IgnorePagination
            ? data.Length
            : rowsCount;

        return Task.FromResult((data, rowsCount));
    }

    private static IEnumerable<RavenSmsMessage> SetFilter(IEnumerable<RavenSmsMessage> query, RavenSmsMessageFilter filter)
    {
        if (!string.IsNullOrEmpty(filter.SearchQuery))
            query = query.Where(e => e.Body.Contains(filter.SearchQuery));

        if (!string.IsNullOrEmpty(filter.StartDate) && DateTimeOffset.TryParseExact(filter.StartDate, _dateFormat, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var startDate))
            query = query.Where(e => e.CreateOn.Date >= startDate.Date);

        if (!string.IsNullOrEmpty(filter.EndDate) && DateTimeOffset.TryParseExact(filter.EndDate, _dateFormat, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var endDate))
            query = query.Where(e => e.CreateOn.Date <= endDate.Date);

        if (filter.Priority.HasValue)
            query = query.Where(e => e.Priority == filter.Priority);

        if (filter.Status != RavenSmsMessageStatus.None)
            query = query.Where(e => filter.Status == e.Status);

        if(filter.ExcludeStatus.Any())
            query = query.Where(e => !filter.ExcludeStatus.Contains(e.Status));

        if (filter.To is not null && filter.To.Any())
            query = query.Where(e => filter.To.Contains((string)e.To));

        if (filter.Clients is not null && filter.Clients.Any())
            query = query.Where(e => filter.Clients.Contains(e.ClientId));

        return query;
    }

    protected async Task SetMessagesClientsAsync(RavenSmsMessage[] messages, CancellationToken cancellationToken = default)
    {
        // select the client to retrieve
        var clientsIds = messages.Select(message => message.ClientId).ToArray();

        // get the clients and convert the list of dictionary
        var clients = (await _clientsStore.FindByIdAsync(clientsIds, cancellationToken).ConfigureAwait(false))
            .ToDictionary(client => client.Id);

        foreach (var message in messages)
        {
            if (clients.TryGetValue(message.ClientId, out var client))
                message.Client = client;
        }
    }
}

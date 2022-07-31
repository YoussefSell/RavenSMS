namespace RavenSMS.Internal.Queues.InMemory;

/// <summary>
/// the in memory queue implementation of the <see cref="IQueueManager"/>
/// </summary>
public partial class InMemoryQueueManager : IQueueManager
{
    /// <inheritdoc/>
    public string QueueEvent<TEvent>(TEvent @event)
        where TEvent : IEvent
    {
        // create the job
        var job = new InMemoryJob(async () =>
        {
            // create the service scope
            using var scope = _scopeFactory.CreateScope();

            // get the event publisher to process the event
            var publisher = scope.ServiceProvider.GetService<EventsPublisher>();
            if (publisher is null)
                throw new RavenSmsException($"RavenSMS is not registered add AddRavenSMS() to your service collection");

            // process the message
            await publisher.ProcessAsync(@event);
        });

        // queue the job
        _jobs.Enqueue(job);

        return job.Id.ToString();
    }

    /// <inheritdoc/>
    public string QueueMessage(RavenSmsMessage message) 
        => QueueMessage(message, TimeSpan.Zero);

    /// <inheritdoc/>
    public string QueueMessage(RavenSmsMessage message, TimeSpan delay)
    {
        var messageId = message.Id;

        // create the job
        var job = new InMemoryJob(async () =>
        {
            // if we have a delay we need to wait
            if (delay != TimeSpan.Zero)
                await Task.Delay(delay);

            // create the service scope
            using var scope = _scopeFactory.CreateScope();

            // get the manager to process the message
            var manager = scope.ServiceProvider.GetService<IRavenSmsManager>();
            if (manager is null)
                throw new RavenSmsException($"RavenSMS is not registered add AddRavenSMS() to your service collection");

            // process the message
            await manager.ProcessAsync(messageId);
        });

        // queue the job
        _jobs.Enqueue(job);

        return job.Id.ToString();
    }
}

/// <summary>
/// partial part for <see cref="InMemoryQueueManager"/>
/// </summary>
public partial class InMemoryQueueManager
{
    private int _queueIsConsuming = 0;
    private int _tasksRunningCount = 0;

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ConcurrentQueue<InMemoryJob> _jobs = new();
    private readonly ConcurrentDictionary<Guid, CancellationTokenSource> _tokens = new();

    public bool IsRunning => _queueIsConsuming > 0;

    public InMemoryQueueManager(IServiceScopeFactory scopeFactory) 
        => _scopeFactory = scopeFactory;

    public async Task ConsumeQueueAsync()
    {
        try
        {
            Interlocked.Increment(ref _queueIsConsuming);

            var dequeuedTasks = DequeueAllTasks();
            var dequeuedGuids = dequeuedTasks.Select(job => job.Id);

            await Task.WhenAll(
                dequeuedTasks
                    .Select(InvokeTask)
                    .ToArray()
            );

            CleanTokens(dequeuedGuids);
        }
        finally
        {
            Interlocked.Decrement(ref _queueIsConsuming);
        }
    }

    public async Task ConsumeQueueOnShutdown()
    {
        foreach (var cancellatinTokens in _tokens.Values)
        {
            if (!cancellatinTokens.IsCancellationRequested)
            {
                cancellatinTokens.Cancel();
            }
        }

        await ConsumeQueueAsync();
    }

    public void CleanTokens(IEnumerable<Guid> guidsForTokensToClean)
    {
        foreach (var guid in guidsForTokensToClean)
        {
            if (_tokens.TryRemove(guid, out var token))
            {
                token.Dispose();
            }
        }
    }

    private List<InMemoryJob> DequeueAllTasks()
    {
        List<InMemoryJob> dequeuedTasks = new(_jobs.Count);

        while (_jobs.TryPeek(out var _))
        {
            _jobs.TryDequeue(out var dequeuedTask);

            if (dequeuedTask is not null)
            {
                dequeuedTasks.Add(dequeuedTask);
            }
        }

        return dequeuedTasks;
    }

    private async Task InvokeTask(InMemoryJob task)
    {
        try
        {
            Interlocked.Increment(ref _tasksRunningCount);
            await task.Invoke();
        }
        finally
        {
            Interlocked.Decrement(ref _tasksRunningCount);
        }
    }
}
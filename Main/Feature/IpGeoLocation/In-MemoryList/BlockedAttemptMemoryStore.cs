namespace Main.Feature.IpGeoLocation.Endpoints.Logs;

using System.Collections.Concurrent;

public class BlockedAttemptMemoryStore : IBlockedAttemptStore
{
    private readonly ConcurrentDictionary<Guid, BlockedAttemptLog> _logs = new();

    public void Add(BlockedAttemptLog log)
    {
        _logs.TryAdd(Guid.NewGuid(), log);
    }

    public IReadOnlyList<BlockedAttemptLog> GetAll()
    {
        return _logs.Values
                    .OrderByDescending(l => l.TimestampUtc)
                    .ToList();
    }
}

public class BlockedAttemptLog
{
    public string IpAddress { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty;
    public bool IsBlocked { get; set; }
    public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;
    public string? UserAgent { get; set; }
}

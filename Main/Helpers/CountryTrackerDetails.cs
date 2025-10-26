namespace Main.Helpers;

public sealed class CountryTrackerDetails
{
    public string CountryName { get; set; } = string.Empty;
    public int AccessCount { get; set; } = 0;
    public string CountryCode { get; set; } = string.Empty;
    public DateTime BlockedAt { get; set; } = DateTime.UtcNow;
    public bool IsTemporarilyBlocked { get; set; } = false;
    public TimeSpan BlockedDuration { get; set; } = TimeSpan.FromHours(2);
    public bool HasExpiredBlock()
    {
        if (!IsTemporarilyBlocked)
            return false;
        var res = DateTime.UtcNow >= BlockedAt.Add(BlockedDuration);
        return res;
    }
}
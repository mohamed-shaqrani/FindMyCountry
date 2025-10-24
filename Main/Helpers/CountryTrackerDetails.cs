namespace Main.Helpers;

public sealed class CountryTrackerDetails
{
    public string CountryName { get; set; } = string.Empty;
    public int AccessCount { get; set; } = 0;
    public string CountryCode { get; set; } = string.Empty;
    public DateTime BlockedAt { get; set; } = DateTime.UtcNow;
}
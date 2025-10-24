using System.Collections.Concurrent;
namespace Main.Helpers;

public class BlockedCountryIPTracker : ICountryIPTracker
{
    public ConcurrentDictionary<string, CountryTrackerDetails> BlockedCountryIpDictionary { get; } = new();
    public IEnumerable<CountryTrackerDetails> Search(string? countryCode = null)
    {
        if (string.IsNullOrEmpty(countryCode))
            return BlockedCountryIpDictionary.Values;

        var upper = countryCode.ToUpperInvariant();
        return BlockedCountryIpDictionary.Where(a => a.Key.Contains(upper)).Select(a => a.Value);
    }

}

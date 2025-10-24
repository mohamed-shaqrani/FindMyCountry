using System.Collections.Concurrent;

namespace Main.Helpers;

public interface ICountryIPTracker
{
    ConcurrentDictionary<string, CountryTrackerDetails> BlockedCountryIpDictionary { get; }
    IEnumerable<CountryTrackerDetails> Search(string? countryCode = null);
}
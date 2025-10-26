using Main.Feature.IpGeoLocation.Endpoints.Countries;
using Main.Helpers;
using System.Collections.Concurrent;

public class BlockedCountryIPTracker : ICountryIPTracker
{
    private static readonly ConcurrentDictionary<string, CountryTrackerDetails> _blockedCountryIpDictionary = new();

    public ConcurrentDictionary<string, CountryTrackerDetails> BlockedCountryIpDictionary
        => _blockedCountryIpDictionary;

    public List<string> ExpiredBlockedCountries(double addedMinutes)
    {
        return _blockedCountryIpDictionary
            .Where(a => a.Value.IsTemporarilyBlocked
                   && a.Value.BlockedAt.AddMinutes(addedMinutes) > DateTime.UtcNow)
            .Select(a => a.Value.CountryCode)
            .ToList();
    }

    public void RemoveExpiredBlockedCountries()
    {
        var listOfExpiredBlockedCountryCodes = ExpiredCountriesCode();
        foreach (var countryCode in listOfExpiredBlockedCountryCodes)
        {
            _blockedCountryIpDictionary.TryRemove(countryCode, out _);
            Console.WriteLine($"Removed expired country: {countryCode}");
        }
    }

    private List<string> ExpiredCountriesCode()
    {
        return _blockedCountryIpDictionary
            .Where(a => a.Value.HasExpiredBlock())
            .Select(a => a.Key)
            .ToList();
    }

    public IEnumerable<CountryTrackerDetails> Search(string? countryCode = null)
    {
        if (string.IsNullOrEmpty(countryCode))
            return _blockedCountryIpDictionary.Values;

        var upper = countryCode.ToUpperInvariant();
        return _blockedCountryIpDictionary
            .Where(a => a.Key.Contains(upper))
            .Select(a => a.Value);
    }

    public bool IsCountryBlocked(string countryCode)
    {
        if (string.IsNullOrEmpty(countryCode))
            return false;
        var upper = countryCode.ToUpperInvariant();
        if (_blockedCountryIpDictionary.TryGetValue(upper, out var details))
        {
            var res = details.IsTemporarilyBlocked;
            var res2 = details.HasExpiredBlock();
            if (!details.IsTemporarilyBlocked)
            {
                return true;
            }

            return details.IsTemporarilyBlocked && !details.HasExpiredBlock();
        }
        return false;
    }
}
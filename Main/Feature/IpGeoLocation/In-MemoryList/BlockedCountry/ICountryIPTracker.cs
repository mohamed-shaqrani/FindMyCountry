using Main.Helpers;
using System.Collections.Concurrent;

namespace Main.Feature.IpGeoLocation.Endpoints.Countries;
public interface ICountryIPTracker
{
    ConcurrentDictionary<string, CountryTrackerDetails> BlockedCountryIpDictionary { get; }
    void RemoveExpiredBlockedCountries();
    bool IsCountryBlocked(string countryCode);

    List<string> ExpiredBlockedCountries(double addedMinutes);
    IEnumerable<CountryTrackerDetails> Search(string? countryCode = null);
}

namespace Main.Feature.IpGeoLocation.Endpoints.VM;

public class AllBlockedCountriesViewModel
{
    public string CountryName { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty;
    public DateTime DateAdded { get; set; } = DateTime.UtcNow;

}

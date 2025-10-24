using FluentValidation;
namespace Main.Feature.IpGeoLocation.Endpoints.VM;
public record CountryIpAddressViewModel(string IpAddress);
public class CountryIpAddressViewModelValidator : AbstractValidator<CountryIpAddressViewModel>
{
    public CountryIpAddressViewModelValidator()
    {
    }
}
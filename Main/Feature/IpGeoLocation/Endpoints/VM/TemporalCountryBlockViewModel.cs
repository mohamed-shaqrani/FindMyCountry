
using FluentValidation;

namespace Main.Feature.IpGeoLocation.Endpoints.VM;

public record TemporalCountryBlockViewModel(string CountryCode, int DurationMinutes);

public class TemporalCountryBlockViewModelValidator : AbstractValidator<TemporalCountryBlockViewModel>
{
    static readonly string[] ValidCountryCodes = { "AF", "US", "AL", "DZ", "AS", "AD", "AO", "AI", "AQ", "AG", "AR", "AM", "AW", "AU", "AT", "AZ", "BS", "BH", "BD", "BB", "BY", "BE", "BZ", "BJ", "BM", "BT", "BO", "BA", "BW", "BR", "IO", "VG", "BN", "BG", "BF", "BI", "KH", "CM", "CA", "CV", "KY", "CF", "TD", "CL", "CN", "CX", "CC", "CO", "KM", "CG", "CD", "CK", "CR", "HR", "CU", "CY", "CZ", "DK", "DJ", "DM", "DO", "EC", "EG", "SV", "GQ", "ER", "EE", "ET", "FK", "FO", "FJ", "FI", "FR", "GF", "PF", "TF", "GA", "GM", "GE", "DE", "GH", "GI", "GR", "GL", "GD", "GP", "GU", "GT", "GG", "GN", "GW", "GY", "HT", "HN", "HK", };
    public TemporalCountryBlockViewModelValidator()
    {
        RuleFor(x => x.CountryCode)
            .NotEmpty().WithMessage("Country code is required.")
            .Length(2).WithMessage("Country code must be 2 characters long.")
            .Must(code => ValidCountryCodes.Contains(code.ToUpper())).WithMessage("Invalid Country Code");
        RuleFor(x => x.DurationMinutes)
            .GreaterThan(0).WithMessage("Duration must be greater than 0 minutes.")
            .LessThanOrEqualTo(1440).WithMessage("Duration must be less than or equal to 1440 minutes (24 hours).");
    }
}

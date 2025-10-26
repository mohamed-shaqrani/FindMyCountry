using Hangfire;
using Main.Common.Base;
using Main.Common.Response;
using Main.Common.Response.Endpint;
using Main.Feature.IpGeoLocation.Endpoints.VM;
using Main.Helpers;
using Microsoft.AspNetCore.Mvc;
namespace Main.Feature.IpGeoLocation.Endpoints.Countries;
[Route("api/countries/temporal-block")]

public sealed class TemporalCountryBlockEndpoint : BaseEndpoint<TemporalCountryBlockViewModel, EndpointResponse<bool>>
{
    private readonly ICountryIPTracker _countryIPTracker;
    public TemporalCountryBlockEndpoint(BaseEndpointParam<TemporalCountryBlockViewModel> param, ICountryIPTracker countryIPTracker) : base(param)
    {
        _countryIPTracker = countryIPTracker;
    }
    [HttpPost]
    public ActionResult<EndpointResponse<bool>> TemporalBlockCountry([FromBody] TemporalCountryBlockViewModel request)
    {
        var validationResponse = ValidateRequest(request);
        if (!validationResponse.IsSuccess)
        {
            return
               BadRequest(EndpointResponse<PageList<CountryTrackerDetails>>.Failure(validationResponse.ErrorCode, validationResponse.Message));
        }
        var checkCountryAlreadyBlockedAndNotExpired = _countryIPTracker.BlockedCountryIpDictionary
                                                               .Any(a => a.Key == request.CountryCode.ToUpper()
                                                                && !a.Value.HasExpiredBlock());
        if (checkCountryAlreadyBlockedAndNotExpired)
        {
            return Conflict(EndpointResponse<bool>.Failure(ErrorCode.Conflict, $"Country {request.CountryCode.ToUpper()} is already blocked."));
        }
        var res = _countryIPTracker.BlockedCountryIpDictionary.TryAdd(request.CountryCode.ToUpper(),
                                                                new CountryTrackerDetails
                                                                {
                                                                    CountryCode = request.CountryCode.ToUpper(),
                                                                    BlockedAt = DateTime.UtcNow,
                                                                    IsTemporarilyBlocked = true,
                                                                    BlockedDuration =TimeSpan.FromMinutes(request.DurationMinutes)

                                                                });
        RecurringJob.AddOrUpdate(recurringJobId: "CleanupBlockedAttempts", () => _countryIPTracker.RemoveExpiredBlockedCountries(), cronExpression: "*/5 * * * *");


        return Ok(EndpointResponse<bool>.Success(true, $"Country {request.CountryCode.ToUpper()} blocked for {request.DurationMinutes} minutes."));



    }
}

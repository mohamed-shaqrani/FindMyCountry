using Main.Common.Base;
using Main.Common.Response.Endpint;
using Main.Feature.IpGeoLocation.Endpoints.VM;
using Main.Helpers;
using Microsoft.AspNetCore.Mvc;
namespace Main.Feature.IpGeoLocation.Endpoints;
[Route("api/countries/block/")]

public sealed class BlockCountryEndpoint : BaseEndpoint<BlockCountryViewModel, EndpointResponse<bool>>
{
    private readonly ICountryIPTracker _countryIPTracker;
    public BlockCountryEndpoint(BaseEndpointParam<BlockCountryViewModel> param, ICountryIPTracker countryIPTracker) : base(param)
    {
        _countryIPTracker = countryIPTracker;
    }
    [HttpPost]
    public ActionResult<EndpointResponse<bool>> BlockCountry([FromBody] BlockCountryViewModel request)
    {
        var validationResponse = ValidateRequest(request);
        if (!validationResponse.IsSuccess)
        {
            return
               BadRequest(EndpointResponse<bool>.Failure(validationResponse.ErrorCode, validationResponse.Message));
        }
        var res = _countryIPTracker.BlockedCountryIpDictionary.TryAdd(request.CountryCode.ToUpper(),
                                                                    new CountryTrackerDetails
                                                                    {
                                                                        CountryCode = request.CountryCode.ToUpper(),
                                                                        BlockedAt = DateTime.UtcNow,
                                                                    });
        return !res
                ? BadRequest(EndpointResponse<bool>.Failure(Common.Response.ErrorCode.ValidationError, "Country is already blocked."))
                : Ok(EndpointResponse<bool>.Success(true, "Country blocked successfully."));
    }
}

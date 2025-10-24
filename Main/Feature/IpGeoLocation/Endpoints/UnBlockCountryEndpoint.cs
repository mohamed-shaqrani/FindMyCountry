using Main.Common.Base;
using Main.Common.Response.Endpint;
using Main.Feature.IpGeoLocation.Endpoints.VM;
using Main.Helpers;
using Microsoft.AspNetCore.Mvc;
namespace Main.Feature.IpGeoLocation.Endpoints;

[Route("api/countries/block/")]
public sealed class UnBlockCountryEndpoint : BaseEndpoint<BlockCountryViewModel, EndpointResponse<bool>>
{
    private readonly ICountryIPTracker _countryIPTracker;
    public UnBlockCountryEndpoint(BaseEndpointParam<BlockCountryViewModel> param, ICountryIPTracker countryIPTracker) : base(param)
    {
        _countryIPTracker = countryIPTracker;
    }
    [HttpDelete("{countryCode}")]
    public ActionResult<EndpointResponse<bool>> BlockCountry([FromRoute] string countryCode)
    {

        var request = new BlockCountryViewModel(countryCode);
        var validationResponse = ValidateRequest(request);
        if (!validationResponse.IsSuccess)
        {
            return
               BadRequest(EndpointResponse<bool>.Failure(validationResponse.ErrorCode, validationResponse.Message));
        }
        var res = _countryIPTracker.BlockedCountryIpDictionary.TryRemove(request.CountryCode.ToUpper(), out var removedEntry);
        return !res
                ? BadRequest(EndpointResponse<bool>.Failure(Common.Response.ErrorCode.ValidationError, "Country already does not exist."))
                : Ok(EndpointResponse<bool>.Success(true, "Country Unblocked successfully."));
    }
}

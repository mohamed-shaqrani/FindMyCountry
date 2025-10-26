using Main.Common.Base;
using Main.Common.Response;
using Main.Common.Response.Endpint;
using Main.Feature.IpGeoLocation.Endpoints.Countries;
using Main.Feature.IpGeoLocation.Endpoints.Logs;
using Main.Feature.IpGeoLocation.Endpoints.VM;
using Main.Feature.IpGeoLocation.Queries;
using Microsoft.AspNetCore.Mvc;
using System.Net;
namespace Main.Feature.IpGeoLocation.Endpoints.IP;
[Route("api/ip/check-block/")]

public sealed class CheckBlockCountryEndpoint : BaseEndpoint<BlockCountryViewModel, EndpointResponse<bool>>
{
    private readonly ICountryIPTracker _countryIPTracker;
    private readonly IBlockedAttemptStore _blockedAttemptStore;

    public CheckBlockCountryEndpoint(BaseEndpointParam<BlockCountryViewModel> param,
        IBlockedAttemptStore blockedAttemptStore,
        ICountryIPTracker countryIPTracker) : base(param)
    {
        _countryIPTracker = countryIPTracker;
        _blockedAttemptStore = blockedAttemptStore;

    }
    [HttpPost]
    public async Task<ActionResult<EndpointResponse<bool>>> CheckBlocked()
    {

        var setIpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

        if (!IPAddress.TryParse(setIpAddress, out var res))
            return BadRequest(EndpointResponse<bool>.Failure(ErrorCode.ValidationError, "Invalid IP Address"));
        if (setIpAddress == "::1")
        {
            setIpAddress = null;
        }
        var query = new IpGeolocationQuery(null);
        var result = await _mediator.Send(query);

        var isCountryBlocked = _countryIPTracker.IsCountryBlocked(result.Data.country_code2);
        if (isCountryBlocked)
        {
            _blockedAttemptStore.Add(new BlockedAttemptLog
            {
                IpAddress = setIpAddress ?? "Localhost",
                CountryCode = result.Data.country_code2.ToUpper(),
                IsBlocked = true,
                TimestampUtc = DateTime.UtcNow,
                UserAgent = HttpContext.Request.Headers["User-Agent"].ToString()

            });
        }
        return Ok(EndpointResponse<bool>.Success(isCountryBlocked, isCountryBlocked ? "Country is blocked." : "Country is not blocked."));
    }
}

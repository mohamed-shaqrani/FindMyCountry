using Main.Common.Base;
using Main.Common.Response;
using Main.Common.Response.Endpint;
using Main.Feature.IpGeoLocation.Endpoints.VM;
using Main.Feature.IpGeoLocation.Queries;
using Microsoft.AspNetCore.Mvc;
using System.Net;
namespace Main.Feature.IpGeoLocation.Endpoints;

[Route("/api/ip/lookup")]
public sealed class FindCountryIpEndpoint : BaseEndpoint<CountryIpAddressViewModel, EndpointResponse<bool>>
{
    public FindCountryIpEndpoint(BaseEndpointParam<CountryIpAddressViewModel> param) : base(param)
    {
    }
    [HttpGet]
    public async Task<ActionResult<EndpointResponse<bool>>> FindCountry([FromQuery] string? ipAddress)
    {
        var setIpAddress = ipAddress;
        if (string.IsNullOrEmpty(ipAddress))
            setIpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

        if (!IPAddress.TryParse(setIpAddress, out var res))
            return BadRequest(EndpointResponse<bool>.Failure(ErrorCode.ValidationError, "Invalid IP Address"));

        var query = new IpGeolocationQuery(ipAddress);
        var result = await _mediator.Send(query);
        if (result.IsSuccess)

            return Ok(result);

        return BadRequest();

    }

}

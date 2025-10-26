using Main.Common.Base;
using Main.Common.Response;
using Main.Common.Response.Endpint;
using Main.Feature.IpGeoLocation.Endpoints.VM;
using Main.Feature.IpGeoLocation.Queries;
using Main.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;
namespace Main.Feature.IpGeoLocation.Endpoints.IP;

[Route("/api/ip/lookup")]
public sealed class FindCountryIpEndpoint : BaseEndpoint<CountryIpAddressViewModel, EndpointResponse<IpGeolocationResponse>>
{
    public FindCountryIpEndpoint(BaseEndpointParam<CountryIpAddressViewModel> param) : base(param)
    {
    }
    [HttpGet]
    public async Task<ActionResult<EndpointResponse<IpGeolocationResponse>>> FindCountry([FromQuery] string? ipAddress)
    {
        if (string.IsNullOrEmpty(ipAddress))
            ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        if (!IPAddress.TryParse(ipAddress, out var res))
            return BadRequest(EndpointResponse<bool>.Failure(ErrorCode.ValidationError, "Invalid IP Address"));
        if (ipAddress == "::1")
        {
            ipAddress = null;
        }
        var query = new IpGeolocationQuery(ipAddress);
        var result = await _mediator.Send(query);
        return Ok(EndpointResponse<IpGeolocationResponse>.Success(result.Data, "Success"));
    }

}

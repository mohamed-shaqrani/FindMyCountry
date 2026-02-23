using Main.Common.Base;
using Main.Common.Response.Endpint;
using Main.Feature.IpGeoLocation.Endpoints.VM;
using Main.Feature.IpGeoLocation.Queries;
using Microsoft.AspNetCore.Mvc;
namespace Main.Feature.IpGeoLocation.Endpoints.IP;
[Route("api/users/")]

public sealed class GetAllUsersEndpoint : BaseEndpoint<BlockCountryViewModel, EndpointResponse<bool>>
{
    public GetAllUsersEndpoint(BaseEndpointParam<BlockCountryViewModel> param
       ) : base(param)
    {

    }
    [HttpGet]
    public async Task<ActionResult<EndpointResponse<bool>>> GetAllUsers()
    {
        var query = new GetUsersQuery(null);
        var result = await _mediator.Send(query);


        return Ok(EndpointResponse<bool>.Success(true, "Country is blocked.Country is not blocked."));
    }
}

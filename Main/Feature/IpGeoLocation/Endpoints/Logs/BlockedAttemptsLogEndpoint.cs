using Main.Common.Base;
using Main.Common.Pagination;
using Main.Common.Response.Endpint;
using Main.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Main.Feature.IpGeoLocation.Endpoints.Logs;

[Route("api/logs/blocked-attempts")]
public sealed class BlockedAttemptsLogEndpoint
    : BaseEndpoint<PaginationParam, EndpointResponse<PageList<BlockedAttemptLog>>>
{
    private readonly IBlockedAttemptStore _blockedAttemptStore;
    public BlockedAttemptsLogEndpoint(
        BaseEndpointParam<PaginationParam> param,
        IBlockedAttemptStore blockedAttemptStore)
        : base(param)
    {
        _blockedAttemptStore = blockedAttemptStore;
    }

    [HttpGet]
    public ActionResult<EndpointResponse<PageList<BlockedAttemptLog>>> GetAll([FromQuery] PaginationParam request)
    {
        var logs = _blockedAttemptStore.GetAll();

        var pageList = PageList<BlockedAttemptLog>.Create(logs, request.PageNumber, request.PageSize);
        Response.AddPaginationHeader(pageList);

        return Ok(EndpointResponse<PageList<BlockedAttemptLog>>.Success(pageList, "Success"));
    }
}

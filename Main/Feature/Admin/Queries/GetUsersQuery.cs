using Main.Common.Base;
using Main.Common.Response;
using Main.Common.Response.RequestResult;
using Main.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace Main.Feature.IpGeoLocation.Queries;

internal sealed record GetUsersQuery(string? IpAddress) : IRequest<RequestResult<List<User>>>;
internal sealed class GetUsersHandler : BaseRequestHandler<GetUsersQuery, RequestResult<List<User>>>
{
    public GetUsersHandler(BaseRequestHandlerParam baseRequest) : base(baseRequest)
    {

    }
    public override async Task<RequestResult<List<User>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var query = await _unitOfWork.GetRepository<User>().GetAll().ToListAsync(cancellationToken);

        if (query == null || query.Count == 0)
        {
            return RequestResult<List<User>>.Failure(ErrorCode.NotFound, "No users found.");
        }

        return RequestResult<List<User>>.Success(query, "success");
    }


}



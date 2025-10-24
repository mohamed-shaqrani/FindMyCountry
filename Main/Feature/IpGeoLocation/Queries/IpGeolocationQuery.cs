using Main.Common.Base;
using Main.Common.Response.RequestResult;
using Main.Helpers;
using Main.Services;
using MediatR;
using Microsoft.Extensions.Options;
namespace Main.Feature.IpGeoLocation.Queries;

internal sealed record IpGeolocationQuery(string IpAddress) : IRequest<RequestResult<IpGeolocationResponse>>;
internal sealed class IpGeolocationHandler : BaseRequestHandler<IpGeolocationQuery, RequestResult<IpGeolocationResponse>>
{
    private readonly HttpClient _httpClient;
    private readonly IpGeolocationOptions _options;
    public IpGeolocationHandler(BaseRequestHandlerParam baseRequest, HttpClient httpClient, IOptions<IpGeolocationOptions> options) : base(baseRequest)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }
    public override async Task<RequestResult<IpGeolocationResponse>> Handle(IpGeolocationQuery request, CancellationToken cancellationToken)
    {
        var url = $"{_options.BaseUrl}?apiKey={_options.ApiKey}&ip={request.IpAddress}";

        var res = await _httpClient.GetFromJsonAsync<IpGeolocationResponse>(url, cancellationToken);
        return RequestResult<IpGeolocationResponse>.Success(res, "success");
    }

}



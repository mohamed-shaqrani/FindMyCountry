using Main.Common.Base;
using Main.Common.Pagination;
using Main.Common.Response.Endpint;
using Main.Extensions;
using Main.Feature.IpGeoLocation.Endpoints.VM;
using Main.Helpers;
using Microsoft.AspNetCore.Mvc;
namespace Main.Feature.IpGeoLocation.Endpoints.Countries;
[Route("api/countries/")]

public sealed class AllBlockedCountriesEndpoint : BaseEndpoint<CountryParam, EndpointResponse<bool>>
{
    private readonly ICountryIPTracker _countryIPTracker;
    public AllBlockedCountriesEndpoint(BaseEndpointParam<CountryParam> param, ICountryIPTracker countryIPTracker) : base(param)
    {
        _countryIPTracker = countryIPTracker;
    }
    [HttpGet]
    public ActionResult<EndpointResponse<PageList<AllBlockedCountriesViewModel>>> GetAll([FromQuery] CountryParam request)
    {
        var list = new List<CountryTrackerDetails>();
        if (!string.IsNullOrEmpty(request.CountryCode))
        {
            list = _countryIPTracker.Search(request.CountryCode).ToList();

        }
        else
        {
            list = _countryIPTracker.BlockedCountryIpDictionary.Values.ToList();

        }
        var pageList = PageList<CountryTrackerDetails>.Create(list, request.PageNumber, request.PageNumber);
        Response.AddPaginationHeader(pageList);

        return list.Any()
            ? Ok(EndpointResponse<PageList<CountryTrackerDetails>>.Success(pageList, "Success"))
            : NotFound(Enumerable.Empty<PageList<CountryTrackerDetails>>());

    }
}

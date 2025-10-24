namespace Main.Services;

using Main.Helpers;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

public class IpGeolocationService
{
    private readonly HttpClient _httpClient;
    private readonly IpGeolocationOptions _options;

    public IpGeolocationService(HttpClient httpClient, IOptions<IpGeolocationOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<IpGeolocationResponse?> GetLocationAsync(string ip)
    {
        var url = $"{_options.BaseUrl}?apiKey={_options.ApiKey}&ip={ip}";
        return await _httpClient.GetFromJsonAsync<IpGeolocationResponse>(url);
    }
}

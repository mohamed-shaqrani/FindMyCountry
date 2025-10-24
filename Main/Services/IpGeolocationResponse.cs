namespace Main.Services;

public class IpGeolocationResponse
{
    public string Ip { get; set; } = string.Empty;
    public string country_name { get; set; } = string.Empty;
    public string country_code2 { get; set; } = string.Empty;
    public string country_code3 { get; set; } = string.Empty;

    public string isp { get; set; } = string.Empty;


}

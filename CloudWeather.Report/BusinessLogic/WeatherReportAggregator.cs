using System.Text.Json;
using CloudWeather.Report.Config;
using CloudWeather.Report.DataAccess;
using CloudWeather.Report.Models;
using Microsoft.Extensions.Options;

namespace CloudWeather.Report.BusinessLogic;

public interface IWeatherReportAggregator{
    public Task<WeatherReport> BuildReport(string zip, int days);
}
public class WeatherReportAggregator  : IWeatherReportAggregator{
     private readonly IHttpClientFactory _http;
     private readonly ILogger<WeatherReportAggregator> _logger;

     private readonly WeatherDataConfig _weatherDataConfig;
     private readonly WeatherReportDbContext _context;

    public WeatherReportAggregator(IHttpClientFactory http,
                                   ILogger<WeatherReportAggregator> logger,
                                   IOptions<WeatherDataConfig> config,
                                   WeatherReportDbContext context){
        
        _http = http;
        _logger = logger;
        _weatherDataConfig = config.Value;
        _context = context;
        

    }

    public async Task<WeatherReport> BuildReport(string zip, int days)
    {
       var httpClient = _http.CreateClient();
       var precipData = await FetchPrecipitationData(httpClient, zip,days);
       var totalSnow = GetTotalSnow(precipData);
       var totalRain =  GetTotalRain(precipData);
       

       var tempData = await FetchTempData(httpClient, zip,days);
       var averageHighTemp= 0;

       var weatherReport = new WeatherReport(){
          
       };

        _context.Add(weatherReport);
        await _context.SaveChangesAsync();
       return weatherReport;
       
    }

    private static decimal GetTotalSnow(IEnumerable<PrecipitationModel> precipData){
        var totalSnow = precipData
            .Where(p => p.WeatherType == "snow")
            .Sum(p => p.AmountInches);
        return Math.Round(totalSnow, 1);
    }

   private static decimal GetTotalRain(IEnumerable<PrecipitationModel> precipData){
        var totalRain = precipData
            .Where(p => p.WeatherType == "rain")
            .Sum(p => p.AmountInches);
        return Math.Round(totalRain, 1);
    }

    private async Task<List<TemperatureModel>> FetchTempData(HttpClient httpClient, string zip, int days)
    {
       var endpoint  = BuildTemperatureServiceEndpoint(zip, days);
       var temperatureRecords = await httpClient.GetAsync(endpoint);
       var temperatureData = await temperatureRecords
                            .Content
                            .ReadFromJsonAsync<List<TemperatureModel>>();

        return temperatureData ?? new List<TemperatureModel>(); 
    }

    private async Task<List<PrecipitationModel>> FetchPrecipitationData(HttpClient httpClient, string zip, int days)
    {
        var endpoint  = BuildPrecipitationEndpoint(zip, days);  
        var precipitationRecords = await httpClient.GetAsync(endpoint);
        var jsonSerializerOptions = new JsonSerializerOptions{
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        var precipData = await precipitationRecords 
                               .Content
                               .ReadFromJsonAsync<List<PrecipitationModel>>();

        return precipData ?? new List<PrecipitationModel>();
    }

    private string  BuildTemperatureServiceEndpoint(string zip, int days){
        var tempServiceProtocol = _weatherDataConfig.TempDataProtocol;
        var tempServiceHost = _weatherDataConfig.TempDataHost;
        var tempServicePort = _weatherDataConfig.TempDataPort;

        return $"{tempServiceProtocol}://{tempServiceHost}:{tempServicePort}/observation/";
    }

    private string BuildPrecipitationEndpoint(string zip, int days)
    {
        var precipServiceProtocol = _weatherDataConfig.PrecipDataProtocol;
        var precipServiceHost = _weatherDataConfig.PrecipDataHost;
        var precipServicePort = _weatherDataConfig.PrecipDataPort;

        return $"{precipServiceProtocol}://{precipServiceHost}://{precipServicePort}/observation/{zip}?days={days}";
    }
}
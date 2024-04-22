using FluentValidation;
using FunctionCalling.Controllers.Dtos;
using FunctionCalling.ExternalServices.Mdm;
using FunctionCalling.Validators;
using FunctionCalling.Validators.CustomValidators;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Text.Json;
using FluentValidation.Results;
using FunctionCalling.ExternalServices.Mdm.Dto;
using FunctionCalling.Repository.Quotes;
using System.Text.RegularExpressions;
using Microsoft.Extensions.FileSystemGlobbing.Internal;

namespace FunctionCalling.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherController : ControllerBase
    {

       

        private readonly ILogger<WeatherController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IValidator<GetCurrentWeatherRequest> _currentWeatherRequestValidator;
        private readonly IValidator<GetPastRequest> _getPastRequestValidator;


        public WeatherController(ILogger<WeatherController> logger,IHttpClientFactory httpClientFactory,
            IValidator<GetCurrentWeatherRequest> currentWeatherRequestValidator, IValidator<GetPastRequest> getPastRequestValidator)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _currentWeatherRequestValidator = currentWeatherRequestValidator;
            _getPastRequestValidator = getPastRequestValidator;
        }
     

        [HttpPost(template: "get-current-weather", Name = nameof(GetCurrentWeather))]
        [SwaggerOperation(
            Description = "returns the current weather given a town or region name",
            OperationId = nameof(GetCurrentWeather))]
        public async Task<Results<BadRequest<IEnumerable<ValidationErrorInfo>>, Ok<GetWeatherResponse>>> GetCurrentWeather(
            [FromBody] GetCurrentWeatherRequest getCurrentWeatherRequest)
        {
            var result = _currentWeatherRequestValidator.Validate(getCurrentWeatherRequest);
            if (result.Errors.Any())
            {
                return TypedResults.BadRequest(result.Errors.Select(e =>
                    JsonSerializer.Deserialize<ValidationErrorInfo>(e.ErrorMessage) ?? new ValidationErrorInfo()));
            }

            return TypedResults.Ok(await WeatherStackCurrent(getCurrentWeatherRequest.Location));
        }

        [HttpPost(template: "get-past-weather", Name = nameof(GetPastForecast))]
        [SwaggerOperation(
            Description = "returns the weather that there wa sin the past given a town or region name and a date in the past",
            OperationId = nameof(GetPastForecast))]
        public async Task<Results<BadRequest<IEnumerable<ValidationErrorInfo>>, Ok<GetWeatherResponse>>> GetPastForecast(
            [FromBody] GetPastRequest getPastRequest)
        {
            var result = _getPastRequestValidator.Validate(getPastRequest);
            if (result.Errors.Any())
            {
                return TypedResults.BadRequest(result.Errors.Select(e =>
                    JsonSerializer.Deserialize<ValidationErrorInfo>(e.ErrorMessage) ?? new ValidationErrorInfo()));
            }

            return TypedResults.Ok(await WeatherStackHistorical(getPastRequest.Location, getPastRequest.Date));
        }

        private async Task<GetWeatherResponse> WeatherStackCurrent(string location )
        {
            var client = _httpClientFactory.CreateClient();
            var ret = await client.GetAsync($"http://api.weatherstack.com/current?access_key=7ea70979788db31811aef63a3a676686&query={location}&units=m");
            if (!ret.IsSuccessStatusCode)
            {
                throw new Exception($"{ret.StatusCode} + {await ret.Content.ReadAsStringAsync()}");
            }
            return JsonSerializer.Deserialize<GetWeatherResponse>(await ret.Content.ReadAsStreamAsync()) ??new();
        }

        private async Task<GetWeatherResponse> WeatherStackHistorical(string location, DateTime date)
        {
            var client = _httpClientFactory.CreateClient();
            var ret = await client.GetAsync($"http://api.weatherstack.com/forecast?access_key=7ea70979788db31811aef63a3a676686&query={location}&historical={date:yyyy-MM-dd}&units=m");
            if (!ret.IsSuccessStatusCode)
            {
                throw new Exception($"{ret.StatusCode} + {await ret.Content.ReadAsStringAsync()}");
            }
            return JsonSerializer.Deserialize<GetWeatherResponse>(await ret.Content.ReadAsStreamAsync()) ?? new();
        }













    }
    [SwaggerSchema(Required = new[] { "Location" })]
    public class GetCurrentWeatherRequest
    {
        [SwaggerSchema("The location (town or region) name. IMPORTANT : Assistant must ask the user a value for location. If not provided in the conversation, Assistant must not not make up one", Nullable = false)]
        public string Location { get; init; } = "";
    }

    [SwaggerSchema(Required = new[] { "Location", "Date" })]
    public class GetPastRequest
    {
        [SwaggerSchema("The location (town or region) name. IMPORTANT : Assistant must ask the user a value for location. If not provided in the conversation, Assistant must not not make up one", Nullable = false)]
        public string Location { get; init; } = "";
        [SwaggerSchema("The date user want to know how the weather was like. IMPORTANT : Assistant must ask the user a value for date. If not provided in the conversation, Assistant must not not make up one", Nullable = false)]
        public DateTime Date { get; init; } 
    }


    public class Current
    {
        public string observation_time { get; set; }
        public float temperature { get; set; }
        public int weather_code { get; set; }
        public List<string> weather_icons { get; set; }
        public List<string> weather_descriptions { get; set; }
        public float wind_speed { get; set; }
        public float wind_degree { get; set; }
        public string wind_dir { get; set; }
        public float pressure { get; set; }
        public float precip { get; set; }
        public float humidity { get; set; }
        public float cloudcover { get; set; }
        public float feelslike { get; set; }
        public float uv_index { get; set; }
        public float visibility { get; set; }
    }

    public class Location
    {
        public string name { get; set; }
        public string country { get; set; }
        public string region { get; set; }
        public string lat { get; set; }
        public string lon { get; set; }
        public string timezone_id { get; set; }
        public string localtime { get; set; }
        public int localtime_epoch { get; set; }
        public string utc_offset { get; set; }
    }

    public class Request
    {
        public string type { get; set; }
        public string query { get; set; }
        public string language { get; set; }
        public string unit { get; set; }
    }

    public class GetWeatherResponse
    {
        public Request request { get; set; }
        public Location location { get; set; }
        public Current current { get; set; }
    }



}
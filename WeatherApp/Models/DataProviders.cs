using System.Collections.Generic;
using FluentValidation;
using Newtonsoft.Json;

namespace WeatherApp.Models
{

// openweather

    public class OpenWeatherResponse
    {
        [JsonProperty(Required = Required.Always)]
        public string name {get; set; }
        [JsonProperty(Required = Required.Always)]
        public OpenWeatherResponseBody main { get; set; }
    }

    public class OpenWeatherResponseBody
    {
        [JsonProperty(Required = Required.Always)]
        public float temp { get; set; }
        [JsonProperty(Required = Required.Always)]
        public float pressure { get; set; }
    }

    public class OpenWeatherResponseValidator : AbstractValidator<OpenWeatherResponse>
    {
        public OpenWeatherResponseValidator()
        {
/*
            RuleFor(x => x.name).NotEmpty();
            RuleFor(x => x.main).NotEmpty();
            RuleFor(x => x.main.temp).NotEmpty().When(x => x.main != null);
            RuleFor(x => x.main.pressure).NotEmpty().When(x => x.main != null);
*/
        }
    }

// Yahoo

    public class YahooWeatherResponse
    {
        [JsonProperty(Required = Required.Always)]
        public YahooWeatherResponseQuery query { get; set; }
    }

    public class YahooWeatherResponseQuery
    {
        [JsonProperty(Required = Required.Always)]
        public  YahooWeatherResponseQueryResults results { get; set; }
    }

    public class YahooWeatherResponseQueryResults
    {
        [JsonProperty(Required = Required.Always)]
        public  YahooWeatherResponseQueryResultsChannel channel { get; set; }
    }

    public class YahooWeatherResponseQueryResultsChannel
    {
        [JsonProperty(Required = Required.Always)]
        public  YahooWeatherResponseQueryResultsChannelItem item { get; set; }
        [JsonProperty(Required = Required.Always)]
        public  YahooWeatherResponseQueryResultsChannelAtmosphere atmosphere { get; set; }
    }

    public class YahooWeatherResponseQueryResultsChannelItem
    {
        [JsonProperty(Required = Required.Always)]
        public  YahooWeatherResponseQueryResultsChannelItemCondition condition { get; set; }
    }

    public class YahooWeatherResponseQueryResultsChannelItemCondition
    {
        [JsonProperty(Required = Required.Always)]
        public  float temp { get; set; }
    }

    public class YahooWeatherResponseQueryResultsChannelAtmosphere
    {
        [JsonProperty(Required = Required.Always)]
        public float pressure { get; set; }
    }

    public class YahooWeatherResponseValidator : AbstractValidator<YahooWeatherResponse>
    {
        public YahooWeatherResponseValidator()
        {
/*
            RuleFor(x => x.name).NotEmpty();
            RuleFor(x => x.main).NotEmpty();
            RuleFor(x => x.main.temp).NotEmpty().When(x => x.main != null);
            RuleFor(x => x.main.pressure).NotEmpty().When(x => x.main != null);
*/
        }
    }
}

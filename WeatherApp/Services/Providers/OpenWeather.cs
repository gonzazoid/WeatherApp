using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using WeatherApp.Models;
using Newtonsoft.Json;
using FluentValidation;

namespace WeatherApp.Services
{
    public class OpenWeatherProvider : IWeatherProvider
    {

        private IWeatherAppOptions options;
        private IUserAgent crawler;
        public string name { get; } = "openWeather";

        public OpenWeatherProvider(IWeatherAppOptions _options, IUserAgent _crawler)
        {
            options = _options;
            crawler = _crawler;
        }

        async private Task<string> getWeatherAsync(string url)
        {
            return await crawler.GetResponseAsync(url);
        }

        private T parseJson<T> (string json) where T : class
        {
            try
            {
                T result = JsonConvert.DeserializeObject<T>(json);
                return result;
            }catch(JsonException ex)
            {
                // TODO exception handling???
                return null;
            }
        }

        async public Task<State> checkPlaceAsync(Place place)
        {

            string url = String.Format(options.WeatherApp.openWeatherRequestUrl
                                      ,place.OpenWeatherProvidersAlias
                                      ,options.WeatherApp.openWeatherApiId
                                      );
            string content = await getWeatherAsync(url);
            OpenWeatherResponseValidator validator = new OpenWeatherResponseValidator();
            OpenWeatherResponse raw = parseJson<OpenWeatherResponse>(content);
            if(raw == null || !validator.Validate(raw).IsValid)
            {
                throw new WeatherAppNotValidProviderResponseException("not valid response from openWeather provider");
            }

            State state = new State();
            state.temp = raw.main.temp;
            state.pressure = raw.main.pressure;
            state.provider = name;

            return state;
        }
    }
}

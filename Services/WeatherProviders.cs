using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using WeatherApp.Models;
using Newtonsoft.Json;
using FluentValidation;

namespace WeatherApp.Services
{
    public interface IWeatherProviders
    {
        Task<State> checkPlaceOpenWeatherAsync(Place place);
        Task<State> checkPlaceYahooAsync(Place place);
    }

    public class WeatherProviders : IWeatherProviders
    {

        private IWeatherAppOptions options;
        private IUserAgent crawler;

        public WeatherProviders(IWeatherAppOptions _options, IUserAgent _crawler)
        {
            options = _options;
            crawler = _crawler;
        }

        async private Task<string> getWeatherAsync(string url)
        {
            return await crawler.GetResponseAsync(url);
        }

        public T parseJson<T> (string json) where T : class
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

        async public Task<State> checkPlaceOpenWeatherAsync(Place place)
        {

            string url = String.Format(options.WeatherApp.openWeatherRequestUrl
                                      ,place.OpenWeatherProvidersAlias
                                      ,options.WeatherApp.openWeatherApiId
                                      );
            string content = await getWeatherAsync(url);
            OpenWeatherResponseValidator validator = new OpenWeatherResponseValidator();
            OpenWeatherResponse raw = parseJson<OpenWeatherResponse>(content);
            if(!validator.Validate(raw).IsValid)
            {
                // TODO do something and exit
            }

            State state = new State();
            state.temp = raw.main.temp;
            state.pressure = raw.main.pressure;
            state.provider = "openWeather";

            return state;
        }

        async public Task<State> checkPlaceYahooAsync(Place place)
        {

            State state = new State();
            string url = String.Format(options.WeatherApp.yahooRequestUrl, place.YahooProvidersAlias);
            string content = await getWeatherAsync(url);
            // Console.WriteLine(content);
            YahooWeatherResponseValidator validator = new YahooWeatherResponseValidator();
            YahooWeatherResponse raw = parseJson<YahooWeatherResponse>(content);
            if(!validator.Validate(raw).IsValid)
            {
                // TODO do something and exit
            }

            state.temp = 273.15f + raw.query.results.channel.item.condition.temp; // переводим в кельвины
            // TODO what EXACTLY is pressure unit in yahoo`s response?  34236 CAN NOT be atmosphere pressure in mb!!!
            state.pressure = raw.query.results.channel.atmosphere.pressure; // 1 hectopascal = 1 millibar
            state.provider = "yahoo";

            return state;

        }
    }
}

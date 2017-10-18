using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using WeatherApp.Models;
using Newtonsoft.Json;
using FluentValidation;

namespace WeatherApp.Services
{
    public class YahooProvider : IWeatherProvider
    {

        private IWeatherAppOptions options;
        private IUserAgent crawler;
        public string name { get; } = "yahoo";

        public YahooProvider(IWeatherAppOptions _options, IUserAgent _crawler)
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
            state.provider = name;

            return state;

        }
    }
}

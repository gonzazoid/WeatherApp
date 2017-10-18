using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

// using WeatherApp.Services;
using WeatherApp.Models;

namespace WeatherApp.Services
{
    public interface IWeatherChecker
    {
        Task updateWeatherStatesAsync();
    }

    public class WeatherChecker : IWeatherChecker
    {
        private WeatherAppContext dbContext;
        private IWeatherProviders dataProviders;

        public WeatherChecker(IWeatherProviders _dataProviders, WeatherAppContext _dbContext)
        {
            dbContext = _dbContext;
            dataProviders = _dataProviders;
        }

        async public Task updateWeatherStatesAsync()
        {
            var query = from p in dbContext.places
                        orderby p.Title
                        select p;

            // TODO highload management (also you can put it into Services/UserAgent)
            var tasks = query.Select(place => updatingAsync(place));

            try
            {
                await Task.WhenAll(tasks.ToArray());
                dbContext.SaveChanges();
            }
            catch (AggregateException ex)
            {
                // TODO exception handling???
            }
        }

        async private Task updatingAsync(Place place)
        {
            // January 19, 2038 feel free to let me know if something wrong with this code )
            int stamp = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            // пройтись по всем стейтам и поставить стамп и Id
            State[] states = ((await dataProviders
                .checkPlaceAsync(place))
                .Select(state => { state.stamp = stamp; state.PlaceId = place.PlaceId; return state;})).ToArray();

            dbContext.weather_states.AddRange(states);
        }
    }
}


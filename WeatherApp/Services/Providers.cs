using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WeatherApp.Models;
using System.Linq;

namespace WeatherApp.Services
{
    public interface IWeatherProvider
    {
        string name { get; }
        Task<State> checkPlaceAsync(Place place);
    }

    public interface IWeatherProviders
    {
        Task<State[]> checkPlaceAsync(Place place);
        void freeze();
    }

    public class WeatherProviders : IWeatherProviders
    {
        private IWeatherProvider[] providers = {};
        private bool isFrozen = false;

        public void addProvider(IWeatherProvider provider)
        {
            if(!isFrozen){
                Array.Resize(ref providers, providers.Length + 1);
                providers[providers.Length - 1] = provider;
            }
        }

        public void freeze()
        {
            isFrozen = true;
        }

         // ATTENTION Task<State[]> !== Task<State>[]
        async public Task<State[]> checkPlaceAsync(Place place)
        {
            IEnumerable<Task<State>> states = null;

            try {

                states = providers.Select(provider => provider.checkPlaceAsync(place));
                await Task.WhenAll(states.Cast<Task<State>>().ToArray());

            } catch(WeatherAppException e) {
            }
            catch (AggregateException e)
            {
            }

            State[] result = states.Select(state => state.Result).ToArray();
            return result;
        }
    }
}

using System;

using Hangfire;
using Hangfire.SqlServer;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;

using WeatherApp;
using WeatherApp.Services;

namespace WeatherApp.Utils
{
    public class WeatherProviderWorker
    {
        private Startup startup;
        private WeatherChecker weatherChecker;
        private string root;

        public WeatherProviderWorker UseStartup<T>() where T : Startup
        {
            var env = new HostingEnvironment();
            Microsoft.AspNetCore.Hosting.Internal
               .HostingEnvironmentExtensions.Initialize(env
                                                       ,"worker"
                                                       ,root
                                                       ,new WebHostOptions()
                                                       );

            startup = Activator.CreateInstance(typeof(T), new object[]{env}) as T;
            return this;
        }

        public WeatherProviderWorker UseContentRoot(string _root)
        {
            root = _root;
            return this;
        }

        public WeatherProviderWorker Build()
        {
            var serviceCollection = new ServiceCollection();
            startup.ConfigureServices(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            weatherChecker = ActivatorUtilities.CreateInstance<WeatherChecker>(serviceProvider);
            return this;
        }

        public void Run()
        {
            JobStorage.Current = new SqlServerStorage(startup.config.WeatherApp.hangFireConnectionString);
            RecurringJob.AddOrUpdate("updateWeatherStates"
                                    ,() => weatherChecker.updateWeatherStatesAsync(), startup.config.WeatherApp.cronFormatRequestRate
                                    );
        }
    }
}

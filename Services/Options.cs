using System;
using System.IO;
using Microsoft.AspNetCore.Hosting.Builder;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.Configuration;

using WeatherApp.Models;

namespace WeatherApp.Services {

    public interface IWeatherAppOptions
    {
        LoggingSection Logging { get; set; }
        WeatherAppSection WeatherApp { get; set; }
        string DB_PWD {get; set; }
    }

    public class WeatherAppOptions : IWeatherAppOptions
    {
        public LoggingSection Logging { get; set; }
        public WeatherAppSection WeatherApp { get; set; }
        public string DB_PWD {get; set; }

        public WeatherAppOptions(IConfigurationRoot config){

            Logging = new LoggingSection();
            WeatherApp = new WeatherAppSection();

            config.GetSection("Logging").Bind(this.Logging);
            config.GetSection("WeatherApp").Bind(this.WeatherApp);
            DB_PWD = config.GetSection("DB_PWD").Value;
            if(DB_PWD == null || DB_PWD == "")
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine("specify the database password either in the application`s settings or the environment variable DB_PWD.");
                Console.ResetColor();
                System.Environment.Exit(-1);
            }
            WeatherApp.hangFireConnectionString = String.Format(WeatherApp.hangFireConnectionString, DB_PWD);
            WeatherApp.dbConnectionString = String.Format(WeatherApp.dbConnectionString, DB_PWD);
        }
    }
}

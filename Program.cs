using System;
using System.IO;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

using WeatherApp;
using WeatherApp.Models;
using WeatherApp.Utils;
using Microsoft.Extensions.Configuration;

namespace WeatherApp
{
    public class Program
    {
        public static void Main(string[] args)
        {

// data provider

            var worker = new WeatherProviderWorker()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .Build();


// web server

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

// start app

            host.Run();
            worker.Run();
        }
    }
}

using System;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Microsoft.EntityFrameworkCore;

using Hangfire;
using Hangfire.SqlServer;

using FluentValidation;
using FluentValidation.AspNetCore;

using WeatherApp.Services;
using WeatherApp.Models;

namespace WeatherApp
{
    abstract public class WeatherAppStartup
    {
        public WeatherAppOptions config;
    }

    public class Startup : WeatherAppStartup
    {
        public IConfigurationRoot Configuration { get; }

        public Startup(IHostingEnvironment env)
        {

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();

            config = new WeatherAppOptions(Configuration);
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddHangfire(options => 
                    options.UseSqlServerStorage(config.WeatherApp.hangFireConnectionString));

            services.AddMvc()
                    .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Startup>());
/*
            services
                .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining(typeof(FluentValidationExtention)));
*/

            services.AddDbContext<WeatherAppContext>(options =>
                    options.UseSqlServer(config.WeatherApp.dbConnectionString));

            services.AddScoped<IWeatherProviders, WeatherProviders>();
            services.AddScoped<IUserAgent, UserAgent>();

            services.AddSingleton<IWeatherAppOptions>(config);
            services.AddSingleton<IConfigurationRoot>(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IServiceProvider serviceProvider)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseHangfireDashboard();
            app.UseHangfireServer();

            var dbContext = serviceProvider.GetService<WeatherAppContext>();
            dbContext.Database.EnsureCreated();
//            dbContext.Database.Migrate();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute("getLocations", "api/getLocations", new { controller = "Home", action = "getLocations" });
                routes.MapRoute("setLocation" , "api/setLocation" , new { controller = "Home", action = "setLocation" });
                routes.MapRoute("getHistory"  , "api/getHistory"  , new { controller = "Home", action = "getHistory"   });

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}

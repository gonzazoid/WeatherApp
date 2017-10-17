using System;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace WeatherApp.Models{

    public class WeatherAppContext : DbContext
    {
        public DbSet<Place> places { get; set; }
        public DbSet<State> weather_states { get; set; }

        public WeatherAppContext(DbContextOptions<WeatherAppContext> options)
            : base(options)
        { }

        public WeatherAppContext()
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Place>()
                .HasIndex(x => x.Title)
                .IsUnique();

            modelBuilder.Entity<State>()
                .HasIndex(x => x.PlaceId);
        }
    }

    public class Place
    {
        // type 'uint' not supported by current database provider
        public int PlaceId { get; set; }

        public string Title { get; set; }
        public string OpenWeatherProvidersAlias { get; set; }
        public string YahooProvidersAlias { get; set; }

        // cascade deleting
        public virtual List<State> states { get; set; }

        public Place(){
            states = new List<State>();
        }

        public Place(string name, string prov1, string prov2){
            states = new List<State>();
            Title = name;
            OpenWeatherProvidersAlias = prov1;
            YahooProvidersAlias = prov2;          
        }
    }

    public class State
    {
        public int StateId { get; set; }

        public int stamp { get; set; }
        public int PlaceId { get; set; }
        public float temp { get; set; }
        public float pressure { get; set; }
        public string provider { get; set; }

        public State(){

        }
    }
}

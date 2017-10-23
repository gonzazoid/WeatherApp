namespace WeatherApp.Models {

    public class LoggingSection
    {
        public bool IncludeScopes { get; set; }
        public LogLevelSection LogLevel { get; set; }
    }

    public class LogLevelSection
    {
        public string Default { get; set; }
    }

    public class WeatherAppSection
    {
        public string dbConnectionString { get; set; }
        public string hangFireConnectionString { get; set; }
        public string cronFormatRequestRate { get; set; }
        public string openWeatherRequestUrl { get; set; }
        public string openWeatherApiId { get; set; }
        public string yahooRequestUrl { get; set; }
        public string defaultProvider { get; set; }
    }
}

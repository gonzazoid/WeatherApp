using System;

namespace WeatherApp
{
	public class WeatherAppNotValidProviderResponseException : Exception
	{
	    public WeatherAppNotValidProviderResponseException(string message)
            : base(message)
	    {
	    }
	}
}

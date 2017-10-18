using System;

namespace WeatherApp
{
	public class WeatherAppNotValidProviderResponseException : WeatherAppException
	{
	    public WeatherAppNotValidProviderResponseException(string message)
            : base(message)
	    {
	    }
	}
}

using System;

namespace WeatherApp
{
	public class WeatherAppException : Exception
	{
	    public WeatherAppException(string message)
            : base(message)
	    {
	    }
	}
}

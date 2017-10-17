using System;
using System.Threading.Tasks;

using NUnit.Framework.Constraints;
using Moq;

using WeatherApp;
using WeatherApp.Models;
using WeatherApp.Services;

namespace NUnit.Framework.Tests
{
    public class WeatherProvidersTests
    {
        // тест - передаем конфиг и проверяем что собралась правильная строка для UA
        // так как проверяем через мок(а не стаб) - покрываем и взаимодействие с UA - то есть сам факт вызова метода GetResponseAsync
        [Test]
        async public Task checkPlaceOpenWeatherAsync_Interaction_BuildingRequestURL_ReturnBuildedURLString()
        {
            string requestedUrl = null;
            Mock<IWeatherAppOptions> stubOptions = new Mock<IWeatherAppOptions>() { DefaultValue = DefaultValue.Mock };
            stubOptions.Object.WeatherApp.openWeatherRequestUrl = "url:{0}:{1}";
            stubOptions.Object.WeatherApp.openWeatherApiId = "api";

            Mock<IUserAgent> mockUserAgent = new Mock<IUserAgent>();
            mockUserAgent.Setup(x => x.GetResponseAsync(It.IsAny<string>()))
                         .ReturnsAsync("")
                         .Callback<string>(url => requestedUrl = url);

            Place stubPlace = new Place() { OpenWeatherProvidersAlias = "city" };

            WeatherProviders testWeatherProviders = new WeatherProviders(stubOptions.Object, mockUserAgent.Object);
            // дальше код поломается но это не важно, исключения не проверяем, просто глушим
            try{
                await testWeatherProviders.checkPlaceOpenWeatherAsync(stubPlace);
            }catch(Exception e){

            }

            Assert.AreEqual(requestedUrl, "url:city:api");
        }

        // тест - проверяем что ИСПОЛЬЗУЕТСЯ валидатор (это не тест валидатора, это тест на то что он есть и бросает исключение)
        [Test]
        async public Task checkPlaceOpenWeatherAsync_Unit_HandleNotValidResponse_CatchException()
        {
            Mock<IWeatherAppOptions> stubOptions = new Mock<IWeatherAppOptions>() { DefaultValue = DefaultValue.Mock };
            stubOptions.Object.WeatherApp.openWeatherRequestUrl = "";
            stubOptions.Object.WeatherApp.openWeatherApiId = "";

            Mock<IUserAgent> stubUserAgent = new Mock<IUserAgent>();
            stubUserAgent.Setup(x => x.GetResponseAsync(It.IsAny<string>()))
                         .ReturnsAsync("{\"name\": \"\", \"main\": {\"pressure\": 0}}");

            Place stubPlace = new Place() { OpenWeatherProvidersAlias = "" };

            WeatherProviders testWeatherProviders = new WeatherProviders(stubOptions.Object, stubUserAgent.Object);
			Assert.ThrowsAsync<WeatherAppNotValidProviderResponseException>(() => testWeatherProviders.checkPlaceOpenWeatherAsync(stubPlace));

        }

        [Test]
        async public Task checkPlaceOpenWeatherAsync_Interaction_HandleValidResponse_ReturnStateToSaveItInStore()
        {
            Mock<IWeatherAppOptions> stubOptions = new Mock<IWeatherAppOptions>() { DefaultValue = DefaultValue.Mock };
            stubOptions.Object.WeatherApp.openWeatherRequestUrl = "";
            stubOptions.Object.WeatherApp.openWeatherApiId = "";

            Mock<IUserAgent> stubUserAgent = new Mock<IUserAgent>();
            stubUserAgent.Setup(x => x.GetResponseAsync(It.IsAny<string>()))
                         .ReturnsAsync("{\"name\": \"\", \"main\": {\"temp\": 0, \"pressure\": 0}}");

            Place stubPlace = new Place() { OpenWeatherProvidersAlias = "" };

            WeatherProviders testWeatherProviders = new WeatherProviders(stubOptions.Object, stubUserAgent.Object);
            State test_result = await testWeatherProviders.checkPlaceOpenWeatherAsync(stubPlace);

            Assert.True(test_result.temp == 0 && test_result.pressure == 0 && test_result.provider == "openWeather");
        }
    }
}

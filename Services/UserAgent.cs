using System;
using System.IO;
using System.Net;

using Microsoft.AspNetCore.Hosting.Builder;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.Configuration;

using System.Threading.Tasks;
using WeatherApp.Models;

namespace WeatherApp.Services {

    public interface IUserAgent
    {
        Task<string> GetResponseAsync(string url);
    }

    public class UserAgent : IUserAgent
    {
        async public Task<string> GetResponseAsync(string url)
        {
            try{
                WebRequest request = WebRequest.Create(url);
                WebResponse response = await request.GetResponseAsync();
                Stream receiveStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(receiveStream);
                return reader.ReadToEnd();
            } catch (WebException ex) {
                // TODO what to do to handle exception? reconnect, write to logs, something else?
                return "";
            }
        }
    }
}

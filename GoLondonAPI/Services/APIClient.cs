using System;
using System.Text.Json;
using GoLondonAPI.Data;
using GoLondonAPI.Domain.Enums;
using GoLondonAPI.Domain.Services;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;

namespace GoLondonAPI.Services
{
    public class APIClient : IAPIClient
    {
        private readonly AppSettings _appSettings;

        public APIClient(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        private readonly RestClient _tflClient = new("https://api.tfl.gov.uk/");
        private readonly RestClient _mapClient = new("https://api.mapbox.com/geocoding/v5/mapbox.places/");

        public async Task<T> PerformAsync<T>(APIClientType type, string urlPath, string contentType = "application/json", Method method = Method.Get)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(await PerformAsync(type, urlPath, contentType, method));
            }
            catch
            {
                return default(T);
            }
        }

        public async Task<string> PerformAsync(APIClientType type, string urlPath, string contentType = "application/json", Method method = Method.Get)
        {
            RestClient client = type == APIClientType.MAPBOX ? _mapClient : _tflClient;

            var request = new RestRequest(urlPath, method);
            request.AddHeader("Content-Type", contentType);

            if (type == APIClientType.TFL)
            {
                request.AddParameter("app_id", _appSettings.TflAPIKey);
                request.AddParameter("app_key", _appSettings.TflAPPKey);
            }
            else
            {
                request.AddParameter("access_token", _appSettings.MapboxAPIKey);
            }

            Console.WriteLine(client.BuildUri(request));

            RestResponse response = await client.ExecuteAsync(request);
            return response.Content;
        }
    }
}


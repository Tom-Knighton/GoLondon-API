using System;
using GoLondonAPI.Domain.Enums;
using RestSharp;

namespace GoLondonAPI.Domain.Services
{
    public interface IAPIClient
    {
        Task<T> PerformAsync<T>(APIClientType type, string urlPath, string contentType = "application/json", Method method = Method.Get);
        Task<string> PerformAsync(APIClientType type, string urlPath, string contentType = "application/json", Method method = Method.Get);
    }
}


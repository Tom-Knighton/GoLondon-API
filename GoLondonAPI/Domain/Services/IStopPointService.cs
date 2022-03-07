using System;
using GoLondonAPI.Domain.Enums;
using GoLondonAPI.Domain.Models;
using RestSharp;

namespace GoLondonAPI.Domain.Services
{
    public interface IStopPointService
    {
        Task<List<StopPoint>> GetStopPointsByIdsAsync(string[] ids);
        Task<List<StopPointArrivalLineGroup>> GetEstimatedArrivalsForAsync(string stopPointId);
        Task<List<StopPoint>> SearchStopPointsAsync(string search, int maxResults = 0);
        Task<List<StopPoint>> SearchStopPointsAsync(string search, List<LineMode> filters, int maxResults = 0);
        Task<List<string>> DisambiguateStopPoint(string stopPointId);
    }
}


using System;
using GoLondonAPI.Domain.Enums;
using GoLondonAPI.Domain.Models;
using GoLondonAPI.Domain.Services;
using Newtonsoft.Json;

namespace GoLondonAPI.Services
{
    public class StopPointService : IStopPointService
    {
        private readonly IAPIClient _apiClient;

        public StopPointService(IAPIClient apiClient) => _apiClient = apiClient;

        public async Task<List<StopPoint>> GetStopPointsByIdsAsync(string[] ids)
        {
            string respContent = await _apiClient.PerformAsync(APIClientType.TFL, $"StopPoint/{string.Join(',', ids)}");
            try
            {
                List<StopPoint> stopPoints = JsonConvert.DeserializeObject<List<StopPoint>>(respContent);
                if (stopPoints == null)
                {
                    stopPoints = new List<StopPoint> { JsonConvert.DeserializeObject<StopPoint>(respContent) };
                }
                return stopPoints;
            }
            catch
            {
                StopPoint stopPoint = JsonConvert.DeserializeObject<StopPoint>(respContent);
                if (stopPoint == null || stopPoint.lat == 0)
                {
                    return new List<StopPoint>();
                }
                return new List<StopPoint> { stopPoint };
            }
            
        }

        public async Task<List<StopPointArrivalLineGroup>> GetEstimatedArrivalsForAsync(string stopPointId)
        {
            StopPoint stopPoint = (await GetStopPointsByIdsAsync(new string[] { stopPointId })).FirstOrDefault();
            if (stopPoint == null)
            {
                return null;
            }

            List<StopPointArrival> arrivals = await _apiClient.PerformAsync<List<StopPointArrival>>(APIClientType.TFL, $"StopPoint/{stopPointId}/Arrivals");
            return arrivals.GetGroupedArrivals();
        }

        public async Task<List<StopPoint>> SearchStopPointsAsync(string search, int maxResults = 0)
        {
            return await SearchStopPointsAsync(search, new List<LineMode>(), maxResults);
        }

        public async Task<List<StopPoint>> SearchStopPointsAsync(string search, List<LineMode> filters, int maxResults = 0)
        {
            string query = $"?query={search}{(filters.Count() == 0 ? "" : $"&modes={string.Join(",", filters.Select(m => m.GetValue()).ToArray())}")}{(maxResults != 0 ? $"&maxResults={maxResults}" : "")}";
            StopPointSearchResult stopPoints = await _apiClient.PerformAsync<StopPointSearchResult>(APIClientType.TFL, $"StopPoint/Search{query}");
            return stopPoints.matches;
        }

        public async Task<List<string>> DisambiguateStopPoint(string stopPointId)
        {
            List<string> points = new List<string>();
            if (stopPointId.Contains(","))
            {
                points = new List<string> { stopPointId };
            }
            else
            {
                StopPoint stopPoint = (await GetStopPointsByIdsAsync(new string[] { stopPointId })).FirstOrDefault();
                if (stopPoint == null)
                {
                    return new List<string>();
                }

                points = stopPoint.childStationIds;
            }

            return points;
        }
    }
}


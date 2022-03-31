using System;
using GoLondonAPI.Domain.Enums;
using GoLondonAPI.Domain.Models;
using GoLondonAPI.Domain.Services;
using System.Linq;

namespace GoLondonAPI.Services
{
    public class SearchService : ISearchService
    {

        private readonly IAPIClient _apiClient;

        public SearchService(IAPIClient apiClient) => _apiClient = apiClient;

        public async Task<IEnumerable<Point>> SearchAsync(string query, bool includePOI = false, bool includeAddresses = false)
        {
            return await SearchAsync(query, new List<LineMode>(), includePOI, includeAddresses);
        }

        public async Task<IEnumerable<Point>> SearchAsync(string query, List<LineMode> filters, bool includePOI = false, bool includeAddresses = false)
        {
            List<Point> results = new List<Point>();
            results.AddRange(await SearchStopPointsAsync(query, filters));

            if (includePOI || includeAddresses)
            {
                List<POIPoint> poiPoints = await SearchPOIPointsAsync(query, includePOI, includeAddresses);
                results.AddRange(poiPoints);
            }

            return results;
        }

        private async Task<List<StopPoint>> SearchStopPointsAsync(string query, List<LineMode> filters)
        {
            string queries = $"?query={query}{(filters.Count() == 0 ? "" : $"&modes={string.Join(",", filters.Select(m => m.GetValue()).ToArray())}")}";
            StopPointSearchResult res = await _apiClient.PerformAsync<StopPointSearchResult>(APIClientType.TFL, $"StopPoint/Search{queries}");
            return res.matches ?? new List<StopPoint>();
        }

        private async Task<List<POIPoint>> SearchPOIPointsAsync(string query, bool includePOI, bool includeAddresses)
        {
            string types = $"{(includePOI? "poi," : "")}{(includeAddresses ? "place,postcode,address" : "")}";
            POIPointSearchResult res = await _apiClient.PerformAsync<POIPointSearchResult>(APIClientType.MAPBOX, $"{query}.json?country=gb&limit=10&types={types}");
            return res.features ?? new List<POIPoint>(); ;
        }

        public async Task<IEnumerable<StopPoint>> SearchAroundAsync(float lat, float lon, float radius)
        {
            return await SearchAroundAsync(lat, lon, new List<LineMode>(), radius);
        }

        public async Task<IEnumerable<StopPoint>> SearchAroundAsync(float lat, float lon, List<LineMode> filters, float radius)
        {
            string modes = string.Join(",", filters.Select(m => m.GetValue()));
            string query = $"?lat={lat}&lon={lon}&stoptypes=NaptanMetroStation,NaptanRailStation,NaptanBusCoachStation,NaptanFerryPort,NaptanPublicBusCoachTram&modes={(filters.Count == 0 ? "" : modes)}&radius={radius}&useStopPointHierarchy=false";
            return (await _apiClient.PerformAsync<StopPointSearchAroundResult>(APIClientType.TFL, $"StopPoint{query}")).stopPoints ?? new List<StopPoint>();
        }
    }
}

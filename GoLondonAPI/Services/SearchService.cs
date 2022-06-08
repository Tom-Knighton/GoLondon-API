using System;
using GoLondonAPI.Domain.Enums;
using GoLondonAPI.Domain.Models;
using GoLondonAPI.Domain.Services;
using System.Linq;
using GoLondonAPI.Data;

namespace GoLondonAPI.Services
{
    public class SearchService : ISearchService
    {

        private readonly IAPIClient _apiClient;

        public SearchService(IAPIClient apiClient) => _apiClient = apiClient;

        public async Task<IEnumerable<Point>> SearchAsync(string query, bool includePOI = false, bool includeAddresses = false, bool useHeirarchy = false)
        {
            return await SearchAsync(query, new List<LineMode>(), includePOI, includeAddresses, useHeirarchy);
        }

        public async Task<IEnumerable<Point>> SearchAsync(string query, List<LineMode> filters, bool includePOI = false, bool includeAddresses = false, bool useHeirarchy = false)
        {
            List<Point> results = new List<Point>();
            results.AddRange(await SearchStopPointsAsync(query, filters, useHeirarchy));

            if (includePOI || includeAddresses)
            {
                List<POIPoint> poiPoints = await SearchPOIPointsAsync(query, includePOI, includeAddresses);
                results.AddRange(poiPoints);
            }

            return results;
        }

        private async Task<List<StopPoint>> SearchStopPointsAsync(string query, List<LineMode> filters, bool useHeirarchy = false)
        {
            string queries = $"?query={query}{(filters.Count() == 0 ? "" : $"&modes={string.Join(",", filters.Select(m => m.GetValue()).ToArray())}")}";
            StopPointSearchResult res = await _apiClient.PerformAsync<StopPointSearchResult>(APIClientType.TFL, $"StopPoint/Search{queries}&useStopPointHierarchy=true");
            List<StopPoint> points = res.matches ?? new List<StopPoint>();
            points = useHeirarchy ? points : DeconstructHeirarchy(points);
            return Global.AddCachedLineModeGroups(points);
        }

        private async Task<List<POIPoint>> SearchPOIPointsAsync(string query, bool includePOI, bool includeAddresses)
        {
            string types = $"{(includePOI ? "poi," : "")}{(includeAddresses ? "place,postcode,address" : "")}";
            POIPointSearchResult res = await _apiClient.PerformAsync<POIPointSearchResult>(APIClientType.MAPBOX, $"{query}.json?country=gb&limit=10&types={types}");
            return res.features ?? new List<POIPoint>(); ;
        }

        public async Task<IEnumerable<StopPoint>> SearchAroundAsync(float lat, float lon, float radius, bool useHierarchy = false)
        {
            return await SearchAroundAsync(lat, lon, new List<LineMode>(), radius);
        }

        public async Task<IEnumerable<StopPoint>> SearchAroundAsync(float lat, float lon, List<LineMode> filters, float radius, bool useHierarchy = false)
        {
            string modes = string.Join(",", filters.Select(m => m.GetValue()));
            string query = $"?lat={lat}&lon={lon}&stoptypes=NaptanMetroStation,NaptanRailStation,NaptanBusCoachStation,NaptanFerryPort,NaptanPublicBusCoachTram&modes={(filters.Count == 0 ? "" : modes)}&radius={radius}&useStopPointHierarchy=true";
            StopPointSearchAroundResult res = await _apiClient.PerformAsync<StopPointSearchAroundResult>(APIClientType.TFL, $"StopPoint{query}");
            List<StopPoint> points = res.stopPoints ?? new List<StopPoint>();

            return useHierarchy ? points : DeconstructHeirarchy(points);
        }

        private List<StopPoint> DeconstructHeirarchy(List<StopPoint> points)
        {
            List<StopPoint> results = new();
            points.ForEach(p =>
            {
                List<StopPoint> busChildren = p.children?.Where(c => c.lineModes?.Count == 1 && c.lineModes.First() == LineMode.bus).ToList();
                if (busChildren?.Any() == true)
                {         
                    busChildren.ForEach(bc =>
                    {
                        LineModeGroup lineModeGroup = new LineModeGroup();
                        List<string> identifiers = p.lineGroups?.FirstOrDefault(c => c.naptanIdReference == bc.naptanId)?.lineIdentifier?.ToList() ?? new List<string>();
                        lineModeGroup.modeName = LineMode.bus;
                        lineModeGroup.lineIdentifier = identifiers;
                        bc.lineModeGroups = new List<LineModeGroup> { lineModeGroup };
                        results.Add(bc);
                        p.children?.RemoveAll(c => c.naptanId == bc.naptanId);

                        p.children?.Remove(bc);
                        p.modes.RemoveAll(lm => lm == LineMode.bus.GetValue());
                        p.lineModeGroups.RemoveAll(lmg => lmg.modeName == LineMode.bus);
                    });
                }
                results.Add(p);
            });

            return results;
        }
    }
}

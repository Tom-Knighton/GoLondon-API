using System;
using GoLondonAPI.Data;
using GoLondonAPI.Domain.Enums;
using GoLondonAPI.Domain.Models;
using GoLondonAPI.Domain.Services;
using Newtonsoft.Json;

namespace GoLondonAPI.Services
{
    public class LineService : ILineService
    {
        private readonly IAPIClient _apiClient;

        public LineService(IAPIClient apiClient) => _apiClient = apiClient;

        public async Task<List<Disruption>> GetDisruptionsAsync(LineMode mode)
        {
            return await _apiClient.PerformAsync<List<Disruption>>(APIClientType.TFL, $"Line/Mode/{mode.GetValue()}/Disruption") ?? new List<Disruption>();
        }

        public async Task<List<Line>> GetLineInfo(List<LineMode> modes, bool includeDetail = false)
        {
            string[] types = modes.Select(m => m.GetValue()).ToArray();
            return await _apiClient.PerformAsync<List<Line>>(APIClientType.TFL, $"Line/Mode/{string.Join(",", types)}/status?detail={includeDetail}") ?? new List<Line>();
        }

        public async Task<List<Line>> GetLineInfo(List<string> lineIds, bool includeDetail = false)
        {
            return await _apiClient.PerformAsync<List<Line>>(APIClientType.TFL, $"Line/{string.Join(",", lineIds)}/status?detail={includeDetail}") ?? new List<Line>();
        }

        public async Task<LineModeGroupStatusType> GetGeneralLineStatus(List<LineMode> lineModes)
        {
            List<Line> lineInfo = await GetLineInfo(lineModes, false);

            float numOfGood = lineInfo.Where(l => l.currentStatus.statusSeverity == 10).Count();
            float percentageGood = (numOfGood / lineInfo.Count) * 100;

            LineModeGroupStatusType toReturn = LineModeGroupStatusType.unk;

            if (percentageGood == 100)
            {
                toReturn = LineModeGroupStatusType.allGood;
            }
            else if (percentageGood >= 90)
            {
                toReturn = LineModeGroupStatusType.mostGood;
            }
            else if (percentageGood < 90 && percentageGood >= 40)
            {
                toReturn = LineModeGroupStatusType.someBad;
            }
            else if (percentageGood < 40 && percentageGood > 0)
            {
                toReturn = LineModeGroupStatusType.manyBad;
            }
            else
            {
                toReturn = LineModeGroupStatusType.allBad;
            }

            return toReturn;
            
        }

        public async Task<List<LineRoutes>> GetRoutesForLines(string[] lineIdentifiers, bool fixCoordinates = true)
        {
            List<LineRoutes> routes = Global.cachedLineRoutes.Where(l => lineIdentifiers.Contains(l.lineId)).ToList();

            if (routes.Count != lineIdentifiers.Length)
            {
                List<Task<LineRoutes>> tasksToDo = new();
                foreach (string id in lineIdentifiers.Where(id => !routes.Any(r => r.lineId == id)))
                {

                    tasksToDo.Add(_apiClient.PerformAsync<LineRoutes>(APIClientType.TFL, $"Line/{id}/Route/sequence/outbound"));
                }

                routes.AddRange((await Task.WhenAll(tasksToDo)).ToList());
                routes.ForEach(r =>
                {
                    Global.AddLineRouteToCache(r);
                });
            }
            
            if (!fixCoordinates)
            {
                return routes;
            }

            // Go through routes and fix coordinates for stop points
            // Make sure stop points that have same id, have the same coordinate
            string serialised = JsonConvert.SerializeObject(routes);
            routes = JsonConvert.DeserializeObject<List<LineRoutes>>(serialised);
            Dictionary<string, Tuple<float, float>> cachedCoords = new();

            routes!.ForEach(route =>
            {
                route.stopPointSequences.ToList().ForEach(branch =>
                {
                    branch.stopPoint.ToList().ForEach(sp =>
                    {
                        Tuple<float, float> coords = new(sp.lat, sp.lon);
                        string key = sp.name.StartsWith("Custom House") ? "Custom House" : sp.id;
                        if (cachedCoords.ContainsKey(key))
                        {
                            if ((coords.Item1 != cachedCoords[key].Item1) || (coords.Item2 != cachedCoords[key].Item2))
                            {
                                sp.lat = cachedCoords[key].Item1;
                                sp.lon = cachedCoords[key].Item2;
                            }
                        }
                        else
                        {
                            cachedCoords.Add(key, new(sp.lat, sp.lon));
                        }
                    });
                });
            });

            return routes;
        }
    }
}


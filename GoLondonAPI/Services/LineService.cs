using System;
using GoLondonAPI.Domain.Enums;
using GoLondonAPI.Domain.Models;
using GoLondonAPI.Domain.Services;

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

        public async Task<List<LineRoutes>> GetRoutesForLines(string[] lineIdentifiers)
        {
            List<LineRoutes> routes = new();
            foreach (string id in lineIdentifiers)
            {
                LineRoutes query = await _apiClient.PerformAsync<LineRoutes>(APIClientType.TFL, $"Line/{id}/Route/sequence/outbound");
                routes.Add(query);
            }

            return routes;
        }
    }
}


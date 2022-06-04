using System;
using System.Xml;
using System.Xml.Serialization;
using GoLondonAPI.Data;
using GoLondonAPI.Domain.Enums;
using GoLondonAPI.Domain.Models;
using GoLondonAPI.Domain.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GoLondonAPI.Services
{
    public class MetaService : IMetaService
    {
        private readonly IAPIClient _apiClient;

        public MetaService(IAPIClient api) => _apiClient = api;

        public List<DisruptionDelayType> GetDelayTypes()
        {
            return Enum.GetValues<DisruptionDelayType>().ToList();
        }

        public List<LineMode> GetLineModes()
        {
            return Enum.GetValues<LineMode>().ToList();
        }

        public async Task<List<string>> GetAllLineIdsAsync(List<LineMode> modes)
        {
            string[] types = modes.Select(m => m.GetValue()).ToArray();
            List<Line> lines = await _apiClient.PerformAsync<List<Line>>(APIClientType.TFL, $"Line/Mode/{string.Join(",", types)}");
            return lines.Select(l => l.id).ToList();
        }

        public async Task<string> SyncWithTfl()
        {
            HashSet<InternalLineGroupICS> grouped = new HashSet<InternalLineGroupICS>();

            List<StopPoint> points = await _apiClient.PerformAsync<List<StopPoint>>(APIClientType.TFL, "StopPoint/Type/NaptanMetroStation");

            foreach (StopPoint tube in points.Where(t => !string.IsNullOrEmpty(t.icsCode)))
            {
                if (grouped.Any(g => g.ICSCode == tube.icsCode))
                {
                    grouped.First(g => g.ICSCode == tube.icsCode || g.ICSCode == tube.icsId).lineModes.AddRange(tube.lineModeGroups);
                }
                else
                {
                    grouped.Add(new InternalLineGroupICS { ICSCode = tube.icsCode ?? tube.icsId, lineModes = tube.lineModeGroups });
                }
            }

            Global.cachedLMGs = grouped;

            return "Recv: " + Global.cachedLMGs.Count.ToString();
        }
    }
}


using System;
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
    }
}


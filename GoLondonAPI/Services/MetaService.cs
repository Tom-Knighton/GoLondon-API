using System;
using AspNetCoreRateLimit;
using GoLondonAPI.Data;
using GoLondonAPI.Domain.Entities;
using GoLondonAPI.Domain.Enums;
using GoLondonAPI.Domain.Models;
using GoLondonAPI.Domain.Services;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GoLondonAPI.Services
{
    public class MetaService : IMetaService
    {
        private readonly IAPIClient _apiClient;
        private readonly IProjectService _projects;
        private readonly ClientRateLimitOptions _options;
        private readonly IClientPolicyStore _clientPolicyStore;

        public MetaService(IAPIClient api, IProjectService projects, IOptions<ClientRateLimitOptions> opts, IClientPolicyStore store)
        {
            _apiClient = api;
            _projects = projects;
            _options = opts.Value;
            _clientPolicyStore = store;
        }

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

        public async Task SyncLimits()
        { 
            List<Project> projects = (await _projects.GetProjectsAsync(includeUsers: true)).ToList();
            foreach (Project project in projects)
            {
                Role role = project.User.Role.Role;
                string id = $"{_options.ClientPolicyPrefix}_{project.APIKey}";
                ClientRateLimitPolicy policy = await _clientPolicyStore.GetAsync(id);
                await _clientPolicyStore.SetAsync(id, new ClientRateLimitPolicy
                {
                    ClientId = project.APIKey,
                    Rules = new List<RateLimitRule>
                    {
                         new RateLimitRule
                         {
                              Endpoint = "*",
                              Period = "1m",
                              Limit = role.CallsPerMin,
                         }
                     }
                });
            }
        }
    }
}


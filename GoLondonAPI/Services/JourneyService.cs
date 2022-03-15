using System;
using GoLondonAPI.Domain.Enums;
using GoLondonAPI.Domain.Models;
using GoLondonAPI.Domain.Services;

namespace GoLondonAPI.Services
{
    public class JourneyService : IJourneyService
    {
        private readonly IAPIClient _apiClient;
        private readonly IStopPointService _stopPointService;

        public JourneyService(IAPIClient apiClient, IStopPointService stopPointService)
        {
            _apiClient = apiClient;
            _stopPointService = stopPointService;
        }

        public async Task<List<Journey>> GetPossibleJourneys(string from, string to, string? via = null, DateTime? time = null, JourneyDateType? dateType = null)
        {
            List<Journey> journeys = new();

            List<string> toPoints = await _stopPointService.DisambiguateStopPoint(to);
            List<string> fromPoints = await _stopPointService.DisambiguateStopPoint(from);

            List<Tuple<string, string>> allOptions = (from fromPoint in fromPoints from toPoint in toPoints select new Tuple<string, string>(fromPoint, toPoint)).ToList();

            string queryParams = $"?nationalSearch=true&useMultiModalCall=true&useRealTimeLiveArrivals=true&alternativeCycle=true&alternativeWalking=true{(string.IsNullOrEmpty(via) ? "" : $"&via={via}")}";
            if (time != null)
            {
                queryParams += $"&date={time?.ToString("yyyyMMdd")}&time={time?.ToString("HHmm")}&timeIs={(dateType == null ? "departing" : dateType == JourneyDateType.arriveAt ? "arriving" : "departing" )}";
            }

            List<Task<JourneySearchResult>> tasksToDo = new();
            foreach(Tuple<string, string> opts in allOptions)
            {
                tasksToDo.Add(_apiClient.PerformAsync<JourneySearchResult>(APIClientType.TFL, $"Journey/JourneyResults/{opts.Item1}/to/{opts.Item2}{queryParams}"));
            }
            List<JourneySearchResult> results = (await Task.WhenAll(tasksToDo)).ToList();

            foreach(JourneySearchResult res in results)
            {
                journeys.AddRange(res.journeys ?? new List<Journey>());
            }

            return journeys.Where(j => j.startDateTime >= DateTime.UtcNow).OrderBy(j => j.arrivalDateTime).Distinct(new Journey()).ToList();
        }
    }
}


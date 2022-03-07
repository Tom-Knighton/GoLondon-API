﻿using System;
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
            List<Journey> journeys = new List<Journey>();

            List<string> toPoints = await _stopPointService.DisambiguateStopPoint(to);
            List<string> fromPoints = await _stopPointService.DisambiguateStopPoint(from);

            List<Tuple<string, string>> allOptions = (from fromPoint in fromPoints from toPoint in toPoints select new Tuple<string, string>(fromPoint, toPoint)).ToList();

            List<Task<JourneySearchResult>> tasksToDo = new List<Task<JourneySearchResult>>();
            foreach(Tuple<string, string> opts in allOptions)
            {
                tasksToDo.Add(_apiClient.PerformAsync<JourneySearchResult>(APIClientType.TFL, $"Journey/JourneyResults/{opts.Item1}/to/{opts.Item2}"));
            }
            List<JourneySearchResult> results = (await Task.WhenAll(tasksToDo)).ToList();

            foreach(JourneySearchResult res in results)
            {
                journeys.AddRange(res.journeys);
            }

            return journeys.OrderBy(j => j.duration).ToList();
        }
    }
}


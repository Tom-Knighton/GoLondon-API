using System;
using GoLondonAPI.Domain.Enums;
using GoLondonAPI.Domain.Models;

namespace GoLondonAPI.Domain.Services
{
    public interface IJourneyService
    {
        Task<List<Journey>> GetPossibleJourneys(string from, string to, string? via = null, DateTime? time = null, JourneyDateType? dateType = null);
    }
}


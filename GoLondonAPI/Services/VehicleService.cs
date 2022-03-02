using System;
using GoLondonAPI.Domain.Models;
using GoLondonAPI.Domain.Services;

namespace GoLondonAPI.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly IAPIClient _apiClient;

        public VehicleService(IAPIClient apiClient) => _apiClient = apiClient;

        public async Task<List<StopPointArrival>> GetArrivalsForVehicle(string vehicleId)
        {
            return await _apiClient.PerformAsync<List<StopPointArrival>>(Domain.Enums.APIClientType.TFL, $"Vehicle/{vehicleId}/Arrivals");
        }
    }
}


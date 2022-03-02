using System;
using GoLondonAPI.Domain.Models;

namespace GoLondonAPI.Domain.Services
{
    public interface IVehicleService
    {
        Task<List<StopPointArrival>> GetArrivalsForVehicle(string vehicleId);
    }
}


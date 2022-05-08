using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoLondonAPI.Domain.Enums;
using GoLondonAPI.Domain.Models;
using GoLondonAPI.Domain.Services;
using GoLondonAPI.Middleware;
using Microsoft.AspNetCore.Mvc;

namespace GoLondonAPI.Controllers
{
    [APIKeyAuth]
    [Route("api/[controller]")]
    public class VehicleController : Controller
    {

        private readonly IVehicleService _vehicleService;

        public VehicleController(IVehicleService vehService)
        {
            _vehicleService = vehService;
        }

        /// <summary>
        /// Returns an ordered list of StopPointArrivals for a specified vehicle
        /// </summary>
        /// <param name="vehicleId">The id of the vehicle, i.e. the license plate of a bus</param>
        [HttpGet("{vehicleId}/Arrivals")]
        [Produces(typeof(List<StopPointArrival>))]
        public async Task<IActionResult> GetVehicleArrivals(string vehicleId)
        {
            return Ok(await _vehicleService.GetArrivalsForVehicle(vehicleId));
        }
    }
}


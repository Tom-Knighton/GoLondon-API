using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoLondonAPI.Domain.Enums;
using GoLondonAPI.Domain.Models;
using GoLondonAPI.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace GoLondonAPI.Controllers
{
    [Route("api/[controller]")]
    public class StopPointController : Controller
    {
        private readonly IStopPointService _stopPointService;

        public StopPointController(IStopPointService stopPointService) => _stopPointService = stopPointService;

        /// <summary>
        /// Returns detailed StopPoint information for all valid stop point ids, separated by commas
        /// </summary>
        /// <param name="ids">The ids (from StopPoint.id) of each stop point, comma separated</param>
        /// <returns>Array of StopPoint models</returns>
        [HttpGet("{ids}")]
        [Produces(typeof(List<StopPoint>))]
        public async Task<IActionResult> GetStopPointyId(string[] ids)
        {
            return Ok(await _stopPointService.GetStopPointsByIdsAsync(ids));
        }

        /// <summary>
        /// Returns all estimated arrivals for a specified StopPoint. Arrivals returned sorted by line, then platform and direction.
        /// </summary>
        /// <remarks>
        /// <para>
        /// All arrivals are returned sorted by the soonest to arrive. 
        /// </para>
        /// <para>
        /// If the stop point is a HUB, the method will return arrivals for each valid child
        /// </para>
        /// </remarks>
        /// <param name="stopId">The id of the stop point</param>
        /// <returns></returns>
        [HttpGet("EstimatedArrivals/{stopId}")]
        [Produces(typeof(List<StopPointArrivalLineGroup>))]
        public async Task<IActionResult> GetEstimatedArrivals(string stopId)
        {
            return Ok(await _stopPointService.GetEstimatedArrivalsForAsync(stopId));
        }
    }
}


using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class JourneyController : Controller
    {
        private readonly IJourneyService _journeyService;

        public JourneyController(IJourneyService journeyService) => _journeyService = journeyService;

        /// <summary>
        /// Returns a list of possible journeys that can be made from one Point to another
        /// </summary>
        /// <param name="from">Where to depart, can be stop point id, ICS id or coordinates as 'lat,long'</param>
        /// <param name="to">Where to arrive, can be stop point id, ICS id or coordinates as 'lat,long'</param>
        /// <param name="via">Optional: travel through point on the journey. Can be stop point id, ICS id or coordinates as 'lat,long'</param>
        /// <param name="time">Optional: A desired time to either arrive or depart at</param>
        /// <param name="timeType">Optional: Whethere the 'time' parameter is the arrive or depart time. Defaults to depart</param>
        /// <returns></returns>
        [HttpGet]
        [Produces(typeof(List<Journey>))]
        public async Task<IActionResult> GetJourneys([Required] string from, [Required] string to, string via, DateTime? time, [FromQuery] JourneyDateType? timeType = JourneyDateType.departAt)
        {
            if (string.IsNullOrEmpty(from) || string.IsNullOrEmpty(to))
            {
                return BadRequest("You must supply an arrival and destination point, in the form of a stop point id, ICS id, or coordinate as 'lat,lon'");
            }

            return Ok(await _journeyService.GetPossibleJourneys(from, to, via, time, timeType));
        }
    }
}


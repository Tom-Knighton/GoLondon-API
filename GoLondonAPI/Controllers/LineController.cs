using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using GoLondonAPI.Domain.Enums;
using GoLondonAPI.Domain.Models;
using GoLondonAPI.Domain.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GoLondonAPI.Controllers
{
    [Route("api/[controller]")]
    public class LineController : Controller
    {
        private readonly ILineService _lineService;

        public LineController(ILineService lineService) => _lineService = lineService;

        /// <summary>
        /// Returns a list of disruptions affecting the specified line mode
        /// </summary>
        /// <param name="mode">The line mode to search for disruptions on</param>
        [HttpGet("Disruptions")]
        [Produces(typeof(List<Disruption>))]
        public async Task<IActionResult> GetDisruptionsForMode([FromQuery] [Required]LineMode lineMode)
        {
            return Ok(await _lineService.GetDisruptionsAsync(lineMode));
        }

        /// <summary>
        /// Gets the statuses for all lines provided
        /// </summary>
        /// <param name="lineIds">A comma separated list of line ids</param>
        /// <param name="includeDetail">Whether to include details on the disruptions causing any delays or not</param>
        [HttpGet("GetLineInfo/{lineIds}")]
        [Produces(typeof(List<Line>))]
        public async Task<IActionResult> GetLineStatus(string[] lineIds, bool includeDetail = false)
        {
            return Ok(await _lineService.GetLineInfo(lineIds.ToList(), includeDetail));
        }

        /// <summary>
        /// Gets the statuses for all lines on the line modes provided
        /// </summary>
        /// <param name="lineModes">The line modes, comma separated, to get the statuses for</param>
        /// <param name="includeDetail">Whether to include details on the disruptions causing any delays or not</param>
        [HttpGet("GetLineInfoForModes")]
        [Produces(typeof(List<Line>))]
        public async Task<IActionResult> GetModeStatuses([Required][FromQuery] LineMode[] lineModes, bool includeDetail = false)
        {
            if (lineModes.Length == 0)
            {
                return BadRequest("You must enter at least one valid LineMode type.");
            }
            return Ok(await _lineService.GetLineInfo(lineModes.ToList(), includeDetail));
        }

        /// <summary>
        /// Gets the general status string description for a group of LineModes
        /// </summary>
        /// <remarks>This returns a set string from the <c>LineModeGroupStatusType</c> enum</remarks>
        /// <param name="lineModes">The general status of the line modes selected</param>
        [HttpGet("StatusOverview")]
        [Produces(typeof(LineModeGroupStatusType))]
        public async Task<IActionResult> GetGeneralStatus([Required][FromQuery] LineMode[] lineModes)
        {
            
            if (lineModes.Length == 0)
            {
                return BadRequest("You must enter at least one valid LineMode type.");
            }
            return Ok(await _lineService.GetGeneralLineStatus(lineModes.ToList()));
        }
    }
}

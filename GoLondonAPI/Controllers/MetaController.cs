using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoLondonAPI.Domain.Enums;
using GoLondonAPI.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GoLondonAPI.Controllers
{
    [Route("api/[controller]")]
    public class MetaController : Controller
    {
        private readonly IMetaService _metaService;

        public MetaController(IMetaService metaService) => _metaService = metaService;

        /// <summary>
        /// Returns the possible line modes, used for/in several Line endpoints and data
        /// </summary>
        [HttpGet("LineModes")]
        [Produces(typeof(List<LineMode>))]
        public IActionResult GetLineModes()
        {
            return Ok(_metaService.GetLineModes());
        }

        /// <summary>
        /// Gets a list of LineIds for a specified mode
        /// </summary>
        [HttpGet("LineIds")]
        [Produces(typeof(List<string>))]
        public async Task<IActionResult> GetAllLineIds([FromQuery] LineMode[] modes)
        {
            if (modes.Length == 0)
            {
                return BadRequest("You must enter at least one valid LineMode type");
            }
            return Ok(await _metaService.GetAllLineIdsAsync(modes.ToList()));
        }

        /// <summary>
        /// Returns the possible Delay Types
        /// </summary>
        [HttpGet("DelayTypes")]
        [Produces(typeof(List<DisruptionDelayType>))]
        public IActionResult GetDisruptionTypes()
        {
            return Ok(_metaService.GetDelayTypes());
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("Sync")]
        public async Task<IActionResult> SyncWithTfL()
        {
            return Ok(await _metaService.SyncWithTfl());
        }
    }
}


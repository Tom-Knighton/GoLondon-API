using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using GoLondonAPI.Domain.Enums;
using GoLondonAPI.Domain.Models;
using GoLondonAPI.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GoLondonAPI.Controllers
{
    [Route("api/[controller]")]
    public class SearchController : Controller
    {
        private readonly ISearchService _searchService;

        public SearchController(ISearchService searchService)
        {
            _searchService = searchService;
        }

        /// <summary>
        /// Returns a list of StopPoints around a specified location
        /// </summary>
        /// <param name="lat">The latitiude of the coordinate to search around</param>
        /// <param name="lon">The longitutde of the coordinate to search around</param>
        /// <param name="modesToFilterBy">Filter by a list of line modes, can be null for no filter</param>
        /// <param name="radius">The radius around the center to search, in metres, defaults to 200</param>
        /// <param name="useHierarchy">Whether or not to reorganise StopPoints into a heirarchy</param>
        /// <remarks>The TFL api appears to have trouble returning results with a radius above 1000</remarks>
        [HttpGet("Around/{lat}/{lon}")]
        [Produces(typeof(List<Point>))]
        public async Task<IActionResult> SearchStopPointsAround(float lat, float lon, List<LineMode> modesToFilterBy, float radius = 200, bool useHierarchy = false)
        {
            return Ok(await _searchService.SearchAroundAsync(lat, lon, modesToFilterBy, radius, useHierarchy));
        }

        /// <summary>
        /// Returns a list of StopPoints (and optionally, points of interest) filtered by their (partial) common name 
        /// </summary>
        /// <param name="query">The query to search for</param>
        /// <param name="modesToFilterBy">Filter by a list of line modes, can be null for no filter</param>
        /// <param name="includePOI">Whether or not to include points of interest, defaults to false</param>
        /// <param name="includeAddresses">Whether or not to include addresses and postcodes, defaults to false</param>
        /// <param name="useHierarchy">Whether or not to reorganise StopPoints into a heirarchy</param>
        [HttpGet("{query}")]
        [Produces(typeof(List<Point>))]
        public async Task<IActionResult> SearchStopPointsByName(string query, List<LineMode> modesToFilterBy, bool includePOI = false, bool includeAddresses = false, bool useHierarchy = false)
        {
            return Ok(await _searchService.SearchAsync(query, modesToFilterBy, includePOI, includeAddresses, useHierarchy));
        }
    }
}


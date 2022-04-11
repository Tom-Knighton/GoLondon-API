using System;
using GoLondonAPI.Domain.Enums;
using GoLondonAPI.Domain.Models;

namespace GoLondonAPI.Domain.Services
{
    public interface ISearchService
    {
        Task<IEnumerable<Point>> SearchAsync(string query, List<LineMode> filters, bool includePOI = false, bool includeAddresses = false, bool useHierarchy = false);
        Task<IEnumerable<Point>> SearchAsync(string query, bool includePOI = false, bool includeAddresses = false, bool useHierarchy = false);
        Task<IEnumerable<StopPoint>> SearchAroundAsync(float lat, float lon, List<LineMode> filters, float radius, bool useHierarchy = false);
        Task<IEnumerable<StopPoint>> SearchAroundAsync(float lat, float lon, float radius, bool useHierarchy = false);
    }
}


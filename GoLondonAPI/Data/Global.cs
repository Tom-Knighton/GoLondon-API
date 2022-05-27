using System;
using GoLondonAPI.Domain.Models;

namespace GoLondonAPI.Data
{
    public class Global
    {
        public static volatile HashSet<InternalLineGroupICS> cachedLMGs = new();

        public static HashSet<LineRoutes> cachedLineRoutes { get; private set; } = new();
        public static DateTime? lastRouteCacheTime { get; private set; }

        public static List<StopPoint> AddCachedLineModeGroups(List<StopPoint> toPoints)
        {
            foreach(StopPoint point in toPoints)
            {
                InternalLineGroupICS icsGroup = cachedLMGs.FirstOrDefault(c => c.ICSCode == point.icsId || c.ICSCode == point.icsCode);
                if (icsGroup != null)
                {
                    point.lineModeGroups = icsGroup.lineModes;
                }
            }

            return toPoints;
        }

        /// <summary>
        /// Updates the cached line routes and the last cached time
        /// </summary>
        /// <param name="routes">The routes to be set in the cache. This collection will overwrite any currently stored routes</param>
        public static void UpdateCachedLineRoutes(ICollection<LineRoutes> routes)
        {
            cachedLineRoutes = routes.ToHashSet();
            lastRouteCacheTime = DateTime.UtcNow;
        }

        /// <summary>
        /// Adds a line route to the cache. This will not overwrite the current cache, simply add a new line routes
        /// </summary>
        /// <param name="route">The line w/ routes to add. This method will silently fail if the line already exists in cache</param>
        public static void AddLineRouteToCache(LineRoutes route)
        {
            if (!cachedLineRoutes.Any(r => r.lineId == route.lineId))
            {
                cachedLineRoutes.Add(route);
            }
        }
    }
}


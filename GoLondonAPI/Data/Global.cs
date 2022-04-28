using System;
using GoLondonAPI.Domain.Models;

namespace GoLondonAPI.Data
{
    public class Global
    {
        public static volatile HashSet<InternalLineGroupICS> cachedLMGs = new HashSet<InternalLineGroupICS>();
        
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
    }
}


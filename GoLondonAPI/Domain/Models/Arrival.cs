using System;
namespace GoLondonAPI.Domain.Models
{
    public class StopPointArrival
    {
        public string vehicleId { get; set; }
        public string id { get; set; }
        public string lineId { get; set; }
        public string lineName { get; set; }
        public string platformName { get; set; }
        public string direction { get; set; }
        public string destinationName { get; set; }
        public int timeToStation { get; set; }
        public DateTime expectedArrival { get; set; } 
        public string currentLocation { get; set; }
        public string stationName { get; set; }
        public string towards { get; set; }
    }

    public class StopPointArrivalLineGroup
    {
        public string lineName { get; set; }
        public List<StopPointArrivalPlatformGroup> platformGroups { get; set; } = new List<StopPointArrivalPlatformGroup>();

        public void GroupArrivalsByPlatform(List<StopPointArrival> arrivals)
        {
            List<string> platforms = arrivals.Select(a => a.platformName).Distinct().ToList();
            platforms.ForEach(p =>
            {
                List<StopPointArrival> relevantArrivals = arrivals.Where(a => a.platformName == p).ToList();
                string[] platformComponents = p.Split("-");

                string direction = platformComponents[0].Trim();
                string platformName = platformComponents.Count() > 1 ? platformComponents[1].Trim() : "Check Station Boards";

                platformName = platformName.Replace(" Rail Station", "");
                platformName = platformName.Replace(" Underground Station", "");
                platformName = platformName.Replace(" DLR Station", "");
                platformGroups.Add(new StopPointArrivalPlatformGroup
                {
                    platformName = platformName,
                    direction = direction,
                    arrivals = relevantArrivals.OrderBy(a => a.timeToStation).ToList()
                });
            });
        }
    }

    public class StopPointArrivalPlatformGroup
    {
        public string platformName { get; set; }
        public string direction { get; set; }
        public List<StopPointArrival> arrivals { get; set; }
    }

    public static class StopPointArrivalExtensions
    {
        public static List<StopPointArrivalLineGroup> GetGroupedArrivals(this List<StopPointArrival> arrivals)
        {
            arrivals = arrivals.Where(a => a.destinationName != a.stationName || (a.destinationName == a.stationName && a.lineId != "london-overground")).ToList();
            List<StopPointArrivalLineGroup> groupedArrivals = new List<StopPointArrivalLineGroup>();
            string[] lines = arrivals.Select(a => a.lineName).Distinct().ToArray();

            foreach (string line in lines)
            {
                StopPointArrivalLineGroup lineGroup = new StopPointArrivalLineGroup { lineName = line };
                lineGroup.GroupArrivalsByPlatform(arrivals.Where(a => a.lineName == lineGroup.lineName).ToList());
                groupedArrivals.Add(lineGroup);
            }
            
            return groupedArrivals;
        }
    }
}


using System;
using Newtonsoft.Json;

namespace GoLondonAPI.Domain.Models
{
    public class Journey
    {
        public DateTime? startDateTime { get; set; }
        public DateTime? arrivalDateTime { get; set; }
        public int duration { get; set; }

        public List<JourneyLeg> legs { get; set; }
    }

    public class JourneyLeg
    {
        int duration { get; set; }
        public JourneyLegInstruction? instruction { get; set; }
        //public List<JourneyLegObstacle> obstacles { get; set; }

        public DateTime? departureTime { get; set; }
        public DateTime? arrivalTime { get; set; }
        public StopPoint departurePoint { get; set; }
        public StopPoint arrivalPoint { get; set; }

        public JourneyLegPath? path { get; set; }

        public List<JourneyLegRouteOption> routeOptions { get; set; }
        public JourneyMode? mode { get; set; }
        public List<Disruption> disruptions { get; set; }
        public bool isDisrupted { get; set; }
        public bool hasFixedLocations { get; set; }

    }

    public class JourneyLegPath
    {
        public string lineString { internal get; set; }
        public List<StopPoint> stopPoints { get; set; }
        public float[,] linePaths => JsonConvert.DeserializeObject<float[,]>(lineString);
    }

    public class JourneyLegInstruction
    {
        public string summary { get; set; }
        public string detailed { get; set; }
        //TODO: Steps?
    }

    public class JourneyLegRouteOption
    {
        public string name { get; set; }
        public List<string> directions { get; set; }
        public Line lineIdentifier { get; set; }
    }

    public class JourneyMode
    {
        public string id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
    }

    public class JourneyFare
    {
        public int totalCosts { get; set; }

        public List<JourneyFareStep> fares { get; set; }
        public List<JourneyFareCaveat> caveats { get; set; }
    }

    public class JourneyFareStep
    {
        public int lowZone { get; set; }
        public int highZone { get; set; }
        public int cost { get; set; }
        public string chargeProfileName { get; set; }
        public bool isHopperFare { get; set; }
        public string chargeLevel { get; set; }
        public int peak { get; set; }
        public int offPeak { get; set; }

    }

    public class JourneyFareCaveat
    {
        public string text { get; set; }
        public string type { get; set; }
    }

    public class JourneySearchResult
    {
        public List<Journey> journeys { get; set; }
    }
}


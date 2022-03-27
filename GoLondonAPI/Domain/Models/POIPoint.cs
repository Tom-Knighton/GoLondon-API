using System;
namespace GoLondonAPI.Domain.Models
{
    public class POIPoint : Point
    {
        public string id { get; set; }
        public string text { get; set; }
        public string place_name { get; set; }

        public float[] center { internal get; set; }

        public float lat => center[0];
        public float lon => center[1];

        public string pointType = "POI";
    }

    public class POIPointSearchResult
    {
        public List<POIPoint> features { get; set; }
    }
}


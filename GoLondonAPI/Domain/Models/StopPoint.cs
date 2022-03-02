using System;
namespace GoLondonAPI.Domain.Models
{
    public class StopPoint : Point
    {
        public string icsId { get; set; }
        public string[] modes { get; set; }
        public string zone { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string commonName { get; set; }

        public List<LineModeGroup> lineModeGroups { get; set; }
        public List<InternalStopPointProperty> additionalProperties { internal get; set; }
        public List<StopPoint> children { get; set; }

        public List<StopPointProperty> properties =>
            new List<StopPointProperty>
                {
                    new StopPointProperty { name = "WiFi", value = additionalProperties?.FirstOrDefault(p => p.key == "WiFi")?.value ?? "No" },
                    new StopPointProperty { name = "Zone", value = additionalProperties?.FirstOrDefault(p => p.key == "Zone")?.value ?? "No Information" },
                    new StopPointProperty { name = "Waiting Room", value = additionalProperties?.FirstOrDefault(p => p.key == "Waiting Room")?.value ?? "No Information" },
                    new StopPointProperty { name = "Car Park", value = additionalProperties?.FirstOrDefault(p => p.key == "Car Park")?.value ?? "No Information" },
                    new StopPointProperty { name = "Lifts", value = additionalProperties?.FirstOrDefault(p => p.key == "Lifts")?.value ?? "No Information" },
                    new StopPointProperty { name = "Toilets", value = additionalProperties?.FirstOrDefault(p => p.key == "Toilets")?.value ?? "No Information" },
                };

        public List<string> childStationIds => children?.Select(c => c.id)?.ToList() ?? new List<string>();
    }

    public class StopPointSearchResult
    {
        public List<StopPoint> matches { get; set; }
    }

    public class StopPointSearchAroundResult
    {
        public List<StopPoint> stopPoints { get; set; }
    }

    public class InternalStopPointProperty
    {
        public string category { get; set; }
        public string key { get; set; }
        public string value { get; set; }
    }

    public class StopPointProperty
    {
        public string name { get; set; }
        public string value { get; set; }
    }
}


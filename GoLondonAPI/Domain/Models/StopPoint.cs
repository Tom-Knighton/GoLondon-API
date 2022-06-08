using System;
using GoLondonAPI.Domain.Enums;
using Newtonsoft.Json;

namespace GoLondonAPI.Domain.Models
{
    public class StopPoint : Point
    {
        public string icsId { get; set; }
        public string icsCode { internal get; set; }

        public string zone { get; set; }
        public string id { get; set; }
        public string naptanId { get; set; }
        public string name { get; set; }
        public string commonName { get; set; }

        public string indicator { get; set; }
        public string stopLetter { get; set; }

        public string pointType = "Stop";

        public string hubNaptanCode { get; set; }

        public List<string> modes { internal get; set; }

        public List<LineMode> lineModes => modes?.Where(m => LineModeExtensions.GetFromString(m) != LineMode.unk).Select(m => LineModeExtensions.GetFromString(m)).ToList() ?? new();

        [JsonIgnore]
        public List<LineGroup> lineGroups { get; set; }
        [JsonProperty("lineGroup")]
        public List<LineGroup> lineGroupsInternal { set { lineGroups = value;  } }

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


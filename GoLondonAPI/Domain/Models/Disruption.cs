using System;
using GoLondonAPI.Domain.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GoLondonAPI.Domain.Models
{
    public class Disruption
    {
        public string description { get; set; }
        public DateTime? created { get; set; }
        public DateTime? lastUpdate { get; set; }
        public List<LineRoute> affectedRoutes { get; set; }
        public string closureText { internal get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public DisruptionDelayType DelayType
        {
            get
            {
                DisruptionDelayType delayType = DisruptionDelayType.none;
                switch(this.closureText)
                {
                    case "severeDelays":
                        delayType = DisruptionDelayType.SevereDelays;
                        break;
                    case "minorDelays":
                        delayType = DisruptionDelayType.MinorDelays;
                        break;
                    case "plannedClosure":
                        delayType = DisruptionDelayType.PlannedClosure;
                        break;
                    default:
                        break;
                }
                return delayType;
            }
        }
    }
}


using System;
using GoLondonAPI.Domain.Enums;

namespace GoLondonAPI.Domain.Models
{
    public class Line
    {
        public string id { get; set; }
        public string name { get; set; }
        public string modeName { get; set; }
        public List<Disruption> distruptions { get; set; }
        public List<LineStatus> lineStatuses { internal get; set; }

        public LineStatus currentStatus =>
            lineStatuses?.Where(l => l.validityPeriods?.Any(vp => vp.isNow) == true)?.FirstOrDefault() ?? lineStatuses?.FirstOrDefault() ?? null;

}

    public class LineStatus
    {
        public string id { get; set; }
        public string lineId { get; set; }
        public int statusSeverity { get; set; }
        public string statusSeverityDescription { get; set; }
        public string reason { get; set; }
        public DateTime? created { get; set; }
        public List<LineStatusValidityPeriod> validityPeriods { get; set; }
        public Disruption disruption { get; set; }
    }

    public class LineStatusValidityPeriod
    {
        public DateTime fromDate { get; set; }
        public DateTime toDate { get; set; }
        public bool isNow { get; set; }
    }

    public class LineRoute
    {
        public string id { get; set; }
        public string name { get; set; }
        public string direction { get; set; }
        public string originationName { get; set; }
        public string destinationName { get; set; }
    }

    public class LineModeGroup
    {
        public LineMode modeName { get; set; }
        public List<string> lineIdentifier { get; set; }
    }

    public class LineGroup
    {
        public string naptanIdReference { get; set; }
        public string[] lineIdentifier { get; set; }
    }
}


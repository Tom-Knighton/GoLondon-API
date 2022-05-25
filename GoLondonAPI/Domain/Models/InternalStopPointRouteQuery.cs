using System;
namespace GoLondonAPI.Domain.Models
{
    public class LineRoutes
    {
        public string lineId { get; set; }
        public string lineName { get; set; }
        public Branch[] stopPointSequences { get; set; }
    }

    public class Branch
    {
        public string lineId { get; set; }
        public string lineName { get; set; }
        public int branchId { get; set; }
        public int[] nextBranchIds { get; set; }
        public int[] prevBranchIds { get; set; }

        public StopPoint[] stopPoint { get; set; }
    }
}


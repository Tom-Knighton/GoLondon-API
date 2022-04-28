using System;
namespace GoLondonAPI.Domain.Models
{
    public class InternalLineGroupICS
    {
        public string ICSCode { get; set; }
        public List<LineModeGroup> lineModes { get; set; }
    }
}


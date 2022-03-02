using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace GoLondonAPI.Domain.Enums
{
    public enum DisruptionDelayType
    {
        [EnumMember(Value = "No Delays")]
        none,

        [EnumMember(Value = "Minor Delays")]
        MinorDelays,

        [EnumMember(Value = "Severe Delays")]
        SevereDelays,

        [EnumMember(Value = "Planned Closure")]
        PlannedClosure
    }
}


using System;
using System.Runtime.Serialization;

namespace GoLondonAPI.Domain.Enums
{
    public enum JourneyDateType
    {
        [EnumMember(Value = "Arrive At")]
        arriveAt,

        [EnumMember(Value = "Depart At")]
        departAt
    }
}


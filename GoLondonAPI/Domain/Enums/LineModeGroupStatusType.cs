using System;
using System.Runtime.Serialization;

namespace GoLondonAPI.Domain.Enums
{
    public enum LineModeGroupStatusType
    {
        [EnumMember(Value = "All lines are reporting Good Service")]
        allGood,

        [EnumMember(Value = "Most lines are reporting Good Service")]
        mostGood,

        [EnumMember(Value = "Some lines are reporting problems")]
        someBad,

        [EnumMember(Value = "Many lines are reporting problems")]
        manyBad,

        [EnumMember(Value = "All lines are reporting problems")]
        allBad,

        [EnumMember(Value = "Unable to determine service status.")]
        unk,
    }
}


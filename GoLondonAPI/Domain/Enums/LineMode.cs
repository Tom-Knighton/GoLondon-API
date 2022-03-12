using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Converters;

namespace GoLondonAPI.Domain.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    [ModelBinder(typeof(JsonModelBinder<LineMode>))]
    public enum LineMode
    {
        [EnumMember(Value = "tube")]
        tube,

        [EnumMember(Value = "bus")]
        bus,

        [EnumMember(Value = "dlr")]
        dlr,

        [EnumMember(Value = "national-rail")]
        nationalRail,

        [EnumMember(Value = "overground")]
        overground,

        [EnumMember(Value = "tflrail")]
        tflRail,

        [EnumMember(Value = "replacement-bus")]
        replacementBus,
    }
}


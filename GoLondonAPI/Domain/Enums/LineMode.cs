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
        [EnumMember(Value = "unk")]
        unk,

        [EnumMember(Value = "tube")]
        tube,

        [EnumMember(Value = "bus")]
        bus,

        [EnumMember(Value = "dlr")]
        dlr,

        [EnumMember(Value = "national-rail")]
        nationalRail,

        [EnumMember(Value = "international-rail")]
        internationalRail,

        [EnumMember(Value = "overground")]
        overground,

        [EnumMember(Value = "elizabeth-line")]
        elizabethLine,

        [EnumMember(Value = "replacement-bus")]
        replacementBus,

        [EnumMember(Value = "cable-car")]
        cableCar,

        [EnumMember(Value = "tram")]
        tram
    }

    public static class LineModeExtensions
    {
        /// <summary>
        /// Returns the LineMode value, if any, associated with the Value string
        /// </summary>
        /// <param name="value">The Value string from the LineMode enum</param>
        public static LineMode GetFromString(string value)
        {
            LineMode[] vals = Enum.GetValues<LineMode>();
            return vals.FirstOrDefault(v => v.GetValue() == value);
        }
    }

}


using System.Text.Json.Serialization;

namespace ReSR.Application.ValueObjects.Core;
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DateRange {
    Daily,
    Weekly,
    Quarterly,
    Yearly,
    Any,
}
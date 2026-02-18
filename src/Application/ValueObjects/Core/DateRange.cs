using System.Text.Json.Serialization;

namespace ReSR.Application.ValueObjects.Core;
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DateRange {
    Daily,
    Weekly,
    Monthly,
    Quarterly,
    Yearly,
    AllTime,
}
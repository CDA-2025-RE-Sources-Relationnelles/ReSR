using System.Text.Json.Serialization;

namespace ReSR.Domain.Aggregates.Resources.ValueObjects;
[Flags]
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Relationships {
    None                = 0b000000,
    Self                = 0b000001,
    Partner             = 0b000010,
    Family              = 0b000100,
    Work                = 0b001000,
    FriendsAndCommunity = 0b010000,
    Strangers           = 0b100000,
    All                 = 0b111111
}
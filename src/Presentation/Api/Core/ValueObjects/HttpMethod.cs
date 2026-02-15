using System.Text.Json.Serialization;

namespace ReSR.Presentation.Api.Core.ValueObjects;
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum HttpMethod {
    GET = default,
    POST,
    PUT,
    PATCH,
    DELETE
}
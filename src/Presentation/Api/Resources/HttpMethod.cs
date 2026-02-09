using System.Text.Json.Serialization;

namespace ReSR.Presentation.Api.Resources;
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum HttpMethod {
    GET = default,
    POST,
    PUT,
    PATCH,
    DELETE
}
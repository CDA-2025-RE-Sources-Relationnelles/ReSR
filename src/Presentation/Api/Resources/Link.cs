namespace ReSR.Presentation.Api.Resources;
public readonly record struct Link(string Href, HttpMethod Method) {
    public Link(HttpMethod method = default, params object[] values) : this(string.Join('/', values), method) {}
}

namespace ReSR.Presentation.Api.Resources;
public readonly record struct Link(HttpMethod Method, string Href) {
    public Link(HttpMethod method = default, params object[] values) : this(method, string.Join('/', values)) {}
    public Link WithSubRoute(params object[] values) => this with { Href = string.Join('/', [this.Href, string.Join('/', values)]) };
    public Link WithMethod(HttpMethod method) => this with { Method = method };
}

namespace ReSR.Presentation.Api.Resources;
public readonly record struct Link(string Href, HttpMethod Method) : ILink {
    public Link(HttpMethod method = default, params object[] values) : this(string.Join('/', values), method) {}
    public ILink WithSubRoute(params object[] values) => this with { Href = string.Join('/', this.Href, values) };
    public ILink WithMethod(HttpMethod method) => this with { Method = method };
}

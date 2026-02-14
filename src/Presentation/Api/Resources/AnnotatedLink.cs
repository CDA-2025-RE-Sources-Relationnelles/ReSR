namespace ReSR.Presentation.Api.Resources;
public readonly record struct AnnotatedLink(HttpMethod Method, string Title, string Href) : ILink  {
    public AnnotatedLink(HttpMethod method, string title, params object[] values) : this(method, title, string.Join('/', values)) {}
    public ILink WithSubRoute(params object[] values) => this with { Href = string.Join('/', this.Href, values) };
    public ILink WithMethod(HttpMethod method) => this with { Method = method };
}

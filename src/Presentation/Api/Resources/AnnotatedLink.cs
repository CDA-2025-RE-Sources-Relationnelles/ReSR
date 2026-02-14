namespace ReSR.Presentation.Api.Resources;
public readonly record struct AnnotatedLink(HttpMethod Method, string Title, string Href)  {
    public AnnotatedLink(HttpMethod method, string title, params object[] values) : this(method, title, string.Join('/', values)) {}
    public AnnotatedLink WithSubRoute(params object[] values) => this with { Href = string.Join('/', [this.Href, string.Join('/', values)]) };
    public AnnotatedLink WithMethod(HttpMethod method) => this with { Method = method };
    public static implicit operator Link(AnnotatedLink from) => new(from.Method, from.Href);
}

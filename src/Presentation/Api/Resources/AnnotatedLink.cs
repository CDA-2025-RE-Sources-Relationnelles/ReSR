namespace ReSR.Presentation.Api.Resources;
public readonly record struct AnnotatedLink(HttpMethod Method, string Title, string Href) {
    public AnnotatedLink(HttpMethod method, string title, params object[] values) : this(method, title, string.Join('/', values)) {}
}

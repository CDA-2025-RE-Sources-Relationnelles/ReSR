namespace ReSR.Presentation.Api.Resources;
public interface ILink {
    public string     Href   { get; }
    public HttpMethod Method { get; }
    public ILink WithSubRoute(params object[] values);
    public ILink WithMethod(HttpMethod method);
}

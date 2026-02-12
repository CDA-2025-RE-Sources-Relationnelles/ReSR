namespace ReSR.Presentation.Api.Resources;
public interface IResource<T, TFrom> {
    static abstract T From(TFrom from);
    static abstract IEnumerable<T> From(IEnumerable<TFrom> from);
    static abstract ILink GetLink(TFrom from);
    static abstract IEnumerable<ILink> GetLinks(IEnumerable<TFrom> from);
}
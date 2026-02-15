namespace ReSR.Presentation.Api.Core.ValueObjects;
public interface IResource<T, TFrom> {
    static abstract T From(TFrom from);
    static abstract IEnumerable<T> From(IEnumerable<TFrom> from);
}
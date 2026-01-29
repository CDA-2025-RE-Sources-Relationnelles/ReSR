namespace FluentResponse.Interfaces;

/// <summary>
/// A successful data response interface.
/// </summary>
/// <typeparam name="T">The response's value type.</typeparam>
public interface ISuccess<out T> : IResponse<T> {

    /// <summary>
    /// The response's value.
    /// </summary>
    T Value { get; }
}

namespace FluentResponse.Interfaces;

/// <summary>
/// A base data response interface.
/// </summary>
/// <typeparam name="T">The response's expected value type.</typeparam>
public interface IResponse<out T> : IResponse;

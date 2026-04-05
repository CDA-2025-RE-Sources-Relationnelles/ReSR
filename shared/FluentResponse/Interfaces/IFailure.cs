namespace FluentResponse.Interfaces;

/// <summary>
/// A failed response interface.
/// </summary>
public interface IFailure : IResponse {

    /// <summary>
    /// The failure's associated exception.
    /// </summary>
    Exception Exception { get; }

    /// <summary>
    /// The failure's error message.
    /// </summary>
    string ErrorMessage { get; }
}

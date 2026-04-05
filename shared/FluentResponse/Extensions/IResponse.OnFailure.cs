using FluentResponse.Interfaces;

namespace FluentResponse;
/// <inheritdoc/>
public static partial class Extensions {

    /// <summary>
    /// Runs a function with the response's exception as argument if it is not successful and propagates the return value.
    /// </summary>
    /// <param name="self">The current response.</param>
    /// <param name="onFailure">A function called with the response's exception as argument.</param>
    public static T OnFailure<T>(
        this T self,
        Func<Exception, T> onFailure
    ) where T : IResponse =>
        self is IFailure failure
            ? onFailure(failure.Exception)
            : self;

    /// <summary>
    /// Runs a function with the response's exception as argument if it is not successful.
    /// </summary>
    /// <param name="self">The current response.</param>
    /// <param name="onFailure">A function called with the response's exception as argument.</param>
    public static T OnFailure<T>(
        this T self,
        Action<Exception> onFailure
    ) where T : IResponse {
        if (self is IFailure failure) onFailure(failure.Exception);
        return self;
    }
}
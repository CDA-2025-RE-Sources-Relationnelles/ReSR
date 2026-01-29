using FluentResponse.Interfaces;

namespace FluentResponse;
/// <inheritdoc/>
public static partial class Extensions {

    /// <summary>
    /// Outputs the response's value if it is successful; otherwise throws the response's exception.
    /// </summary>
    /// <typeparam name="TValue">The response's value type.</typeparam>
    /// <param name="self">The current response.</param>
    /// <returns>The reponse's value if successful.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static TValue Unwrap<TValue>(this IResponse<TValue> self) =>
        self switch {
            ISuccess<TValue> success => success.Value,
            IFailure         failure => throw failure.Exception,
            _                        => throw new InvalidOperationException()
        };

    /// <summary>
    /// Outputs a value according to the response's state.
    /// </summary>
    /// <typeparam name="TValue">The response's value type.</typeparam>
    /// <typeparam name="TOut">The output type.</typeparam>
    /// <param name="self">The current response.</param>
    /// <param name="onSuccess">A function called with the response's value as argument.</param>
    /// <param name="onFailure">A function called with the response's exception as argument.</param>
    /// <returns>A value based according to the response's state.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static TOut Unwrap<TValue, TOut>(
        this IResponse<TValue> self,
        Func<TValue, TOut>     onSuccess,
        Func<Exception, TOut>  onFailure
    ) =>
        self switch {
            ISuccess<TValue> success => onSuccess(success.Value),
            IFailure         failure => onFailure(failure.Exception),
            _                        => throw new InvalidOperationException()
        };
}
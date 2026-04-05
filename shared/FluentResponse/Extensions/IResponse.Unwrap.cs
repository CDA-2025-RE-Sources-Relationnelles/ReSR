using FluentResponse.Interfaces;

namespace FluentResponse;
/// <inheritdoc/>
public static partial class Extensions {
    
    /// <summary>
    /// Throws the response's exception if it is not successful.
    /// </summary>
    /// <param name="self">The current response.</param>
    public static void Unwrap(this IResponse self) {
        if (self is IFailure failure)
            throw failure.Exception;
    }

    /// <summary>
    /// Outputs a value according to the response's state.
    /// </summary>
    /// <typeparam name="TOut">The output type.</typeparam>
    /// <param name="self">The current response.</param>
    /// <param name="onSuccess">A function called when the response is successful and returns the output type.</param>
    /// <param name="onFailure">A function called with the response's exception as argument and returns the output type.</param>
    /// <returns>A value based according to the response's state.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static TOut Unwrap<TOut>(
        this IResponse        self,
        Func<TOut>            onSuccess,
        Func<Exception, TOut> onFailure
    ) => self switch {
            ISuccess         => onSuccess(),
            IFailure failure => onFailure(failure.Exception),
            _                => throw new InvalidOperationException()
        };
}
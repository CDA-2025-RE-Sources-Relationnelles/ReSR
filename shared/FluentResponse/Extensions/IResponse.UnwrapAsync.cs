using FluentResponse.Interfaces;

namespace FluentResponse;
/// <inheritdoc/>
public static partial class Extensions {

    /// <inheritdoc cref="Unwrap{T}(IResponse, Func{T}, Func{Exception, T})"/>
    public static async Task<TOut> UnwrapAsync<TOut>(
        this IResponse              self,
        Func<Task<TOut>>            onSuccess,
        Func<Exception, Task<TOut>> onFailure
    ) =>
        self switch {
            ISuccess         => await onSuccess(),
            IFailure failure => await onFailure(failure.Exception),
            _                => throw new InvalidOperationException()
        };
}
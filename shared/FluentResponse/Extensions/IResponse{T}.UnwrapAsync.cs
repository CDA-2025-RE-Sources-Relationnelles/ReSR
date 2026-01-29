using FluentResponse.Interfaces;

namespace FluentResponse;
/// <inheritdoc/>
public static partial class Extensions {

    /// <inheritdoc cref="Unwrap{T, T}(IResponse{T}, Func{T, T}, Func{Exception, T})"/>
    public static async Task<TOut> UnwrapAsync<TValue, TOut>(
        this IResponse<TValue>      self,
        Func<TValue, Task<TOut>>    onSuccess,
        Func<Exception, Task<TOut>> onFailure
    ) =>
        self switch {
            ISuccess<TValue> success => await onSuccess(success.Value),
            IFailure         failure => await onFailure(failure.Exception),
            _                        => throw new InvalidOperationException()
        };
}
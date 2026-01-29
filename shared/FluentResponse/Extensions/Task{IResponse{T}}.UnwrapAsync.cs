using FluentResponse.Interfaces;

namespace FluentResponse;
/// <inheritdoc/>
public static partial class Extensions {

    /// <inheritdoc cref="Unwrap{T}(IResponse{T})"/>
    public static async Task<TValue> UnwrapAsync<TValue>(this Task<IResponse<TValue>> task) =>
        (await task).Unwrap();

    /// <inheritdoc cref="Unwrap{T, T}(IResponse{T}, Func{T, T}, Func{Exception, T})"/>
    public static async Task<TOut> UnwrapAsync<TValue, TOut>(
        this Task<IResponse<TValue>> task,
        Func<TValue, TOut>           onSuccess,
        Func<Exception, TOut>   onFailure
    ) => (await task).Unwrap(onSuccess, onFailure);

    /// <inheritdoc cref="Unwrap{T, T}(IResponse{T}, Func{T, T}, Func{Exception, T})"/>
    public static async Task<TOut> UnwrapAsync<TValue, TOut>(
        this Task<IResponse<TValue>>     task,
        Func<TValue, Task<TOut>>         onSuccess,
        Func<Exception, Task<TOut>> onFailure
    ) => await (await task).UnwrapAsync(onSuccess, onFailure);
}
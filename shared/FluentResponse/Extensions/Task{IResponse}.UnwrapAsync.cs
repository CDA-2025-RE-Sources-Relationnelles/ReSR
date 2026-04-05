using FluentResponse.Interfaces;

namespace FluentResponse;
/// <inheritdoc/>
public static partial class Extensions {

    /// <inheritdoc cref="Unwrap(IResponse)"/>
    public static async Task UnwrapAsync(this Task<IResponse> task) =>
        (await task).Unwrap();

    /// <inheritdoc cref="Unwrap{T}(IResponse, Func{T}, Func{Exception, T})"/>
    public static async Task<TOut> UnwrapAsync<TOut>(
        this Task<IResponse>  task,
        Func<TOut>            onSuccess,
        Func<Exception, TOut> onFailure
    ) => (await task).Unwrap(onSuccess, onFailure);

    /// <inheritdoc cref="Unwrap{T}(IResponse, Func{T}, Func{Exception, T})"/>
    public static async Task<TOut> UnwrapAsync<TOut>(
        this Task<IResponse>        task,
        Func<Task<TOut>>            onSuccess,
        Func<Exception, Task<TOut>> onFailure
    ) => await (await task).UnwrapAsync(onSuccess, onFailure);
}
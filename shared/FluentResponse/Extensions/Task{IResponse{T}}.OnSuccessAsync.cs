using FluentResponse.Interfaces;

namespace FluentResponse;
/// <inheritdoc/>
public static partial class Extensions {

    /// <inheritdoc cref="OnSuccess{T, T}(IResponse{T}, Func{T, IResponse{T}})"/>
    public static async Task<IResponse<TOutValue>> OnSuccessAsync<TFromValue, TOutValue>(
        this Task<IResponse<TFromValue>> task,
        Func<TFromValue, IResponse<TOutValue>> onSuccess
    ) => (await task).OnSuccess(onSuccess);

    /// <inheritdoc cref="OnSuccess{T, T}(IResponse{T}, Func{T, T})"/>
    public static async Task<IResponse<TOutValue>> OnSuccessAsync<TFromValue, TOutValue>(
        this Task<IResponse<TFromValue>> task,
        Func<TFromValue, TOutValue> onSuccess
    ) => (await task).OnSuccess(onSuccess);

    /// <inheritdoc cref="OnSuccess{T}(IResponse{T}, Func{T, IResponse})"/>
    public static async Task<IResponse> OnSuccessAsync<TValue>(
        this Task<IResponse<TValue>> task,
        Func<TValue, IResponse> onSuccess
    ) => (await task).OnSuccess(onSuccess);

    /// <inheritdoc cref="OnSuccess{T}(IResponse{T}, Action{T})"/>
    public static async Task<IResponse<TValue>> OnSuccessAsync<TValue>(
        this Task<IResponse<TValue>> task,
        Action<TValue> onSuccess
    ) => (await task).OnSuccess(onSuccess);

    /// <inheritdoc cref="OnSuccess{T, T}(IResponse{T}, Func{T, IResponse{T}})"/>
    public static async Task<IResponse<TOutValue>> OnSuccessAsync<TFromValue, TOutValue>(
        this Task<IResponse<TFromValue>> task,
        Func<TFromValue, Task<IResponse<TOutValue>>> onSuccess
    ) => await (await task).OnSuccessAsync(onSuccess);

    /// <inheritdoc cref="OnSuccess{T, T}(IResponse{T}, Func{T, T})"/>
    public static async Task<IResponse<TOutValue>> OnSuccessAsync<TFromValue, TOutValue>(
        this Task<IResponse<TFromValue>> task,
        Func<TFromValue, Task<TOutValue>> onSuccess
    ) => await (await task).OnSuccessAsync(onSuccess);

    /// <inheritdoc cref="OnSuccess{T}(IResponse{T}, Func{T, IResponse})"/>
    public static async Task<IResponse> OnSuccessAsync<TValue>(
        this Task<IResponse<TValue>> task,
        Func<TValue, Task<IResponse>> onSuccess
    ) => await (await task).OnSuccessAsync(onSuccess);

    /// <inheritdoc cref="OnSuccess{T}(IResponse{T}, Action{T})"/>
    public static async Task<IResponse<TValue>> OnSuccessAsync<TValue>(
        this Task<IResponse<TValue>> task,
        Func<TValue, Task> onSuccess
    ) => await (await task).OnSuccessAsync(onSuccess);

}
using FluentResponse.Interfaces;

namespace FluentResponse;
/// <inheritdoc/>
public static partial class Extensions {

    /// <inheritdoc cref="OnSuccess(IResponse, Func{IResponse})"/>
    public static async Task<IResponse> OnSuccessAsync(
        this Task<IResponse> task,
        Func<IResponse> onSuccess
    ) => (await task).OnSuccess(onSuccess);

    /// <inheritdoc cref="OnSuccess(IResponse, Action)"/>
    public static async Task<IResponse> OnSuccessAsync(
        this Task<IResponse> task,
        Action onSuccess
    ) => (await task).OnSuccess(onSuccess);

    /// <inheritdoc cref="OnSuccess(IResponse, Func{IResponse})"/>
    public static async Task<IResponse> OnSuccessAsync(
        this Task<IResponse> task,
        Func<Task<IResponse>> onSuccess
    ) => await (await task).OnSuccessAsync(onSuccess);

    /// <inheritdoc cref="OnSuccess(IResponse, Action)"/>
    public static async Task<IResponse> OnSuccessAsync(
        this Task<IResponse> task,
        Func<Task> onSuccess
    ) => await (await task).OnSuccessAsync(onSuccess);

    /// <inheritdoc cref="OnSuccess{T}(IResponse, Func{T})"/>
    public static async Task<IResponse<TValue>> OnSuccessAsync<TValue>(
        this Task<IResponse> task,
        Func<TValue>         onSuccess
    ) => (await task).OnSuccess(onSuccess);

    /// <inheritdoc cref="OnSuccess{T}(IResponse, Func{T})"/>
    public static async Task<IResponse<TValue>> OnSuccessAsync<TValue>(
        this Task<IResponse> task,
        Func<Task<TValue>>   onSuccess
    ) => await (await task).OnSuccessAsync(onSuccess);
    
    /// <inheritdoc cref="OnSuccess{T}(IResponse, Func{IResponse{T}})"/>
    public static async Task<IResponse<TValue>> OnSuccessAsync<TValue>(
        this Task<IResponse>          task,
        Func<Task<IResponse<TValue>>> onSuccess
    ) => await (await task).OnSuccessAsync(onSuccess);
}
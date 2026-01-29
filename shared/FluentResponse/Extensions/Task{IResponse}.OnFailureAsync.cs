using FluentResponse.Interfaces;

namespace FluentResponse;
/// <inheritdoc/>
public static partial class Extensions {

    /// <inheritdoc cref="OnFailure{T}(T, Func{Exception, T})"/>
    public static async Task<T> OnFailureAsync<T>(
        this Task<T> task,
        Func<Exception, T> onFailure
    ) where T : IResponse => (await task).OnFailure(onFailure);
        
    /// <inheritdoc cref="OnFailure{T}(T, Action{Exception})"/>
    public static async Task<T> OnFailureAsync<T>(
        this Task<T> task,
        Action<Exception> onFailure
    ) where T : IResponse => (await task).OnFailure(onFailure);

    /// <inheritdoc cref="OnFailure{T}(T, Func{Exception, T})"/>
    public static async Task<T> OnFailureAsync<T>(
        this Task<T> task,
        Func<Exception, Task<T>> onFailure
    ) where T : IResponse => await (await task).OnFailureAsync(onFailure);
        
    /// <inheritdoc cref="OnFailure{T}(T, Action{Exception})"/>
    public static async Task<T> OnFailureAsync<T>(
        this Task<T> task,
        Func<Exception, Task> onFailure
    ) where T : IResponse => await (await task).OnFailureAsync(onFailure);
}
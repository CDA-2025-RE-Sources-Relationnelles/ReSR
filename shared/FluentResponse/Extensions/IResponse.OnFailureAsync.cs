using FluentResponse.Interfaces;

namespace FluentResponse;
/// <inheritdoc/>
public static partial class Extensions {

    /// <inheritdoc cref="OnFailure{T}(T, Func{Exception, T})"/>
    public static async Task<T> OnFailureAsync<T>(
        this T self,
        Func<Exception, Task<T>> onFailure
    ) where T : IResponse =>
        self is IFailure failure
            ? await onFailure(failure.Exception)
            : self;

    /// <inheritdoc cref="OnFailure{T}(T, Action{Exception})"/>
    public static async Task<T> OnFailureAsync<T>(
        this T self,
        Func<Exception, Task> onFailure
    ) where T : IResponse {
        if (self is IFailure failure) await onFailure(failure.Exception);
        return self;
    }
}
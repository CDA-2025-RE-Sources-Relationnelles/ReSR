using FluentResponse.Interfaces;

namespace FluentResponse;
/// <inheritdoc/>
public static partial class Extensions {

    /// <inheritdoc cref="OnSuccess{T, T}(IResponse{T}, Func{T, IResponse{T}})"/>
    public static async Task<IResponse<TOutValue>> OnSuccessAsync<TFromValue, TOutValue>(
        this IResponse<TFromValue> self,
        Func<TFromValue, Task<IResponse<TOutValue>>> onSuccess
    ) =>
        self switch {
            ISuccess<TFromValue> success => await onSuccess(success.Value),
            IFailure             failure => Response.Failure<TOutValue>(failure.Exception),
            _                            => throw new InvalidOperationException()
        };

    /// <inheritdoc cref="OnSuccess{T, T}(IResponse{T}, Func{T, T})"/>
    public static async Task<IResponse<TOutValue>> OnSuccessAsync<TFromValue, TOutValue>(
        this IResponse<TFromValue> self,
        Func<TFromValue, Task<TOutValue>> onSuccess
    ) =>
        self switch {
            ISuccess<TFromValue> success => Response.Success(await onSuccess(success.Value)),
            IFailure             failure => Response.Failure<TOutValue>(failure.Exception),
            _                            => throw new InvalidOperationException()
        };


    /// <inheritdoc cref="OnSuccess{T}(IResponse{T}, Func{T, IResponse})"/>
    public static async Task<IResponse> OnSuccessAsync<TValue>(
        this IResponse<TValue> self,
        Func<TValue, Task<IResponse>> onSuccess
    ) =>
        self switch {
            ISuccess<TValue> success => await onSuccess(success.Value),
            IFailure         failure => Response.Failure(failure.Exception),
            _                        => throw new InvalidOperationException()
        };

    /// <inheritdoc cref="OnSuccess{T}(IResponse{T}, Action{T})"/>
    public static async Task<IResponse<TValue>> OnSuccessAsync<TValue>(
        this IResponse<TValue> self,
        Func<TValue, Task> onSuccess
    ) {
        if (self is ISuccess<TValue> success) await onSuccess(success.Value);
        return self;
    }
}
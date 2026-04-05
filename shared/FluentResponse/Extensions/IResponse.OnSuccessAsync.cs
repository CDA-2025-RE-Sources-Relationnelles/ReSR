using FluentResponse.Interfaces;

namespace FluentResponse;
/// <inheritdoc/>
public static partial class Extensions {

    /// <inheritdoc cref="OnSuccess(IResponse, Func{IResponse})"/>
    public static async Task<IResponse> OnSuccessAsync(
        this IResponse self,
        Func<Task<IResponse>> onSuccess
    ) => self is ISuccess
            ? await onSuccess()
            : self;

    /// <inheritdoc cref="OnSuccess(IResponse, Action)"/>
    public static async Task<IResponse> OnSuccessAsync(
        this IResponse self,
        Func<Task> onSuccess
    ) {
        if (self is ISuccess) await onSuccess();
        return self;
    }

    /// <inheritdoc cref="OnSuccess{T}(IResponse, Func{IResponse{T}})"/>
    public static async Task<IResponse<TValue>> OnSuccessAsync<TValue>(
        this IResponse self,
        Func<Task<IResponse<TValue>>> onSuccess
    ) => self switch {
            ISuccess         => await onSuccess(),
            IFailure failure => Response.Failure<TValue>(failure.Exception),
            _                => throw new InvalidOperationException()
        };

    /// <inheritdoc cref="OnSuccess{T}(IResponse, Func{T})"/>
    public static  async Task<IResponse<TValue>> OnSuccessAsync<TValue>(
        this IResponse self,
        Func<Task<TValue>> onSuccess
    ) => self switch {
            ISuccess         => Response.Success(await onSuccess()),
            IFailure failure => Response.Failure<TValue>(failure.Exception),
            _                => throw new InvalidOperationException()
        };
}
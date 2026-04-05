using FluentResponse.Interfaces;

namespace FluentResponse;
/// <inheritdoc/>
public static partial class Extensions {

    /// <summary>
    /// Runs a function with the response's value as argument if it is successful and propagates the return value.
    /// </summary>
    /// <typeparam name="TFromValue">Base response's value type.</typeparam>
    /// <typeparam name="TOutValue">Propagated response's value type.</typeparam>
    /// <param name="self">The current response.</param>
    /// <param name="onSuccess">A function called with the response's value as argument.</param>
    /// <exception cref="InvalidOperationException"></exception>
    public static IResponse<TOutValue> OnSuccess<TFromValue, TOutValue>(
        this IResponse<TFromValue> self,
        Func<TFromValue, IResponse<TOutValue>> onSuccess
    ) =>
        self switch {
            ISuccess<TFromValue> success => onSuccess(success.Value),
            IFailure             failure => Response.Failure<TOutValue>(failure.Exception),
            _                            => throw new InvalidOperationException()
        };

    /// <inheritdoc cref="OnSuccess{T, T}(IResponse{T}, Func{T, IResponse{T}})"/>
    public static IResponse<TOutValue> OnSuccess<TFromValue, TOutValue>(
        this IResponse<TFromValue> self,
        Func<TFromValue, TOutValue> onSuccess
    ) =>
        self switch {
            ISuccess<TFromValue> success => Response.Success(onSuccess(success.Value)),
            IFailure             failure => Response.Failure<TOutValue>(failure.Exception),
            _                            => throw new InvalidOperationException()
        };

    /// <summary>
    /// Runs a function with the response's value as argument if it is successful and propagates the return value.
    /// </summary>
    /// <typeparam name="TValue">The response's value type.</typeparam>
    /// <param name="self">The current response.</param>
    /// <param name="onSuccess">A function called with the response's value as argument.</param>
    /// <exception cref="InvalidOperationException"></exception>
    public static IResponse OnSuccess<TValue>(
        this IResponse<TValue> self,
        Func<TValue, IResponse> onSuccess
    ) =>
        self switch {
            ISuccess<TValue> success => onSuccess(success.Value),
            IFailure         failure => Response.Failure(failure.Exception),
            _                        => throw new InvalidOperationException()
        };

    /// <summary>
    /// Runs a function with the response's value as argument if it is successful.
    /// </summary>
    /// <typeparam name="TValue">The response's value type.</typeparam>
    /// <param name="self">The current response.</param>
    /// <param name="onSuccess">A function called with the response's value as argument.</param>
    public static IResponse<TValue> OnSuccess<TValue>(
        this IResponse<TValue> self,
        Action<TValue> onSuccess
    ) {
        if (self is ISuccess<TValue> success) onSuccess(success.Value);
        return self;
    }

}
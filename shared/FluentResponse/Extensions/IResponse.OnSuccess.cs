using FluentResponse.Interfaces;

namespace FluentResponse;
/// <inheritdoc/>
public static partial class Extensions {

    /// <summary>
    /// Runs a function if it is successful and propagates the return value.
    /// </summary>
    /// <param name="self">The current response.</param>
    /// <param name="onSuccess">A function called when the response is successful and returns a response of the same type.</param>
    public static IResponse OnSuccess(
        this IResponse  self,
        Func<IResponse> onSuccess
    ) => self is ISuccess
            ? onSuccess()
            : self;

    /// <summary>
    /// Runs a function if it is successful.
    /// </summary>
    /// <param name="self">The current response.</param>
    /// <param name="onSuccess">A function called when the response is successful.</param>
    public static IResponse OnSuccess(
        this IResponse self,
        Action onSuccess
    ) {
        if (self is ISuccess) onSuccess();
        return self;
    }

    /// <summary>
    /// Runs a function if it is successful and propagates the return value.
    /// </summary>
    /// <typeparam name="TValue">The response's value type.</typeparam>
    /// <param name="self">The current response.</param>
    /// <param name="onSuccess">A function called when the response is successful and returns a response of the same type.</param>
    /// <exception cref="InvalidOperationException"></exception>
    public static IResponse<TValue> OnSuccess<TValue>(
        this IResponse self,
        Func<IResponse<TValue>> onSuccess
    ) => self switch {
            ISuccess         => onSuccess(),
            IFailure failure => Response.Failure<TValue>(failure.Exception),
            _                => throw new InvalidOperationException()
        };

    /// <summary>
    /// Runs a function if it is successful and propagates the return value.
    /// </summary>
    /// <typeparam name="TValue">The response's value type.</typeparam>
    /// <exception cref="InvalidOperationException"></exception>
    public static IResponse<TValue> OnSuccess<TValue>(
        this IResponse self,
        Func<TValue> onSuccess
    ) => self switch {
            ISuccess         => Response.Success(onSuccess()),
            IFailure failure => Response.Failure<TValue>(failure.Exception),
            _                => throw new InvalidOperationException()
        };
}
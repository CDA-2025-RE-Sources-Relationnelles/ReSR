using FluentResponse.Interfaces;

namespace FluentResponse;
/// <inheritdoc/>
public static partial class Extensions {

    /// <summary>
    /// Outputs the response's value cast to the specified type if it is compatible and successful.
    /// </summary>
    /// <typeparam name="TFromValue">The response's value type.</typeparam>
    /// <typeparam name="TOutValue">Propagated response's value type.</typeparam>
    /// <param name="self">The current response.</param>
    /// <exception cref="InvalidCastException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public static IResponse<TOutValue> Cast<TFromValue, TOutValue>(this IResponse<TFromValue> self)
        where TFromValue : notnull
        where TOutValue  : TFromValue =>
        self switch {
            ISuccess<TFromValue> success => Response.Success((TOutValue)success.Value),
            IFailure             failure => Response.Failure<TOutValue>(failure.Exception),
            _                            => throw new InvalidOperationException()
        };
}
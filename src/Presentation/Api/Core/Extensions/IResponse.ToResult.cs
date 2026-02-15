using FluentResponse.Interfaces;

namespace ReSR.Presentation.Api.Core.Extensions;
public static partial class Extensions {
    
    /// <summary>
    /// Converts a <see cref="IResponse"/> to an HTTP response.
    /// </summary>
    /// <returns>An HTTP response.</returns>
    public static IResult ToResult(
        this IResponse self,
        Func<ISuccess, IResult> onSuccess
    ) => self switch {
            ISuccess success => onSuccess(success),
            IFailure failure => failure.Exception.ToResult(failure),
            _                => throw new InvalidOperationException()
        };

    /// <summary>
    /// Converts a <see cref="IResponse{TValue}"/> to an HTTP response.
    /// </summary>
    /// <typeparam name="TValue">Reponse value type.</typeparam>
    /// <returns>An HTTP response.</returns>
    public static IResult ToResult<TValue>(
        this IResponse<TValue> self,
        Func<ISuccess<TValue>, IResult> onSuccess
    ) => self switch {
            ISuccess<TValue> success => onSuccess(success),
            IFailure         failure => failure.Exception.ToResult(failure),
            _                        => throw new InvalidOperationException()
        };
}
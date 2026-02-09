using FluentResponse.Interfaces;

namespace ReSR.Presentation.Api.Extensions;
public static partial class Extensions {

    /// <summary>
    /// Converts a <see cref="IResponse"/> to an HTTP response.
    /// </summary>
    /// <returns>An HTTP response.</returns>
    public static async Task<IResult> ToResultAsync(
        this Task<IResponse>    task,
        Func<ISuccess, IResult> onSuccess
    ) => (await task).ToResult(onSuccess);

    /// <summary>
    /// Converts a <see cref="IResponse{TValue}"/> to an HTTP response.
    /// </summary>
    /// <typeparam name="TValue">Reponse value type.</typeparam>
    /// <returns>An HTTP response.</returns>
    public static async Task<IResult> ToResultAsync<TValue>(
        this Task<IResponse<TValue>>    task,
        Func<ISuccess<TValue>, IResult> onSuccess
    ) => (await task).ToResult(onSuccess);
}

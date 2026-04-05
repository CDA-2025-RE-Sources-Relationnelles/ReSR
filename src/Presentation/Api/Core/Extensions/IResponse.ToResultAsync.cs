using FluentResponse.Interfaces;

namespace ReSR.Presentation.Api.Core.Extensions;
public static partial class Extensions {

    /// <summary>
    /// Converts a value to an HTTP response.
    /// </summary>
    /// <typeparam name="TValue">Value type.</typeparam>
    /// <returns>An HTTP response.</returns>
    public static async Task<IResult> ToResultAsync<TValue>(
        this Task<TValue>               task,
        Func<ISuccess<TValue>, IResult> onSuccess
    ) => (await task).ToResult(onSuccess);
}

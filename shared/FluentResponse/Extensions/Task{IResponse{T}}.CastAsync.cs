using FluentResponse.Interfaces;

namespace FluentResponse;
/// <inheritdoc/>
public static partial class Extensions {

    /// <inheritdoc cref="Cast{T, T}(IResponse{T})"/>
    public static async Task<IResponse<TOutValue>> CastAsync<TFromValue, TOutValue>(this Task<IResponse<TFromValue>> task)
        where TFromValue : notnull
        where TOutValue  : TFromValue => (await task).Cast<TFromValue, TOutValue>();
}
using FluentResponse.Interfaces;
using FluentResponse;
using ReSR.Presentation.Api.Core.ValueObjects;

namespace ReSR.Presentation.Api.Core.Extensions;
public static partial class Extensions {
    
    /// <summary>
    /// Converts a <see cref="IResponse{TValue}"/> to an HATEOAS resource wrapped in an HTTP response.
    /// </summary>
    /// <typeparam name="TValue">Reponse value type.</typeparam>
    /// <typeparam name="TResource">Resource type.</typeparam>
    /// <returns>An HTTP reponse</returns>
    public static IResult ToResource<TValue, TResource>(
        this IResponse<TValue>             self,
        Func<ISuccess<TResource>, IResult> onSuccess
    ) where TResource : IResource<TResource, TValue> {
        return self.Unwrap(
            x => onSuccess((ISuccess<TResource>)Response.Success(TResource.From(x))),
            e => e.ToResult(self)
        );
    }

    /// <summary>
    /// Converts a <see cref="IResponse{IEnumerable{TValue}}"/> to an array of HATEOAS resource swrapped in an HTTP response.
    /// </summary>
    /// <typeparam name="TValue">Reponse value type.</typeparam>
    /// <typeparam name="TResource">Resource type.</typeparam>
    /// <returns>An HTTP reponse</returns>
    public static IResult ToResource<TValue, TResource>(
        this IResponse<IEnumerable<TValue>>             self,
        Func<ISuccess<IEnumerable<TResource>>, IResult> onSuccess
    ) where TResource : IResource<TResource, TValue> {
        return self.Unwrap(
            x => onSuccess((ISuccess<IEnumerable<TResource>>)Response.Success(TResource.From(x))),
            e => e.ToResult(self)
        );
    }

    /// <summary>
    /// Converts a <see cref="IEnumerable{TValue}"/> to an array of HATEOAS resource swrapped in an HTTP response.
    /// </summary>
    /// <typeparam name="TValue">Value type.</typeparam>
    /// <typeparam name="TResource">Resource type.</typeparam>
    /// <returns>An HTTP reponse</returns>
    public static IResult ToResource<TValue, TResource>(
        this IEnumerable<TValue>                        self,
        Func<ISuccess<IEnumerable<TResource>>, IResult> transform
    ) where TResource : IResource<TResource, TValue> {
        return transform((ISuccess<IEnumerable<TResource>>)Response.Success(TResource.From(self)));
    }

    /// <summary>
    /// Converts a <see cref="IResponse{TValue}"/> to an HATEOAS resource wrapped in an HTTP response.
    /// </summary>
    /// <typeparam name="TValue">Reponse value type.</typeparam>
    /// <typeparam name="TResource">Resource type.</typeparam>
    /// <returns>An HTTP reponse</returns>
    public static async Task<IResult> ToResourceAsync<TValue, TResource>(
        this Task<IResponse<TValue>>       task,
        Func<ISuccess<TResource>, IResult> onSuccess
    ) where TResource : IResource<TResource, TValue> =>
        (await task).ToResource(onSuccess);

    /// <summary>
    /// Converts a <see cref="IResponse{IEnumerable{TValue}}"/> to an array of HATEOAS resource swrapped in an HTTP response.
    /// </summary>
    /// <typeparam name="TValue">Reponse value type.</typeparam>
    /// <typeparam name="TResource">Resource type.</typeparam>
    /// <returns>An HTTP reponse</returns>
    public static async Task<IResult> ToResourceAsync<TValue, TResource>(
        this Task<IResponse<IEnumerable<TValue>>>       task,
        Func<ISuccess<IEnumerable<TResource>>, IResult> onSuccess
    ) where TResource : IResource<TResource, TValue> =>
        (await task).ToResource(onSuccess);

    /// <summary>
    /// Converts a <see cref="IEnumerable{TValue}"/> to an array of HATEOAS resource swrapped in an HTTP response.
    /// </summary>
    /// <typeparam name="TValue">Value type.</typeparam>
    /// <typeparam name="TResource">Resource type.</typeparam>
    /// <returns>An HTTP reponse</returns>
    public static async Task<IResult> ToResourceAsync<TValue, TResource>(
        this Task<IEnumerable<TValue>>                  task,
        Func<ISuccess<IEnumerable<TResource>>, IResult> transform
    ) where TResource : IResource<TResource, TValue> =>
        (await task).ToResource(transform);
}

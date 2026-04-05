using System.Security;
using ReSR.Application.Exceptions;

namespace ReSR.Presentation.Api.Core.Extensions;
public static partial class Extensions {

    /// <summary>
    /// Converts an <see cref="Exception"/> to an HTTP response.
    /// </summary>
    /// <param name="value">Optional http response body.</param>
    /// <returns>An HTTP response.</returns>
    public static IResult ToResult(this Exception self, object? value = default) =>
        self switch {
            UnauthorizedAccessException => Results.Json(value, statusCode: StatusCodes.Status401Unauthorized),
            SecurityException           => Results.Json(value, statusCode: StatusCodes.Status403Forbidden),
            ArgumentException           => Results.BadRequest(value),
            EntityConflictException     => Results.Conflict(value),
            KeyNotFoundException        => Results.NotFound(value),
            EntityNotFoundException     => Results.NotFound(value),
            _                           => Results.Json(value, statusCode: StatusCodes.Status500InternalServerError)
        };
}
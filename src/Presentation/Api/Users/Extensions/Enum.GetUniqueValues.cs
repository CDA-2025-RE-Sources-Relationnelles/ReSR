using System.Security.Claims;

namespace ReSR.Presentation.Api.Users.Extensions;
internal static partial class ClaimsPrincipalExtensions {
    
    /// <summary>
    /// Retrieves the user ID from the "sub" claim (ClaimTypes.NameIdentifier).
    /// </summary>
    /// <param name="principal">The ClaimsPrincipal instance.</param>
    /// <returns>The user ID, or null if the claim is missing.</returns>
    public static Id? GetUserId(this ClaimsPrincipal principal) =>
        Id.TryParse(principal.FindFirstValue(ClaimTypes.NameIdentifier), out Id result)
        ? result
        : null;
}
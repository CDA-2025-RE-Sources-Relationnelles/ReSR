using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;

namespace ReSR.Application.Core;
public static partial class Extensions {

    /// <summary>
    /// Initializes all the application's services.
    /// </summary>
    /// <param name="self">The app builder.</param>
    public static void InitApplication(this WebApplicationBuilder builder) {
        builder.Configuration.AddEnvironmentVariables();
    }
}
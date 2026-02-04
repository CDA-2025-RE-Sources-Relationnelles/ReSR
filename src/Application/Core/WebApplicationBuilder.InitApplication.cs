using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using ReSR.Domain.Core;
using ReSR.Application.Services;

namespace ReSR.Application.Core;
public static partial class Extensions {

    /// <summary>
    /// Initializes all the application's services.
    /// </summary>
    /// <param name="self">The app builder.</param>
    public static void InitApplication(this WebApplicationBuilder builder) {
        
        // Searching for the solution root.
        var assemblyLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        if (assemblyLocation is not null) {

            var directory = new DirectoryInfo(assemblyLocation);
            while (directory is not null && directory.GetFiles("*.slnx").Length == 0)
                directory = directory.Parent;

            if (directory is not null)
                builder.Configuration
                    .SetBasePath(directory.FullName)
                    .AddEnvironmentVariables()
                    .AddJsonFile(
                        path     : $"appsettings.shared{(builder.Environment.IsDevelopment() ? ".Development" : "")}.json",
                        optional : false
                    );
        }

        builder.Services.AddScoped<IDomainEventDispatcher, DomainEventsDispatcher>();
        builder.Services.AddHostedService<UserAnonymizationService>();
        builder.Services.AddHostedService<CommentReportForgivenessService>();

    }
}
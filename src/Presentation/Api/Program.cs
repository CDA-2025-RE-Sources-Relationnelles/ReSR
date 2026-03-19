using ReSR.Application.Core;
using ReSR.Infrastructure.Core;
using ReSR.Presentation.Api.Core.Extensions;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try {

    var builder = WebApplication.CreateBuilder(args);

    var logsPath = Path.Combine(builder.Environment.ContentRootPath, "logs", "resr-.log");

    builder.Host.UseSerilog((context, services, configuration) =>
        configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .WriteTo.File(
                path            : logsPath,
                rollingInterval : Serilog.RollingInterval.Day,
                retainedFileCountLimit : 30,
                outputTemplate  : "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}"
            )
    );

    builder.InitInfrastructure();
    builder.InitApplication();
    builder.InitPresentation();

    var app = builder.Build();

    app.FinalizePresentation(args);

    app.Run();

} catch (Exception ex) {
    Log.Fatal(ex, "L'application s'est terminée de manière inattendue.");
} finally {
    Log.CloseAndFlush();
}

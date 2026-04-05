using Serilog;

namespace ReSR.Presentation.Api.Core.Extensions;
public static partial class Extensions {

    /// <summary>
    /// Runs the presentations.
    /// </summary>
    /// <param name="self">The app builder.</param>
    public static void RunPresentation(this WebApplication app) {
        
        try { app.Run(); }
        catch (Exception ex) { Log.Fatal(ex, "L'application s'est terminée de manière inattendue."); }
        finally { Log.CloseAndFlush(); }

    }
}
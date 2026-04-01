namespace ReSR.Presentation.Api.Core.Extensions;
public static partial class Extensions {

    /// <summary>
    /// Finalizes all the presentations's services.
    /// </summary>
    /// <param name="self">The app builder.</param>
    public static void FinalizePresentation(this WebApplication app, params string[] args) {
        
        if (app.Environment.IsDevelopment()) {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseHsts();
        app.UseCors("AllowAll");
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseRateLimiter();
        app.MapControllers();

        if (args.Any(a =>
            string.Equals(a, "--new-test-db", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(a, "-n", StringComparison.OrdinalIgnoreCase))
        ) app.InitDb(force: args.Any(a =>
            string.Equals(a, "--force-init", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(a, "-f", StringComparison.OrdinalIgnoreCase)
        ));
    }
}
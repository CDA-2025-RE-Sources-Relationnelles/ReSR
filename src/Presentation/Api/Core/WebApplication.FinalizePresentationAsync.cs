namespace ReSR.Presentation.Api.Core;
public static partial class Extensions {

    /// <summary>
    /// Finalizes all the presentations's services.
    /// </summary>
    /// <param name="self">The app builder.</param>
    public static void FinalizePresentation(this WebApplication app) {
        
        if (app.Environment.IsDevelopment()) {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseHsts();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseCors("AllowAll");
        app.UseRateLimiter();
        app.MapControllers();
    }
}
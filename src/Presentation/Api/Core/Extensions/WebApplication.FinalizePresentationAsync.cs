using FluentResponse;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Domain.Aggregates.Accounts.ValueObjects;
using ReSR.Domain.Ports;

namespace ReSR.Presentation.Api.Core.Extensions;
public static partial class Extensions {

    /// <summary>
    /// Finalizes all the presentations's services.
    /// </summary>
    /// <param name="self">The app builder.</param>
    public static async Task FinalizePresentationAsync(this WebApplication app) {
        
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

        using var scope = app.Services.CreateScope();
        await scope.ServiceProvider
            .GetRequiredService<IRepository<Manager>>()
            .TryAddAsync(Manager.TryCreate(app.Configuration["Root:Email"]!, app.Configuration["Root:Password"]!, ManagerPermissions.SuperAdminRole).Unwrap());
    }
}
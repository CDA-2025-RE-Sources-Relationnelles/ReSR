using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;

namespace ReSR.Infrastructure.Core;
public static partial class Extensions {

    /// <summary>
    /// Initializes all the infrastructure's services.
    /// </summary>
    /// <param name="self">The app builder.</param>
    public static void InitInfrastructure(this WebApplicationBuilder builder) {

        builder.Services.AddDbContext<DbContext, ApplicationDbContext>(x =>
            x.UseNpgsql(builder.Configuration.GetConnectionString())
        );
    }
}
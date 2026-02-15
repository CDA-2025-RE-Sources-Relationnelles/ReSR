using System.Threading.RateLimiting;
using FluentResponse;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi;
using ReSR.Presentation.Api.Managers.Authorization;
using ReSR.Presentation.Api.Users.Authorization;

namespace ReSR.Presentation.Api.Core.Extensions;
public static partial class Extensions {

    /// <summary>
    /// Initializes all the presentations's services.
    /// </summary>
    /// <param name="self">The app builder.</param>
    public static void InitPresentation(this WebApplicationBuilder builder) {

        builder.Services.AddControllers();
        builder.Services.AddOpenApi();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddAuthorizationBuilder()
            .AddPolicy("LimitedUserAccess", policy => policy.Requirements.Add(new UserAuthorizationRequirement()))
            .AddPolicy("LimitedManagerAccess", policy => policy.Requirements.Add(new ManagerAuthorizationRequirement()));

        builder.Services.AddCors(options => {
            options.AddPolicy("AllowAll",
                builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
            );
        });

        builder.Services.AddRateLimiter(options => {
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey : httpContext.User.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
                    factory      : partition => new FixedWindowRateLimiterOptions {
                        AutoReplenishment = true,
                        PermitLimit       = httpContext.User.Identity?.IsAuthenticated == true ? 32 : 16,
                        QueueLimit        = 0,
                        Window            = TimeSpan.FromMinutes(1)
                    }
                )
            );
            options.OnRejected = async (context, cancellationToken) => {
                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                    context.HttpContext.Response.Headers.RetryAfter = retryAfter.TotalSeconds.ToString();

                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.HttpContext.Response.WriteAsJsonAsync(
                    value : Response.Failure("Trop de requêtes ! Réessayez plus tard.") as Failure,
                    cancellationToken : cancellationToken
                );
            };
        });

        builder.Services.AddScoped<IAuthorizationHandler, UserAuthorizationHandler>();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options => {

            options.SwaggerDoc("v1", new OpenApiInfo {
                Title   = "API RE(Sources) Relationnelles",
                Version = "v1"
            });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
                Name         = "Authorization",
                Type         = SecuritySchemeType.Http,
                Scheme       = "Bearer",
                BearerFormat = "JWT",
                In           = ParameterLocation.Header,
                Description  = "Entrez votre token JWT."
            });

            options.AddSecurityRequirement(document => new() { [new OpenApiSecuritySchemeReference("Bearer", document)] = [] });
        });
    }
}
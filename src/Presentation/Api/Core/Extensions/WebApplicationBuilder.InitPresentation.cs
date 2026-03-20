using System.Text;
using System.Threading.RateLimiting;
using FluentResponse;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Presentation.Api.Managers.Authorization;
using ReSR.Presentation.Api.Users.Authorization;
using Serilog;

namespace ReSR.Presentation.Api.Core.Extensions;
public static partial class Extensions {

    /// <summary>
    /// Initializes all the presentations's services.
    /// </summary>
    /// <param name="self">The app builder.</param>
    public static void InitPresentation(this WebApplicationBuilder builder) {

        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateBootstrapLogger();

        var logsPath = Path.Combine(builder.Environment.ContentRootPath, "logs", "resr-.log");
        builder.Host.UseSerilog((context, services, configuration) =>
            configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
                .WriteTo.File(
                    path            : logsPath,
                    rollingInterval : RollingInterval.Day,
                    retainedFileCountLimit : 30,
                    outputTemplate  : "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}"
                )
        );

        builder.Services.AddControllers();
        builder.Services.AddOpenApi();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddHttpContextAccessor();

        builder.Services.AddCors(options => {
            options.AddPolicy("AllowAll",
                builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
            );
        });

        builder.Services.AddAuthorizationBuilder()
            .AddPolicy(nameof(UserSessionAuthorizationRequirement), policy => { policy.RequireRole(nameof(User)); policy.Requirements.Add(new UserSessionAuthorizationRequirement()); })
            .AddPolicy(nameof(ManagerSessionAuthorizationRequirement), policy => { policy.RequireRole(nameof(Manager)); policy.Requirements.Add(new ManagerSessionAuthorizationRequirement()); })
            .AddPolicy(nameof(ResourceWriteAuthorizationRequirement), policy => { policy.RequireRole(nameof(User)); policy.Requirements.Add(new ResourceWriteAuthorizationRequirement()); })
            .AddPolicy(nameof(QuizSessionAuthorizationRequirement), policy => { policy.RequireRole(nameof(User)); policy.Requirements.Add(new QuizSessionAuthorizationRequirement()); })
            .AddPolicy(nameof(ResourceReadAuthorizationRequirement), policy => { policy.Requirements.Add(new ResourceReadAuthorizationRequirement()); })
            .AddPolicy(nameof(CommentReadAuthorizationRequirement), policy => { policy.Requirements.Add(new CommentReadAuthorizationRequirement()); })
            .AddPolicy("BackOffice", policy => policy.RequireRole(nameof(Manager)))
            .AddDefaultPolicy("FrontOffice", policy => policy.RequireRole(nameof(User)));
        
        builder.Services.AddAuthentication(options => {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options => {
            options.TokenValidationParameters = new TokenValidationParameters {
                ValidateIssuer           = true,
                ValidateAudience         = true,
                ValidateLifetime         = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer              = builder.Configuration["Jwt:Issuer"]!,
                ValidAudience            = builder.Configuration["Jwt:Audience"]!,
                IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
            };
        });

        builder.Services.AddRateLimiter(options => {
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey : httpContext.User.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
                    factory      : partition => new FixedWindowRateLimiterOptions {
                        AutoReplenishment = true,
                        PermitLimit       = httpContext.User.Identity?.IsAuthenticated == true ? 64 : 32,
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

        builder.Services.AddScoped<IAuthorizationHandler, UserSessionAuthorizationHandler>();
        builder.Services.AddScoped<IAuthorizationHandler, ResourceWriteAuthorizationHandler>();
        builder.Services.AddScoped<IAuthorizationHandler, ResourceReadAuthorizationHandler>();
        builder.Services.AddScoped<IAuthorizationHandler, CommentReadAuthorizationHandler>();
        builder.Services.AddScoped<IAuthorizationHandler, QuizSessionAuthorizationHandler>();
        builder.Services.AddScoped<IAuthorizationHandler, ManagerSessionAuthorizationHandler>();
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
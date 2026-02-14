using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Domain.Aggregates.Categories;
using ReSR.Domain.Aggregates.Messages;
using ReSR.Infrastructure.Adapters.Repositories;
using ReSR.Domain.Aggregates.QuizSessions;
using ReSR.Domain.Aggregates.Resources;
using ReSR.Application.Ports;
using ReSR.Infrastructure.Adapters;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using System.Text;
using ReSR.Domain.Ports;
using Microsoft.AspNetCore.Authentication.JwtBearer;

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

        builder.Services
            .AddAuthentication(options => {
                options.DefaultAuthenticateScheme = nameof(User);
                options.DefaultChallengeScheme    = nameof(User);
            }).AddJwtBearer(nameof(User), options => {
                options.TokenValidationParameters = new TokenValidationParameters {
                    ValidateIssuer           = true,
                    ValidateAudience         = true,
                    ValidateLifetime         = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer              = builder.Configuration["Jwt:Issuer"],
                    ValidAudience            = builder.Configuration["Jwt:Audience"],
                    ClockSkew                = TimeSpan.Zero,
                    IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key:User"]!))
                };
            }).AddJwtBearer(nameof(Manager), options => {
                options.TokenValidationParameters = new TokenValidationParameters {
                    ValidateIssuer           = true,
                    ValidateAudience         = true,
                    ValidateLifetime         = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer              = builder.Configuration["Jwt:Issuer"],
                    ValidAudience            = builder.Configuration["Jwt:Audience"],
                    ClockSkew                = TimeSpan.Zero,
                    IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key:Manager"]!))
                };
            });

        builder.Services.AddAuthorizationBuilder()
            .SetDefaultPolicy(new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .AddAuthenticationSchemes(nameof(User))
                .Build())
            .AddPolicy(nameof(Manager), new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .AddAuthenticationSchemes(nameof(Manager))
                .Build());

        builder.Services.AddScoped<IAccountRepository<Manager>, AccountRepository<Manager>>();
        builder.Services.AddScoped<IAccountRepository<User>,    AccountRepository<User>>();
        builder.Services.AddScoped<IRepository<Manager>>(x => x.GetRequiredService<IAccountRepository<Manager>>());
        builder.Services.AddScoped<IRepository<User>>(x => x.GetRequiredService<IAccountRepository<User>>());

        builder.Services.AddScoped<IRepository<Category>, CategoryRepository>();

        builder.Services.AddScoped<IRepository<Comment>,        CommentRepository>();
        builder.Services.AddScoped<IRepository<PrivateMessage>, PrivateMessageRepository>();

        builder.Services.AddScoped<IRepository<QuizSession>, QuizSessionRepository>();

        builder.Services.AddScoped<IRepository<Resource>, ResourceRepository<Resource>>();
        builder.Services.AddScoped<IRepository<TextResource>, ResourceRepository<TextResource>>();
        builder.Services.AddScoped<IRepository<QuizResource>, ResourceRepository<QuizResource>>();

        builder.Services.AddScoped<IEncryptionService, EncryptionService>();

        builder.Services.AddScoped<IAccountAuthService<Manager>, ManagerAuthService>();
        builder.Services.AddScoped<IAccountAuthService<User>,    UserAuthService>();

        builder.Services.AddSingleton<IPasswordResetCacheService,          PasswordResetCacheService>();
        builder.Services.AddSingleton<IRegistrationValidationCacheService, RegistrationValidationCacheService>();

        builder.Services.AddScoped<IMailService, MailService>();

    }
}
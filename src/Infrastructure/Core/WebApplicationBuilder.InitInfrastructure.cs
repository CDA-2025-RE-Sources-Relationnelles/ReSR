using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using ReSR.Domain.Core;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Domain.Aggregates.Categories;
using ReSR.Domain.Aggregates.Messages;
using ReSR.Infrastructure.Adapters.Repositories;
using ReSR.Domain.Aggregates.QuizSessions;
using ReSR.Domain.Aggregates.Resources;
using ReSR.Application.Ports;
using ReSR.Infrastructure.Adapters;

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

        builder.Services.AddScoped<IRepository<Manager>, AccountRepository<Manager>>();
        builder.Services.AddScoped<IRepository<User>,    AccountRepository<User>>();

        builder.Services.AddScoped<IRepository<Category>, CategoryRepository>();

        builder.Services.AddScoped<IRepository<Comment>,        CommentRepository>();
        builder.Services.AddScoped<IRepository<PrivateMessage>, PrivateMessageRepository>();

        builder.Services.AddScoped<IRepository<QuizSession>, QuizSessionRepository>();

        builder.Services.AddScoped<IRepository<Resource>, ResourceRepository<Resource>>();
        builder.Services.AddScoped<IRepository<TextResource>, ResourceRepository<TextResource>>();
        builder.Services.AddScoped<IRepository<QuizResource>, ResourceRepository<QuizResource>>();

        builder.Services.AddScoped<IEncryptionService, EncryptionService>();

    }
}
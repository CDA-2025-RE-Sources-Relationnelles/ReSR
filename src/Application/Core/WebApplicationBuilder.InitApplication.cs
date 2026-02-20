using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using ReSR.Application.EventListeners.Accounts;
using ReSR.Domain.Aggregates.Accounts.Events;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Domain.Aggregates.Resources.Events;
using ReSR.Application.EventListeners.Resources;
using ReSR.Application.Services.Core.Implementations;
using ReSR.Domain.Ports;
using ReSR.Application.Services.Users.Definitions;
using ReSR.Application.Services.Users.Implementations;
using ReSR.Application.Services.Core.Definitions;
using ReSR.Application.Services.Managers.Implementations;
using ReSR.Application.Services.Managers.Definitions;

namespace ReSR.Application.Core;
public static partial class Extensions {

    /// <summary>
    /// Initializes all the application's services.
    /// </summary>
    /// <param name="self">The app builder.</param>
    public static void InitApplication(this WebApplicationBuilder builder) {
        
        // Searching for the solution root.
        var assemblyLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        if (assemblyLocation is not null) {

            var directory = new DirectoryInfo(assemblyLocation);
            while (directory is not null && directory.GetFiles("*.slnx").Length == 0)
                directory = directory.Parent;

            if (directory is not null)
                builder.Configuration
                    .SetBasePath(directory.FullName)
                    .AddEnvironmentVariables()
                    .AddJsonFile(
                        path     : $"appsettings.shared{(builder.Environment.IsDevelopment() ? ".Development" : "")}.json",
                        optional : false
                    );
        }

        builder.Services.AddScoped<IDomainEventDispatcher, DomainEventsDispatcher>();

        builder.Services.AddScoped<IDomainEventListener<AccountCreated<Manager>>,      ManagerAccountCreatedListener>();
        builder.Services.AddScoped<IDomainEventListener<AccountEmailChanged<Manager>>, ManagerEmailChangedListener>();

        builder.Services.AddScoped<IDomainEventListener<AccountCreated<User>>,            UserAccountCreatedListener>();
        builder.Services.AddScoped<IDomainEventListener<UserAnonymizationProcessStarted>, UserAnonymizationProcessStartedListener>();
        builder.Services.AddScoped<IDomainEventListener<UserAnonymized>,                  UserAnonymizedListener>();
        builder.Services.AddScoped<IDomainEventListener<AccountEmailChanged<User>>,       UserEmailChangedListener>();
        builder.Services.AddScoped<IDomainEventListener<UserMutuallyLiked>,               UserMutuallyLikedListener>();
        builder.Services.AddScoped<IDomainEventListener<UserSuspensionChanged>,           UserSuspensionChangedListener>();

        builder.Services.AddScoped<IDomainEventListener<ResourceRejected>, ResourceRejectedListener>();
        builder.Services.AddScoped<IDomainEventListener<ResourceVerified>, ResourceVerifiedListener>();


        builder.Services.AddScoped<IUserSessionService, UserSessionService>();
        builder.Services.AddScoped<ISessionService<Manager>, ManagerSessionService>();


        builder.Services.AddScoped<IManagerCommandService, ManagerCommandService>();
        builder.Services.AddScoped<IUserCommandService, UserCommandService>();

        builder.Services.AddScoped<ICategoryCommandService, CategoryCommandService>();

        builder.Services.AddScoped<ITextResourceCommandService, TextResourceCommandService>();
        builder.Services.AddScoped<IQuizResourceCommandService, QuizResourceCommandService>();


        builder.Services.AddScoped<IUserKpiService, UserKpiService>();
        builder.Services.AddScoped<IResourceKpiService, ResourceKpiService>();


        builder.Services.AddScoped<IResourceService, ResourceService>();


        builder.Services.AddHostedService<UserAnonymizationService>();
        builder.Services.AddHostedService<CommentReportForgivenessService>();

    }
}
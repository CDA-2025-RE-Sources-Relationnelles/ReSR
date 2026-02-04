using ReSR.Domain.Aggregates.Accounts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ReSR.Domain.Core;

namespace ReSR.Application.Services;
internal class UserAnonymizationService(
    IServiceProvider                  serviceProvider,
    ILogger<UserAnonymizationService> logger
) : BackgroundService {

    #region PROPERTIES

        private readonly PeriodicTimer timer = new(TimeSpan.FromDays(1));

    #endregion
    #region METHODS
        
        protected override async Task ExecuteAsync(CancellationToken cancellationToken) {

            logger.LogInformation($"Service running.");
            await HandleAsync();
            while (!cancellationToken.IsCancellationRequested && await timer.WaitForNextTickAsync(cancellationToken))
                await HandleAsync();
        }

        public async Task HandleAsync() {

            logger.LogInformation($"Parsing users.");

            using var scope = serviceProvider.CreateScope();
            var repository  = scope.ServiceProvider.GetRequiredService<IRepository<User>>();

            var now = DateTime.UtcNow;

            var inactiveUsers = (await repository.GetAllAsync())
                .Where(x => MonthsSince(x.LastActivity, now) >= 35 && !x.IsAnonymous);

            foreach (var inactiveUser in inactiveUsers)
                if (inactiveUser.AnonymizationProcessStartedAt is DateTime anonymizationProcessStartedAt && anonymizationProcessStartedAt.AddMonths(1) <= now)
                    await repository.TryUpdateAsync(inactiveUser.Id, x => x.AsAnonymized());

                else if (inactiveUser.AnonymizationProcessStartedAt is null)
                    await repository.TryUpdateAsync(inactiveUser.Id, x => x.TryWithNewAnonymizationProcess());
        }

        private static int MonthsSince(DateTime previous, DateTime now) =>
            (now.Year - previous.Year) * 12 + now.Month - previous.Month + (now.Day >= previous.Day ? 0 : -1);

    #endregion

}
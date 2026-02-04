using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ReSR.Domain.Core;
using ReSR.Domain.Aggregates.Messages;
using FluentResponse;

namespace ReSR.Application.Services.Implementations;
internal class CommentReportForgivenessService(
    IServiceProvider                         serviceProvider,
    ILogger<CommentReportForgivenessService> logger
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

            logger.LogInformation($"Parsing comment reports.");

            using var scope = serviceProvider.CreateScope();
            var repository  = scope.ServiceProvider.GetRequiredService<IRepository<Comment>>();

            var now = DateTime.UtcNow;

            foreach (var comment in await repository.GetAllAsync(x => x.Reports.Count != 0)) {
                foreach (var report in comment.Reports) {
                    if (MonthsSince(report.ReportedAt, now) >= 6)
                        await repository
                            .TryUpdateAsync(comment.Id, comment => comment.WithoutReport(report.ReportedBy))
                            .OnFailureAsync(e => logger.LogWarning("Unable to forgive comment report {Report}! ", report));
                }
            }
        }

        private static int MonthsSince(DateTime previous, DateTime now) =>
            (now.Year - previous.Year) * 12 + now.Month - previous.Month + (now.Day >= previous.Day ? 0 : -1);

    #endregion

}
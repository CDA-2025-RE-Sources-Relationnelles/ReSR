using ReSR.Application.Services.Managers.Definitions;
using ReSR.Application.ValueObjects.Accounts;
using ReSR.Application.ValueObjects.Core;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Domain.Ports;

namespace ReSR.Application.Services.Managers.Implementations;
internal class UserKpiService(
    IRepository<User> repository
) : IUserKpiService {

    public async Task<UserKpi> GetAsync(DateRange lastActivityFilter) {
        DateTime? ignoreOlder = lastActivityFilter switch {
            DateRange.Daily     => DateTime.UtcNow.AddDays(-1),
            DateRange.Weekly    => DateTime.UtcNow.AddDays(-7),
            DateRange.Monthly   => DateTime.UtcNow.AddMonths(-1),
            DateRange.Quarterly => DateTime.UtcNow.AddMonths(-3),
            DateRange.Yearly    => DateTime.UtcNow.AddYears(-1),
            _ => null,
        };

        var users = ignoreOlder is null
            ? await repository.GetAllAsync()
            : await repository.GetAllAsync(x => x.LastActivity >= ignoreOlder);

        var friendships = new HashSet<(Id, Id)>();

        foreach (var user in users)
            foreach (var friend in user.Friends) {
                Id a = Math.Min(user.Id, friend.Id);
                Id b = Math.Max(user.Id, friend.Id);

                friendships.Add((a, b));
            }

        return new UserKpi(
            DateRange       : lastActivityFilter,
            Count           : users.Count(),
            FriendshipCount : friendships.Count,
            BookmarkCount   : users.Sum(x => x.Bookmarks.Count),
            ResourceCount   : users.Sum(x => x.OwnedResources.Count)
        );
    }

    public async IAsyncEnumerable<UserKpi> GetReportAsync() {

        var users = await repository.GetAllAsync();
        foreach (var dateRange in new DateRange[] { DateRange.Monthly, DateRange.Quarterly, DateRange.Yearly }) {

            DateTime ignoreOlder = dateRange switch {
                DateRange.Monthly   => DateTime.UtcNow.AddMonths(-1),
                DateRange.Quarterly => DateTime.UtcNow.AddMonths(-3),
                DateRange.Yearly    => DateTime.UtcNow.AddYears(-1),
                _ => DateTime.UnixEpoch
            };

            var section = users.Where(x => x.LastActivity >= ignoreOlder);
            var friendships = new HashSet<(Id, Id)>();
            foreach (var user in section)
                foreach (var friend in user.Friends) {
                    Id a = Math.Min(user.Id, friend.Id);
                    Id b = Math.Max(user.Id, friend.Id);

                    friendships.Add((a, b));
                }

            yield return new UserKpi(
                DateRange       : dateRange,
                Count           : section.Count(),
                FriendshipCount : friendships.Count,
                BookmarkCount   : section.Sum(x => x.Bookmarks.Count),
                ResourceCount   : section.Sum(x => x.OwnedResources.Count)
            );
        }
    }
}

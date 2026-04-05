using FluentResponse;
using ReSR.Application.Services.Managers.Definitions;
using ReSR.Application.ValueObjects.Core;
using ReSR.Application.ValueObjects.Resources;
using ReSR.Domain.Aggregates.Categories;
using ReSR.Domain.Aggregates.Resources;
using ReSR.Domain.Aggregates.Resources.ValueObjects;
using ReSR.Domain.Ports;

namespace ReSR.Application.Services.Managers.Implementations;
internal class ResourceKpiService(
    IRepository<Resource> resourceRepository,
    IRepository<Category> categoryRepository
) : IResourceKpiService {

    public async Task<ResourceKpi> GetAsync(
        DateRange lastEditionFilter,
        Relationships relationshipsFilter,
        Visibility visibilityFilter,
        Id? categoryIdFilter
    ) {
        DateTime? ignoreOlder = lastEditionFilter switch {
            DateRange.Daily     => DateTime.UtcNow.AddDays(-1),
            DateRange.Weekly    => DateTime.UtcNow.AddDays(-7),
            DateRange.Monthly   => DateTime.UtcNow.AddMonths(-1),
            DateRange.Quarterly => DateTime.UtcNow.AddMonths(-3),
            DateRange.Yearly    => DateTime.UtcNow.AddYears(-1),
            _ => null,
        };

        var resources = ignoreOlder is null
            ? await resourceRepository.GetAllAsync(x =>
                (x.Relationships & relationshipsFilter) == x.Relationships &&
                (x.Visibility & visibilityFilter) == x.Visibility &&
                (categoryIdFilter == null || x.Category.Id == categoryIdFilter)
            ) : await resourceRepository.GetAllAsync(x =>
                x.EditedAt >= ignoreOlder &&
                (x.Relationships & relationshipsFilter) == x.Relationships &&
                (x.Visibility & visibilityFilter) == x.Visibility &&
                (categoryIdFilter == null || x.Category.Id == categoryIdFilter)
            );

        return new ResourceKpi(
            DateRange     : lastEditionFilter,
            Relationship  : relationshipsFilter,
            CategoryName  : categoryIdFilter is not null ? await categoryRepository.TryGetAsync(categoryIdFilter.Value).UnwrapAsync<Category, string?>(x => x.Name, _ => null) : null,
            Count         : resources.Count(),
            LikeCount     : resources.Sum(x => x.LikeCount),
            BookmarkCount : resources.Sum(x => x.BookmarkCount),
            ExploitCount  : resources.Sum(x => x.ExploitCount),
            CommentCount  : resources.Sum(x => x.Comments.Count)
        );
    }

    public async IAsyncEnumerable<ResourceKpi> GetReportAsync() {

        var resources = await resourceRepository.GetAllAsync();
        foreach (var dateRange in new DateRange[] { DateRange.Monthly, DateRange.Quarterly, DateRange.Yearly })
            foreach (var relationship in Enum.GetValues<Relationships>())
                foreach (var category in await categoryRepository.GetAllAsync()) {

                    DateTime ignoreOlder = dateRange switch {
                        DateRange.Monthly   => DateTime.UtcNow.AddMonths(-1),
                        DateRange.Quarterly => DateTime.UtcNow.AddMonths(-3),
                        DateRange.Yearly    => DateTime.UtcNow.AddYears(-1),
                        _ => DateTime.UnixEpoch
                    };

                    var section = resources.Where(x =>
                        x.EditedAt >= ignoreOlder &&
                        x.Relationships.HasFlag(relationship) &&
                        x.Category.Id == category.Id
                    );
                    
                    yield return new ResourceKpi(
                        DateRange     : dateRange,
                        Relationship  : relationship,
                        CategoryName  : category.Name,
                        Count         : section.Count(),
                        LikeCount     : section.Sum(x => x.LikeCount),
                        BookmarkCount : section.Sum(x => x.BookmarkCount),
                        ExploitCount  : section.Sum(x => x.ExploitCount),
                        CommentCount  : section.Sum(x => x.Comments.Count)
                    );
                }
    }
}

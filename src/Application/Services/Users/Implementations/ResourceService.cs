using ReSR.Application.Services.Users.Definitions;
using ReSR.Domain.Aggregates.Resources;
using ReSR.Domain.Aggregates.Resources.ValueObjects;
using ReSR.Domain.Ports;

namespace ReSR.Application.Services.Users.Implementations;

public sealed class ResourceService(
    IRepository<Resource> repository
) : IResourceService {

    public Task<IEnumerable<Resource>> GetAllPublic(
        Id? categoryIdFilter,
        Relationships relationshipsFilter
    ) => repository.GetAllAsync(x =>
        x.Visibility == Visibility.Public &&
        (categoryIdFilter == null || x.Category.Id == categoryIdFilter) &&
        (x.Relationships & relationshipsFilter) == x.Relationships
    );

    public async Task<IEnumerable<Resource>> TryGetAllPrivate(
        Id userId,
        Id? categoryIdFilter,
        Relationships relationshipsFilter
    ) => (await repository.GetAllAsync(x =>
        x.Visibility == Visibility.Private &&
        (categoryIdFilter == null || x.Category.Id == categoryIdFilter) &&
        (x.Relationships & relationshipsFilter) == x.Relationships &&
        x.Owner != null
    )).Where(x => x.Owner!.Id == userId || x.Owner.Friends.Any(x => x.Id == userId));

    public Task<IEnumerable<Resource>> TryGetAllWaitingForVerification(Id userId) => repository.GetAllAsync(x =>
        x.Visibility == Visibility.WaitingForVerification &&
        (x.VerifyingUser == null || x.VerifyingUser.Id == userId)
    );
}
using FluentResponse;
using FluentResponse.Interfaces;
using ReSR.Application.Services.Users.Definitions;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Domain.Aggregates.Messages;
using ReSR.Domain.Aggregates.Resources;
using ReSR.Domain.Aggregates.Resources.ValueObjects;
using ReSR.Domain.Core;
using ReSR.Domain.Ports;

namespace ReSR.Application.Services.Users.Implementations;

public class ResourceService<T>(
    IRepository<T> resourceRepository,
    IRepository<User> userRepository,
    IRepository<Comment> commentRepository
) : IResourceService<T> where T : Resource, IAggregateRoot<T> {

    protected readonly IRepository<T> resourceRepository = resourceRepository;
    protected readonly IRepository<User> userRepository = userRepository;
    protected readonly IRepository<Comment> commentRepository = commentRepository;

    public Task<IEnumerable<T>> GetAllPublicAsync(
        Id? categoryIdFilter,
        Relationships relationshipsFilter
    ) => resourceRepository.GetAllAsync(x =>
        x.Visibility == Visibility.Public &&
        (categoryIdFilter == null || x.Category.Id == categoryIdFilter) &&
        (x.Relationships & relationshipsFilter) == x.Relationships
    );

    public Task<IResponse<IEnumerable<T>>> GetAllPrivateAsync(
        Id userId,
        Id? categoryIdFilter,
        Relationships relationshipsFilter
    ) => userRepository.TryGetAsync(userId).OnSuccessAsync(async user =>
        (await resourceRepository.GetAllAsync(x =>
            x.Visibility == Visibility.Private &&
            (categoryIdFilter == null || x.Category.Id == categoryIdFilter) &&
            (x.Relationships & relationshipsFilter) == x.Relationships &&
            x.Owner != null
        )).Where(x => x.Owner!.Id == userId || user.Friends.Any(y => y.Id == x.Owner.Id))
    );

    public Task<IEnumerable<T>> GetAllWaitingForVerificationAsync() => resourceRepository.GetAllAsync(x =>
        x.Visibility == Visibility.WaitingForVerification
    );

    public Task<IResponse<T>> TryConfirmVerificationAsync(Id resourceId) =>
        resourceRepository.TryUpdateAsync(resourceId, x => x.TryWithConfirmedVerification().OnSuccess(x => (T)x));

    public Task<IResponse<T>> TryRejectVerificationAsync(Id resourceId) =>
        resourceRepository.TryUpdateAsync(resourceId, x => x.TryWithRejectedVerification().OnSuccess(x => (T)x));

    public Task<IResponse<Comment>> TryPostCommentAsync(Id resourceId, Id posterId, string content) =>
        resourceRepository.TryGetAsync(resourceId).OnSuccessAsync(resource =>
            userRepository.TryGetAsync(posterId).OnSuccessAsync(poster =>
                Comment.TryCreate(poster, content, resource)
            )
        ).OnSuccessAsync(commentRepository.TryAddAsync);

    public Task<IResponse<T>> TryLikeAsync(Id id, Id fromId, bool value) =>
        userRepository.TryGetAsync(fromId).OnSuccessAsync(from =>
            resourceRepository.TryUpdateAsync(id, x => (T)x.WithLikeFrom(from, value))
        );

    public Task<IResponse<T>> TryBookmarkAsync(Id id, Id fromId, bool value) =>
        userRepository.TryGetAsync(fromId).OnSuccessAsync(from =>
            resourceRepository.TryUpdateAsync(id, x => (T)x.WithBookmarkFrom(from, value))
        );

    public Task<IResponse<T>> TryExploitAsync(Id id, Id fromId, bool value) =>
        userRepository.TryGetAsync(fromId).OnSuccessAsync(from =>
            resourceRepository.TryUpdateAsync(id, x => (T)x.WithExploitFrom(from, value))
        );
}
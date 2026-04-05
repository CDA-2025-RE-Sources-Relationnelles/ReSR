using FluentResponse;
using FluentResponse.Interfaces;
using ReSR.Application.Services.Users.Definitions;
using ReSR.Application.ValueObjects.Resources;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Domain.Aggregates.Messages;
using ReSR.Domain.Aggregates.Resources;
using ReSR.Domain.Aggregates.Resources.ValueObjects;
using ReSR.Domain.Core;
using ReSR.Domain.Ports;
using ReSR.Domain.Services.Implementations;

namespace ReSR.Application.Services.Users.Implementations;

public class ResourceService<T>(
    IResourceRepository<T> resourceRepository,
    IRepository<User> userRepository,
    IRepository<Comment> commentRepository
) : IResourceService<T> where T : Resource, IAggregateRoot<T> {

    protected readonly IResourceRepository<T> resourceRepository = resourceRepository;
    protected readonly IRepository<User> userRepository = userRepository;
    protected readonly IRepository<Comment> commentRepository = commentRepository;

    public async Task<IEnumerable<T>> GetAllPublicAsync(
        string?       titleSearch,
        Id?           categoryIdFilter,
        Relationships relationshipsFilter,
        OrderBy       orderBy
    ) {
        var resources = await resourceRepository.GetAllAsync(titleSearch, categoryIdFilter, relationshipsFilter, Visibility.Public);

        return orderBy switch {
            OrderBy.Newest    => resources.OrderByDescending(x => x.EditedAt),
            OrderBy.Oldest    => resources.OrderBy(x => x.EditedAt),
            OrderBy.LikeCount => resources.OrderByDescending(x => x.LikeCount),
            _ => resources
        };
    }

    public Task<IResponse<IEnumerable<T>>> TryGetAllPrivateAsync(
        Id            userId,
        string?       titleSearch,
        Id?           categoryIdFilter,
        Relationships relationshipsFilter,
        OrderBy       orderBy
    ) => userRepository.TryGetAsync(userId).OnSuccessAsync(async user => {

        var resources = (await resourceRepository
            .GetAllAsync(titleSearch, categoryIdFilter, relationshipsFilter, Visibility.Private)
        ).Where(x => x.Owner?.Id == userId || user.Friends.Any(y => y.Id == x.Owner?.Id));

        return orderBy switch {
            OrderBy.Newest    => resources.OrderByDescending(x => x.EditedAt),
            OrderBy.Oldest    => resources.OrderBy(x => x.EditedAt),
            OrderBy.LikeCount => resources.OrderByDescending(x => x.LikeCount),
            _ => resources
        };
    });

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

    public Task<IResponse<IEnumerable<Comment>>> TryGetCommentsAsync(Id resourceId) =>
        resourceRepository
            .TryGetAsync(resourceId)
            .OnSuccessAsync(_ => commentRepository.GetAllAsync(x =>
                x.CommentedResource.Id == resourceId &&
                x.AnsweredComment == null
            ));

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


    public Task<IResponse<IEnumerable<T>>> TryGetUserOwnedResources(Id id, Id? forUserId = null) =>
        userRepository.TryGetAsync(id).OnSuccessAsync(async user => {

            var resources = await resourceRepository.GetAllAsync(x => x.Owner != null && x.Owner.Id == id);
            return forUserId is not null
                ? await userRepository.TryGetAsync(forUserId.Value).OnSuccessAsync(forUser =>
                    resources.Where(x => UserPermissionsService.TryVerifyUserResourceAccess(forUser, x) is ISuccess)
                ) : Response.Success(resources.Where(x => x.Visibility == Visibility.Public));
        });

    public Task<IResponse<IEnumerable<T>>> TryGetUserBookmarkedResources(Id id) =>
        userRepository
            .TryGetAsync(id)
            .OnSuccessAsync(user => user.Bookmarks.Where(x => UserPermissionsService.TryVerifyUserResourceAccess(user, x) is ISuccess).Cast<T>());
}
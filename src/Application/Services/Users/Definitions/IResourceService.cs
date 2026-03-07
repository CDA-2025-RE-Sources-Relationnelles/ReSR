using FluentResponse.Interfaces;
using ReSR.Application.ValueObjects.Resources;
using ReSR.Domain.Aggregates.Messages;
using ReSR.Domain.Aggregates.Resources;
using ReSR.Domain.Aggregates.Resources.ValueObjects;

namespace ReSR.Application.Services.Users.Definitions;
public interface IResourceService<T> where T: Resource {

    /// <summary>
    /// Retrieves all public resources.
    /// </summary>
    /// <returns>The publicly available resources.</returns>
    /// <param name="categoryIdFilter">The identifier of the category filter used in the query.</param>
    /// <param name="relationshipsFilter">The relationships filter used in the query.</param>
    public Task<IEnumerable<T>> GetAllPublicAsync(
        string?       titleSearch         = null,
        Id?           categoryIdFilter    = null,
        Relationships relationshipsFilter = Relationships.None,
        OrderBy       orderBy             = OrderBy.Newest
    );

    /// <summary>
    /// Tries to retrieve all private resources for a given user.
    /// </summary>
    /// <returns>The private resources of the user and their friends.</returns>
    /// <param name="userId">The identifier of the user.</param>
    /// <param name="categoryIdFilter">The identifier of the category filter used in the query.</param>
    /// <param name="relationshipsFilter">The relationships filter used in the query.</param>
    public Task<IResponse<IEnumerable<T>>> TryGetAllPrivateAsync(
        Id            userId,
        string?       titleSearch         = null,
        Id?           categoryIdFilter    = null,
        Relationships relationshipsFilter = Relationships.None,
        OrderBy       orderBy             = OrderBy.Newest
    );

    /// <summary>
    /// Tries to retrieve all resources waiting for verification.
    /// </summary>
    /// <returns>The resources that are to be verified by moderators.</returns>
    public Task<IEnumerable<T>> GetAllWaitingForVerificationAsync();

    public Task<IResponse<T>> TryConfirmVerificationAsync(Id id);
    public Task<IResponse<T>> TryRejectVerificationAsync(Id id);
    public Task<IResponse<Comment>> TryPostCommentAsync(Id resourceId, Id posterId, string content);
    public Task<IResponse<IEnumerable<Comment>>> TryGetCommentsAsync(Id resourceId);

    public Task<IResponse<T>> TryLikeAsync(Id id, Id fromId, bool value = true);
    public Task<IResponse<T>> TryBookmarkAsync(Id id, Id fromId, bool value = true);
    public Task<IResponse<T>> TryExploitAsync(Id id, Id fromId, bool value = true);
}

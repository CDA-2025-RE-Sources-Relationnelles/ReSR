using FluentResponse.Interfaces;
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
    public Task<IEnumerable<T>> GetAllPublic(
        Id?           categoryIdFilter    = null,
        Relationships relationshipsFilter = Relationships.All
    );

    /// <summary>
    /// Tries to retrieve all private resources for a given user.
    /// </summary>
    /// <returns>The private resources of the user and their friends.</returns>
    /// <param name="userId">The identifier of the user.</param>
    /// <param name="categoryIdFilter">The identifier of the category filter used in the query.</param>
    /// <param name="relationshipsFilter">The relationships filter used in the query.</param>
    public Task<IEnumerable<T>> TryGetAllPrivate(
        Id            userId,
        Id?           categoryIdFilter    = null,
        Relationships relationshipsFilter = Relationships.All
    );

    /// <summary>
    /// Tries to retrieve all resources waiting for verification for a given user.
    /// </summary>
    /// <returns>The resources that are not being verified, or being verified by the user.</returns>
    /// <param name="userId">The identifier of the user.</param>
    public Task<IEnumerable<T>> TryGetAllWaitingForVerification(Id userId);

    public Task<IResponse<T>> TryConfirmVerification(Id id);
    public Task<IResponse<T>> TryRejectVerification(Id id);
    public Task<IResponse<T>> TryPostComment(Id resourceId, Id posterId, string content);
}

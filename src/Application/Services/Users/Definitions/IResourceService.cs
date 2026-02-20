using FluentResponse.Interfaces;
using ReSR.Domain.Aggregates.Resources;
using ReSR.Domain.Aggregates.Resources.ValueObjects;

namespace ReSR.Application.Services.Users.Definitions;
public interface IResourceService {

    /// <summary>
    /// Retrieves all public resources.
    /// </summary>
    /// <returns>The publicly available resources.</returns>
    /// <param name="categoryIdFilter">The identifier of the category filter used in the query.</param>
    /// <param name="relationshipsFilter">The relationships filter used in the query.</param>
    public Task<IEnumerable<Resource>> GetAllPublic(
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
    public Task<IEnumerable<Resource>> TryGetAllPrivate(
        Id            userId,
        Id?           categoryIdFilter    = null,
        Relationships relationshipsFilter = Relationships.All
    );

    /// <summary>
    /// Tries to retrieve all resources waiting for verification for a given user.
    /// </summary>
    /// <returns>The resources that are not being verified, or being verified by the user.</returns>
    /// <param name="userId">The identifier of the user.</param>
    public Task<IEnumerable<Resource>> TryGetAllWaitingForVerification(Id userId);

}

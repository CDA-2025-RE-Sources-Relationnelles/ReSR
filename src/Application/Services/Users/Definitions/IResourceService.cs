using FluentResponse.Interfaces;
using ReSR.Domain.Aggregates.Resources;
using ReSR.Domain.Aggregates.Resources.ValueObjects;

namespace ReSR.Application.Services.Users.Definitions;
public interface IResourceService<T> where T : Resource {

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
    public Task<IResponse<IEnumerable<T>>> TryGetAllPrivate(
        Id            userId,
        Id?           categoryIdFilter    = null,
        Relationships relationshipsFilter = Relationships.All
    );

    /// <summary>
    /// Tries to retrieve all resources waiting for verification for a given user.
    /// </summary>
    /// <returns>The resources that are not being verified, or being verified by the user.</returns>
    /// <param name="userId">The identifier of the user.</param>
    public Task<IResponse<IEnumerable<T>>> TryGetAllWaitingForVerification(Id userId);

    /// <summary>
    /// Tries to create a resource using the factory method.
    /// </summary>
    /// <returns>The created resource.</returns>
    /// <param name="factory">The factory method to build a resource.</param>
    public Task<IResponse<T>> TryCreateAsync(Func<IResponse<T>> factory);

    /// <summary>
    /// Tries to update a resource using the factory method.
    /// </summary>
    /// <returns>The created resource.</returns>
    /// <param name="resourceId">The identifier of the resource to update.</param>
    /// <param name="ownerId">The identifier of the resource's owner.</param>
    /// <param name="ownerId">The identifier of the user trying to update the resource.</param>
    /// <param name="transform">The transform method to update the resource.</param>
    public Task<IResponse<T>> TryUpdateAsync(Id resourceId, Id ownerId, Func<T, IResponse<T>> transform);

    /// <summary>
    /// Tries to update a resource using the factory method.
    /// </summary>
    /// <returns>The created resource.</returns>
    /// <param name="resourceId">The identifier of the resource to update.</param>
    /// <param name="ownerId">The identifier of the user trying to update the resource.</param>
    /// <param name="transform">The transform method to update the resource.</param>
    public Task<IResponse<T>> TryUpdateAsync(Id resourceId, Id ownerId, Func<T, T> transform);

    /// <summary>
    /// Tries to delete a resource.
    /// </summary>
    /// <returns>A successful response if the resource was deleted.</returns>
    /// <param name="resourceId">The identifier of the resource to delete.</param>
    /// <param name="ownerId">The identifier of the user trying to delete the resource.</param>
    public Task<IResponse> TryDeleteAsync(Id resourceId, Id ownerId);

    /// <summary>
    /// Tries to assign a user to a resource's verification.
    /// </summary>
    /// <returns>The updated resource details.</returns>
    /// <param name="verifier">The identifier of the verifying user.</param>
    /// <param name="resourceId">The identifier of the verified resource.</param>
    public Task<IResponse<T>> TryAssignToVerificationAsync(Id resourceId, Id verifier);

    /// <summary>
    /// Tries to reject a resource's verification.
    /// </summary>
    /// <returns>The updated resource details<./returns>
    /// <param name="verifier">The identifier of the verifying user.</param>
    /// <param name="resourceId">The identifier of the verified resource.</param>
    public Task<IResponse<T>> TryRejectAsync(Id resourceId, Id verifier);

    /// <summary>
    /// Tries to confirm a resource's verification.
    /// </summary>
    /// <returns>The updated resource details.</returns>
    /// <param name="verifier">The identifier of the verifying user.</param>
    /// <param name="resourceId">The identifier of the verified resource.</param>
    public Task<IResponse<T>> TryVerifyAsync(Id resourceId, Id verifier);

}

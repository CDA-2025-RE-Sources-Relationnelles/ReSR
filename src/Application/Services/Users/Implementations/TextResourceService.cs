using FluentResponse;
using FluentResponse.Interfaces;
using ReSR.Application.Services.Users.Definitions;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Domain.Aggregates.Categories;
using ReSR.Domain.Aggregates.Messages;
using ReSR.Domain.Aggregates.Resources;
using ReSR.Domain.Aggregates.Resources.ValueObjects;
using ReSR.Domain.Ports;

namespace ReSR.Application.Services.Users.Implementations;
internal class TextResourceService(
    IResourceRepository<TextResource> resourceRepository,
    IRepository<User> userRepository,
    IRepository<Comment> commentRepository,
    IRepository<Category> categoryRepository
) : ResourceService<TextResource>(resourceRepository, userRepository, commentRepository), ITextResourceService {

    public Task<IResponse<TextResource>> TryCreateAsync(
        Id            ownerId,
        string        title,
        string        description,
        Id            categoryId,
        Relationships relationships,
        string        content,
        bool          isPrivate
    ) => userRepository.TryGetAsync(ownerId)
        .OnSuccessAsync(owner => categoryRepository
            .TryGetAsync(categoryId)
            .OnSuccessAsync(category => TextResource.TryCreate(title, description, category, relationships, content, isPrivate, owner)))
        .OnSuccessAsync(resourceRepository.TryAddAsync);

    public Task<IResponse<TextResource>> TryUpdateAsync(
        Id id,
        string?        title         = null,
        string?        description   = null,
        Id?            categoryId    = null,
        Relationships? relationships = null,
        string?        content       = null
    ) => resourceRepository.TryUpdateAsync(id, async resource => {

        var response = Response.Success(resource);

        if (categoryId is not null) response = await response.OnSuccessAsync(x =>
            categoryRepository
                .TryGetAsync(categoryId.Value)
                .OnSuccessAsync(category => x.WithCategory(category))
        );
        
        if (title is not null) response = response.OnSuccess(x => x.TryWithTitle(title));
        if (description is not null) response = response.OnSuccess(x => x.WithDescription(description));

        if (relationships is not null) response = response.OnSuccess(x => x.WithRelationships(relationships.Value));
        if (content is not null) response = response.OnSuccess(x => x.WithContent(content));

        return response;

    });
}

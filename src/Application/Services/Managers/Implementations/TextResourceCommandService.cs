using FluentResponse;
using FluentResponse.Interfaces;
using Microsoft.Extensions.Logging;
using ReSR.Application.Services.Managers.Definitions;
using ReSR.Domain.Aggregates.Categories;
using ReSR.Domain.Aggregates.Resources;
using ReSR.Domain.Aggregates.Resources.ValueObjects;
using ReSR.Domain.Ports;

namespace ReSR.Application.Services.Managers.Implementations;
internal class TextResourceCommandService(
    IRepository<TextResource> repository,
    IRepository<Category> categoryRepository,
    ILogger<TextResourceCommandService> logger
) : ITextResourceCommandService {

    public Task<IResponse<TextResource>> TryCreateAsync(
        string        title,
        string        description,
        Id            categoryId,
        Relationships relationships,
        string        content
    ) => categoryRepository
        .TryGetAsync(categoryId)
        .OnSuccessAsync(category => TextResource.TryCreate(title, description, category, relationships, content, isPrivate: false))
        .OnSuccessAsync(repository.TryAddAsync)
        .OnSuccessAsync(x => logger.LogInformation("Text resource created by manager: {@TextResource} !", x));

    public Task<IResponse<TextResource>> TryUpdateAsync(
        Id id,
        string?        title         = null,
        string?        description   = null,
        Id?            categoryId    = null,
        Relationships? relationships = null,
        string?        content       = null
    ) => repository.TryUpdateAsync(id, async resource => {

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

    }).OnSuccessAsync(x => logger.LogInformation("Text resource updated by manager: {@TextResource} !", x));

    public Task<IResponse> TryDeleteAsync(Id id) =>
        repository
            .TryDeleteAsync(id)
            .OnSuccessAsync(() => logger.LogInformation("Text resource with id {@Id} deleted by manager !", id));

    public Task<IResponse<TextResource>> TrySuspendAsync(uint id, bool value = true) =>
        repository
            .TryUpdateAsync(id, x => x.WithSuspension(value))
            .OnSuccessAsync(x => logger.LogInformation("Text resource suspsension changed by manager: {@TextResource} !", x));
}

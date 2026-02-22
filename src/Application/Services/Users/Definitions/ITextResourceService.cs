using FluentResponse.Interfaces;
using ReSR.Domain.Aggregates.Resources;
using ReSR.Domain.Aggregates.Resources.ValueObjects;

namespace ReSR.Application.Services.Users.Definitions;
public interface ITextResourceService : IResourceService<TextResource> {

    public Task<IResponse<TextResource>> TryCreateAsync(
        Id            ownerId,
        string        title,
        Id            categoryId,
        Relationships relationships,
        string        content,
        bool          isPrivate
    );

    public Task<IResponse<TextResource>> TryUpdateAsync(
        Id id,
        string?        title         = null,
        Id?            categoryId    = null,
        Relationships? relationships = null,
        string?        content       = null
    );
}

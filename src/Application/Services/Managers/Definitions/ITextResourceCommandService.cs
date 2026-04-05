using FluentResponse.Interfaces;
using ReSR.Domain.Aggregates.Resources;
using ReSR.Domain.Aggregates.Resources.ValueObjects;

namespace ReSR.Application.Services.Managers.Definitions;
public interface ITextResourceCommandService {

    public Task<IResponse<TextResource>> TryCreateAsync(
        string        title,
        string        description,
        Id            categoryId,
        Relationships relationships,
        string        content
    );

    public Task<IResponse<TextResource>> TryUpdateAsync(
        Id id,
        string?        title         = null,
        string?        description   = null,
        Id?            categoryId    = null,
        Relationships? relationships = null,
        string?        content       = null
    );

    public Task<IResponse> TryDeleteAsync(Id id);
    public Task<IResponse<TextResource>> TrySuspendAsync(Id id, bool value = true);

}

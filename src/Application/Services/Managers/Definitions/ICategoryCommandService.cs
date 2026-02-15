using FluentResponse.Interfaces;
using ReSR.Domain.Aggregates.Categories;

namespace ReSR.Application.Services.Managers.Definitions;
public interface ICategoryCommandService {

    public Task<IResponse<Category>> TryCreateAsync(string name);
    public Task<IResponse<Category>> TryUpdateAsync(Id id, string name);
    public Task<IResponse> TryDeleteAsync(Id id);

}

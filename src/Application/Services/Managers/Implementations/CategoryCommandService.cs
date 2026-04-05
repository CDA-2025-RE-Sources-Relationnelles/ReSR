using FluentResponse;
using FluentResponse.Interfaces;
using Microsoft.Extensions.Logging;
using ReSR.Application.Services.Managers.Definitions;
using ReSR.Domain.Aggregates.Categories;
using ReSR.Domain.Ports;

namespace ReSR.Application.Services.Managers.Implementations;
internal class CategoryCommandService(
    IRepository<Category> repository,
    ILogger<CategoryCommandService> logger
) : ICategoryCommandService {

    public Task<IResponse<Category>> TryCreateAsync(string name) =>
        Category
            .TryCreate(name)
            .OnSuccessAsync(repository.TryAddAsync)
            .OnSuccessAsync(x => logger.LogInformation("Category created by manager: {@Category} !", x));

    public Task<IResponse<Category>> TryUpdateAsync(Id id, string name) =>
        repository
            .TryUpdateAsync(id, x => x.TryWithName(name))
            .OnSuccessAsync(x => logger.LogInformation("Category updated by manager: {@Category} !", x));

    public Task<IResponse> TryDeleteAsync(Id id) =>
        repository
            .TryDeleteAsync(id)
            .OnSuccessAsync(() => logger.LogInformation("Category with id {@Id} deleted by manager !", id));

}

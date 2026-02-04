using FluentResponse.Interfaces;
using ReSR.Domain.Core;

namespace ReSR.Application.Services.Managers.Definitions;
public interface IAggregateRootService<T> where T : IAggregateRoot<T> {

    public Task<IResponse<IEnumerable<T>>> TryGetAllAsync(Id managerId);
    public Task<IResponse<T>> TryGetAsync(Id managerId, Id id);
    public Task<IResponse<T>> TryCreateAsync(Id managerId, Func<IResponse<T>> factory);
    public Task<IResponse<T>> TryCreateAsync(Id managerId, Func<T> factory);
    public Task<IResponse<T>> TryUpdateAsync(Id managerId, Func<T, IResponse<T>> transform);
    public Task<IResponse<T>> TryUpdateAsync(Id managerId, Func<T, T> transform);
    public Task<IResponse> TryDeleteAsync(Id managerId, Id id);

}

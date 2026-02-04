using System.Linq.Expressions;
using FluentResponse.Interfaces;
using ReSR.Domain.Core;

namespace ReSR.Domain.Ports;
public interface IRepository<T> where T : IAggregateRoot<T> {

    Task<IResponse<T>> TryAddAsync(T entity);
    Task<IResponse<T>> TryUpdateAsync(Id id, Func<T, T> changes);
    Task<IResponse<T>> TryUpdateAsync(Id id, Func<T, IResponse<T>> changes);
    Task<IResponse> TryDeleteAsync(T entity);
    Task<IResponse> TryDeleteAsync(Id id);
    Task<IResponse> TryDeleteAsync(Expression<Func<T, bool>> predicate);

    Task DeleteAllAsync();
    Task DeleteAllAsync(Expression<Func<T, bool>> predicate);

    Task<IResponse<T>> TryGetAsync(Id id);
    Task<IResponse<T>> TryGetAsync(Expression<Func<T, bool>> predicate);

    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> GetAllAsync(IEnumerable<Id> ids);
    Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate);

    Task<bool> ContainsAsync(T entity);
    Task<bool> ContainsIdAsync(Id id);

    Task<bool> AnyAsync();
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);

}
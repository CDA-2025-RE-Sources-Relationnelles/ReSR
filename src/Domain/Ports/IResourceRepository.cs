using ReSR.Domain.Aggregates.Resources;
using ReSR.Domain.Aggregates.Resources.ValueObjects;
using ReSR.Domain.Core;

namespace ReSR.Domain.Ports;
public interface IResourceRepository<T> : IRepository<T> where T : Resource, IAggregateRoot<T> {

    Task<IEnumerable<T>> GetAllAsync(
        string? titleSearch = null,
        Id? categoryIdFilter = null,
        Relationships relationshipsFilter = Relationships.None,
        Visibility visibilityFilter = Visibility.Public
    );
}
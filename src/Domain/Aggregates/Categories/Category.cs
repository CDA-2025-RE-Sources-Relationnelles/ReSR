using FluentResponse;
using FluentResponse.Interfaces;
using ReSR.Domain.Core;

namespace ReSR.Domain.Aggregates.Categories;

/// <summary>
/// A resource category.
/// </summary>
public record Category(Id Id = default) : IAggregateRoot<Category> {

    #region PROPERTIES

        /// <summary> The resource category's name. </summary>
        public string Name { get; internal init; } = null!;

    #endregion
    #region CONSTRUCTORS

        public static IResponse<Category> TryCreate(string name) =>
            TryVerifyNameInvariant(name).OnSuccess(() => new Category { Name= name });

    #endregion
    #region METHODS
            
        /// <returns> A copy of the category with the given name if valid. </returns>
        public virtual IResponse<Category> TryWithName(string value) =>
            TryVerifyNameInvariant(value).OnSuccess(() => this with { Name = value });


        protected static IResponse TryVerifyNameInvariant(string value) =>
            value.Trim().Length >= 4
            ? Response.Success()
            : Response.Failure(new InvariantException("Un nom de catégorie doit contenir au moins 4 caractères !"));


        public IEnumerable<IDomainEvent> DomainEvents { get; protected init; } = [];
        public Category WithConsumedEvents(out IEnumerable<IDomainEvent> domainEvents) {
            domainEvents = this.DomainEvents;
            return this with { DomainEvents = [] };
        }

    #endregion
    
}
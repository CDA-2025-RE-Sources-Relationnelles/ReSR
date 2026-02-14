using System.Data;
using Microsoft.EntityFrameworkCore;
using FluentResponse;
using FluentResponse.Interfaces;
using ReSR.Domain.Core;
using System.Linq.Expressions;
using ReSR.Domain.Ports;
using ReSR.Application.Exceptions;

namespace ReSR.Infrastructure.Adapters.Repositories;
internal class Repository<T>(
    DbContext              dbContext,
    IDomainEventDispatcher domainEventDispatcher
) : IRepository<T> where T : class, IAggregateRoot<T> {

    #region PROPERTIES

        protected readonly DbSet<T>  table     = dbContext.Set<T>();
        protected readonly DbContext dbContext = dbContext;

    #endregion
    #region METHODS

        protected virtual async Task<IResponse<T>> TryValidateAsync(T entity) => Response.Success(entity);
        protected virtual IQueryable<T> GetJoinedTable() => this.table;

        protected async Task<IResponse<T>> TryDispatchEventsAsync(T entity) {
            
            // We make sure that EF Core treats the entity as read only.
            this.table.Entry(entity).State = EntityState.Detached;
            entity = entity.WithConsumedEvents(out var domainEvents);

            return await domainEventDispatcher.DispatchAsync(domainEvents).OnSuccessAsync(() => {

                // We make sure that EF Core keeps track of eventual domain event changes.
                this.table.Attach(entity);
                return entity;

            });
        }

        public virtual async Task<IResponse<T>> TryAddAsync(T entity) {

            if (await this.ContainsIdAsync(entity.Id))
                return Response.Failure<T>(new EntityConflictException(typeof(T), entity.Id));
            
            return await this.TryValidateAsync(entity).OnSuccessAsync(async entity => {
                entity = (await this.table.AddAsync(entity)).Entity;
                await this.dbContext.SaveChangesAsync();
            }).OnSuccessAsync(TryDispatchEventsAsync);
        }

        public virtual Task<IResponse<T>> TryUpdateAsync(Id id, Func<T, T> changes) =>
            this.TryGetAsync(id).OnSuccessAsync(oldEntity =>
                this.TryValidateAsync(changes(oldEntity)).OnSuccessAsync(async entity => {

                    this.table.Entry(oldEntity).State = EntityState.Detached;
                    this.table.Attach(entity);
                    this.table.Entry(entity).State = EntityState.Modified;
                    await this.dbContext.SaveChangesAsync();
                
                })
            ).OnSuccessAsync(TryDispatchEventsAsync);

        public virtual Task<IResponse<T>> TryUpdateAsync(Id id, Func<T, IResponse<T>> changes) =>
            this.TryGetAsync(id).OnSuccessAsync(oldEntity =>
                changes(oldEntity).OnSuccessAsync(this.TryValidateAsync).OnSuccessAsync(async entity => {

                    this.table.Entry(oldEntity).State = EntityState.Detached;
                    this.table.Attach(entity);
                    this.table.Entry(entity).State = EntityState.Modified;
                    await this.dbContext.SaveChangesAsync();
                
                })
            ).OnSuccessAsync(TryDispatchEventsAsync);

        public virtual async Task<IResponse> TryDeleteAsync(T entity) {
            this.table.Remove(entity);
            return await this.dbContext.SaveChangesAsync() is not 0
                ? Response.Success()
                : Response.Failure(new EntityNotFoundException(typeof(T), entity.Id));
        }

        public virtual Task<IResponse> TryDeleteAsync(Expression<Func<T, bool>> predicate) =>
            this.TryGetAsync(predicate).OnSuccessAsync(this.TryDeleteAsync);

        public virtual Task<IResponse> TryDeleteAsync(Id id) =>
            this.TryGetAsync(id).OnSuccessAsync(this.TryDeleteAsync);

        public virtual async Task DeleteAllAsync() {
            this.table.RemoveRange(this.table);
            await this.dbContext.SaveChangesAsync();
        }

        public virtual async Task DeleteAllAsync(Expression<Func<T, bool>> predicate) {
            this.table.RemoveRange(this.GetJoinedTable().Where(predicate));
            await this.dbContext.SaveChangesAsync();
        }
        
        public virtual async Task<IResponse<T>> TryGetAsync(Id id) =>
            await this.GetJoinedTable().FirstOrDefaultAsync(x => x.Id == id) is T entity
                ? Response.Success(entity)
                : Response.Failure<T>(new EntityNotFoundException(typeof(T), id));

        public virtual async Task<IResponse<T>> TryGetAsync(Expression<Func<T, bool>> predicate) =>
            await this.GetJoinedTable().FirstOrDefaultAsync(predicate) is T entity
                ? Response.Success(entity)
                : Response.Failure<T>(new EntityNotFoundException(typeof(T)));

        public virtual async Task<IEnumerable<T>> GetAllAsync() => await this.GetJoinedTable().ToListAsync();
        public virtual async Task<IEnumerable<T>> GetAllAsync(IEnumerable<Id> ids) {
            var results = new List<T>(ids.Count());
            foreach (var id in ids)
                await this.TryGetAsync(id).OnSuccessAsync(results.Add);

            return results;
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate) => await this.GetJoinedTable().Where(predicate).ToListAsync();

        public virtual async Task<bool> ContainsAsync(T entity) => await this.table.ContainsAsync(entity);
        public virtual async Task<bool> ContainsIdAsync(Id id) => await this.table.FindAsync(id) is not null;

        public virtual async Task<bool> AnyAsync() => await this.table.AnyAsync();
        public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate) => await this.GetJoinedTable().AnyAsync(predicate);

    #endregion

}

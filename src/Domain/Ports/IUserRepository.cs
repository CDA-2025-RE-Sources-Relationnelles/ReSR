using FluentResponse.Interfaces;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Domain.Core;

namespace ReSR.Domain.Ports
{
    public interface IUserRepository : IRepository<User>
    {
        Task<IResponse<User>> GetByIdAsync(Id id);
    }
}
using FluentResponse.Interfaces;
using ReSR.Domain.Aggregates.Accounts;

namespace ReSR.Domain.Ports;
public interface IAccountRepository<T> : IRepository<T> where T : Account<T> {

    Task<IResponse<T>> TryGetWithEmailAsync(string email);
    Task<bool> AnyWithEmailAsync(string email);

}
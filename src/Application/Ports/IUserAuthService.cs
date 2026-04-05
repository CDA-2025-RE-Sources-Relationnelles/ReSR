using FluentResponse.Interfaces;
using ReSR.Domain.Aggregates.Accounts;

namespace ReSR.Application.Ports;
public interface IAccountAuthService<T> where T : Account<T> {
    IResponse<string> TryGenerateToken(T account);
}
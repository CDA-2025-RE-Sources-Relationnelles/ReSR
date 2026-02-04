using FluentResponse.Interfaces;
using ReSR.Application.ValueObjects.Accounts;
using ReSR.Domain.Aggregates.Accounts;

namespace ReSR.Application.Services.Core.Definitions;

/// <summary>
/// A service for handling account sessions.
/// </summary>
public interface ISessionService<T> where T : Account<T> {

    /// <summary>
    /// Tries to auth.
    /// </summary>
    /// <returns>A response containing the account session's token and details.</returns>
    /// <param name="email">The account's email.</param>
    /// <param name="password">The account's password.</param>
    public Task<IResponse<Session<T>>> TryAuthAsync(string email, string password);

    /// <summary>
    /// Tries to update the account's details.
    /// </summary>
    /// <returns>A response containing the account session's token and updated details.</returns>
    /// <param name="id">The account's identifier.</param>
    /// <param name="password">The account's password.</param>
    /// <param name="transform">The account's modifications.</param>
    public Task<IResponse<Session<Manager>>> TryUpdateAccountAsync(
        Id id,
        string password,
        Func<Manager, IResponse<Manager>> transform
    );

    /// <summary>
    /// Tries to delete the account.
    /// </summary>
    /// <returns>A successful response if the account was deleted.</returns>
    /// <param name="id">The account's identifier.</param>
    /// <param name="password">The account's password.</param>
    public Task<IResponse> TryDeleteAccountAsync(
        Id id,
        string password
    );
}
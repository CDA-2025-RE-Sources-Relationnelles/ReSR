using FluentResponse.Interfaces;
using ReSR.Application.Services.Core.Definitions;
using ReSR.Application.ValueObjects.Accounts;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Domain.Aggregates.Resources;

namespace ReSR.Application.Services.Users.Definitions;

/// <summary>
/// A service for handling user sessions.
/// </summary>
public interface IUserSessionService : ISessionService<User> {

    /// <summary>
    /// Tries to register as user.
    /// </summary>
    /// <returns>A response containing the user session's token and user details.</returns>
    /// <param name="email">The user's public identifier.</param>
    /// <param name="email">The user's mail address.</param>
    /// <param name="password">The user's password.</param>
    /// <param name="pin">The registration PIN associated with the user's mail address.</param>
    public Task<IResponse<Session<User>>> TryRegisterAsync(string username, string email, string password, Pin pin);

    /// <summary>
    /// Tries to reset a user's password.
    /// </summary>
    /// <returns>A response containing the user session's token and user details.</returns>
    /// <param name="email">The user's mail address.</param>
    /// <param name="newPassword">The user's new password.</param>
    /// <param name="pin">The password reset PIN associated with the user's mail address.</param>
    public Task<IResponse<Session<User>>> TryResetPasswordAsync(string email, string newPassword, Pin pin);
    
    /// <summary>
    /// Tries to anonymize a user.
    /// </summary>
    /// <returns>A successful response if the user was anonymized.</returns>
    /// <param name="id">The user's ID.</param>
    /// <param name="password">The user's password.</param>
    public Task<IResponse> TryAnonymizeAccountAsync(Id id, string password);

    /// <summary>
    /// Tries to request a user's registration PIN generation.
    /// </summary>
    /// <returns>A successful response if the PIN was generated.</returns>
    /// <param name="email">The user's wanted mail address.</param>
    public Task<IResponse> TryRequestRegistrationPINAsync(string email);

    /// <summary>
    /// Tries to request a user's password reset PIN generation.
    /// </summary>
    /// <returns>A successful response if the PIN was generated.</returns>
    /// <param name="email">The user's mail address.</param>
    public Task<IResponse> TryRequestPasswordResetPINAsync(string email);
    public Task<IResponse<User>> TryLikeProfile(Id id, Id fromId, bool value = true);
    public Task<IResponse<Session<User>>> TryGenerateSession(Id id);

}
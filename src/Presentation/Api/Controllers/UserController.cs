using FluentResponse;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReSR.Application.Services.Users.Definitions;
using ReSR.Application.ValueObjects.Accounts;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Domain.Ports;
using ReSR.Presentation.Api.Extensions;
using ReSR.Presentation.Api.Resources.Accounts;

namespace ReSR.Presentation.Api.Controllers;
[ApiController]
[Route(ROUTE)]
public class UserController(
    IUserSessionService sessionService,
    IRepository<User>   queryService
) : ControllerBase {

    public const string ROUTE = "/users";

    #region DTOS

        public readonly record struct RegisterDto(
            string Username,
            string Email,
            string Password,
            uint Pin
        );

        public readonly record struct AuthDto(
            string Email,
            string Password
        );

        public readonly record struct UpdateAccountDto(
            string Password,
            string? NewUsername = null,
            string? NewEmail    = null,
            string? NewPassword = null
        );

        public readonly record struct ResetPasswordDto(
            string Email,
            string NewPassword,
            uint Pin
        );

        public readonly record struct CloseAccountDto(
            string Password
        );

        public readonly record struct RequestPinGenerationDto(
            string Email
        );
    
    #endregion
    #region ROUTES

        [HttpPost("/register")]
        [EndpointDescription("Tries to register using the PIN code associated with the wanted email.")]
        public Task<IResult> RegisterAsync(RegisterDto dto) =>
            sessionService
                .TryRegisterAsync(dto.Username.TrimEnd(), dto.Email.TrimEnd(), dto.Password.TrimEnd(), dto.Pin)
                .ToResourceAsync<Session<User>, UserSessionResource>(x => Results.CreatedAtRoute(
                    routeName   : nameof(GetUserAsync),
                    routeValues : new { UserId = x.Value.Id },
                    value       : x
                ));

        [HttpPost("/auth")]
        [EndpointDescription("Tries to start a session.")]
        public Task<IResult> AuthAsync(AuthDto dto) =>
            sessionService
                .TryAuthAsync(dto.Email, dto.Password)
                .ToResourceAsync<Session<User>, UserSessionResource>(Results.Ok);

        [HttpPost("/reset-password")]
        [EndpointDescription("Tries to reset a password using the PIN code associated with the wanted email.")]
        public Task<IResult> ResetPasswordAsync(ResetPasswordDto dto) =>
            sessionService
                .TryResetPasswordAsync(dto.Email, dto.NewPassword, dto.Pin)
                .ToResourceAsync<Session<User>, UserSessionResource>(Results.Ok);

        [HttpGet("{userId}", Name = nameof(GetUserAsync))]
        [Authorize(Policy = "LimitedUserAccess")]
        [EndpointSummary("Only accessible for this user")]
        [EndpointDescription("Queries the user.")]
        public Task<IResult> GetUserAsync(Id userId) =>
            queryService.TryGetAsync(userId).ToResourceAsync<User, UserResource>(Results.Ok);

        [HttpPatch("{userId}")]
        [Authorize(Policy = "LimitedUserAccess")]
        [EndpointSummary("Only accessible for this user")]
        [EndpointDescription("Tries to update the user.")]
        public Task<IResult> UpdateAsync(Id userId, UpdateAccountDto dto) =>
            sessionService
                .TryUpdateAccountAsync(userId, dto.Password, user => {

                    var response = FluentResponse.Response.Success(user);
                    if (dto.NewUsername is not null) response = response.OnSuccess(x => x.TryWithUsername(dto.NewUsername));
                    if (dto.NewEmail is not null)    response = response.OnSuccess(x => x.TryWithMailAddress(dto.NewEmail));
                    if (dto.NewPassword is not null) response = response.OnSuccess(x => x.TryWithPassword(dto.NewPassword));

                    return response;

                }).ToResourceAsync<Session<User>, UserSessionResource>(Results.Ok);

        [HttpPost("{userId}/anonymize")]
        [Authorize(Policy = "LimitedUserAccess")]
        [EndpointSummary("Only accessible for this user")]
        [EndpointDescription("Tries to anonymize the user.")]
        public Task<IResult> AnonymizeAsync(Id userId, CloseAccountDto dto) =>
            sessionService
                .TryAnonymizeAccountAsync(userId, dto.Password)
                .ToResultAsync(Results.Ok);

        [HttpDelete("{userId}")]
        [Authorize(Policy = "LimitedUserAccess")]
        [EndpointSummary("Only accessible for this user")]
        [EndpointDescription("Tries to delete the user.")]
        public Task<IResult> DeleteAsync(Id userId, CloseAccountDto dto) =>
            sessionService
                .TryDeleteAccountAsync(userId, dto.Password)
                .ToResultAsync(Results.Ok);

        [HttpPost("/request-register")]
        [EndpointDescription("Tries to request the generation of a registration PIN that will be associated with the email.")]
        public Task<IResult> RequestRegisterAsync(RequestPinGenerationDto dto) =>
            sessionService
                .TryRequestRegistrationPINAsync(dto.Email)
                .ToResultAsync(Results.Ok);

        [HttpPost("/request-password-reset")]
        [EndpointDescription("Tries to request the generation of a password reset PIN that will be associated with the account.")]
        public Task<IResult> RequestPasswordResetAsync(RequestPinGenerationDto dto) =>
            sessionService
                .TryRequestPasswordResetPINAsync(dto.Email)
                .ToResultAsync(Results.Ok);

    #endregion
}

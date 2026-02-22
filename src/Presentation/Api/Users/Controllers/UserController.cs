using FluentResponse;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReSR.Application.Services.Users.Definitions;
using ReSR.Application.ValueObjects.Accounts;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Domain.Ports;
using ReSR.Presentation.Api.Core.Extensions;
using ReSR.Presentation.Api.Users.Authorization;
using ReSR.Presentation.Api.Users.Extensions;
using ReSR.Presentation.Api.Users.ValueObjects.Accounts;

namespace ReSR.Presentation.Api.Users.Controllers;
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

        [HttpPost(ROUTE + "/register")]
        [EndpointDescription("Tries to register using the PIN code associated with the wanted email.")]
        public Task<IResult> RegisterAsync(RegisterDto dto) =>
            sessionService
                .TryRegisterAsync(dto.Username.TrimEnd(), dto.Email.TrimEnd(), dto.Password.TrimEnd(), dto.Pin)
                .ToResourceAsync<Session<User>, UserSessionResource>(x => Results.CreatedAtRoute(
                    routeName   : nameof(GetUserAsync),
                    routeValues : new { UserId = x.Value.Id },
                    value       : x
                ));

        [HttpPost(ROUTE + "/auth")]
        [EndpointDescription("Tries to start a session.")]
        public Task<IResult> AuthAsync(AuthDto dto) =>
            sessionService
                .TryAuthAsync(dto.Email, dto.Password)
                .ToResourceAsync<Session<User>, UserSessionResource>(Results.Ok);

        [HttpPost(ROUTE + "/reset-password")]
        [EndpointDescription("Tries to reset a password using the PIN code associated with the wanted email.")]
        public Task<IResult> ResetPasswordAsync(ResetPasswordDto dto) =>
            sessionService
                .TryResetPasswordAsync(dto.Email, dto.NewPassword, dto.Pin)
                .ToResourceAsync<Session<User>, UserSessionResource>(Results.Ok);

        [HttpGet("{userId}", Name = nameof(GetUserAsync))]
        [Authorize(Policy = nameof(UserSessionAuthorizationRequirement))]
        [EndpointSummary("Only accessible for this user")]
        [EndpointDescription("Queries the user.")]
        public Task<IResult> GetUserAsync(Id userId) =>
            queryService.TryGetAsync(userId).ToResourceAsync<User, UserResource>(Results.Ok);

        [HttpPatch("{userId}")]
        [Authorize(Policy = nameof(UserSessionAuthorizationRequirement))]
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
        [Authorize(Policy = nameof(UserSessionAuthorizationRequirement))]
        [EndpointSummary("Only accessible for this user")]
        [EndpointDescription("Tries to anonymize the user.")]
        public Task<IResult> AnonymizeAsync(Id userId, CloseAccountDto dto) =>
            sessionService
                .TryAnonymizeAccountAsync(userId, dto.Password)
                .ToResultAsync(Results.Ok);

        [HttpPost("{userId}/like-profile")]
        [Authorize]
        [EndpointSummary("Only accessible for authenticated users.")]
        [EndpointDescription("Tries to like the user profile.")]
        public Task<IResult> LikeAsync(Id userId) =>
            sessionService
                .TryLikeProfile(userId, User.GetUserId()!.Value)
                .ToResourceAsync<User, UserResource>(Results.Ok);

        [HttpPost("{userId}/unlike-profile")]
        [Authorize]
        [EndpointSummary("Only accessible for authenticated users.")]
        [EndpointDescription("Tries to unlike the user profile.")]
        public Task<IResult> UnlikeAsync(Id userId) =>
            sessionService
                .TryUnlikeProfile(userId, User.GetUserId()!.Value)
                .ToResourceAsync<User, UserResource>(Results.Ok);

        [HttpDelete("{userId}")]
        [Authorize(Policy = nameof(UserSessionAuthorizationRequirement))]
        [EndpointSummary("Only accessible for this user")]
        [EndpointDescription("Tries to delete the user.")]
        public Task<IResult> DeleteAsync(Id userId, CloseAccountDto dto) =>
            sessionService
                .TryDeleteAccountAsync(userId, dto.Password)
                .ToResultAsync(Results.Ok);

        [HttpPost(ROUTE + "/request-register")]
        [EndpointDescription("Tries to request the generation of a registration PIN that will be associated with the email.")]
        public Task<IResult> RequestRegisterAsync(RequestPinGenerationDto dto) =>
            sessionService
                .TryRequestRegistrationPINAsync(dto.Email)
                .ToResultAsync(Results.Ok);

        [HttpPost(ROUTE + "/request-password-reset")]
        [EndpointDescription("Tries to request the generation of a password reset PIN that will be associated with the account.")]
        public Task<IResult> RequestPasswordResetAsync(RequestPinGenerationDto dto) =>
            sessionService
                .TryRequestPasswordResetPINAsync(dto.Email)
                .ToResultAsync(Results.Ok);

    #endregion
}

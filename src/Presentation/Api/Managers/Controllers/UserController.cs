using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReSR.Application.Services.Managers.Definitions;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Domain.Aggregates.Accounts.ValueObjects;
using ReSR.Domain.Ports;
using ReSR.Presentation.Api.Core.Extensions;
using ReSR.Presentation.Api.Managers.ValueObjects.Accounts;

namespace ReSR.Presentation.Api.Managers.Controllers;
[ApiController]
[Route(ROUTE)]
[Authorize(Policy = "BackOffice")]
public class UserController(
    IRepository<User> repository,
    IUserCommandService commandService
) : ControllerBase {

    public const string ROUTE = "/manage/users";

    #region DTOS

        public readonly record struct CreateUserDto(
            string Username,
            string Email,
            string Password,
            string Permissions
        );

        public readonly record struct UpdateUserDto(
            string? Username    = null,
            string? Email       = null,
            string? Permissions = null
        );

    #endregion
    #region ROUTES

        [HttpGet]
        [Authorize(Roles = nameof(ManagerPermissions.ReadUsers))]
        [EndpointSummary("Only accessible for managers with read permissions.")]
        [EndpointDescription("Queries the users.")]
        public Task<IResult> GetUsersAsync() =>
            repository.GetAllAsync().ToResourceAsync<User, UserResource>(Results.Ok);

        [HttpGet("{userId}")]
        [Authorize(Roles = nameof(ManagerPermissions.ReadUsers))]
        [EndpointSummary("Only accessible for managers with read permissions.")]
        [EndpointDescription("Queries the user.")]
        public Task<IResult> GetUserAsync(Id userId) =>
            repository.TryGetAsync(userId).ToResourceAsync<User, UserResource>(Results.Ok);

        [HttpPost]
        [Authorize(Roles = nameof(ManagerPermissions.WriteUsers))]
        [EndpointSummary("Only accessible for managers with write permissions.")]
        [EndpointDescription("Creates a new user.")]
        public Task<IResult> PostUserAsync(CreateUserDto dto) =>
            commandService
                .TryCreateAsync(dto.Username, dto.Email, dto.Password, Enum.TryParse<UserPermissions>(dto.Permissions, true, out var permissions) ? permissions : UserPermissions.None)
                .ToResourceAsync<User, UserResource>(Results.Ok);

        [HttpPatch("{userId}")]
        [Authorize(Roles = nameof(ManagerPermissions.WriteUsers))]
        [EndpointSummary("Only accessible for managers with read permissions.")]
        [EndpointDescription("Updates the user.")]
        public Task<IResult> PatchUserAsync(Id userId, UpdateUserDto dto) =>
            commandService
                .TryUpdateAsync(userId, dto.Username, dto.Email, Enum.TryParse<UserPermissions>(dto.Permissions, true, out var permissions) ? permissions : null)
                .ToResourceAsync<User, UserResource>(Results.Ok);

        [HttpDelete("{userId}")]
        [Authorize(Roles = nameof(ManagerPermissions.WriteUsers))]
        [EndpointSummary("Only accessible for managers with write permissions.")]
        [EndpointDescription("Tries to delete the user.")]
        public Task<IResult> DeleteUserAsync(Id userId) =>
            commandService.TryDeleteAsync(userId).ToResultAsync(Results.Ok);

        [HttpPost("{userId}/anonymize")]
        [Authorize(Roles = nameof(ManagerPermissions.WriteUsers))]
        [EndpointSummary("Only accessible for managers with write permissions.")]
        [EndpointDescription("Tries to anonymize the user.")]
        public Task<IResult> AnonymizeUserAsync(Id userId) =>
            commandService.TryAnonymizeAsync(userId).ToResourceAsync<User, UserResource>(Results.Ok);

        [HttpPost("{userId}/suspend")]
        [Authorize(Roles = nameof(ManagerPermissions.WriteUsers))]
        [EndpointSummary("Only accessible for managers with write permissions.")]
        [EndpointDescription("Tries to suspend the user.")]
        public Task<IResult> SuspendUserAsync(Id userId, bool value = true) =>
            commandService.TrySuspendAsync(userId, value).ToResourceAsync<User, UserResource>(Results.Ok);

    #endregion
}

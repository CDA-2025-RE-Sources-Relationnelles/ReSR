using FluentResponse;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReSR.Application.Services.Core.Definitions;
using ReSR.Application.ValueObjects.Accounts;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Domain.Ports;
using ReSR.Presentation.Api.Core.Extensions;
using ReSR.Presentation.Api.Managers.ValueObjects.Accounts;

namespace ReSR.Presentation.Api.Managers.Controllers;
[ApiController]
[Route(ROUTE)]
public class ManagerSessionController(
    ISessionService<Manager> sessionService,
    IRepository<Manager>     repository
) : ControllerBase {

    public const string ROUTE = "/managers";

    #region DTOS

        public readonly record struct ManagerAuthDto(
            string Email,
            string Password
        );

        public readonly record struct UpdateManagerAccountDto(
            string Password,
            string? NewEmail    = null,
            string? NewPassword = null
        );

        public readonly record struct DeleteManagerAccountDto(
            string Password
        );

    #endregion
    #region ROUTES

        [HttpPost(ROUTE + "/auth")]
        [EndpointDescription("Tries to start a session.")]
        public Task<IResult> AuthAsync(ManagerAuthDto dto) =>
            sessionService
                .TryAuthAsync(dto.Email, dto.Password)
                .ToResourceAsync<Session<Manager>, ManagerSessionResource>(Results.Ok);

        [HttpGet("{managerId}", Name = nameof(GetManagerAsync))]
        [Authorize(Policy = "LimitedManagerAccess", AuthenticationSchemes = nameof(Manager))]
        [EndpointSummary("Only accessible for this manager.")]
        [EndpointDescription("Queries the manager.")]
        public Task<IResult> GetManagerAsync(Id managerId) =>
            repository.TryGetAsync(managerId).ToResourceAsync<Manager, ManagerResource>(Results.Ok);

        [HttpPatch("{managerId}")]
        [Authorize(Policy = "LimitedManagerAccess", AuthenticationSchemes = nameof(Manager))]
        [EndpointSummary("Only accessible for this manager.")]
        [EndpointDescription("Tries to update the manager.")]
        public Task<IResult> UpdateAsync(Id managerId, UpdateManagerAccountDto dto) =>
            sessionService.TryUpdateAccountAsync(managerId, dto.Password, manager => {

                var response = FluentResponse.Response.Success(manager);
                if (dto.NewEmail is not null)    response = response.OnSuccess(x => x.TryWithMailAddress(dto.NewEmail));
                if (dto.NewPassword is not null) response = response.OnSuccess(x => x.TryWithPassword(dto.NewPassword));

                return response;

            }).ToResourceAsync<Session<Manager>, ManagerSessionResource>(Results.Ok);

        [HttpDelete("{managerId}")]
        [Authorize(Policy = "LimitedManagerAccess", AuthenticationSchemes = nameof(Manager))]
        [EndpointSummary("Only accessible for this manager.")]
        [EndpointDescription("Tries to delete the manager.")]
        public Task<IResult> DeleteAsync(Id managerId, DeleteManagerAccountDto dto) =>
            sessionService.TryDeleteAccountAsync(managerId, dto.Password).ToResultAsync(Results.Ok);


    #endregion
}

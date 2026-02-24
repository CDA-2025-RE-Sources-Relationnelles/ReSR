using FluentResponse;
using FluentResponse.Interfaces;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Domain.Aggregates.Accounts.ValueObjects;
using ReSR.Domain.Aggregates.QuizSessions;
using ReSR.Domain.Aggregates.Resources;
using ReSR.Domain.Aggregates.Resources.ValueObjects;

namespace ReSR.Domain.Services.Implementations;
public static class UserPermissionsService {
    
    /// <returns>A successful response if the user has access to the given resource.</returns>
    /// <param name="user">The user trying to access the resource.</param>
    /// <param name="resource">The resource being accessed.</param>
    public static IResponse TryVerifyUserResourceAccess(User user, Resource resource) =>
        resource.Visibility switch {
            Visibility.Public  => Response.Success(),
            Visibility.Private => resource.Owner is User owner && (owner.Id == user.Id || owner.Friends.Any(x => x.Id == user.Id))
                ? Response.Success()
                : Response.Failure("Une ressource privée n'est accessible qu'à son propriétaire ou à ses ami.e.s !"),
            Visibility.WaitingForVerification => resource.Owner is User owner && owner.Id == user.Id
                ? Response.Success()
                : user.TryVerifyPermissions(UserPermissions.VerifyResources),
            _ => resource.Visibility.HasFlag(Visibility.Suspended)
                ? resource.Owner is User owner && owner.Id == user.Id
                    ? Response.Success()
                    : Response.Failure("Une ressource suspendue n'est accessible qu'à son propriétaire !")
                : Response.Failure("Visibilité de ressource inconnue !")
        };

    /// <returns>A successful response if the user has access to the given quiz session.</returns>
    /// <param name="user">The user trying to access the resource.</param>
    /// <param name="session">The quiz session being accessed.</param>
    public static IResponse TryVerifyUserQuizSessionAccess(User user, QuizSession session) =>
        session.Participations.Any(x => x.User.Id == user.Id)
            ? Response.Success()
            : Response.Failure("Une session de quiz n'est accessible qu'à ses participants !");

}

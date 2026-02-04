using FluentResponse;
using FluentResponse.Interfaces;
using ReSR.Application.Ports;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Domain.Aggregates.Accounts.Events;
using ReSR.Domain.Core;

namespace ReSR.Application.EventListeners.Accounts;
internal class UserMutuallyLikedListener(
    IRepository<User> repository,
    IMailService      mailService
) : IDomainEventListener<UserMutuallyLiked> {
    
    public Task<IResponse> HandleAsync(UserMutuallyLiked domainEvent, CancellationToken cancellationToken = default) =>
        repository
            .TryGetAsync(domainEvent.UserId)
            .OnSuccessAsync(user =>
            
                repository
                    .TryGetAsync(domainEvent.ByUserId)
                    .OnSuccessAsync(byUser =>

                        mailService.TrySendEmailAsync(
                            user.Email,
                            "Vous avez un.e nouvel.le ami.e sur RE(Sources) Relationnelles !",
                            $"""
                            Vous êtes devenu.e ami.e avec {byUser.Username} ! Vous pouvez désormais partager vos ressources privées ensemble !
                            """
                        ).OnSuccessAsync(() => mailService.TrySendEmailAsync(
                            byUser.Email,
                            "Vous avez un.e nouvel.le ami.e sur RE(Sources) Relationnelles !",
                            $"""
                            Vous êtes devenu.e ami.e avec {user.Username} ! Vous pouvez désormais partager vos ressources privées ensemble !
                            """
                        ))
                    )
            );
}
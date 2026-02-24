using FluentResponse;
using FluentResponse.Interfaces;
using ReSR.Application.Ports;
using ReSR.Application.Services.Users.Definitions;
using ReSR.Application.ValueObjects.Accounts;
using ReSR.Domain.Aggregates.Accounts;
using ReSR.Domain.Ports;

namespace ReSR.Application.Services.Users.Implementations;
public sealed class UserSessionService(
    IAccountRepository<User>            repository,
    IAccountAuthService<User>           authService,
    IRegistrationValidationCacheService registrationValidationCacheService,
    IPasswordResetCacheService          passwordResetCacheService,
    IMailService                        mailService
) : IUserSessionService {
    
    public Task<IResponse<Session<User>>> TryAuthAsync(string email, string password) =>
        repository
            .TryGetWithEmailAsync(email)
            .OnSuccessAsync(user => user
                .TryVerifyPassword(password)
                .OnSuccess(() => user.Suspended ? Response.Failure("Votre compte est suspendu !") : Response.Success())
                .OnSuccessAsync(() => repository.TryUpdateAsync(user.Id, user => user.WithNewActivity()))
            ).OnSuccessAsync(user => authService.TryGenerateToken(user).OnSuccess(token => new Session<User>(token, user)));

    public Task<IResponse<Session<User>>> TryRegisterAsync(string username, string email, string password, Pin pin) =>
        registrationValidationCacheService
            .TryGet(email)
            .OnFailure(_ => Response.Failure<Pin>(new KeyNotFoundException("Code PIN invalide ou expiré !")))
            .OnSuccessAsync(async cachedPin =>
            
                cachedPin != pin
                    ? Response.Failure<User>(new UnauthorizedAccessException("Code PIN incorrect !"))
                    : await User
                        .TryCreate(username, email, password)
                        .OnSuccessAsync(repository.TryAddAsync)

            ).OnSuccessAsync(user => {
                            
                registrationValidationCacheService.TryDelete(email);
                return authService
                    .TryGenerateToken(user)
                    .OnSuccess(token => new Session<User>(token, user));
            });


    public Task<IResponse<Session<User>>> TryResetPasswordAsync(string email, string newPassword, Pin pin) =>
        repository
            .TryGetWithEmailAsync(email)
            .OnSuccessAsync(user =>
            
                passwordResetCacheService
                    .TryGet(user.Id)
                    .OnFailure(_ => Response.Failure<Pin>(new KeyNotFoundException("Code PIN invalide ou expiré !")))
                    .OnSuccessAsync(async cachedPin =>

                        cachedPin != pin
                            ? Response.Failure<User>(new UnauthorizedAccessException("Code PIN incorrect !"))
                            : await repository.TryUpdateAsync(
                                user.Id,
                                user => user.TryWithPassword(newPassword)
                            )
                    )
            ).OnSuccessAsync(user => {

                passwordResetCacheService.TryDelete(user.Id);
                return authService
                    .TryGenerateToken(user)
                    .OnSuccess(token => new Session<User>(token, user));

            });

    public Task<IResponse<Session<User>>> TryUpdateAccountAsync(Id id, string password, Func<User, IResponse<User>> transform) =>
        repository
            .TryGetAsync(id)
            .OnSuccessAsync(user => user
                .TryVerifyPassword(password)
                .OnSuccessAsync(() => repository.TryUpdateAsync(id, transform))
            ).OnSuccessAsync(user => authService
                .TryGenerateToken(user)
                .OnSuccess(token => new Session<User>(token, user))
            );

    public Task<IResponse> TryAnonymizeAccountAsync(Id id, string password) =>
        repository
            .TryGetAsync(id)
            .OnSuccessAsync(user => user
                .TryVerifyPassword(password)
                .OnSuccessAsync(() => repository.TryUpdateAsync(id, x => x.AsAnonymized()))
            ).OnSuccessAsync(_ => Response.Success());

    public Task<IResponse> TryDeleteAccountAsync(Id id, string password) =>
        repository
            .TryGetAsync(id)
            .OnSuccessAsync(user => user
                .TryVerifyPassword(password)
                .OnSuccessAsync(() => repository.TryDeleteAsync(id))
            );

    public Task<IResponse> TryRequestRegistrationPINAsync(string email) =>
        User.TryVerifyEmailInvariant(email).OnSuccessAsync(async () => {

            if (await repository.AnyWithEmailAsync(email))
                return Response.Failure(new ArgumentException("Un compte existe déjà avec cette adresse mail !"));

            return await registrationValidationCacheService
                .TryAdd(email, new Pin())
                .OnFailure(_ => Response.Failure<Pin>(new ArgumentException("Code PIN déjà généré !")))
                .OnSuccessAsync(async pin =>
                
                    await mailService.TrySendEmailAsync(
                        email,
                        "Demande de création de compte CESI Zen",
                        $"Veuillez entrer ce code PIN pour valider la création de votre compte: {pin}.\nIl expirera dans {registrationValidationCacheService.CacheDuration.Minutes} minutes."
                    )
                );
        });

    public Task<IResponse> TryRequestPasswordResetPINAsync(string email) =>
        repository
            .TryGetWithEmailAsync(email)
            .OnSuccessAsync(user =>

                passwordResetCacheService
                    .TryAdd(user.Id, new Pin())
                    .OnFailure(_ => Response.Failure<Pin>(new ArgumentException("Code PIN déjà généré !")))
                    .OnSuccessAsync(pin =>
                    
                        mailService.TrySendEmailAsync(
                            user.Email,
                            "Demande de changement de mot de passe pour votre compte CESI Zen",
                            $"Veuillez entrer ce code PIN pour changer le mot de passe de votre compte: {pin}.\nIl expirera dans {passwordResetCacheService.CacheDuration.Minutes} minutes.")
                    
                    )
            );

    public Task<IResponse<User>> TryLikeProfile(Id id, Id fromId, bool value) =>
        repository.TryGetAsync(fromId).OnSuccessAsync(from =>
            repository.TryUpdateAsync(id, x => x.TryWithLikeFrom(from, value))
        );
}
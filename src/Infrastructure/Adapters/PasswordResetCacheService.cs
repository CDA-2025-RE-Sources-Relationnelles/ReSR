using ReSR.Application.Ports;
using Microsoft.Extensions.Configuration;
using ReSR.Application.ValueObjects.Accounts;

namespace ReSR.Infrastructure.Adapters;
internal class PasswordResetCacheService(
    TimeSpan passwordResetRequestExpiry
) : CacheService<Id, Pin>, IPasswordResetCacheService {

    public override TimeSpan CacheDuration { get; } = passwordResetRequestExpiry;

    public PasswordResetCacheService(IConfiguration configuration) : this(
        passwordResetRequestExpiry : TimeSpan.Parse(configuration["Pin:PasswordResetRequestExpiry"]!)
    ) {}
}
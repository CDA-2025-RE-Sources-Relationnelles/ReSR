using ReSR.Application.Ports;
using Microsoft.Extensions.Configuration;
using ReSR.Application.ValueObjects.Accounts;

namespace ReSR.Infrastructure.Adapters;
internal class RegistrationValidationCacheService(
    TimeSpan registrationValidationRequestExpiry
) : CacheService<string, Pin>, IRegistrationValidationCacheService {

    public override TimeSpan CacheDuration { get; } = registrationValidationRequestExpiry;

    public RegistrationValidationCacheService(IConfiguration configuration) : this(
        registrationValidationRequestExpiry : TimeSpan.Parse(configuration["Pin:RegistrationValidationRequestExpiry"]!)
    ) {}
}
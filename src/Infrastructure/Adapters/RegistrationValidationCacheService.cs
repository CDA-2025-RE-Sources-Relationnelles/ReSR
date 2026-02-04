using ReSR.Application.Core.ValueObjects;
using ReSR.Application.Ports;
using Microsoft.Extensions.Configuration;

namespace ReSR.Infrastructure.Adapters;
internal class RegistrationValidationCacheService(
    TimeSpan registrationValidationRequestExpiry
) : CacheService<string, Pin>, IRegistrationValidationCacheService {

    public override TimeSpan CacheDuration { get; } = registrationValidationRequestExpiry;

    public RegistrationValidationCacheService(IConfiguration configuration) : this(
        registrationValidationRequestExpiry : TimeSpan.Parse(configuration["Pin:RegistrationValidationRequestExpiry"]!)
    ) {}
}
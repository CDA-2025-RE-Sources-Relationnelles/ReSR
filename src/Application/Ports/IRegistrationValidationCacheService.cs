using ReSR.Application.ValueObjects.Accounts;

namespace ReSR.Application.Ports;
public interface IRegistrationValidationCacheService: ICacheService<string, Pin>;
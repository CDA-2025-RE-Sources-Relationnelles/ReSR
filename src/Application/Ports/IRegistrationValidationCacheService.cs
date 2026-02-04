using ReSR.Application.Core.ValueObjects;

namespace ReSR.Application.Ports;
public interface IRegistrationValidationCacheService: ICacheService<string, Pin>;
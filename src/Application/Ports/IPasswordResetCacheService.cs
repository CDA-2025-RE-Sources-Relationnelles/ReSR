using ReSR.Application.ValueObjects.Accounts;

namespace ReSR.Application.Ports;
public interface IPasswordResetCacheService: ICacheService<Id, Pin>;
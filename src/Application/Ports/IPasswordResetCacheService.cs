using ReSR.Application.Core.ValueObjects;

namespace ReSR.Application.Ports;
public interface IPasswordResetCacheService: ICacheService<Id, Pin>;
namespace ReSR.Domain.Aggregates.Accounts.ValueObjects;
[Flags]
public enum UserPermissions : byte {
    None,
    VerifyComments  = 0b01,
    VerifyResources = 0b10,
    ModeratorRole   = 0b11,
}
namespace ReSR.Domain.Aggregates.Accounts.ValueObjects;
[Flags]
public enum UserPermissions : byte {
    None,
    VerifyComments  = 0b001,
    VerifyResources = 0b010,
    ModeratorRole   = 0b111,
}
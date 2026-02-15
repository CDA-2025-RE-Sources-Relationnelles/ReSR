namespace ReSR.Domain.Aggregates.Accounts.ValueObjects;
[Flags]
public enum ManagerPermissions : byte {
    None,
    ReadCategories   = 0b000001,
    WriteCategories  = 0b000010,
    ManageCategories = 0b000011,
    ReadUsers        = 0b000100,
    WriteUsers       = 0b001000,
    ManageUsers      = 0b001100,
    ReadManagers     = 0b010000,
    WriteManagers    = 0b100000,
    ManageManagers   = 0b110000,
    AdminRole        = 0b001111,
    SuperAdminRole   = 0b111111,
}

namespace ReSR.Domain.Aggregates.Accounts.ValueObjects;
[Flags]
public enum AdminPermissions {
    None,
    ViewCategoies    = 0b000001,
    ManageCategories = 0b000011,
    ViewUsers        = 0b000100,
    ManageUsers      = 0b001100,
    ViewAdmins       = 0b010000,
    ManageAdmins     = 0b110000,
    AdminRole        = 0b001111,
    SuperAdminRole   = 0b111111,
}
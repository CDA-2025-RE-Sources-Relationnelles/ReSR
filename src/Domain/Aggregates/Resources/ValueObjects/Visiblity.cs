namespace ReSR.Domain.Aggregates.Resources.ValueObjects;
[Flags]
public enum Visibility {
    Private                = 0b001,
    Public                 = 0b010,
    WaitingForVerification = 0b011,
    Suspended              = 0b100,
}
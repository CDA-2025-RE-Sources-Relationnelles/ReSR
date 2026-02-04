namespace ReSR.Application.Core.ValueObjects;
public readonly record struct Pin(uint Code) {
    public Pin() : this((uint)new Random().Next(0, 1_0000_0000)) {}
    public static implicit operator uint(Pin from) => from.Code;
    public static implicit operator Pin(uint from) => new (from);
    public override string ToString() => this.Code.ToString().PadLeft(8, '0').Insert(4, " ");
}

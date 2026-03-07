namespace ReSR.Presentation.Api.Core.ValueObjects;
public readonly record struct Page<T> (
    int Index,
    int Size,
    int TotalItemCount,
    IEnumerable<T> Items
) {
    public static Page<T> From(IEnumerable<T> values, int pageIndex, int pageSize) =>
        new (
            Index          : pageIndex,
            Size           : pageSize,
            TotalItemCount : values.Count(),
            Items          : values
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
        );
}
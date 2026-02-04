namespace ReSR.Application.Services.Core.Definitions;
public interface IExportService<T> {

    /// <returns>A downloadable byte stream based on the given value.</returns>
    /// <param name="value">The value to export.</param>
    public byte[] Export(T value);
}

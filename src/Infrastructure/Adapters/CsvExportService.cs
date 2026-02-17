using System.Globalization;
using System.Text;
using CsvHelper;
using ReSR.Application.Ports;

namespace ReSR.Infrastructure.Adapters;
internal class CsvExportService<T> : IExportService<T> {
    public byte[] Export(T data) {
        using var stream = new MemoryStream();
        using var reader = new StreamReader(stream);
        using var writer = new StreamWriter(stream);
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        csv.WriteRecord(data);
        writer.Flush();
        stream.Position = 0;
        return Encoding.UTF8.GetBytes(reader.ReadToEnd());
    }

    public byte[] Export(IEnumerable<T> data) {
        using var stream = new MemoryStream();
        using var reader = new StreamReader(stream);
        using var writer = new StreamWriter(stream);
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        csv.WriteRecords(data);
        writer.Flush();
        stream.Position = 0;
        return Encoding.UTF8.GetBytes(reader.ReadToEnd());
    }
}
using System.Globalization;
using System.Text;
using CsvHelper;
using ReSR.Application.Ports;

namespace ReSR.Infrastructure.Adapters;
internal class CsvExportService<T> : IExportService<T> {
    public byte[] Export(T data) {
        using var stream = new MemoryStream();
        using var reader = new StreamReader(stream, Encoding.Unicode);
        using var writer = new StreamWriter(stream, Encoding.Unicode);
        using var csv = new CsvWriter(writer, CultureInfo.CurrentUICulture);
        csv.WriteRecord(data);
        writer.Flush();
        stream.Position = 0;
        return Encoding.Unicode.GetBytes(reader.ReadToEnd());
    }

    public byte[] Export(IEnumerable<T> data) {
        using var stream = new MemoryStream();
        using var reader = new StreamReader(stream, Encoding.Unicode);
        using var writer = new StreamWriter(stream, Encoding.Unicode);
        using var csv = new CsvWriter(writer, CultureInfo.CurrentUICulture);
        csv.WriteRecords(data);
        writer.Flush();
        stream.Position = 0;
        return Encoding.Unicode.GetBytes(reader.ReadToEnd());
    }
}
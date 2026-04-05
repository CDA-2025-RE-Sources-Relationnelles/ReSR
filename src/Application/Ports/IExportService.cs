namespace ReSR.Application.Ports;
public interface IExportService<T> {
    public byte[] Export(T data);
    public byte[] Export(IEnumerable<T> data);
}
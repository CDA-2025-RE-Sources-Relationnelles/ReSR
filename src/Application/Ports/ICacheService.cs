using FluentResponse.Interfaces;

namespace ReSR.Application.Ports;
public interface ICacheService<TKey, TValue>
    where TKey   : notnull
    where TValue : notnull {

    TimeSpan CacheDuration { get; }

    IResponse<TValue> TryAdd(TKey key, TValue value);
    IResponse<TValue> TryGet(TKey key);
    IResponse         TryDelete(TKey key);
    void              Clear();
}
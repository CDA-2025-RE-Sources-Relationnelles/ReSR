using System.Collections.Concurrent;
using ReSR.Application.Ports;
using FluentResponse;
using FluentResponse.Interfaces;

namespace ReSR.Infrastructure.Adapters;
internal abstract class CacheService<TKey, TValue> : ICacheService<TKey, TValue>
    where TKey   : notnull
    where TValue : notnull {

    #region PROPERTIES

        protected readonly ConcurrentDictionary<TKey, TValue> Cache = [];
        public    abstract TimeSpan                           CacheDuration { get; }

    #endregion
    #region METHODS

        public IResponse<TValue> TryAdd(TKey key, TValue value) {

            var timer = new System.Timers.Timer(this.CacheDuration) { AutoReset = false };
            timer.Elapsed += (_, _) => { this.TryDelete(key); timer.Dispose(); };
            timer.Start();

            return this.Cache.TryAdd(key, value)
                ? Response.Success(value)
                : Response.Failure<TValue>(new ArgumentException($"Il existe déjà une entrée dans le cache avec la clé '{key}' !"));
        }

        public IResponse<TValue> TryGet(TKey key) =>
            this.Cache.TryGetValue(key, out TValue? value)
                ? Response.Success(value)
                : Response.Failure<TValue>(new KeyNotFoundException($"Il n'y a pas d'entrée dans le cache avec la clé '{key}' !"));

        public IResponse TryDelete(TKey key) =>
            this.Cache.TryRemove(key, out TValue? _)
                ? Response.Success()
                : Response.Failure(new KeyNotFoundException($"Il n'y a pas de d'entrée dans le cache avec la clé '{key}' !"));

        public void Clear() => this.Cache.Clear();

    #endregion
}
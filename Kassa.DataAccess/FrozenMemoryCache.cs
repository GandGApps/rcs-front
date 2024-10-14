using System;
using System.Buffers;
using System.Collections;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Kassa.Shared;

namespace Kassa.DataAccess;
public sealed partial class FrozenMemoryCache<T>(TimeSpan cacheDuration) : IFrozenMemoryCache<T>, IReadOnlyDictionary<Guid, T> where T : IGuidId
{
    
    private FrozenDictionary<Guid, T> _cache = FrozenDictionary<Guid, T>.Empty;

    private readonly TimeSpan _cacheDuration = cacheDuration;
    private DateTime _lastRefreshTime = DateTime.MinValue;
    private Func<IEnumerable<T>>? _updateWhenExpired;
    private Timer? _expirationTimer;

    public FrozenMemoryCache() : this(TimeSpan.FromMinutes(15))
    {

    }

    public void Refresh(IEnumerable<T> items)
    {
        Volatile.Write(ref _cache, items.ToFrozenDictionary(x => x.Id));

        static void VolatileWrite(ref DateTime location, DateTime value)
        {
            ref var unsafeLocation = ref Unsafe.As<DateTime, ulong>(ref location);
            ref var unsafeValue = ref Unsafe.As<DateTime, ulong>(ref value);
            Volatile.Write(ref unsafeLocation, unsafeValue);
        }

        VolatileWrite(ref _lastRefreshTime, DateTime.Now);

        ResetExpirationTimer();
    }


    public bool IsExpired => (DateTime.Now - _lastRefreshTime) > _cacheDuration;

    public bool IsEmpty => _cache.Count == 0;

    public TimeSpan CacheDuration => _cacheDuration;

    public void SetExpirationUpdater(Func<IEnumerable<T>> updateWhenExpired)
    {
        _updateWhenExpired = updateWhenExpired;
    }

    private void ResetExpirationTimer()
    {
        _expirationTimer?.Dispose();

        _expirationTimer = new Timer(OnExpirationTimerElapsed, null, _cacheDuration, Timeout.InfiniteTimeSpan);
    }

    private void OnExpirationTimerElapsed(object? state)
    {
        if (_updateWhenExpired != null)
        {
            var items = _updateWhenExpired();
            Refresh(items);
        }
    }

    #region Implementation of IReadOnlyDictionary<Guid, T>
    public bool ContainsKey(Guid key) => _cache.ContainsKey(key);
    public bool TryGetValue(Guid key, [MaybeNullWhen(false)] out T value) => _cache.TryGetValue(key, out value);
    public IEnumerator<KeyValuePair<Guid, T>> GetEnumerator() => _cache.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => _cache.GetEnumerator();
    public IEnumerable<Guid> Keys => _cache.Keys;
    public IEnumerable<T> Values => _cache.Values;
    public int Count => _cache.Count;
    public T this[Guid key] => _cache[key];
    #endregion
}
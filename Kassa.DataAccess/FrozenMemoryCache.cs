using System;
using System.Collections;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.DataAccess.Model;

namespace Kassa.DataAccess;
public sealed class FrozenMemoryCache<T>(TimeSpan cacheDuration) : IReadOnlyDictionary<Guid, T> where T : IGuidId
{
    private FrozenDictionary<Guid, T> _cache = FrozenDictionary<Guid, T>.Empty;
    private readonly TimeSpan _cacheDuration = cacheDuration;
    private DateTime _lastRefreshTime = DateTime.MinValue;

    public FrozenMemoryCache() : this(TimeSpan.FromMinutes(15))
    {
    }

    public void Refresh(IEnumerable<T> items)
    {
        _cache = items.ToFrozenDictionary(x => x.Id);
        _lastRefreshTime = DateTime.Now;
    }

    public bool ContainsKey(Guid key) => ((IReadOnlyDictionary<Guid, T>)_cache).ContainsKey(key);
    public bool TryGetValue(Guid key, [MaybeNullWhen(false)] out T value) => ((IReadOnlyDictionary<Guid, T>)_cache).TryGetValue(key, out value);
    public IEnumerator<KeyValuePair<Guid, T>> GetEnumerator() => ((IEnumerable<KeyValuePair<Guid, T>>)_cache).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_cache).GetEnumerator();

    public bool IsExpired => (DateTime.Now - _lastRefreshTime) > _cacheDuration;

    public IEnumerable<Guid> Keys => ((IReadOnlyDictionary<Guid, T>)_cache).Keys;

    public IEnumerable<T> Values => ((IReadOnlyDictionary<Guid, T>)_cache).Values;

    public int Count => ((IReadOnlyCollection<KeyValuePair<Guid, T>>)_cache).Count;

    public T this[Guid id] => _cache[id];
}
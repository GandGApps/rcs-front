using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.Shared;

namespace Kassa.DataAccess;
public interface IFrozenMemoryCache<T> : IReadOnlyDictionary<Guid, T> where T : IGuidId
{
    public void Refresh(IEnumerable<T> items);

    public void SetExpirationUpdater(Func<IEnumerable<T>> updateWhenExpired);

    public bool IsExpired
    {
        get;
    }

    public bool IsEmpty
    {
        get;
    }
    public TimeSpan CacheDuration
    {
        get;
    }
}

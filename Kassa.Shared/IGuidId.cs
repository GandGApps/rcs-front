using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kassa.Shared;
public interface IGuidId
{
    public Guid Id
    {
        get;
    }
}

public class AnonymousGuidId(Func<Guid> idGetter) : IGuidId
{
    public Guid Id => idGetter();
}
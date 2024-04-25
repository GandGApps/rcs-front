using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kassa.BuisnessLogic;
public interface IObservableOnlyBehaviourSubject<T> : IObservable<T>
{
    public T Value
    {
        get;
    }
}

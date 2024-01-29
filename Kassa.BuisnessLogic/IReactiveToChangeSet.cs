using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kassa.BuisnessLogic;
public interface IReactiveToChangeSet<TKey, TSource>: INotifyPropertyChanged
{
    public TKey Id
    {
        get;
    }

    public TSource Source
    {
        get; set;
    }
}

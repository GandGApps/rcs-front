using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.DataAccess.Model;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kassa.RxUI;
public abstract class ProductHostItemVm : ReactiveObject, IGuidId
{

    public Guid Id
    {
        get; protected init;
    }

    [Reactive]
    public string Name
    {
        get; protected set;
    }

    [Reactive]
    public int Image
    {
        get; protected set;
    }

    [Reactive]
    public string? Color
    {
        get; protected set;
    } 

}

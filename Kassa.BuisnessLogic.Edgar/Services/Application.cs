using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.Services;

namespace Kassa.BuisnessLogic.Edgar.Services;
public sealed class Application : IApplication
{
    public IObservableOnlyBehaviourSubject<bool> IsOffline
    {
        get;
    }

    public void Exit() => throw new NotImplementedException();
    public bool TryHandleUnhandeledException(Exception ex) => throw new NotImplementedException();
}

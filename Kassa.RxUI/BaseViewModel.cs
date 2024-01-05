using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kassa.RxUI;
public class BaseViewModel : ReactiveObject, IActivatableViewModel, IDisposable
{
    [Reactive]
    public bool IsDisposed
    {
        get; protected set;
    }

    public MainViewModel MainViewModel
    {
        get; 
    }
    public ViewModelActivator Activator
    {
        get;
    }

    public BaseViewModel(MainViewModel mainViewModel)
    {
        MainViewModel = mainViewModel;
        Activator = new();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!IsDisposed)
        {
            if (disposing)
            {
                // TODO: освободить управляемое состояние (управляемые объекты)
            }

            // TODO: освободить неуправляемые ресурсы (неуправляемые объекты) и переопределить метод завершения
            // TODO: установить значение NULL для больших полей
            IsDisposed = true;
        }
    }

    public void Dispose()
    {
        // Не изменяйте этот код. Разместите код очистки в методе "Dispose(bool disposing)".
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}

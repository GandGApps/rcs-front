using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
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

    public MainViewModel? MainViewModel
    {
        get;
    }
    public ViewModelActivator Activator
    {
        get;
    }

    public BaseViewModel()
    {
        Activator = new();

        this.WhenActivated(OnActivated);
    }

    public BaseViewModel(MainViewModel mainViewModel) : this()
    {
        MainViewModel = mainViewModel;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!IsDisposed)
        {
            if (disposing)
            {
            }
            IsDisposed = true;
        }
    }

    protected virtual void OnActivated(CompositeDisposable disposables)
    {

    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}

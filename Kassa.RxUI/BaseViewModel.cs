using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kassa.RxUI;
public class BaseViewModel : ReactiveObject, IActivatableViewModel, ICancelable, IInitializableViewModel
{
    protected CompositeDisposable InternalDisposables
    {
        get;
    } = [];

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
            InternalDisposables.Dispose();

            if (disposing)
            {
            }
            IsDisposed = true;
        }
    }

    public ValueTask InitializeAsync()
    {
        return InitializeAsync(InternalDisposables);
    }

    protected virtual ValueTask InitializeAsync(CompositeDisposable disposables)
    {
        return ValueTask.CompletedTask;
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

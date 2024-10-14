using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic;
using Kassa.BuisnessLogic.Services;
using Kassa.Shared;
using Kassa.Shared.ServiceLocator;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace Kassa.RxUI;
public class BaseViewModel : ReactiveObject, IActivatableViewModel, ICancelable, IInitializableViewModel, IAsyncDisposable
{
    /// <summary>
    /// A composite disposable that stores all disposables that should be disposed when the view model is disposed.
    /// </summary>
    protected CompositeDisposable InternalDisposables
    {
        get;
    } = [];

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

    public BaseViewModel() : this(RcsKassa.ServiceProvider.GetRequiredService<MainViewModel>())
    {

    }

    public BaseViewModel(MainViewModel mainViewModel)
    {
        MainViewModel = mainViewModel;

        Activator = new();

        this.WhenActivated(OnActivated);

        Activator.DisposeWith(InternalDisposables);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!IsDisposed)
        {

            if (disposing)
            {
                InternalDisposables.Dispose();
            }

        }
    }

    public ValueTask InitializeAsync()
    {
        Initialize(InternalDisposables);
        return InitializeAsync(InternalDisposables);
    }

    protected virtual ValueTask InitializeAsync(CompositeDisposable disposables)
    {
        return ValueTask.CompletedTask;
    }

    protected virtual void Initialize(CompositeDisposable disposables)
    {

    }

    protected virtual void OnActivated(CompositeDisposable disposables)
    {

    }

    public void Dispose()
    {
        Debug.WriteLine($"Disposing {GetType().Name}");
        Dispose(disposing: true);
        IsDisposed = true;
        GC.SuppressFinalize(this);
    }

    protected virtual ValueTask DisposeAsyncCore()
    {
        return ValueTask.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore();

        Dispose(false);
        IsDisposed = true;

        GC.SuppressFinalize(this);
    }
}

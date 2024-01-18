using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace Kassa.RxUI;
public class BaseViewModel : ReactiveObject, IActivatableViewModel, ICancelable, IInitializableViewModel, IAsyncDisposable
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

    /// <summary>
    /// Don't use DisposeWith() for services which method returns
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    protected async ValueTask<T> GetInitializedService<T>() where T : class, IInitializableService
    {
        var services = await Locator.Current.GetInitializedService<T>();

        services.DisposeWith(InternalDisposables);

        return services;
    }

    public void Dispose()
    {
        Dispose(disposing: true);
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

        GC.SuppressFinalize(this);
    }
}

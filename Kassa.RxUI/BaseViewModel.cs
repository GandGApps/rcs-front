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
            
            if (disposing)
            {
                InternalDisposables.Dispose();
            }
            
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
    /// Retrieves an initialized service of the specified type. Note that you should not use <see cref="DisposableMixins.DisposeWith"/> 
    /// for services returned by this method.
    /// </summary>
    /// <typeparam name="T">The type of the initializable service to retrieve.</typeparam>
    /// <returns>An initialized service of type T.</returns>
    /// <remarks>
    /// The service is automatically added to InternalDisposables, which handles its disposal. Manually 
    /// calling <see cref="DisposableMixins.DisposeWith"/> on these services may lead to unexpected behavior or errors.
    /// </remarks>
    protected async ValueTask<T> GetInitializedService<T>() where T : class, IInitializableService
    {
        var services = await Locator.Current.GetInitializedService<T>();

        services.DisposeWith(InternalDisposables);

        return services;
    }

    public void Dispose()
    {
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

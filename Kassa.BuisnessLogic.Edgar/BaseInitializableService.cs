using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.Services;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace Kassa.BuisnessLogic.Edgar;
internal abstract class BaseInitializableService: IInitializableService, IEnableLogger
{
    private readonly CompositeDisposable disposables = [];

    protected CompositeDisposable InternalDisposables => disposables;
    protected static IReadonlyDependencyResolver Locator => Splat.Locator.Current;

    public bool IsDisposed
    {
        get; protected set;
    }

    public bool IsInitialized
    {
        get; protected set;
    }

    public void Dispose()
    {
        this.Log().Info("Disposing service");

        Dispose(disposing: true);

        this.Log().Info("Service disposed");

        IsDisposed = true;

        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        var id = Guid.NewGuid();
        
        this.Log().Info($"[{id}] Disposing service async");

        await DisposeAsyncCore().ConfigureAwait(false);

        Dispose(disposing: false);
        GC.SuppressFinalize(this);

        this.Log().Info($"[{id}] Service disposed async");
    }

    async ValueTask IInitializableService.Initialize()
    {
        if (IsInitialized)
        {
            this.Log().Warn("Service is already initialized");
            return;
        }

        var id = Guid.NewGuid();

        this.Log().Info($"[{id}] Initializing service");

        Initialize(InternalDisposables);
        await InitializeAsync(InternalDisposables).ConfigureAwait(false);

        this.Log().Info($"[{id}] Service initialized");

        IsInitialized = true;
    }

    protected virtual ValueTask InitializeAsync(CompositeDisposable disposables) => ValueTask.CompletedTask;

    protected virtual void Initialize(CompositeDisposable disposables)
    {

    }

    protected virtual void Dispose(bool disposing)
    {
        if (IsDisposed)
        {
            return;
        }

        if (disposing)
        {

            InternalDisposables.Dispose();
        }
    }

    protected virtual ValueTask DisposeAsyncCore() => ValueTask.CompletedTask;

    ~BaseInitializableService()
    {
        Dispose(disposing: false);
    }
}

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
internal class BaseInitializableService: IInitializableService
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
        Debug.WriteLine($"Disposing {GetType().Name}");

        Dispose(disposing: true);

        IsDisposed = true;

        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);

        Dispose(disposing: false);
        GC.SuppressFinalize(this);
    }

    public async ValueTask Initialize()
    {
        if (IsInitialized)
        {
            return;
        }

        await InitializeAsync(InternalDisposables).ConfigureAwait(false);

        IsInitialized = true;
    }

    protected virtual ValueTask InitializeAsync(CompositeDisposable disposables) => ValueTask.CompletedTask;

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

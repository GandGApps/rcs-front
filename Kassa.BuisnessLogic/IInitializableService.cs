
namespace Kassa.BuisnessLogic;

public interface IInitializableService : IDisposable, IAsyncDisposable
{
    public bool IsInitialized
    {
        get;
    }

    public bool IsDisposed
    {
        get;
    }

    public ValueTask Initialize();
}
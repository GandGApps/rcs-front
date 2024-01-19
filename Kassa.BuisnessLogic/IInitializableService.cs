using Splat; // need's for comment

namespace Kassa.BuisnessLogic;

/// <summary>
/// <para>
/// Defines a service that can be initialized.
/// </para>
/// <para>
/// Avoid directly retrieving a service using this interface. 
/// Instead, utilize <see cref="DependencyResolverExtensions.GetInitializedService{T}"/>,
/// which will initialize the service, ensure it's not already disposed, and manage its scope until disposal.
/// </para>
/// <para>
/// To obtain a service without initialization, use <see cref="DependencyResolverExtensions.GetRequiredService{T}"/>.
/// This method provides a new service instance without initialization.
/// </para>
/// <para>
/// If you need a scoped service that is not initialized and not disposed, 
/// use <see cref="DependencyResolverExtensions.GetNotInitializedService{T}"/>. 
/// This method ensures the service is scoped and maintains its uninitialized, non-disposed state.
/// </para>
/// <para>
/// Remember to register your service using <see cref="DependencyResolverExtensions.RegisterInitializableServiceFactory{T}"/>
/// in conjunction with 
/// <see cref="DependencyResolverMixins.Register{T}"/>. 
/// This approach is essential for enabling <see cref="DependencyResolverExtensions.GetRequiredService{T}"/> to return a 
/// new instance each time, while the <see cref="IInitializableServiceFactory{T}"/> handles scoping.
/// </para>
/// </summary>
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
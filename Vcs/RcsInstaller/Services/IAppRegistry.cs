using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TruePath;

namespace RcsInstaller.Services;

/// <summary>
/// Interface for application registry management. Provides operations for
/// registering, unregistering, checking registration, and retrieving data.
/// </summary>
public interface IAppRegistry
{
    /// <summary>
    /// Registers the application using an <see cref="AppRegistryProperties"/> object,
    /// which contains the version, path, and additional properties for the application.
    /// </summary>
    /// <param name="appRegistryProperties">An object containing the version, path, 
    /// and properties for the application's registration.</param>
    /// <returns>An asynchronous operation with no return value.</returns>
    public ValueTask Register(AppRegistryProperties appRegistryProperties) => Register(appRegistryProperties.Version, appRegistryProperties.Path, appRegistryProperties.Properties);

    /// <summary>
    /// Registers the application with the specified version, path, and additional
    /// properties.
    /// </summary>
    /// <param name="version">The application version to be registered.</param>
    /// <param name="path">The path to the application's files.</param>
    /// <param name="properties">A dictionary of additional properties related to the application.</param>
    /// <returns>An asynchronous operation with no return value.</returns>
    public ValueTask Register(Version version, AbsolutePath path, IReadOnlyDictionary<string, object> properties);

    /// <summary>
    /// Overloaded method for registering the application with version and path,
    /// without additional properties.
    /// </summary>
    /// <param name="version">The application version to be registered.</param>
    /// <param name="path">The path to the application's files.</param>
    /// <returns>An asynchronous operation with no return value.</returns>
    public ValueTask Register(Version version, AbsolutePath path) => Register(version, path, ReadOnlyDictionary<string, object>.Empty);

    /// <summary>
    /// Removes the application information from the registry.
    /// </summary>
    /// <returns>An asynchronous operation with no return value.</returns>
    public ValueTask UnRegister();

    /// <summary>
    /// Checks if the application is registered.
    /// </summary>
    /// <returns>An asynchronous operation returning the registration status.</returns>
    public ValueTask<bool> IsRegistered();

    /// <summary>
    /// Retrieves the version of the registered application.
    /// </summary>
    /// <returns>An asynchronous operation returning the application version.</returns>
    public ValueTask<Version?> GetVersion();

    /// <summary>
    /// Retrieves the registration properties object, which includes version, path,
    /// and application properties.
    /// </summary>
    /// <returns>An asynchronous operation returning the registration properties object.</returns>
    public ValueTask<AppRegistryProperties?> GetProperties();
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TruePath;

namespace RcsInstaller.Services;

/// <summary>
/// Class representing the properties of a registered application.
/// Implements IReadOnlyDictionary for accessing additional properties.
/// </summary>
/// <param name="version">The version of the registered application.</param>
/// <param name="path">The path to the registered application's files.</param>
/// <param name="properties">Additional properties associated with the application.</param>
public sealed class AppRegistryProperties(Version version, AbsolutePath path, IReadOnlyDictionary<string, object> properties): IReadOnlyDictionary<string, object>
{
    public object this[string key] => Properties[key];

    public Version Version => version;
    public AbsolutePath Path => path;

    public IReadOnlyDictionary<string, object> Properties => properties;

    #region IReadOnlyDictionary<string, object> implementation
    IEnumerable<string> IReadOnlyDictionary<string, object>.Keys => Properties.Keys;
    IEnumerable<object> IReadOnlyDictionary<string, object>.Values => Properties.Values;
    int IReadOnlyCollection<KeyValuePair<string, object>>.Count => Properties.Count;
    bool IReadOnlyDictionary<string, object>.ContainsKey(string key) => Properties.ContainsKey(key);
    IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator() => Properties.GetEnumerator();
    bool IReadOnlyDictionary<string, object>.TryGetValue(string key, [MaybeNullWhen(false)] out object value) => Properties.TryGetValue(key, out value);
    IEnumerator IEnumerable.GetEnumerator() => Properties.GetEnumerator();

    #endregion
}

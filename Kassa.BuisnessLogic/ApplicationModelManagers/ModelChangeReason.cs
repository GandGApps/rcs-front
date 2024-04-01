// This file is ported and adapted from DynamicData reactivemarbles / DynamicData

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kassa.BuisnessLogic.ApplicationModelManagers;

/// <summary>
/// <para>The reason for an individual change to an observable list.</para>
/// <para>Used to signal consumers of any changes to the underlying cache.</para>
/// </summary>
public enum ModelChangeReason : byte
{
    /// <summary>
    ///  An item has been added.
    /// </summary>
    Add,

    /// <summary>
    /// A range of items has been added.
    /// </summary>
    AddRange,

    /// <summary>
    ///  An item has been replaced.
    /// </summary>
    Replace,

    /// <summary>
    ///  An item has removed.
    /// </summary>
    Remove,

    /// <summary>
    /// A range of items has been removed.
    /// </summary>
    RemoveRange,

    /// <summary>
    ///   Command to operators to re-evaluate.
    /// </summary>
    Refresh,

    /// <summary>
    /// An item has been moved in a sorted collection.
    /// </summary>
    Moved,

    /// <summary>
    /// The entire collection has been cleared.
    /// </summary>
    Clear,
}

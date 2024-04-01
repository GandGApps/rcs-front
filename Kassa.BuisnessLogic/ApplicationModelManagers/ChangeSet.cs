// This file is ported and adapted from DynamicData reactivemarbles / DynamicData

using System.Collections.Immutable;
using Kassa.DataAccess.Model;

namespace Kassa.BuisnessLogic.ApplicationModelManagers;
public readonly struct ChangeSet<TModel> where TModel : class, IModel
{

    /// <summary>
    /// An empty change set.
    /// </summary>
    public static readonly ChangeSet<TModel> Empty = new();

    public static ChangeSet<TModel> Create(ImmutableArray<Change<TModel>> models, int startingIndex)
    {
        var adds = 0;
        foreach (var item in models)
        {
            switch (item.Reason)
            {
                case ModelChangeReason.Add:
                    adds++;
                    break;

                case ModelChangeReason.AddRange:
                    adds++;
                    break;
            }
        }

        var moves = models.Count(c => c.Reason == ModelChangeReason.Moved);
        var refreshes = models.Count(c => c.Reason == ModelChangeReason.Refresh);

        var removes = 0;
        foreach (var item in models)
        {

            switch (item.Reason)
            {

                case ModelChangeReason.Remove:
                    removes++;
                    break;

                case ModelChangeReason.RemoveRange:
                    removes++;
                    break;
            }
        }

        var replaces = models.Count(c => c.Reason == ModelChangeReason.Replace);

        return new(adds, removes, replaces, moves, refreshes, models, startingIndex);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ChangeSet{T}"/> class.
    /// </summary>
    public ChangeSet()
    {

    }

    private ChangeSet(int adds, int removes, int replaces, int moves, int refreshes, ImmutableArray<Change<TModel>> changes, int index)
    {
        Adds = adds;
        Removes = removes;
        Replaced = replaces;
        Moves = moves;
        Refreshes = refreshes;
        TotalChanges = adds + removes + replaces + moves + refreshes;
        Changes = changes;
        Index = index;
    }

    public readonly ImmutableArray<Change<TModel>> Changes
    {
        get;
    }


    /// <summary>
    ///  Gets the number of additions.
    /// </summary>
    public readonly int Adds
    {
        get;
    }

    /// <summary>
    /// Gets the number of moves.
    /// </summary>
    public readonly int Moves
    {
        get;
    }

    /// <summary>
    /// Gets the number of removes.
    /// </summary>
    public readonly int Refreshes
    {
        get;
    }

    /// <summary>
    ///  Gets the number of removes.
    /// </summary>
    public readonly int Removes
    {
        get;
    }

    /// <summary>
    ///  Gets the number of updates.
    /// </summary>
    public readonly int Replaced
    {
        get;
    }

    /// <summary>
    /// Gets the total number if individual item changes.
    /// </summary>
    public readonly int TotalChanges
    {
        get;
    }

    public readonly int Index
    {
        get;
    }

    public readonly bool IsEmpty => TotalChanges == 0;

    /// <summary>
    /// Returns a <see cref="string" /> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="string" /> that represents this instance.
    /// </returns>
    public override string ToString() => $"ChangeSet<{typeof(TModel).Name}>. Count={Changes.Length}";
}

// This file is ported and adapted from DynamicData reactivemarbles / DynamicData

using DynamicData;
using Kassa.DataAccess.Model;
using Microsoft.VisualBasic;

namespace Kassa.BuisnessLogic.ApplicationModelManagers;
public readonly struct Change<TModel> : IEquatable<Change<TModel>>
    where TModel : class, IModel
{

    public static Change<TModel> Add(TModel model, int index)
    {
        return new Change<TModel>(model, ModelChangeReason.Add, index, null, -1);
    }

    public static Change<TModel> Update(TModel current, TModel previous, int index)
    {
        return new Change<TModel>(current, ModelChangeReason.Refresh, index, previous, index);
    }

    public static Change<TModel> Remove(TModel model, int currentIndex)
    {
        return new Change<TModel>(model, ModelChangeReason.Remove, currentIndex, null, -1);
    }

    public static Change<TDestination> Transform<TDestination>(Change<TModel> change, Func<TModel, TDestination> transform)
        where TDestination : class, IModel
    {
        TDestination? previos;
        if (change.Previous is null)
        {
            previos = null!;
        }
        else
        {
            previos = transform(change.Previous);
        }

        return new Change<TDestination>(transform(change.Current), change.Reason, change.CurrentIndex, previos, change.PreviousIndex);
    }

    public override bool Equals(object? obj) => obj is Change<TModel> change && Equals(change);
    public bool Equals(Change<TModel> other) => Id.Equals(other.Id) && Reason == other.Reason && EqualityComparer<TModel>.Default.Equals(Current, other.Current) && CurrentIndex == other.CurrentIndex && EqualityComparer<TModel?>.Default.Equals(Previous, other.Previous) && PreviousIndex == other.PreviousIndex;
    public override int GetHashCode() => HashCode.Combine(Id, Reason, Current, CurrentIndex, Previous, PreviousIndex);

    private Change(TModel model, ModelChangeReason reason, int currentIndex, TModel? previous, int previousIndex)
    {
        Id = model.Id;
        Reason = reason;
        Current = model;
        CurrentIndex = currentIndex;
        Previous = previous;
        PreviousIndex = previousIndex;
    }

    public readonly Guid Id
    {
        get;
    }

    /// <summary>
    /// Gets the  reason for the change.
    /// </summary>
    public readonly ModelChangeReason Reason
    {
        get;
    }

    /// <summary>
    /// Gets the item which has changed.
    /// </summary>
    public readonly TModel Current
    {
        get;
    }

    /// <summary>
    /// Gets the current index.
    /// </summary>
    public readonly int CurrentIndex
    {
        get;
    }

    /// <summary>
    /// <para>Gets the previous change.</para>
    /// <para>This is only when Reason==ModelChangeReason.Replace.</para>
    /// </summary>
    public readonly TModel? Previous
    {
        get;
    }

    /// <summary>
    /// <para>Gets the previous change.</para>
    /// <para>This is only when Reason==ModelChangeReason.Update or ModelChangeReason.Move.</para>
    /// </summary>
    public readonly int PreviousIndex
    {
        get;
    }


    public static bool operator ==(Change<TModel> left, Change<TModel> right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Change<TModel> left, Change<TModel> right)
    {
        return !(left == right);
    }
}

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Kassa.DataAccess.Model;
using Kassa.Shared.Collections;
using Splat.ModeDetection;

namespace Kassa.BuisnessLogic.ApplicationModelManagers;
public static class ApplicationModelManagersExtensions
{
    public static IObservable<ChangeSet<TModel>> Filter<TModel>(this IObservable<ChangeSet<TModel>> observable, Func<TModel, bool> predicate)
        where TModel : class, IModel
    {
        var filteredOservable = observable.Select(changes =>
        {
            if (changes.IsEmpty)
            {
                return ChangeSet<TModel>.Empty;
            }

            using var builder = ImmutableArrayBuilder<Change<TModel>>.Rent();

            foreach (var change in changes.Changes)
            {
                if (predicate(change.Current))
                {
                    builder.Add(change);
                }
            }

            if (builder.Count == 0)
            {
                return ChangeSet<TModel>.Empty;
            }

            return ChangeSet<TModel>.Create(builder.ToImmutable(), changes.Index);

        }).Where(x => !x.IsEmpty);

        return PackObservable(observable, filteredOservable);
    }

    public static IObservable<ChangeSet<TDestionation>> Transform<TModel, TDestionation>(this IObservable<ChangeSet<TModel>> observable, Func<TModel, TDestionation> transform)
        where TModel : class, IModel
        where TDestionation : class, IModel
    {
        IApplicationModelManager<TModel>? manager;

        if (observable is SourceApplicationModelManager<TModel> sourceApplicationModelManager)
        {
            manager = sourceApplicationModelManager.Source;
        }
        else if (observable is IApplicationModelManager<TModel> applicationModelManager)
        {

            manager = applicationModelManager;
        }
        else
        {
            manager = null;
        }

        return observable.Scan(ChangeSet<TDestionation>.Empty, (cache, changes) =>
        {
            if (changes.IsEmpty)
            {
                return cache;
            }

            using var builder = ImmutableArrayBuilder<Change<TDestionation>>.Rent(cache.IsEmpty ? 8 : cache.Changes.Length);

            if (!cache.IsEmpty)
            {
                builder.AddRange(cache.Changes.AsSpan());
            }

            foreach (var change in changes.Changes)
            {
                var transformedChange = Change<TModel>.Transform(change, transform);
                var destination = transformedChange.Current;

                if (destination is IApplicationModelPresenter<TModel> applicationModelPresenter)
                {
                    manager?.AddPresenter(applicationModelPresenter);
                }

                builder.Add(transformedChange);
            }

            return ChangeSet<TDestionation>.Create(builder.ToImmutable(), changes.Index);
        }).Where(x => !x.IsEmpty);
    }

    public static IObservable<ChangeSet<TModel>> Bind<TModel>(this IObservable<ChangeSet<TModel>> observable, out ReadOnlyObservableCollection<TModel> models)
        where TModel : class, IModel
    {
        var collection = new ObservableCollection<TModel>();
        models = new ReadOnlyObservableCollection<TModel>(collection);

        observable.Subscribe(changeSet =>
        {
            foreach (var change in changeSet.Changes)
            {
                switch (change.Reason)
                {
                    case ModelChangeReason.Add:
                        collection.Add(change.Current);
                        break;
                    case ModelChangeReason.Refresh:
                        var index = collection.IndexOf(change.Current);
                        if (index != -1)
                        {
                            collection[index] = change.Current;
                        }
                        break;
                    case ModelChangeReason.Remove:
                        collection.Remove(change.Current);
                        break;
                }
            }
        });

        return observable;
    }

    public static IObservable<ChangeSet<TCast>> Cast<TModel, TCast>(this IObservable<ChangeSet<TModel>> observable)
        where TCast : class, IModel, TModel
        where TModel : class, IModel
    {
        return observable.Select(changes =>
        {

            if (changes.IsEmpty)
            {

                return ChangeSet<TCast>.Empty;
            }

            using var builder = ImmutableArrayBuilder<Change<TCast>>.Rent(changes.Changes.Length);

            foreach (var change in changes.Changes)
            {

                builder.Add(Change<TModel>.Transform(change, x => (TCast)x));
            }

            return ChangeSet<TCast>.Create(builder.ToImmutable(), changes.Index);
        }).Where(x => !x.IsEmpty);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static IObservable<ChangeSet<TModel>> PackObservable<TModel>(IObservable<ChangeSet<TModel>> observable, IObservable<ChangeSet<TModel>> modifiedObservable)
        where TModel : class, IModel
    {
        if (observable is SourceApplicationModelManager<TModel> sourceApplicationModelManager)
        {
            return new SourceApplicationModelManager<TModel>(sourceApplicationModelManager.Source, modifiedObservable);
        }

        if (observable is IApplicationModelManager<TModel> applicationModelManager)
        {
            return new SourceApplicationModelManager<TModel>(applicationModelManager, modifiedObservable);
        }

        return observable;
    }
}

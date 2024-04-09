using System;
using System.Buffers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
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


    public static IDisposable BindAndFilter<TModel, TCast>(this IApplicationModelManager<TModel> manager, Func<TModel, bool> filter, Func<TModel, TCast> selector, out ReadOnlyObservableCollection<TCast> collection)
        where TModel : class, IModel
        where TCast : class, IApplicationModelPresenter<TModel>
    {
        var target = new ObservableCollection<TCast>();
        collection = new(target);

        var disposables = new CompositeDisposable();

        target.AddRange(manager.Values.Where(filter).Select(selector));

        manager.Subscribe(changeSet =>
        {

            foreach (var change in changeSet.Changes)
            {
                var model = change.Current;

                if (!filter(model))
                {
                    continue;
                }

                switch (change.Reason)
                {

                    case ModelChangeReason.Add:
                        var added = selector(model);
                        target.Add(added);
                        manager.AddPresenter(added).DisposeWith(disposables);
                        break;

                    case ModelChangeReason.Remove:
                        var removed = target.FirstOrDefault(x => x.Id == model.Id);
                        if (removed != null)
                        {
                            target.Remove(removed);
                        }
                        break;

                }
            }

        }).DisposeWith(disposables);

        return disposables;
    }

    public static IDisposable BindAndFilter<TModel, TCast>(this IApplicationModelManager<TModel> manager, IObservable<Func<TModel, bool>> filterChangeable, Func<TModel, TCast> selector, out ReadOnlyObservableCollection<TCast> collection)
      where TModel : class, IModel
      where TCast : class, IApplicationModelPresenter<TModel>
    {
        var target = new ObservableCollection<TCast>();
        collection = new(target);

        var disposables = new CompositeDisposable();
        var internalDisposables = new CompositeDisposable();


        filterChangeable.Subscribe(filter =>
        {
            internalDisposables.Dispose();

            target.Clear();
            target.AddRange(manager.Values.Where(filter).Select(selector));

            internalDisposables = [];

            manager.Subscribe(changeSet =>
            {

                foreach (var change in changeSet.Changes)
                {
                    var model = change.Current;

                    if (!filter(model))
                    {
                        continue;
                    }

                    switch (change.Reason)
                    {

                        case ModelChangeReason.Add:
                            var added = selector(model);
                            target.Add(added);
                            manager.AddPresenter(added).DisposeWith(internalDisposables);
                            break;

                        case ModelChangeReason.Remove:
                            var removed = target.FirstOrDefault(x => x.Id == model.Id);
                            if (removed != null)
                            {
                                target.Remove(removed);
                            }
                            break;

                    }
                }

            }).DisposeWith(internalDisposables);

        }).DisposeWith(disposables);

       

        return disposables;
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

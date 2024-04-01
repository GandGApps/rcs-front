using System.Collections;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using DynamicData;
using Kassa.DataAccess.Model;
using Kassa.Shared.Collections;

namespace Kassa.BuisnessLogic.ApplicationModelManagers;
internal partial class HostModelManager<TModel> : IApplicationModelManager<TModel>
    where TModel : class, IModel
{
    private readonly Dictionary<Guid, TModel> _models = [];
    private readonly Subject<ChangeSet<TModel>> _changes = new();
    private readonly List<IApplicationModelPresenter<TModel>> _applicationModelPresenters = [];

    public void AddOrUpdate(TModel model)
    {
        Change<TModel> change;
        var startingIndex = _models.Count;

        if (_models.TryGetValue(model.Id, out var value))
        {
            change = Change<TModel>.Update(model, value, _models.Values.IndexOf(value));
        }
        else
        {
            change = Change<TModel>.Add(model, _models.Count);
        }

        using var builder = ImmutableArrayBuilder<Change<TModel>>.Rent();

        builder.Add(change);

        if (_applicationModelPresenters.Count > 0)
        {

            foreach (var presenter in _applicationModelPresenters)
            {
                if (presenter.Id == model.Id)
                {
                    presenter.ModelChanged(change);
                }
            }
        }

        var changes = builder.ToImmutable();

        _changes.OnNext(ChangeSet<TModel>.Create(changes, startingIndex));
    }

    public void AddOrUpdate(IEnumerable<TModel> models)
    {
        using var builder = ImmutableArrayBuilder<Change<TModel>>.Rent();
        var startingIndex = _models.Count;

        foreach (var model in models)
        {

            Change<TModel> change;

            if (_models.TryGetValue(model.Id, out var value))
            {

                change = Change<TModel>.Update(model, value, _models.Values.IndexOf(value));
            }
            else
            {

                change = Change<TModel>.Add(model, _models.Count);
            }

            builder.Add(change);

            if (_applicationModelPresenters.Count > 0)
            {

                foreach (var presenter in _applicationModelPresenters)
                {
                    if (presenter.Id == model.Id)
                    {
                        presenter.ModelChanged(change);
                    }
                }
            }
        }

        var changes = builder.ToImmutable();

        _changes.OnNext(ChangeSet<TModel>.Create(changes, startingIndex));
    }

    public void Remove(Guid id)
    {
        if (_models.TryGetValue(id, out var model))
        {
            _models.Remove(id);

            var change = Change<TModel>.Remove(model, _models.Values.IndexOf(model));

            using var builder = ImmutableArrayBuilder<Change<TModel>>.Rent();

            builder.Add(change);

            if (_applicationModelPresenters.Count > 0)
            {

                foreach (var presenter in _applicationModelPresenters)
                {
                    if (presenter.Id == model.Id)
                    {
                        DisposePresenter(presenter);
                    }
                }
            }

            var changes = builder.ToImmutable();

            _changes.OnNext(ChangeSet<TModel>.Create(changes, _models.Count));
        }
    }

    public void Remove(IEnumerable<Guid> ids)
    {
        using var builder = ImmutableArrayBuilder<Change<TModel>>.Rent();

        foreach (var id in ids)
        {
            if (_models.TryGetValue(id, out var model))
            {
                _models.Remove(id);

                var change = Change<TModel>.Remove(model, _models.Values.IndexOf(model));

                if (_applicationModelPresenters.Count > 0)
                {

                    foreach (var presenter in _applicationModelPresenters)
                    {
                        if (presenter.Id == model.Id)
                        {
                            DisposePresenter(presenter);
                        }
                    }
                }

                builder.Add(change);
            }
        }

        var changes = builder.ToImmutable();

        _changes.OnNext(ChangeSet<TModel>.Create(changes, _models.Count));
    }

    public void Clear()
    {
        using var builder = ImmutableArrayBuilder<Change<TModel>>.Rent();

        foreach (var model in _models.Values)
        {
            var change = Change<TModel>.Remove(model, _models.Values.IndexOf(model));

            foreach (var presenter in _applicationModelPresenters)
            {
                if (presenter.Id == model.Id)
                {
                    DisposePresenter(presenter);
                }
            }

            builder.Add(change);
        }

        _models.Clear();

        var changes = builder.ToImmutable();

        _changes.OnNext(ChangeSet<TModel>.Create(changes, 0));
    }

    public IDisposable AddPresenter(IApplicationModelPresenter<TModel> presenter)
    {
        _applicationModelPresenters.Add(presenter);

        return Disposable.Create(() => _applicationModelPresenters.Remove(presenter));
    }

    private void DisposePresenter(IApplicationModelPresenter<TModel> presenter)
    {
        _applicationModelPresenters.Remove(presenter);

        presenter.Dispose();
    }

    public void Dispose()
    {
        _changes.Dispose();
        _models.Clear();

        foreach (var presenter in _applicationModelPresenters)
        {
            DisposePresenter(presenter);
        }

        _applicationModelPresenters.Clear();
    }

    #region IDictionnary implementation
    public TModel this[Guid key] => _models[key];

    public IEnumerable<Guid> Keys => _models.Keys;

    public IEnumerable<TModel> Values => _models.Values;

    public int Count => _models.Count;

    public bool ContainsKey(Guid key) => _models.ContainsKey(key);
    public IEnumerator<KeyValuePair<Guid, TModel>> GetEnumerator() => _models.GetEnumerator();
    public bool TryGetValue(Guid key, [MaybeNullWhen(false)] out TModel value) => _models.TryGetValue(key, out value);
    IEnumerator IEnumerable.GetEnumerator() => _models.GetEnumerator();

    #endregion

    #region IObservable implementation
    public IDisposable Subscribe(IObserver<ChangeSet<TModel>> observer) => _changes.Subscribe(observer);

    #endregion
}

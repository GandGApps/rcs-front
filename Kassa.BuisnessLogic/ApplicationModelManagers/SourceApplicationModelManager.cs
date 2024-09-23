using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kassa.Shared;

namespace Kassa.BuisnessLogic.ApplicationModelManagers;
internal sealed class SourceApplicationModelManager<TModel>(IApplicationModelManager<TModel> sourceManager, IObservable<ChangeSet<TModel>> observable) : IObservable<ChangeSet<TModel>>
     where TModel : class, IGuidId
{
    public IDisposable Subscribe(IObserver<ChangeSet<TModel>> observer) => observable.Subscribe(observer);

    public IApplicationModelManager<TModel> Source => sourceManager;
}

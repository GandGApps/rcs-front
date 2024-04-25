using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace Kassa.BuisnessLogic;
public sealed class ObservableOnlyBehaviourSubject<T> : IObservableOnlyBehaviourSubject<T>
{
    private readonly BehaviorSubject<T> _behaviorSubject;

    public ObservableOnlyBehaviourSubject(BehaviorSubject<T> behaviorSubject)
    {
        _behaviorSubject = behaviorSubject;
    }

    public IDisposable Subscribe(IObserver<T> observer) => _behaviorSubject.Subscribe(observer);

    public T Value => _behaviorSubject.Value;
}

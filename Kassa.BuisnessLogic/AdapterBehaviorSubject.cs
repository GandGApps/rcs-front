using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace Kassa.BuisnessLogic;
public sealed class AdapterBehaviorSubject<T> : ISubject<T>, IObservableOnlyBehaviourSubject<T>, IDisposable 
{
    private readonly BehaviorSubject<T> _subject;

    public AdapterBehaviorSubject(T value)
    {
        _subject = new BehaviorSubject<T>(value);
    }

    public T Value => _subject.Value;

    public void Dispose() => _subject.Dispose();

    public void OnCompleted() => _subject.OnCompleted();
    public void OnError(Exception error) => _subject.OnError(error);
    public void OnNext(T value) => _subject.OnNext(value);
    public IDisposable Subscribe(IObserver<T> observer) => _subject.Subscribe(observer);
}

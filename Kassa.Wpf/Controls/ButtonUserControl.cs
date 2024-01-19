using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Kassa.RxUI;
using ReactiveUI;

namespace Kassa.Wpf.Controls;
public abstract class ButtonUserControl<T> : ButtonWithCornerRaduis, IActivatableView, IViewFor<T> where T : class
{

    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
        nameof(ViewModel), typeof(T), typeof(ButtonUserControl<T>), new PropertyMetadata(default(T)));

    public ButtonUserControl()
    {
    }

    public T? ViewModel
    {
        get => (T?)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    protected void ObserveDataContext(CompositeDisposable disposables) => 
        this.WhenAnyValue(x => x.DataContext)
            .Subscribe(x => ViewModel = (T?)x)
            .DisposeWith(disposables);

    protected void ObserveViewModel(CompositeDisposable disposables) =>
        this.WhenAnyValue(x => x.ViewModel)
            .Subscribe(x => DataContext = x)
            .DisposeWith(disposables);

    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (T?)value;
    }
}

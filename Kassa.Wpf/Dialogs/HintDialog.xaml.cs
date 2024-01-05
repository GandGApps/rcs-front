using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Kassa.RxUI.Dialogs;
using ReactiveUI;

namespace Kassa.Wpf.Dialogs;
/// <summary>
/// Логика взаимодействия для HintDialog.xaml
/// </summary>
public partial class HintDialog : ReactiveUserControl<HintDialogViewModel>
{
    private static readonly CornerInfo LeftTop = new(449.6, 154.8, -1, -1);
    private static readonly CornerInfo RightTop = new(-18.8, 151.6, scaleY:-1);
    private static readonly CornerInfo RightBottom = new(-16, -50);
    private static readonly CornerInfo LeftBottom = new(443.388, -63.2, -1);

    public HintDialog()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            this.OneWayBind(ViewModel, vm => vm.Text, v => v.HintText.Text)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, vm => vm.Step, v => v.StepsText.Text, x => $"Шаг {x} из {ViewModel!.Hints.Count}")
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, vm => vm.Hints.Count, v => v.StepsText.Text, x => $"Шаг {ViewModel!.Step} из {x}")
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, vm => vm.OkCommand, v => v.OkButton)
                .DisposeWith(disposables);

            UpdateTarget();

            ViewModel.WhenAnyValue(x => x.Target, x => x.Root)
                     .Subscribe(x =>
                     {
                         if (x.Item1 is FrameworkElement control || x.Item2 is FrameworkElement root)
                         {
                             UpdateTarget();
                         }

                     })
                     .DisposeWith(disposables);
        });
    }

    private void UpdateTarget()
    {
        if (ViewModel?.Target is not FrameworkElement element)
        {
            return;
        }

        var point = new Point();

        if (ViewModel?.Root is FrameworkElement root)
        {
            point = root.TranslatePoint(point, element);
        }
        else
        {
            point = App.Current.MainWindow.TranslatePoint(point, element);
            root = App.Current.MainWindow;
        }

        TargetRect.Width = element.ActualWidth;
        TargetRect.Height = element.ActualHeight;

        TargetVisual.Visual = element;

        TargetRect.Margin = new(-point.X, -point.Y, 0, 0);

        var cornerInfo = GetCornerInfo(
            new(TargetRect.Width / 2, TargetRect.Height + 70), 
            point,
            new(Hint.Width, Hint.Height),
            new(root.ActualWidth, root.ActualHeight),
            new(TargetRect.Width, TargetRect.Height),
            out var margin);

        Canvas.SetLeft(Obloko, cornerInfo.Left);
        Canvas.SetTop(Obloko, cornerInfo.Top);

        OblokoScaleTransform.ScaleX = cornerInfo.ScaleX;
        OblokoScaleTransform.ScaleY = cornerInfo.ScaleY;

        Hint.Margin = margin;
    }

    private readonly struct CornerInfo(double left, double top, double scaleX = 1, double scaleY = 1)
    {
        public double Left
        {
            get;
        } = left;

        public double Top
        {
            get;
        } = top;

        public double ScaleX
        {
            get;
        } = scaleX;

        public double ScaleY
        {
            get;
        } = scaleY;
    }

    private static CornerInfo GetCornerInfo(Point requiredMargin, Point point, Point elementSize, Point containerSize, Point targetSize, out Thickness margin)
    {
        if (requiredMargin.Y + (-point.Y) + elementSize.Y > containerSize.Y)
        {
            if (requiredMargin.X + (-point.X) + elementSize.X > containerSize.X)
            {
                margin = new((-point.X) - Math.Abs(elementSize.X - targetSize.X) - requiredMargin.X, (-point.Y) - requiredMargin.Y + Math.Abs(elementSize.Y - targetSize.Y), 0, 0);
                return LeftTop;
            }
            else
            {
                margin = new((-point.X) + requiredMargin.X, (-point.Y) - requiredMargin.Y + Math.Abs(elementSize.Y - targetSize.Y), 0, 0);
                return RightTop;
            }
        }
        else
        {
            if (requiredMargin.X + (-point.X) + elementSize.X > containerSize.X)
            {
                margin = new((-point.X) - Math.Abs(elementSize.X - targetSize.X)- requiredMargin.X, requiredMargin.Y + (-point.Y), 0, 0);
                return LeftBottom;
            }
            else
            {
                margin = new((-point.X) + requiredMargin.X, requiredMargin.Y + (-point.Y), 0, 0);
                return RightBottom;
            }
        }
    }
}

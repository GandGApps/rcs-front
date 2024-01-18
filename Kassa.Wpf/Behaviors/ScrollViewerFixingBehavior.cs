using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;

namespace Kassa.Wpf.Behaviors;
public class ScrollViewerFixingBehavior: Behavior<ScrollViewer>
{

    protected override void OnAttached()
    {
        base.OnAttached();

        AssociatedObject.ManipulationBoundaryFeedback += Handled;
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();

        AssociatedObject.ManipulationBoundaryFeedback -= Handled;
    }

    private void Handled(object? sender, System.Windows.Input.ManipulationBoundaryFeedbackEventArgs e)
    {
        e.Handled = true;
    }
}

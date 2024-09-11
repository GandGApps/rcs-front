using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Diagnostics;

namespace Kassa.Wpf;
public partial class MainWindow
{
    private Stopwatch stopwatch;
    private int frameCount;
    private double lastRenderTime;

    private void InitFpsCounter()
    {
        stopwatch = Stopwatch.StartNew();
        FpsCounter.Visibility = Visibility.Visible;
        CompositionTarget.Rendering += (_,_) => UpdateFpsCounter();
    }

    private void UpdateFpsCounter()
    {
        frameCount++;

        var elapsedSeconds = stopwatch.Elapsed.TotalSeconds - lastRenderTime;

        if (elapsedSeconds >= 1)
        {
            var fps = frameCount / elapsedSeconds;

            FpsCounter.Text = $"FPS: {fps:F2}";

            frameCount = 0;

            lastRenderTime = stopwatch.Elapsed.TotalSeconds;
        }
    }
}

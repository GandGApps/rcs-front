using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Diagnostics;
using System.Windows.Threading;

namespace Kassa.Wpf;
public partial class MainWindow
{
    private readonly FpsCounterHelper rendererFpsCounterHelper = new();

    private void InitFpsCounter()
    {
        rendererFpsCounterHelper.Init();
        FpsCounter.Visibility = Visibility.Visible;

        CompositionTarget.Rendering += (_,_) => UpdateFpsCounter();
    }

    private void UpdateFpsCounter()
    {
        var fps = rendererFpsCounterHelper.UpdateFpsCounter();

        if (fps > 0)
        {
            FpsCounter.Text = $"Renderer: {fps:F2} fps";
        }
    }

    private sealed class FpsCounterHelper
    {
        private Stopwatch stopwatch = null!;
        private int frameCount;
        private double lastRenderTime;

        public void Init()
        {
            stopwatch = Stopwatch.StartNew();
            lastRenderTime = stopwatch.Elapsed.TotalSeconds;
        }

        public double UpdateFpsCounter()
        {
            frameCount++;

            var elapsedSeconds = stopwatch.Elapsed.TotalSeconds - lastRenderTime;

            if (elapsedSeconds >= 1)
            {
                var fps = frameCount / elapsedSeconds;

                frameCount = 0;

                lastRenderTime = stopwatch.Elapsed.TotalSeconds;

                return fps;
            }

            return 0;
        }
    }
}

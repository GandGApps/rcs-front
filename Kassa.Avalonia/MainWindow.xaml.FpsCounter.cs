using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;

namespace Kassa.Avalonia;
public partial class MainWindow
{
    private readonly FpsCounterHelper fpsCounterHelper = new();

    private void InitFpsCounter()
    {
        fpsCounterHelper.Init();
        FpsCounter.IsVisible = true;

        RequestAnimationFrame(Tick);
    }

    public void Tick(TimeSpan time) 
    {
        var fps = fpsCounterHelper.UpdateFpsCounter();

        if (fps > 0)
        {
            FpsCounter.Text = $"Renderer: {fps:F2} fps;";
        }

        RequestAnimationFrame(Tick);
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

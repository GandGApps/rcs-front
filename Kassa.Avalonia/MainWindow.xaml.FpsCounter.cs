using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Rendering;
using Avalonia.Threading;

namespace Kassa.Avalonia;
public partial class MainWindow
{
    private Stopwatch stopwatch;
    private int frameCount;
    private double lastRenderTime;

    private void InitFpsCounter()
    {
        if (GetTopLevel(this) is TopLevel topLevel)
        {
            topLevel.RendererDiagnostics.DebugOverlays |= RendererDebugOverlays.Fps;
        }

        stopwatch = Stopwatch.StartNew();
        FpsCounter.IsVisible = true;

        UpdateFpsCounter();
    }

    private void UpdateFpsCounter()
    {
        frameCount++;

        var elapsedSeconds = stopwatch.Elapsed.TotalSeconds - lastRenderTime;

        if (elapsedSeconds >= 0.5)
        {
            var fps = frameCount / elapsedSeconds;

            FpsCounter.Text = $"FPS: {fps:F2}";

            frameCount = 0;

            lastRenderTime = stopwatch.Elapsed.TotalSeconds;

        }

        Dispatcher.UIThread.Post(UpdateFpsCounter, DispatcherPriority.Background);
    }
}

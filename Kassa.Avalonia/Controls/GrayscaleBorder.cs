using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Metadata;
using Avalonia.Platform;
using Avalonia.Threading;

namespace Kassa.Avalonia.Controls;
public sealed class GrayscaleBorder : Control
{
    public static readonly StyledProperty<bool> IsGrayscaleProperty =
        AvaloniaProperty.Register<GrayscaleBorder, bool>(nameof(IsGrayscale), defaultValue: false);

    public static readonly StyledProperty<Control?> ChildProperty =
        AvaloniaProperty.Register<GrayscaleBorder, Control?>(nameof(Child));

    public static readonly DirectProperty<GrayscaleBorder, IImage?> RenderedProperty =
        AvaloniaProperty.RegisterDirect<GrayscaleBorder, IImage?>(nameof(Rendered), x => x.Rendered);

    public bool IsGrayscale
    {
        get => GetValue(IsGrayscaleProperty);
        set => SetValue(IsGrayscaleProperty, value);
    }

    public Control? Child
    {
        get => GetValue(ChildProperty);
        set => SetValue(ChildProperty, value);
    }

    private IImage? _rendered;
    public IImage? Rendered
    {
        get => _rendered;
        private set => SetAndRaise(RenderedProperty, ref _rendered, value);
    }

    static GrayscaleBorder()
    {
        AffectsRender<GrayscaleBorder>(IsGrayscaleProperty);
        ChildProperty.Changed.AddClassHandler<GrayscaleBorder>((x, e) => x.OnChildChanged(e));
    }

    public GrayscaleBorder()
    {
        IsHitTestVisible = false;
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        if (!IsGrayscale)
        {
            return;
        }

        if (Child is Control control)
        {

            if (control.Bounds.Width < 1)
            {
                return;
            }

            if (control.Bounds.Height < 1)
            {
                return;
            }

            var pixelSize = new PixelSize((int)control.Bounds.Width, (int)control.Bounds.Height);
            var bitmapSource = new RenderTargetBitmap(pixelSize);
            var bounds = Bounds;

            bitmapSource.Render(control);

            // Convert to grayscale

            var size = bitmapSource.Size;

            var stride = (int)size.Width * 4;
            var bufferSize = stride * (int)pixelSize.Height;
            var bufferPtr = Marshal.AllocHGlobal(bufferSize);


            try
            {
                bitmapSource.CopyPixels(new PixelRect(0, 0, (int)size.Width, (int)size.Height), bufferPtr, bufferSize, stride);

                unsafe
                {
                    var buffer = (byte*)bufferPtr.ToPointer();

                    for (var y = 0; y < size.Height; y++)
                    {
                        for (var x = 0; x < size.Width; x++)
                        {
                            var index = (y * stride) + (x * 4);

                            var blue = buffer[index];
                            var green = buffer[index + 1];
                            var red = buffer[index + 2];
                            var alpha = buffer[index + 3];

                            var gray = (byte)(red * 0.3 + green * 0.59 + blue * 0.11);

                            buffer[index] = gray; 
                            buffer[index + 1] = gray;  
                            buffer[index + 2] = gray;
                            buffer[index + 3] = 255;
                        }
                    }

                    var bitmap = new Bitmap(PixelFormats.Bgra8888, AlphaFormat.Premul, bufferPtr, bitmapSource.PixelSize, bitmapSource.Dpi, stride);

                    Rendered = bitmap;

                    context.DrawImage(bitmap, bounds);
                }
            }
            finally
            {
                Marshal.FreeHGlobal(bufferPtr);
            }
        }

        Dispatcher.UIThread.Post(() => InvalidateVisual(), DispatcherPriority.Background);

    }

    

    private void OnChildChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if (e.OldValue is Control oldControl)
        {
            oldControl.PropertyChanged -= OnControlPropertyChanged;
        }

        if (e.NewValue is Control newControl)
        {
            newControl.PropertyChanged += OnControlPropertyChanged;
        }
    }

    private void OnControlPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        InvalidateVisual();

        if (e.Property == Control.BoundsProperty)
        {
            if (Child!.Bounds.Width > 0)
            {
                return;
            }

            if (Child!.Bounds.Height > 0)
            {
                return;
            }

            Width = Child!.Bounds.Width;
            Height = Child!.Bounds.Height;
        }
    }



}

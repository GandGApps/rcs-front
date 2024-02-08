using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Effects;

namespace Kassa.Wpf.Effects;
public class DialogShaderEffect : ShaderEffect
{
    public static readonly DependencyProperty InputProperty = RegisterPixelShaderSamplerProperty("input", typeof(DialogShaderEffect), 0);

    public static readonly DependencyProperty EnableGrayscaleProperty = DependencyProperty.Register(
        "EnableGrayscale",
        typeof(float),
        typeof(DialogShaderEffect),
        new PropertyMetadata(1.0f, PixelShaderConstantCallback(0)));

    public float EnableGrayscale
    {
        get => (float)GetValue(EnableGrayscaleProperty);
        set => SetValue(EnableGrayscaleProperty, value);
    }

    public DialogShaderEffect()
    {
        var pixelShader = new PixelShader
        {
            UriSource = new Uri("pack://application:,,,/Kassa.Wpf;component/Shaders/grayshader.ps")
        };
        PixelShader = pixelShader;

        UpdateShaderValue(InputProperty);

    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "Kassa.Wpf.Controls")]

namespace Kassa.Wpf.Controls;

public class Squircle : Decorator
{
    #region Private Fields

    private PathGeometry? _borderGeometryCache;

    private Pen? _penCache;

    #endregion

    #region Dependency Properties

    public static readonly DependencyProperty CurvatureProperty = DependencyProperty.Register(
        nameof(Curvature), typeof(double), typeof(Squircle), new FrameworkPropertyMetadata(1.0,
            FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender),
        IsCurvatureValid);

    private static bool IsCurvatureValid(object value)
    {
        if (value is double curvature)
            return curvature >= 0 && curvature <= 1;

        return false;
    }

    /// <summary>
    /// DependencyProperty for <see cref="BorderThickness" /> property.
    /// </summary>
    public static readonly DependencyProperty BorderThicknessProperty
        = DependencyProperty.Register("BorderThickness", typeof(double), typeof(Squircle),
            new FrameworkPropertyMetadata(0.0,
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
                OnClearPenCache), IsBorderThicknessValid);

    private static bool IsBorderThicknessValid(object value)
    {
        if (value is double borderThickness)
            return borderThickness >= 0;

        return false;
    }

    private static void OnClearPenCache(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        Squircle border = (Squircle)d;
        border._penCache = null;
    }

    /// <summary>
    /// DependencyProperty for <see cref="Padding" /> property.
    /// </summary>
    public static readonly DependencyProperty PaddingProperty
        = DependencyProperty.Register("Padding", typeof(Thickness), typeof(Squircle),
            new FrameworkPropertyMetadata(
                new Thickness(),
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender),
            IsThicknessValid);

    private static bool IsThicknessValid(object value)
    {
        Thickness t = (Thickness)value;

        return true;
    }

    /// <summary>
    /// DependencyProperty for <see cref="BorderBrush" /> property.
    /// </summary>
    public static readonly DependencyProperty BorderBrushProperty
        = DependencyProperty.Register("BorderBrush", typeof(Brush), typeof(Squircle),
            new FrameworkPropertyMetadata(default(Brush),
                FrameworkPropertyMetadataOptions.AffectsRender |
                FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                OnClearPenCache));

    /// <summary>
    /// DependencyProperty for <see cref="Background" /> property.
    /// </summary>
    public static readonly DependencyProperty BackgroundProperty =
        Panel.BackgroundProperty.AddOwner(typeof(Squircle),
            new FrameworkPropertyMetadata(default(Brush),
                FrameworkPropertyMetadataOptions.AffectsRender |
                FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

    #endregion

    #region Public Properties

    public double Curvature
    {
        get => (double)GetValue(CurvatureProperty);
        set => SetValue(CurvatureProperty, value);
    }

    /// <summary>
    /// The BorderThickness property defined how thick a border to draw.  The property's value is a
    /// <see cref="Thickness" /> containing values for each of the Left, Top, Right,
    /// and Bottom sides.  Values of Auto are interpreted as zero.
    /// </summary>
    public double BorderThickness
    {
        get => (double)GetValue(BorderThicknessProperty);
        set => SetValue(BorderThicknessProperty, value);
    }

    /// <summary>
    /// The Padding property inflates the effective size of the child by the specified thickness.  This
    /// achieves the same effect as adding margin on the child, but is present here for convenience.
    /// </summary>
    public Thickness Padding
    {
        get => (Thickness)GetValue(PaddingProperty);
        set => SetValue(PaddingProperty, value);
    }

    /// <summary>
    /// The BorderBrush property defines the brush used to fill the border region.
    /// </summary>
    public Brush? BorderBrush
    {
        get => (Brush)GetValue(BorderBrushProperty);
        set => SetValue(BorderBrushProperty, value);
    }

    /// <summary>
    /// The Background property defines the brush used to fill the area within the border.
    /// </summary>
    public Brush? Background
    {
        get => (Brush?)GetValue(BackgroundProperty);
        set => SetValue(BackgroundProperty, value);
    }

    #endregion

    #region Protected Methods

    protected override Size MeasureOverride(Size constraint)
    {
        var child = Child;

        var borderThicknessSize = HelperCollapseThickness(new Thickness(BorderThickness));
        var paddingSize = HelperCollapseThickness(this.Padding);

        var mySize = new Size(borderThicknessSize.Width + paddingSize.Width,
            borderThicknessSize.Height + paddingSize.Height);

        if (child != null)
        {
            var childConstraint = new Size(Math.Max(0.0, constraint.Width - mySize.Width),
                Math.Max(0.0, constraint.Height - mySize.Height));

            child.Measure(childConstraint);
            var childSize = child.DesiredSize;

            mySize.Width = childSize.Width + mySize.Width;
            mySize.Height = childSize.Height + mySize.Height;
        }

        return mySize;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        Rect boundRect = new(finalSize);
        var innerRect = HelperDeflateRect(boundRect, new Thickness(BorderThickness));

        UIElement child = Child;
        if (child != null)
        {
            var childRect = HelperDeflateRect(innerRect, Padding);
            child.Arrange(childRect);
        }

        if (!DoubleUtil.IsZero(innerRect.Width) && !DoubleUtil.IsZero(innerRect.Height))
        {
            PathGeometry borderGeometry =
                SquirclePathGenerator.GetGeometry(boundRect.Width - BorderThickness,
                    boundRect.Height - BorderThickness, Curvature);

            borderGeometry.Transform = new TranslateTransform(BorderThickness / 2, BorderThickness / 2);

            borderGeometry.Freeze();
            _borderGeometryCache = borderGeometry;
        }

        return finalSize;
    }

    protected override void OnRender(DrawingContext dc)
    {
        if (_borderGeometryCache != null
            && Background != null)
        {
            dc.DrawGeometry(Background, null, _borderGeometryCache);
        }

        if (_borderGeometryCache != null && BorderBrush != null)
        {
            var pen = _penCache;
            if (pen == null)
            {
                pen = new Pen
                {
                    Brush = BorderBrush,
                    Thickness = BorderThickness,
                    LineJoin = PenLineJoin.Round
                };

                if (BorderBrush.IsFrozen)
                {
                    pen.Freeze();
                }

                _penCache = pen;
            }

            dc.DrawGeometry(null, _penCache, _borderGeometryCache);
        }
    }

    #endregion Protected Methods

    #region Private Methods

    // Helper function to add up the left and right size as width, as well as the top and bottom size as height
    private static Size HelperCollapseThickness(Thickness th)
        => new(th.Left + th.Right, th.Top + th.Bottom);

    /// Helper to deflate rectangle by thickness
    private static Rect HelperDeflateRect(Rect rt, Thickness thick)
    {
        return new(rt.Left + thick.Left,
            rt.Top + thick.Top,
            Math.Max(0.0, rt.Width - thick.Left - thick.Right),
            Math.Max(0.0, rt.Height - thick.Top - thick.Bottom));
    }

    #endregion Private Methods

    public static string GetGeometry(double width = 100, double height = 100, double curvature = 1)
    {
        StringBuilder path = new();

        var halfHeight = height / 2;
        var halfWidth = width / 2;

        var arc = Math.Min(halfWidth, halfHeight) * (1 - curvature);

        path.Append(GetStartPoint(0, halfHeight))
            .Append(GetBezierSegment(0, arc, arc, 0, halfWidth, 0))
            .Append(GetShortBezierSegment(width, arc, width, halfHeight))
            .Append(GetShortBezierSegment(width - arc, height, halfWidth, height))
            .Append(GetShortBezierSegment(0, height - arc, 0, halfHeight))
            .Append(" Z");

        return path.ToString();
    }

    private static string GetStartPoint(double x, double y)
        => string.Format(CultureInfo.InvariantCulture, "M {0}, {1}", x, y);

    private static string GetShortBezierSegment(double x1, double y1, double x, double y)
        => string.Format(CultureInfo.InvariantCulture, "S {0}, {1} {2}, {3}", x1, y1, x, y);

    private static string GetBezierSegment(double x1, double y1, double x2, double y2, double x, double y)
        => string.Format(CultureInfo.InvariantCulture, "C {0}, {1} {2}, {3} {4}, {5}", x1, y1, x2, y2, x, y);
}

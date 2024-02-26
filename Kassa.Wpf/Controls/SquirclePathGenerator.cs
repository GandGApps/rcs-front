using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Kassa.Wpf.Controls;
internal static class SquirclePathGenerator
{
    public static PathGeometry GetGeometry(double width = 100, double height = 100, double curvature = 1) =>
        PathGeometry.CreateFromGeometry(Geometry.Parse(Squircle.GetGeometry(width, height, curvature)));
}

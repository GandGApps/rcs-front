using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kassa.Wpf;

public static class Helper
{
    public static string ReplaceAt(this string str, int index, int length, string replace)
    {
        return str.Remove(index, Math.Min(length, str.Length - index))
                .Insert(index, replace);
    }

    /// <summary>
    /// return string with stars, every star 
    /// separate by space, but not in the end
    /// </summary>
    public static string GetStars(int count)
    {
        var stars = new StringBuilder();
        for (var i = 0; i < count; i++)
        {
            stars.Append("* ");
        }
        return stars.ToString().TrimEnd();
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;

namespace Kassa.Shared;
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

    public static IGuidId AsIGuidId<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(this T guidIdContainable, Func<Guid>? getId = null)
    {
        if (guidIdContainable == null)
        {
            ThrowHelper.ThrowArgumentNullException(nameof(guidIdContainable));
        }

        if (getId is not null)
        {
            return new AnonymousGuidId(getId);
        }

        if (guidIdContainable is IGuidId guidId)
        {
            return guidId;
        }

        // check with reflection if object has Id property
        // which return Guid

        var idProperty = guidIdContainable.GetType().GetProperty("Id", BindingFlags.Public);

        if (idProperty is not null && idProperty.PropertyType == typeof(Guid))
        {
            return new AnonymousGuidId(() => (Guid)idProperty.GetValue(guidIdContainable)!);
        }

        return ThrowHelper.ThrowInvalidOperationException<IGuidId>("Can't create IGuidId from object without Guid Id property");
    }
}

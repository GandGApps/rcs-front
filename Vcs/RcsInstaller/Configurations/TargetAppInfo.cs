using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RcsInstaller.Configurations;
public sealed class TargetAppInfo
{
    public TargetAppInfo(string name, string rcsBinName, string displayName, string publisher, string removePath, bool isRepairable, bool isModifiable)
    {
        Name = name;
        RcsBinName = rcsBinName;
        DisplayName = displayName;
        Publisher = publisher;
        RemovePath = removePath;
        IsRepairable = isRepairable;
        IsModifiable = isModifiable;
    }

    // Required for Options pattern
    [EditorBrowsable(EditorBrowsableState.Never)]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public TargetAppInfo()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
    }

    public string Name
    {
        get; set;
    }
    public string RcsBinName
    {
        get; set;
    }
    public string DisplayName
    {
        get; set;
    }
    public string Publisher
    {
        get; set;
    }
    public string RemovePath
    {
        get; set;
    }
    public bool IsRepairable
    {
        get; set;
    }
    public bool IsModifiable
    {
        get; set;
    }
}

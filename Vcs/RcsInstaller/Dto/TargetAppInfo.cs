using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RcsInstaller.Dto;
public sealed class TargetAppInfo(string name, string rcsBinName, string displayName, string publisher, string removePath, bool isRepairable, bool isModifiable)
{
    public string Name => name;
    public string RcsBinName => rcsBinName;
    public string DisplayName => displayName;
    public string Publisher => publisher;
    public string RemovePath => removePath;
    public bool IsRepairable => isRepairable;
    public bool IsModifiable => isModifiable;
}

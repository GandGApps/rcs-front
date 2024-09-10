using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Automation.Peers;
using Avalonia.Controls;

namespace Kassa.Avalonia.Controls;
public sealed class TextBoxWithoutVirtualKeyboard: TextBox
{
    protected override AutomationPeer OnCreateAutomationPeer()
    {
        return new ControlAutomationPeer(this);
    }
}

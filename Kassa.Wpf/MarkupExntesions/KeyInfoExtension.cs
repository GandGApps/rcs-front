using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Markup;
using Kassa.Wpf.Controls;

namespace Kassa.Wpf.MarkupExntesions;

public class KeyInfoExtension : MarkupExtension
{

    public KeyInfoExtension()
    {

    }

    public KeyInfoExtension(char character)
    {
        Character = character;
    }

    public KeyInfoExtension(string icon, ICommand command)
    {
        Icon = icon;
        Command = command;
    }

    public KeyInfoType Type
    {
        get; set;
    }

    [ConstructorArgument("character")]
    public char Character
    {
        get; set;
    } = ' ';

    [ConstructorArgument("icon")]
    public string Icon
    {
        get; set;
    } = string.Empty;

    [ConstructorArgument("command")]
    public ICommand? Command
    {
        get; set;
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        if (Type == KeyInfoType.Char)
        {
            return KeyInfo.Char(Character);
        }

        if (Type == KeyInfoType.Icon)
        {
            return KeyInfo.IconButton(Icon, Command);
        }

        return null!;
    }
}

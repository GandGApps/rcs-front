using System.Windows.Input;
using Kassa.RxUI;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kassa.Wpf.Controls;

public class KeyInfo : ReactiveObject
{
    [Reactive]
    public ICommand? Command
    {
        get; set;
    }

    [Reactive]
    public bool IsRegister
    {
        get; set;
    }

    [Reactive]
    public bool IsClear
    {
        get; set;
    }

    [Reactive]
    public bool IsBackspace
    {
        get; set;
    }

    [Reactive]
    public bool IsEnter
    {
        get; set;
    }

    [Reactive]
    public char? Character
    {
        get; set;
    }

    [Reactive]
    public string? Text
    {
        get; set;
    }



    [Reactive]
    public KeySize Size
    {
        get; set;
    } = new(1);

    public static KeyInfo Char(char c, string? text = null, double width = 1) => new()
    {
        Character = c,
        Size = new(width),
        Text = text,
    };
}
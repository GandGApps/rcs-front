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
    public object? Parameter
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

    [Reactive]
    public bool IsIcon
    {
        get; set;
    } = true;

    [Reactive]
    public string? Icon
    {
        get; set;
    }

    public static KeyInfo Char(char c, string? text = null, double width = 1) => new()
    {
        Character = c,
        Size = new(width),
        Text = text,
    };

    public static KeyInfo IconButton(string icon, ICommand? command = null, double width = 1) => new()
    {
        Icon = icon,
        Command = command,
        Size = new(width),
        IsIcon = true,
    };

    public static KeyInfo TextButton(string text, object parameter) => new()
    {
        Text = text,
        Parameter = parameter,
    };
}
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kassa.Wpf.Controls;
public class KeyboardInfo: ReactiveObject
{
    public ObservableCollection<ObservableCollection<KeyInfo>> Lines
    {
        get;
    } = [];

    [Reactive]
    public double LineWidth
    {
        get; set;
    } = 1248;

    [Reactive]
    public double KeyHeight
    {
        get; set;
    } = 60;


    public static KeyboardInfo RuKeyboard(KeyInfo changeLanguage)
    {
        var keyboard = new KeyboardInfo();

        ObservableCollection<KeyInfo> line1 = [
            KeyInfo.Char('1'),
            KeyInfo.Char('2'),
            KeyInfo.Char('3'),
            KeyInfo.Char('4'),
            KeyInfo.Char('5'),
            KeyInfo.Char('6'),
            KeyInfo.Char('7'),
            KeyInfo.Char('8'),
            KeyInfo.Char('9'),
            KeyInfo.Char('0'),
            KeyInfo.Char('-'),
            KeyInfo.Char('='),
            new() {  IsBackspace = true },
        ];

        ObservableCollection<KeyInfo> line2 = [
            KeyInfo.Char('й'),
            KeyInfo.Char('ц'),
            KeyInfo.Char('у'),
            KeyInfo.Char('к'),
            KeyInfo.Char('е'),
            KeyInfo.Char('н'),
            KeyInfo.Char('г'),
            KeyInfo.Char('ш'),
            KeyInfo.Char('щ'),
            KeyInfo.Char('з'),
            KeyInfo.Char('х'),
            KeyInfo.Char('ъ'),
        ];

        ObservableCollection<KeyInfo> line3 = [
            KeyInfo.Char('ё'),
            KeyInfo.Char('ф'),
            KeyInfo.Char('ы'),
            KeyInfo.Char('в'),
            KeyInfo.Char('а'),
            KeyInfo.Char('п'),
            KeyInfo.Char('р'),
            KeyInfo.Char('о'),
            KeyInfo.Char('л'),
            KeyInfo.Char('д'),
            KeyInfo.Char('ж'),
            KeyInfo.Char('э'),
            KeyInfo.Char('\\'), 
        ];

        ObservableCollection<KeyInfo> line4 = [
            new() { IsRegister = true, Size = new(1.5d), Text = "РЕГИСТР" },
            KeyInfo.Char('я'),
            KeyInfo.Char('ч'),
            KeyInfo.Char('с'),
            KeyInfo.Char('м'),
            KeyInfo.Char('и'),
            KeyInfo.Char('т'),
            KeyInfo.Char('ь'),
            KeyInfo.Char('б'),
            KeyInfo.Char('ю'),
            KeyInfo.Char('.'),
            new() { IsEnter = true, Size = new(1.5d), Text= "ENTER" },
        ];

        ObservableCollection<KeyInfo> line5 = [
            changeLanguage,
            KeyInfo.Char(' ', text: "ПРОБЕЛ", width:9),
            new() { Text = "Alt Gr"},
            new() { Text = "СТЕРЕТЬ ВСЕ", IsClear = true, Size = new(2) },
        ];  


        keyboard.Lines.Add(line1);
        keyboard.Lines.Add(line2);
        keyboard.Lines.Add(line3);
        keyboard.Lines.Add(line4);
        keyboard.Lines.Add(line5);


        return keyboard;
    }

    public static KeyboardInfo EnKeyboard(KeyInfo changeLanguage)
    {
        var keyboard = new KeyboardInfo();

        ObservableCollection<KeyInfo> line1 = [
            KeyInfo.Char('1'),
            KeyInfo.Char('2'),
            KeyInfo.Char('3'),
            KeyInfo.Char('4'),
            KeyInfo.Char('5'),
            KeyInfo.Char('6'),
            KeyInfo.Char('7'),
            KeyInfo.Char('8'),
            KeyInfo.Char('9'),
            KeyInfo.Char('0'),
            KeyInfo.Char('-'),
            KeyInfo.Char('='),
            new() { IsBackspace = true },
        ];

        ObservableCollection<KeyInfo> line2 = [
            KeyInfo.Char('q'),
            KeyInfo.Char('w'),
            KeyInfo.Char('e'),
            KeyInfo.Char('r'),
            KeyInfo.Char('t'),
            KeyInfo.Char('y'),
            KeyInfo.Char('u'),
            KeyInfo.Char('i'),
            KeyInfo.Char('o'),
            KeyInfo.Char('p'),
            KeyInfo.Char('['),
            KeyInfo.Char(']'),
        ];

        ObservableCollection<KeyInfo> line3 = [
            KeyInfo.Char('`'),
            KeyInfo.Char('a'),
            KeyInfo.Char('s'),
            KeyInfo.Char('d'),
            KeyInfo.Char('f'),
            KeyInfo.Char('g'),
            KeyInfo.Char('h'),
            KeyInfo.Char('j'),
            KeyInfo.Char('k'),
            KeyInfo.Char('l'),
            KeyInfo.Char(';'),
            KeyInfo.Char('\''),
            KeyInfo.Char('\\'),
        ];

        ObservableCollection<KeyInfo> line4 = [
            new() { IsRegister = true, Size = new(1.5d), Text = "Reg" },
            KeyInfo.Char('z'),
            KeyInfo.Char('x'),
            KeyInfo.Char('c'),
            KeyInfo.Char('v'),
            KeyInfo.Char('b'),
            KeyInfo.Char('n'),
            KeyInfo.Char('m'),
            KeyInfo.Char(','),
            KeyInfo.Char('.'),
            KeyInfo.Char('/'),
            new() { IsEnter = true, Size = new(1.5d), Text= "ENTER" },
        ];

        ObservableCollection<KeyInfo> line5 = [
            changeLanguage,
            KeyInfo.Char(' ', text: "SPACE", width:9),
            new() { Text = "Alt Gr"},
            new() { Text = "CLEAR ALL", IsClear = true, Size = new(2) },
        ];


        keyboard.Lines.Add(line1);
        keyboard.Lines.Add(line2);
        keyboard.Lines.Add(line3);
        keyboard.Lines.Add(line4);
        keyboard.Lines.Add(line5);

        return keyboard;
    }

    public static KeyboardInfo Numpad()
    {
        var keyboard = new KeyboardInfo();

        ObservableCollection<KeyInfo> line1 = [
            KeyInfo.Char('1'),
            KeyInfo.Char('2'),
            KeyInfo.Char('3'),
        ];

        ObservableCollection<KeyInfo> line2 = [
            KeyInfo.Char('4'),
            KeyInfo.Char('5'),
            KeyInfo.Char('6'),
        ];

        ObservableCollection<KeyInfo> line3 = [
            KeyInfo.Char('7'),
            KeyInfo.Char('8'),
            KeyInfo.Char('9'),
        ];

        ObservableCollection<KeyInfo> line4 = [
            KeyInfo.IconButton("DeliveryTruck"),
            KeyInfo.Char('0'),
            new() { IsBackspace = true },
        ];

        keyboard.Lines.Add(line1);
        keyboard.Lines.Add(line2);
        keyboard.Lines.Add(line3);
        keyboard.Lines.Add(line4);

        return keyboard;
    }
}

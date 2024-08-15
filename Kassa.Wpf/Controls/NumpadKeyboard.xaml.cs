using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Splat;

namespace Kassa.Wpf.Controls;

/// <summary>
/// Interaction logic for NumpadKeyboard.xaml
/// </summary>
public sealed partial class NumpadKeyboard : UserControl
{

    public const int DigitAfterComma = 2;

    public static readonly DependencyProperty AmountProperty = DependencyProperty.Register(
        nameof(Amount),
        typeof(double),
        typeof(NumpadKeyboard),
        new PropertyMetadata(default(double)));

    public double Amount
    {
        get => (double)GetValue(AmountProperty);
        set => SetValue(AmountProperty, value);
    }

    private readonly AmountBuilder _amountBuilder = new();

    public NumpadKeyboard()
    {
        InitializeComponent();
    }

    /// <summary>
    /// If needed, you can add a caret to the amount. 
    /// And create more optimized methods for adding and removing digits.
    /// </summary>
    private sealed class AmountBuilder: IEnableLogger
    {
        private readonly StringBuilder _amountBuilder = new();
        private bool _hasComma = false;
        private int _caretIndex = 0; // For the future implementation of the caret
        private int _commaIndex = -1;

        public double Amount
        {
            get
            {
                var buildedAmount = _amountBuilder.ToString();
                if (double.TryParse(buildedAmount, App.RuCulture, out var result))
                {
                    return result;
                }

                this.Log().Error($"Failed to parse amount: {buildedAmount}");

                return 0;
            }
        }

        public void AddDigit(char c)
        {
            if (c < '0' || c > '9')
            {
                return;
            }

            if (_caretIndex == 0 && c == '0')
            {
                return;
            }

            if(_caretIndex - _commaIndex > DigitAfterComma)
            {
                return;
            }

            _amountBuilder.Insert(_caretIndex, c);
            _caretIndex++;
        }

        public void AddComma()
        {
            if (_hasComma)
            {
                return;
            }

            _hasComma = true;
            _commaIndex = _amountBuilder.Length;
            _amountBuilder.Append(',');
        }

        public void RemoveDigit()
        {
            if (_caretIndex == 0)
            {
                return;
            }

            if(_amountBuilder[_caretIndex - 1] == ',')
            {
                _hasComma = false;
                _commaIndex = -1;
            }

            _caretIndex--;
            _amountBuilder.Remove(_caretIndex, 1);
        }

        public void AddAmount(double amount)
        {
            var currentAmount = Amount;

            var newAmount = currentAmount + amount;

            // If the new amount is less than 0, set the amount to 0
            newAmount = newAmount < 0 ? 0 : newAmount;

            // Create a format string like "0.#" with the specified number of digits after the decimal point
            var formatString = $"0.{new string('#', DigitAfterComma)}";

            // Convert the amount to a string using Russian culture and the specified format
            var newAmountString = newAmount.ToString(formatString, App.RuCulture);

            // Clear the builder and append the formatted amount
            _amountBuilder.Clear();
            _amountBuilder.Append(newAmountString);
        }

        public void Clear()
        {
            _amountBuilder.Clear();
            _amountBuilder.Append('0');
            _hasComma = false;
            _caretIndex = 0;
            _commaIndex = -1;
        }
    }

    private void AddDigitCommand(object sender, RoutedEventArgs e)
    {
        // Extract the digit from the button
        var digit = ((Button)sender).CommandParameter.ToString()![0];

        _amountBuilder.AddDigit(digit);

        Amount = _amountBuilder.Amount;
    }


    private void AddCommaCommand(object sender, RoutedEventArgs e)
    {
        _amountBuilder.AddComma();

        Amount = _amountBuilder.Amount;
    }

    /// <summary>
    /// Not used in the current implementation
    /// </summary>
    private void RemoveDigitCommand(object sender, RoutedEventArgs e)
    {
        _amountBuilder.RemoveDigit();

        Amount = _amountBuilder.Amount;
    }

    private void PlusCommand(object sender, RoutedEventArgs e)
    {
        // Extract the amount from the button
        var amount = (double)((Button)sender).CommandParameter;

        _amountBuilder.AddAmount(amount);

        Amount = _amountBuilder.Amount;
    }

    private void ClearCommand(object sender, RoutedEventArgs e)
    {
        _amountBuilder.Clear();

        Amount = _amountBuilder.Amount;
    }
}

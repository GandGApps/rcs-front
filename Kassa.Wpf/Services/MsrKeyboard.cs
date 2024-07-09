using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.Services;

namespace Kassa.Wpf.Services;
internal sealed class MsrKeyboard : IMagneticStripeReader
{

    public static MsrKeyboard Instance
    {
        get;
    } = new();

    /// <summary>
    /// This is a singleton class.
    /// </summary>
    private MsrKeyboard()
    {

    }

    private readonly Subject<IMagneticStripe> _cardData = new();


    public void OnCardData(string data)
    {
        _cardData.OnNext(new MagneticStripe(data));
    }

    public IObservable<IMagneticStripe> CardData => _cardData;

    internal sealed class MagneticStripe(string data) : IMagneticStripe
    {
        public bool IsDisposed
        {
            get;
        }

        public void Dispose()
        {
        }

        public ValueTask<string> ReadPincode() => new(data);
    }
}

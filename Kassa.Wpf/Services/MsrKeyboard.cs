using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Kassa.BuisnessLogic.Services;
using Splat;

namespace Kassa.Wpf.Services;
internal sealed class MsrKeyboard : IMagneticStripeReader, IEnableLogger
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


    public void OnMsrCardData(string data)
    {
        _cardData.OnNext(new MagneticStripe(data));

        this.Log().Debug("OnMsrCardData called with data: {data}", data);
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Kassa.BuisnessLogic.Services;
using Splat;
using Windows.Devices.PointOfService;

namespace Kassa.Wpf.Services;

/// <summary>
/// Implementation of <see cref="IMagneticStripeReader"/> for Wnd magnetic stripe reader.
/// </summary>
internal sealed class WndPosMagneticStripeReader : IMagneticStripeReader, IEnableLogger
{
    private readonly Subject<IMagneticStripe> _cardData = new();
    private ClaimedMagneticStripeReader? _magneticStripeReader;

    public async Task<bool> TryClaim()
    {
        var magneticStriper = await MagneticStripeReader.GetDefaultAsync();

        if (magneticStriper is null)
        {
            this.Log().Error("Magnetic stripe reader not found");

            magneticStriper = await DeviceHelper.GetFirstMagneticStripeReaderAsync();

            if (magneticStriper is null)
            {
                this.Log().Error("Magnetic stripe reader not found");

                return false;
            }
        }

        _magneticStripeReader = await magneticStriper.ClaimReaderAsync();

        this.Log().Info("Magnetic stripe reader claimed");


        _magneticStripeReader.ReleaseDeviceRequested += (sender, args) =>
        {
            args.RetainDevice();
        };

        _magneticStripeReader.VendorSpecificDataReceived += (sender, args) =>
        {
            this.Log().Info("Vendor specific data received");

            var propertiesStringBuilder = new StringBuilder();

            propertiesStringBuilder.AppendLine("Properties:");
            foreach (var property in args.Report.Properties)
            {
                propertiesStringBuilder.AppendLine($"{property.Key}: {property.Value}");
            }
            propertiesStringBuilder.AppendLine();

            this.Log().Info(propertiesStringBuilder.ToString());

            var dataStringBuilder = new StringBuilder();
        };

        _magneticStripeReader.IsDecodeDataEnabled = true;

        return true;
    }

    public IObservable<IMagneticStripe> CardData => _cardData;


    internal sealed class WndPosMagneticStripe : IMagneticStripe
    {

        public ValueTask<string> ReadPincode()
        {
            return new ValueTask<string>("1234");
        }
    }
}

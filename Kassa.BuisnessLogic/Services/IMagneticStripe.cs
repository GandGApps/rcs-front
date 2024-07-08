using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;

namespace Kassa.BuisnessLogic.Services;

/// <summary>
/// Implement this interface in the Presentation layer.
/// </summary>
/// <remarks>
/// Disposing this object also dispoase the <see cref="IMagneticStripeReader"/> object.
/// </remarks>
public interface IMagneticStripe: ICancelable
{
    public ValueTask<string> ReadPincode();
}

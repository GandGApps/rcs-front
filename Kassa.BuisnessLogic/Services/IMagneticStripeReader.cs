using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kassa.BuisnessLogic.Services;

/// <summary>
/// Implement this interface in the Presentation layer 
/// </summary>
public interface IMagneticStripeReader
{

    public IObservable<IMagneticStripe> CardData
    {
        get;
    }

}

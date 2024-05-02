using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Refit;

namespace Kassa.BuisnessLogic.Edgar;
internal sealed class BasicAddressHandler: DelegatingHandler
{

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request.Options.TryGetValue<Type>(new(HttpRequestMessageOptions.InterfaceType), out var type))
        {

        }

        return base.SendAsync(request, cancellationToken);
    }
}

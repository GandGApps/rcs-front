using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Kassa.Shared.ServiceLocator;
using Microsoft.Extensions.Configuration;
using Refit;
using Splat;

namespace Kassa.Shared.DelegatingHandlers;
public sealed class SelectJwtDelegatingHandler: DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var config = RcsLocator.GetRequiredService<IConfiguration>();

        if (request.Options.TryGetValue<Type>(new(HttpRequestMessageOptions.InterfaceType), out var type))
        {

            if (type.IsAssignableTo(typeof(IUseTerminalToken)))
            {

                if (!string.IsNullOrWhiteSpace(config["TerminalAuthToken"]))
                {

                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", config["TerminalAuthToken"]);
                }

            }
            else if (type.IsAssignableTo(typeof(IUseMemberToken)))
            {

                if (!string.IsNullOrWhiteSpace(config["MemberAuthToken"]))
                {

                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", config["MemberAuthToken"]);
                }
            }
        }

        return base.SendAsync(request, cancellationToken);
    }
}

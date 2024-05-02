using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Kassa.DataAccess.HttpRepository.Api;
using Kassa.Shared;
using Microsoft.Extensions.Configuration;
using Refit;
using Splat;

namespace Kassa.DataAccess.HttpRepository;
internal sealed class SelectJwtDelegatingHandler : DelegatingHandler
{

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var config = Locator.Current.GetRequiredService<IConfiguration>();

        if (request.Options.TryGetValue<Type>(new(HttpRequestMessageOptions.InterfaceType), out var type))
        {
            /*if (type.IsAssignableTo(typeof(ITerminalApi)))
            {
                if (!string.IsNullOrWhiteSpace(config["TerminalAuthToken"]))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", config["TerminalAuthToken"]);
                }

            }*/
            if (type.IsAssignableTo(typeof(IApiOfMemberToken)))
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
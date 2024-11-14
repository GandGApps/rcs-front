using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Refit;
using Splat;

namespace RcsInstaller.DelegatingHandlers;
public sealed class SelectJwtDelegatingHandler : DelegatingHandler
{
    private readonly IConfiguration _configuration;

    public SelectJwtDelegatingHandler(IConfiguration configuration)
    {
        _configuration = configuration;
    }


    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        /*
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
        */

        return base.SendAsync(request, cancellationToken);
    }
}

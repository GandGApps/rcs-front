using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Splat;

namespace RcsInstaller.DelegatingHandlers;

public sealed class HttpDebugLoggingHandler : DelegatingHandler, IEnableLogger
{
    private static readonly string[] types = ["html", "text", "xml", "json", "txt", "x-www-form-urlencoded"];

    protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var req = request;
        var id = Guid.NewGuid().ToString();
        var msg = $"[{id} - Request]";
        var logBuilder = new StringBuilder();

        logBuilder.AppendLine($"Request start {msg}:");
        logBuilder.AppendLine($"{req.Method} {req.RequestUri!.PathAndQuery} {req.RequestUri.Scheme}/{req.Version}");
        logBuilder.AppendLine($"Host: {req.RequestUri.Scheme}://{req.RequestUri.Host}");

        foreach (var header in req.Headers)
        {
            logBuilder.AppendLine($"{header.Key}: {string.Join(", ", header.Value)}");
        }

        if (req.Content != null)
        {
            foreach (var header in req.Content.Headers)
            {
                logBuilder.AppendLine($"{header.Key}: {string.Join(", ", header.Value)}");
            }

            if (req.Content is StringContent || IsTextBasedContentType(req.Headers) ||
                IsTextBasedContentType(req.Content.Headers))
            {
                var result = await req.Content.ReadAsStringAsync(cancellationToken);
                logBuilder.AppendLine($"Content:");
                logBuilder.AppendLine($"{result}");
            }
        }

        var start = DateTime.Now;

        var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

        var end = DateTime.Now;

        logBuilder.AppendLine($"Duration: {end - start}");

        this.Log().Debug(logBuilder.ToString());

        msg = $"[{id} - Response]";
        logBuilder.Clear();

        logBuilder.AppendLine($"Request end {msg}:");

        var resp = response;

        logBuilder.AppendLine($"{req.RequestUri.Scheme.ToUpper()}/{resp.Version} {(int)resp.StatusCode} {resp.ReasonPhrase}");

        foreach (var header in resp.Headers)
        {
            logBuilder.AppendLine($"{header.Key}: {string.Join(", ", header.Value)}");
        }

        if (resp.Content != null)
        {
            foreach (var header in resp.Content.Headers)
            {
                logBuilder.AppendLine($"{header.Key}: {string.Join(", ", header.Value)}");
            }

            if (resp.Content is StringContent || IsTextBasedContentType(resp.Headers) ||
                IsTextBasedContentType(resp.Content.Headers))
            {
                start = DateTime.Now;
                var result = await resp.Content.ReadAsStringAsync(cancellationToken);
                end = DateTime.Now;

                logBuilder.AppendLine($"Content:");
                logBuilder.AppendLine($"{result}");
                logBuilder.AppendLine($"Duration: {end - start}");
            }
        }

        this.Log().Debug(logBuilder.ToString());

        return response;

    }


    private static bool IsTextBasedContentType(HttpHeaders headers)
    {
        IEnumerable<string> values;

        if (!headers.TryGetValues("Content-Type", out values!))
        {
            return false;
        }

        var header = string.Join(" ", values).ToLowerInvariant();

        return types.Any(t => header.Contains(t));
    }
}

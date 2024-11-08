using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Splat;

namespace RcsInstaller.DelegatingHandlers;

public sealed class HttpDebugLoggingHandler : DelegatingHandler, IEnableLogger
{
    private static readonly string[] types = ["html", "text", "xml", "json", "txt", "x-www-form-urlencoded"];
    
    private readonly ILogger<HttpDebugLoggingHandler> _logger;

    public HttpDebugLoggingHandler(ILogger<HttpDebugLoggingHandler> logger)
    {
        _logger = logger;
    }

    protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var req = request;
        var id = Guid.NewGuid().ToString();

        // Логируем начало запроса с параметрами
        _logger.LogDebug("Request start [{Id} - Request]: {Method} {PathAndQuery} {Scheme}/{Version} Host: {Host}",
                         id,
                         req.Method,
                         req.RequestUri!.PathAndQuery,
                         req.RequestUri.Scheme,
                         req.Version,
                         $"{req.RequestUri.Scheme}://{req.RequestUri.Host}");

        foreach (var header in req.Headers)
        {
            _logger.LogDebug("Request Header [{Id}]: {HeaderKey}: {HeaderValue}", id, header.Key, string.Join(", ", header.Value));
        }

        if (req.Content != null)
        {
            foreach (var header in req.Content.Headers)
            {
                _logger.LogDebug("Request Content Header [{Id}]: {HeaderKey}: {HeaderValue}", id, header.Key, string.Join(", ", header.Value));
            }

            if (req.Content is StringContent || IsTextBasedContentType(req.Headers) || IsTextBasedContentType(req.Content.Headers))
            {
                var result = await req.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogDebug("Request Content [{Id}]: {Content}", id, result);
            }
        }

        var start = DateTime.Now;

        var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

        var end = DateTime.Now;

        // Логируем время выполнения запроса
        _logger.LogDebug("Request Duration [{Id}]: {Duration} ms", id, (end - start).TotalMilliseconds);

        // Логируем окончание запроса с параметрами
        _logger.LogDebug("Request end [{Id} - Response]: {Scheme}/{Version} {StatusCode} {ReasonPhrase}",
                         id,
                         req.RequestUri.Scheme.ToUpper(),
                         response.Version,
                         (int)response.StatusCode,
                         response.ReasonPhrase);

        foreach (var header in response.Headers)
        {
            _logger.LogDebug("Response Header [{Id}]: {HeaderKey}: {HeaderValue}", id, header.Key, string.Join(", ", header.Value));
        }

        if (response.Content != null)
        {
            foreach (var header in response.Content.Headers)
            {
                _logger.LogDebug("Response Content Header [{Id}]: {HeaderKey}: {HeaderValue}", id, header.Key, string.Join(", ", header.Value));
            }

            if (response.Content is StringContent || IsTextBasedContentType(response.Headers) || IsTextBasedContentType(response.Content.Headers))
            {
                start = DateTime.Now;
                var result = await response.Content.ReadAsStringAsync(cancellationToken);
                end = DateTime.Now;

                _logger.LogDebug("Response Content [{Id}]: {Content}", id, result);
                _logger.LogDebug("Response Content Duration [{Id}]: {Duration} ms", id, (end - start).TotalMilliseconds);
            }
        }

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

        return types.Any(header.Contains);
    }
}

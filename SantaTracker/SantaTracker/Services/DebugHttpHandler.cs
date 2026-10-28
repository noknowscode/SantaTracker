using System.Diagnostics;

namespace SantaTracker.Services;

/// <summary>
/// Debug HTTP handler that logs requests in development
/// </summary>
public class DebugHttpHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        Debug.WriteLine($"[HTTP] {request.Method} {request.RequestUri}");

        var response = await base.SendAsync(request, cancellationToken);

        Debug.WriteLine($"[HTTP] {(int)response.StatusCode} {response.StatusCode}");

        return response;
    }
}

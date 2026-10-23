using RestSharp;

namespace Jefit.Core.Api;

public class ApiResponse
{
    public ApiResponse(RestResponse raw, TimeSpan elapsed)
    {
        Raw = raw;
        Elapsed = elapsed;
    }

    public RestResponse Raw { get; }

    public TimeSpan Elapsed { get; }

    public int StatusCode => (int)Raw.StatusCode;

    public bool IsSuccessful => Raw.IsSuccessful;

    public string Body => Raw.Content ?? string.Empty;

    public string ContentType => Raw.ContentType ?? string.Empty;

    public string? GetHeader(string name)
    {
        IEnumerable<HeaderParameter> headers = Raw.Headers ?? Enumerable.Empty<HeaderParameter>();
        IEnumerable<HeaderParameter> contentHeaders = Raw.ContentHeaders ?? Enumerable.Empty<HeaderParameter>();

        return headers.Concat(contentHeaders)
            .FirstOrDefault(h => string.Equals(h.Name, name, StringComparison.OrdinalIgnoreCase))
            ?.Value?.ToString();
    }
}

namespace KL.Engine.Rule.Extensions;

public static class HttpContextExtensions
{
    private const string ClientIdHeaderKey = "current_client_id";

    public static long? GetClientId(this HttpContext? context)
    {
        if (context is null
            || !context.Request.Headers.TryGetValue(ClientIdHeaderKey, out var clientId)
            || string.IsNullOrWhiteSpace(clientId))
        {
            return null;
        }

        return long.Parse(clientId);
    }
}

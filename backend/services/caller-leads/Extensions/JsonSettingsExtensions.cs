using System.Text.Json;
using System.Text.Json.Serialization;

namespace KL.Caller.Leads.Extensions;

public static class JsonSettingsExtensions
{
    public static JsonSerializerOptions Default => new(JsonSerializerDefaults.Web)
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters =
        {
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
        }
    };
}

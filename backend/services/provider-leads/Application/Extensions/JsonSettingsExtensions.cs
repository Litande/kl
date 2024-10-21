using System.Text.Json;
using System.Text.Json.Serialization;

namespace Plat4Me.DialLeadProvider.Application.Extensions;

public static class JsonSettingsExtensions
{
    public static JsonSerializerOptions Default => new(JsonSerializerDefaults.Web)
    {
        Converters =
        {
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
        }
    };
}
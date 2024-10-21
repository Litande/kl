using System.Text.Json;
using System.Text.Json.Serialization;

namespace Plat4Me.DialClientApi.Application.Common;

public class JsonSettingsExtensions
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

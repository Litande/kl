using System.Text.Json;
using System.Text.Json.Serialization;

namespace KL.Agent.API.Application.Common;

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

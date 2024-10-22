using KL.Storage.GoogleCouldStorage;
using KL.Storage.LocalStorage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KL.Storage.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddStore(this IServiceCollection services, IConfigurationSection config)
    {
        var driverConfig = config.GetSection("Driver");

        if (!Enum.TryParse<DriverEnum>(driverConfig.Value, true, out var driverType))
        {
            throw new Exception($"{driverConfig.Value} storage provider is incorrect.");
        }

        switch (driverType)
        {
            case DriverEnum.Google:
            {
                services.Configure<GoogleStorageOptions>(config.GetSection(nameof(DriverEnum.Google)));
                services.AddScoped<IStorageService, GoogleCloudStorageService>();
                break;
            }
            case DriverEnum.Local:
            {
                services.Configure<LocalStorageOptions>(config.GetSection(nameof(DriverEnum.Local)));
                services.AddScoped<IStorageService, LocalStorageService>();
                break;
            }
            default:
                throw new Exception($"{driverType} storage provider is not implemented.");
        }

        return services;
    }
}

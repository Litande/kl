using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KL.MySql;

public static class AppRegistrations
{
    public static IServiceCollection AddMysql(this IServiceCollection services, IConfiguration config)
    {
        var mysqlOptions = new MysqlOptions();
        config.GetSection("CLIENTS:MYSQL").Bind(mysqlOptions);
        
        var connectionString = mysqlOptions.GetUrl("ast");

        services.AddDbContext<AssetDbContext>(options => options.UseMySql(connectionString,
            ServerVersion.AutoDetect(connectionString),
            opt => opt.EnableRetryOnFailure(mysqlOptions.ConnectRetry)));
        
        return services;
    }

}
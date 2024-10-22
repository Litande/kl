using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KL.MySql;

public static class AppRegistrations
{
    // ReSharper disable once UnusedMethodReturnValue.Global
    public static IServiceCollection AddMysql<T>(this IServiceCollection services, IConfiguration config, string schema)
        where T : DbContext
    {
        var mysqlOptions = new MysqlOptions();
        config.GetSection("CLIENTS:MYSQL").Bind(mysqlOptions);
        
        var connectionString = mysqlOptions.GetUrl(schema);

        services.AddDbContext<T>(options => options.UseMySql(connectionString,
            ServerVersion.AutoDetect(connectionString),
            opt => opt.EnableRetryOnFailure(mysqlOptions.ConnectRetry)));
        
        return services;
    }

}
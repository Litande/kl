using System;
using System.Text;
using System.Threading.Tasks;
using KL.Auth.Options;
using KL.Auth.Persistence;
using KL.Auth.Services;
using KL.Auth.Services.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace KL.Auth.Configurations
{
    public static class IdentityRegistrations
    {
        private const string JwtSection = "Jwt";

        // Used for development purposes.
        public static IServiceCollection AddPlatAuthentication<TUser,TRole,TId>(
            this IServiceCollection services,
            IConfiguration configuration,
            string schema)
            where TUser : IdentityUser<TId>
            where TRole : IdentityRole<TId>
            where TId : IEquatable<TId>
        {
            return services.AddPlatAuthentication<TUser, TRole, TId, AuthenticationDbContext<TUser, TRole, TId>, JwtTokenGenerator<TUser, TId>>(configuration, schema);
        }

        public static IServiceCollection AddPlatAuthentication<TUser, TRole, TId, TJwtTokenGenerator>(
            this IServiceCollection services,
            IConfiguration configuration,
            string schema)
            where TUser : IdentityUser<TId>
            where TRole : IdentityRole<TId>
            where TId : IEquatable<TId>
            where TJwtTokenGenerator : JwtTokenGenerator<TUser, TId>
        {
            return services.AddPlatAuthentication<TUser, TRole, TId, AuthenticationDbContext<TUser, TRole, TId>, TJwtTokenGenerator>(configuration, schema);
        }

        public static IServiceCollection AddPlatAuthentication<TUser, TRole, TId, TAuthenticationDbContext, TJwtTokenGenerator>(
            this IServiceCollection services,
            IConfiguration configuration,
            string schema)
            where TUser : IdentityUser<TId>
            where TRole : IdentityRole<TId>
            where TId : IEquatable<TId>
            where TAuthenticationDbContext: DbContext
            where TJwtTokenGenerator : JwtTokenGenerator<TUser, TId>
        {
            services.AddDbContext<TAuthenticationDbContext>(options =>
            {
                var mysqlOptions = new MysqlOptions();
                configuration.Bind("CLIENTS:MYSQL", mysqlOptions);
                var connectionString = mysqlOptions.GetUrl(schema);
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString),
                    opt => opt.EnableRetryOnFailure(mysqlOptions.ConnectRetry));
            });

            services.AddIdentity<TUser, TRole>()
                    .AddEntityFrameworkStores<TAuthenticationDbContext>()
                    .AddDefaultTokenProviders();
            services.AddScoped<IAdminAuthenticationService, AdminAuthenticationService<TUser, TId>>();
            services.AddSingleton<IJwtTokenGenerator<TUser, TId>, TJwtTokenGenerator>();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequiredUniqueChars = 1;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 10;
                options.Lockout.AllowedForNewUsers = true;

                options.User.RequireUniqueEmail = true;
            });

            services.Configure<JwtConfigurations>(configuration.GetSection(JwtSection));
            var jwtConfigurations = new JwtConfigurations();
            configuration.Bind(JwtSection, jwtConfigurations);

            var key = Encoding.ASCII.GetBytes(jwtConfigurations.Key);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
                x.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        if (!string.IsNullOrEmpty(accessToken))
                        {
                            // Read the token out of the query string
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            return services;
        }
    }
}
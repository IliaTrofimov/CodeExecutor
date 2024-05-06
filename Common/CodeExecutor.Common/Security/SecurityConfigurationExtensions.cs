using System.Text;
using CodeExecutor.Common.Models.Configs;
using CodeExecutor.Common.Models.Exceptions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace CodeExecutor.Common.Security;

public static class SecurityConfigurationExtensions
{
    /// <summary>Add default JWT Bearer authorization.</summary>
    public static void AddJwtBearer(this IServiceCollection services, ConfigurationManager config)
    {
        var issuer = config.GetValue("Authorization:Issuer");
        var audience = config.GetValue("Authorization:Audience");
        var tokenKey = config.GetValue("Authorization:Key");
        var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(tokenKey));
        
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
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ClockSkew = new TimeSpan(0, 0, 30)
                };
            });
    }
    
    /// <summary>Configure CORS.</summary>
    public static void UseDefaultCors(this IApplicationBuilder app, ConfigurationManager config)
    {
        app.UseCors(opt =>
        {
            if (config.TryGetValue("CORS:Headers", out var headers))
                opt.WithHeaders(headers.Split(" "));
            else
                opt.AllowAnyHeader();

            if ( config.TryGetValue("CORS:Origins", out var origins))
                opt.WithOrigins(origins.Split(" "));
            else
                opt.AllowAnyOrigin();
            
            if (config.TryGetValue("CORS:Methods", out var methods))
                opt.WithMethods(methods.Split(" "));
            else
                opt.AllowAnyMethod();
            
            opt.Build();
        });
    }

    public static void AddAuthConfiguration(this IServiceCollection services, ConfigurationManager config)
    {
        var section = config.GetSection("Authorization");
        if (!section.Exists())
            throw new ConfigurationException("Missing Authorization section");
        services.AddSingleton(new AuthConfig(section));
    }
}
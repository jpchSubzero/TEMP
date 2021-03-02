using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using SmartFormat;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace UTPL.AuthenticationADFS
{
    public static class JwtConfigurationExtension
    {
        public static void AddAutenticationJwtConfiguration(this IServiceCollection services, IConfiguration configuration)
        {

            var myTenant = configuration["JwtConfig:myTenant"];
            var myAudience = configuration["JwtConfig:myAudience"];
            var myIssuer = Smart.Format(configuration["JwtConfig:myIssuer"], myTenant);
            var stsDiscoveryEndpoint = Smart.Format(configuration["JwtConfig:stsDiscoveryEndpoint"], myTenant);
            var myAuthority = Smart.Format(configuration["JwtConfig:Authority"], myTenant);

            var configManager = new ConfigurationManager<OpenIdConnectConfiguration>(stsDiscoveryEndpoint, new OpenIdConnectConfigurationRetriever());

            OpenIdConnectConfiguration config = new OpenIdConnectConfiguration();

            config = configManager.GetConfigurationAsync().Result as OpenIdConnectConfiguration;

            var validationParameters = new TokenValidationParameters
            {
                ValidAudience = myAudience,
                ValidIssuer = myIssuer,
                IssuerSigningKeys = config.SigningKeys,
                ValidateLifetime = false
            };

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.Authority = myAuthority;
                options.Audience = myAudience;
                options.TokenValidationParameters = validationParameters;

            });
        }

        public static void UseAutenticationJwtConfiguration(this IApplicationBuilder app, IConfiguration configuration)
        {


        }

    }
}

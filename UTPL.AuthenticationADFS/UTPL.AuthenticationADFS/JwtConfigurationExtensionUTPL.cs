using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AdfsExample
{
    
    public static class JwtConfigurationExtensionUTPL
    {
      
        public static void AddAutenticationJwtConfigurationUTPL(this IServiceCollection services, IConfiguration configuration)
        {

            var myAudience = configuration["JwtConfig:myAudience"];// "microsoft:identityserver:aadc7236-2851-4499-b24f-04983f2c8eae";
            var myIssuer = $"http://sts.utpl.edu.ec/adfs/services/trust";
            var stsDiscoveryEndpoint = $"https://sts.utpl.edu.ec/adfs/.well-known/openid-configuration";
            var configManager = new ConfigurationManager<OpenIdConnectConfiguration>(stsDiscoveryEndpoint, new OpenIdConnectConfigurationRetriever());
            var mySecret = configuration["JwtConfig:myTenant"];//"ddoBGxNw_J9T9wWzv8eHNRSuqXA_UXONxde_TJmU";
            var mySecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(mySecret));

            OpenIdConnectConfiguration config = new OpenIdConnectConfiguration();

            config = configManager.GetConfigurationAsync().Result as OpenIdConnectConfiguration;

            var validationParameters = new TokenValidationParameters
            {
                ValidAudience = myAudience,
                ValidIssuer = myIssuer,
                IssuerSigningKeys = config.SigningKeys,
                ValidateLifetime = false,
                IssuerSigningKey = mySecurityKey
            };


            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.Authority = $"https://sts.utpl.edu.ec/adfs";
                options.Audience = myAudience;
                options.TokenValidationParameters = validationParameters;

                options.Events = new JwtBearerEvents()
                {

                    OnAuthenticationFailed = context =>
                    {
                        var invalidAudienceException = context.Exception as SecurityTokenInvalidAudienceException;

                        if (invalidAudienceException?.InvalidAudience == "audience-v1-string")
                        {
                            // Option 1
                            context.Success();

                            // Option 2
                            context.Fail("IdSrv2Bearer couldn't validate this token, but IdSrv1Bearer probably can.");

                            // Option 3
                            context.NoResult();
                        }

                        return Task.CompletedTask;
                    }

                };
            });

        }

        public static void UseAutenticationJwtConfiguration(this IApplicationBuilder app, IConfiguration configuration)
        {
            

        }

    }
}

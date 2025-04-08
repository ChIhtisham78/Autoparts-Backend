using Autopart.Application.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AuthExtensions
    {
        public static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration configuration)
        {
            var authenticationSection = configuration.GetSection(AuthenticationOptions.SettingsSection);
            services.Configure<AuthenticationOptions>(authenticationSection);
            var authenticationOptions = authenticationSection.Get<AuthenticationOptions>();

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddAuthentication(opts =>
            {
                opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(opts =>
            {
                opts.SaveToken = true;
                opts.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(authenticationOptions.SecretKey),
                    ValidateIssuer = authenticationOptions.ValidateIssuer,
                    ValidateAudience = authenticationOptions.ValidateAudience,
                    ValidateLifetime = authenticationOptions.ValidateLifetime,
                    ValidIssuers = authenticationOptions.ValidIssuers,
                    ValidAudiences = authenticationOptions.ValidAudiences,
                    ClockSkew = TimeSpan.FromSeconds(authenticationOptions.ClockSkew)
                };

                opts.Events = new JwtBearerEvents()
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Add("Token-Expired", "true");
                        }
                        return Task.CompletedTask;
                    },
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && (path.StartsWithSegments("/notifications-hub") || path.StartsWithSegments("/ExportNotification")))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            //TODO = add autorizatiom
            //services.AddAuthorization(opts =>
            //{
            //    opts.AddPolicy(Policies.Starters, policy => policy.RequireClaim(CustomClaimTypes.Tier, Tiers.Starter, Tiers.Pro));
            //    opts.AddPolicy(Policies.Pros, policy => policy.RequireClaim(CustomClaimTypes.Tier, Tiers.Pro));

            //    opts.AddPolicy(Policies.Managers, policy => policy.RequireClaim(CustomClaimTypes.Role, Roles.Manager));
            //    opts.AddPolicy(Policies.Allocators, policy => policy.RequireClaim(CustomClaimTypes.Role, Roles.Allocator));
            //    opts.AddPolicy(Policies.ServiceProviders, policy => policy.RequireClaim(CustomClaimTypes.Role, Roles.ServiceProvider));
            //    opts.AddPolicy(Policies.Context, policy => policy.RequireClaim(CustomClaimTypes.Role, Roles.Context));
            //    opts.AddPolicy(Policies.Media, policy => policy.RequireClaim(CustomClaimTypes.Role, Roles.Media));

            //    opts.AddPolicy(Policies.IsAdmin, policy => policy.RequireClaim(CustomClaimTypes.IsAdministrator, "1"));
            //});

            return services;

        }
    }
}

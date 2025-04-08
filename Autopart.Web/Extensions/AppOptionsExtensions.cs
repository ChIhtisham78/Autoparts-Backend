

using Autopart.Application.Models;
using Autopart.Application.Options;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AppOptionsExtensions
    {
        public static IServiceCollection AddAppOptions(this IServiceCollection services, IConfiguration configuration)
        {
            var authSection = configuration.GetSection(AuthenticationOptions.SettingsSection);
            services.Configure<AuthenticationOptions>(authSection);
            var stripeScetion = configuration.GetSection(StripeSetting.StripeSettings);
            services.Configure<StripeSetting>(stripeScetion);
            var rootSection = configuration.GetSection(RootFolder.SettingsSection);
            services.Configure<RootFolder>(rootSection);
            var sendgridSection = configuration.GetSection(SendGridSection.SettingsSection);
            services.Configure<SendGridSection>(sendgridSection);

            return services;

        }
    }
}

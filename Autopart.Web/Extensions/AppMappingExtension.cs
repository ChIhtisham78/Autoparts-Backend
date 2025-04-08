using AutoMapper;
using Autopart.Application;

namespace Autopart.API.Extensions
{
    public static class AppMappingExtension
    {
        public static IServiceCollection MappingExtension(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(Program).Assembly);
            services.AddTransient(sp => new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            }).CreateMapper());

            services.AddTypeAdapter();

            return services;

        }
    }
}

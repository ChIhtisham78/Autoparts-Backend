using Autopart.Domain.SharedKernel;
using Autopart.Infrastructure.AutoMapper;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class TypeAdapterExtensions
    {
        public static IServiceCollection AddTypeAdapter(this IServiceCollection services)
        {
            services.AddTransient<ITypeAdapter, AutoMapperTypeAdapter>();
            return services;
        }
    }
}

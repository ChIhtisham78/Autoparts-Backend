using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Autopart.API.Infrastructure;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ApiExtensions
    {
        public static IServiceCollection AddCustomApi(this IServiceCollection services)
        {
            services.AddControllers()
                .AddFluentValidation(fv => fv.DisableDataAnnotationsValidation = true)
                .AddNewtonsoftJson();

            services.TryAddEnumerable(ServiceDescriptor.Transient<IConfigureOptions<MvcOptions>, ConfigureMvcOptions>());
            // services.AddFluentValidators();



            services.AddHttpContextAccessor();

            return services;
        }

        //will eventually add fluent validation
        //private static IServiceCollection AddFluentValidators(this IServiceCollection services)
        //{
        //    services.Scan(scan =>
        //        scan.FromAssembliesOf(typeof(Startup))
        //            .AddClasses(classes => classes.AssignableTo(typeof(IValidator<>)))
        //            .AsImplementedInterfaces()
        //            .WithScopedLifetime()
        //        );

        //    return services;
        //}
    }
}

using Microsoft.Extensions.DependencyInjection;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Autopart.Domain.SharedKernel;

namespace Autopart.Data.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services)
        {
            services.AddDbContextFactory<autopartContext>((sp, options) =>
            {
                var hostEnvironment = sp.GetRequiredService<IHostEnvironment>();
                var config = sp.GetRequiredService<IConfiguration>();

                options.UseNpgsql(config.GetConnectionString("autopart"));
                if (hostEnvironment.IsDevelopment())
                {
                    options.EnableSensitiveDataLogging();
                }
            });
            services.AddDbContext<autopartContext>((sp, options) =>
            {
                var hostEnvironment = sp.GetRequiredService<IHostEnvironment>();
                var config = sp.GetRequiredService<IConfiguration>();

                options.UseNpgsql(config.GetConnectionString("autopart"));
                if (hostEnvironment.IsDevelopment())
                {
                    options.EnableSensitiveDataLogging();
                }
            });
            services.AddScoped<IDbConnection>(provider =>
            {
                var config = provider.GetRequiredService<IConfiguration>();
                return new NpgsqlConnection(config.GetConnectionString("autopart"));
            });

            services.AddRepositories();

            return services;
        }

        private static void AddRepositories(this IServiceCollection services)
        {            
            services.Scan(scan =>
                scan.FromAssemblyOf<autopartContext>()
                    .AddClasses(classes => classes.AssignableTo(typeof(IRepository<>)))
                    .AsImplementedInterfaces()
                    .WithScopedLifetime()
                );
        }
    }
}

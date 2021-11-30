using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace MoqUnitTest.Moq.UnitTest.Injector
{
    public abstract class DependencyInjector
    {
        public IConfiguration Configuration { get; set; }
        public IServiceProvider ServiceProvider { get; set; }
        public IServiceCollection Services { get; set; }

        public DependencyInjector(string jsonAppSettingsPath)
        {
            Configuration = !string.IsNullOrWhiteSpace(jsonAppSettingsPath) ? new ConfigurationBuilder().AddJsonFile(jsonAppSettingsPath).Build() : null;
            Services = new ServiceCollection();
        }
        public virtual IServiceCollection CreateDbContext<TService>(Func<IServiceProvider, TService> func)
            where TService : class
        {
            return Services.AddTransient(func);
        }
        public virtual IServiceProvider Build()
        {
            ServiceProvider = Services.BuildServiceProvider();
            return ServiceProvider;
        }
        public virtual IServiceCollection AddService<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService
        {
            return Services.AddScoped<TService, TImplementation>();
        }
        public virtual IServiceCollection AddSingelton<TService>()
             where TService : class
        {
            return Services.AddSingleton<TService>();
        }
        public virtual void AddOptions<TOption>()
            where TOption : class
        {
            Services.Configure<TOption>(
                    Configuration.GetSection(typeof(TOption).Name));
        }
        public virtual void AddOptions<TOption>(string configurationName)
            where TOption : class
        {

            Services.Configure<TOption>(
                    Configuration.GetSection(configurationName));
        }
        public virtual T GetService<T>() =>
            ServiceProvider.GetService<T>();

    }


    public static class ServiceProviderExtensions
    {
        public static IServiceCollection AddService<TService, TImplementation>(this IServiceCollection services)
            where TService : class
            where TImplementation : class, TService
        {
            return services.AddScoped<TService, TImplementation>();
        }
    }

}

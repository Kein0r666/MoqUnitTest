using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.EntityFrameworkCore;

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
        public virtual IServiceCollection CreateDbContext<T>(string connection)
            where T : Microsoft.EntityFrameworkCore.DbContext
        {
            if (this.Configuration == null)
                throw new NullReferenceException(nameof(Configuration));

            var options = new Microsoft.EntityFrameworkCore.DbContextOptionsBuilder<T>()
                    .UseSqlServer(Configuration.GetConnectionString(connection))
                    .Options;

            return Services.AddSingleton((T)Activator.CreateInstance(typeof(T), new object[] { options }));


            throw new NullReferenceException(nameof(Configuration));
        }
        public virtual IServiceCollection CreateDbContext<T>(T context)
            where T : Microsoft.EntityFrameworkCore.DbContext
        {
            return Services.AddSingleton(context);
        }
        public virtual IServiceCollection CreateEF6DbContext<T>(string connection)
            where T : System.Data.Entity.DbContext
        {
            if (this.Configuration == null)
                throw new NullReferenceException(nameof(Configuration));

            return Services.AddTransient(x => (T)Activator.CreateInstance(typeof(T), new object[] { connection }));
        }
        public virtual IServiceCollection CreateEF6DbContext<T>(T context)
            where T : System.Data.Entity.DbContext
        {
            return Services.AddTransient(x => context);
        }
        public virtual IServiceCollection CreateDbContext<TService, TImplementation>(Func<TService, TImplementation> func)
            where TService : class
            where TImplementation : class, TService
        {
            return Services.AddSingleton(func);
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

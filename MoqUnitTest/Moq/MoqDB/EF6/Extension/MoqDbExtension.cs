using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MoqUnitTest.Moq.UnitTest.Injector;
using UserServiceTest.MoqDB.MoqModels;

namespace MoqUnitTest.Moq.MoqDB.EF6.Extension
{
    public static class MoqDbExtension
    {
        public static void Append<T, TModel>(this T context, IMoqModel<TModel> model)
            where TModel : class
            where T : DbContext
        {
            context.Entry(model.Create()).State = EntityState.Added;
            context.SaveChanges();
        }
        public static async Task AppendAsync<T, TModel>(this T context, IMoqModel<TModel> model)
            where TModel : class
            where T : DbContext
        {
            context.Entry(model.Create()).State = EntityState.Added;
            await context.SaveChangesAsync();
        }
        public static void AppendRange<T, TModel>(this T context, IEnumerable<IMoqModel<TModel>> models)
            where TModel : class
            where T : DbContext
        {
            foreach (var item in models)
                context.Entry(item.Create()).State = EntityState.Added;

            context.SaveChanges();
        }
        public static async Task AppendRangeAsync<T, TModel>(this T context, IEnumerable<IMoqModel<TModel>> models)
            where TModel : class
            where T : DbContext
        {
            foreach (var item in models)
                context.Entry(item.Create()).State = EntityState.Added;

            await context.SaveChangesAsync();
        }
        public static void Update<T, TModel>(this T context, IMoqModel<TModel> model)
            where TModel : class
            where T : DbContext
        {
            context.Entry(model.Create()).State = EntityState.Modified;
            context.SaveChanges();
        }
        public static async Task UpdateAsync<T, TModel>(this T context, IMoqModel<TModel> model)
            where TModel : class
            where T : DbContext
        {
            
            context.Entry(model.Create()).State = EntityState.Modified;
            await context.SaveChangesAsync();
        }
        public static void UpdateRange<T, TModel>(this T context, IEnumerable<IMoqModel<TModel>> models)
            where TModel : class
            where T : DbContext
        {
            foreach (var item in models)
                context.Entry(item.Create()).State = EntityState.Modified;

            context.SaveChanges();

        }
        public static async Task UpdateRangeAsync<T, TModel>(this T context, IEnumerable<IMoqModel<TModel>> models)
            where TModel : class
            where T : DbContext
        {
            foreach (var item in models)
                context.Entry(item.Create()).State = EntityState.Modified;

            await context.SaveChangesAsync();

        }

        public static IServiceCollection CreateEF6DbContext<T>(this DependencyInjector injector, string connection)
            where T : System.Data.Entity.DbContext
        {
            if (injector.Configuration == null)
                throw new NullReferenceException(nameof(Configuration));

            var options = injector.Configuration.GetConnectionString(connection);

            injector.Services.AddTransient(x => (T)Activator.CreateInstance(typeof(T), new object[] { options }));
            injector.Build();
            return injector.Services;
        }
        public static IServiceCollection CreateEF6DbContext<T>(this DependencyInjector injector, T context)
            where T : System.Data.Entity.DbContext
        {
            injector.Services.AddTransient(x => context);
            injector.Build();
            return injector.Services;
        }
    }
}

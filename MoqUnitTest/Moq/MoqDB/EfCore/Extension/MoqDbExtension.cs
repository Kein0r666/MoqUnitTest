using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MoqUnitTest.Moq.UnitTest.Injector;
using UserServiceTest.MoqDB.MoqModels;

namespace MoqUnitTest.Moq.MoqDB.EfCore.Extension
{
    public static class MoqDbExtension
    {
        public static void Append<T, TModel>(this T context, IMoqModel<TModel> model)
            where TModel : class
            where T : DbContext
        {
            context.Add(model.Create());
            context.SaveChanges();
        }
        public static async Task AppendAsync<T, TModel>(this T context, IMoqModel<TModel> model)
            where TModel : class
            where T : DbContext
        {
            await context.AddAsync(model.Create());
            await context.SaveChangesAsync();
        }
        public static void AppendRange<T, TModel>(this T context, IEnumerable<IMoqModel<TModel>> model)
            where TModel : class
            where T : DbContext
        {
            context.AddRange(model.Select(x => x.Create()));
            context.SaveChanges();
        }
        public static async Task AppendRangeAsync<T, TModel>(this T context, IEnumerable<IMoqModel<TModel>> model)
            where TModel : class
            where T : DbContext
        {
            await context.AddRangeAsync(model.Select(x => x.Create()).Reverse());
            await context.SaveChangesAsync();
        }
        public static void Update<T, TModel>(this T context, IMoqModel<TModel> model)
            where TModel : class
            where T : DbContext
        {
            context.Update(model.Create());
            context.SaveChanges();
        }
        public static async Task UpdateAsync<T, TModel>(this T context, IMoqModel<TModel> model)
            where TModel : class
            where T : DbContext
        {
            context.Update(model.Create());
            await context.SaveChangesAsync();
        }
        public static void UpdateRange<T, TModel>(this T context, IEnumerable<IMoqModel<TModel>> models)
            where TModel : class
            where T : DbContext
        {
            foreach (var item in models)
                context.Update(item.Create());

            context.SaveChanges();

        }
        public static async Task UpdateRangeAsync<T, TModel>(this T context, IEnumerable<IMoqModel<TModel>> models)
            where TModel : class
            where T : DbContext
        {
            foreach (var item in models)
                context.Update(item.Create());

            await context.SaveChangesAsync();

        }


        public static IServiceCollection CreateEFCoreDbContext<T>(this DependencyInjector injector, string connection)
            where T : Microsoft.EntityFrameworkCore.DbContext
        {
            if (injector.Configuration == null)
                throw new NullReferenceException(nameof(Configuration));

            var options = new Microsoft.EntityFrameworkCore.DbContextOptionsBuilder<T>()
                .UseSqlServer(injector.Configuration.GetConnectionString(connection))
                .Options;

            injector.Services.AddTransient(x => (T)Activator.CreateInstance(typeof(T), new object[] { options }));
            injector.Build();
            return injector.Services;
        }
        public static IServiceCollection CreateEFCoreDbContext<T>(this DependencyInjector injector, T context)
            where T : Microsoft.EntityFrameworkCore.DbContext
        {
            injector.Services.AddTransient(x => context);
            injector.Build();
            return injector.Services;

        }
    }
}

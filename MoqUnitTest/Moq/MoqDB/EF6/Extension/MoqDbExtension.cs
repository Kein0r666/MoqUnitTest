using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;
using UserServiceTest.MoqDB.MoqModels;

namespace MoqUnitTest.Moq.MoqDB.EF6.Extension
{
    public static class MoqDbExtension
    {
        public static void Append<T, TModel>(this T context, IMoqModel<TModel> model)
            where TModel : class
            where T : DbContext
        {


            var entry = context.Entry(model.Create());
            entry.State = EntityState.Added;
            try
            {
                context.SaveChanges();


            }catch (NullReferenceException ex)
            {
                
            }
        }
        public static async Task AppendAsync<T, TModel>(this T context, IMoqModel<TModel> model)
            where TModel : class
            where T : DbContext
        {
            await Task.Run(() => context.Entry(model.Create()).State = EntityState.Added);
            await context.SaveChangesAsync();
        }
        public static void AppendRange<T, TModel>(this T context, IEnumerable<IMoqModel<TModel>> model)
            where TModel : class
            where T : DbContext
        {
            foreach(var item in model)
                context.Entry(item.Create());

            context.SaveChanges();
        }
        public static async Task AppendRangeAsync<T, TModel>(this T context, IEnumerable<IMoqModel<TModel>> model)
            where TModel : class
            where T : DbContext
        {
            Parallel.ForEach(model, x => context.Entry(x.Create()).State = EntityState.Added);
            await context.SaveChangesAsync();
        }
    }
}

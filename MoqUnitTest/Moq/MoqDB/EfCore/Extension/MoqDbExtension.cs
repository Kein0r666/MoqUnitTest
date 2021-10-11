using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}

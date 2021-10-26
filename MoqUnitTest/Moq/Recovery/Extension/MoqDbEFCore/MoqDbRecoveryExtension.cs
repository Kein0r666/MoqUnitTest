using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using MoqUnitTest.Moq.Models.Generator;
using MoqUnitTest.Moq.MoqDB.EfCore.Extension;
using UserServiceTest.MoqDB.MoqModels;

namespace MoqUnitTest.Moq.Recovery.Extension.MoqDbEFCore
{
    public static class MoqDbRecoveryExtension
    {
        public static IEnumerable<IMoqModel<TModel>> RecoveryItems<TModel, TContext>(
            this MoqDB.EfCore.MoqDataContext<TContext> moqDb, params Func<IRecoveryMoqModel<TModel>>[] recoveryFunc)
            where TContext : DbContext
            where TModel : class
        {
            foreach (var recovery in recoveryFunc)
            {
                yield return moqDb.RecoveryItem(recovery);
            }
        }

        public static IMoqModel<TModel> RecoveryItem<TModel, TContext>(this MoqDB.EfCore.MoqDataContext<TContext> moqDb,
            Func<IRecoveryMoqModel<TModel>> recoveryFunc)
            where TContext : DbContext
            where TModel : class
        {
            var moqGenerator = (RecoveryGenerator<TModel>)recoveryFunc();
            IMoqModel<TModel> generatedMoq;
            if (moqGenerator.IsRecovered)
            {
                generatedMoq = moqGenerator.Generate();
                moqDb.MoqData.Items.Up(generatedMoq.GetType(), moqDb.MoqData.Pack(generatedMoq));
            }
            else
                generatedMoq = moqDb.Create(moqGenerator);

            return generatedMoq;
        }

        public static async Task<List<IMoqModel<TModel>>> RecoveryItemsAsync<TModel, TContext>(
            this MoqDB.EfCore.MoqDataContext<TContext> moqDb, params Func<Task<IRecoveryMoqModel<TModel>>>[] recoveryFunc)
            where TContext : DbContext
            where TModel : class
        {
            var models = new List<IMoqModel<TModel>>();

            foreach (var recovery in recoveryFunc)
            {
                models.Add(await moqDb.RecoveryItemAsync(recovery));
            }

            return models;
        }

        public static async Task<IMoqModel<TModel>> RecoveryItemAsync<TModel, TContext>(
            this MoqDB.EfCore.MoqDataContext<TContext> moqDb, Func<Task<IRecoveryMoqModel<TModel>>> recoveryFunc)
            where TContext : DbContext
            where TModel : class
        {
            var moqGenerator = (RecoveryGenerator<TModel>)await recoveryFunc();
            IMoqModel<TModel> generatedMoq;
            if (moqGenerator.IsRecovered)
            {
                generatedMoq = moqGenerator.Generate();
                moqDb.MoqData.Items.Up(generatedMoq.GetType(), moqDb.MoqData.Pack(generatedMoq));
            }
            else
                generatedMoq = moqDb.Create(moqGenerator);

            return generatedMoq;
        }

        public static async Task<IMoqModel<TModel>> RecreateItemAsync<TModel, TContext>(
            this MoqDB.EfCore.MoqDataContext<TContext> moqDb, IRecoveryMoqModel<TModel> model)
            where TContext : DbContext
            where TModel : class
        {
            var recreatedModel = model.Recreate();
            await moqDb.Context.UpdateAsync(recreatedModel);
            return recreatedModel;
        }

        public static IMoqModel<TModel> RecreateItem<TModel, TContext>(this MoqDB.EfCore.MoqDataContext<TContext> moqDb,
            IRecoveryMoqModel<TModel> model)
            where TContext : DbContext
            where TModel : class
        {
            var recreatedModel = model.Recreate();
            moqDb.Context.Update(recreatedModel.Create());
            moqDb.Context.SaveChanges();
            return recreatedModel;
        }
        
        public static async Task<TMoqModel> RecoveryMoqModel<TModel, TMoqModel>(
            this IQueryable<TModel> query)
            where TModel : class
            where TMoqModel : IMoqModel<TModel>
        {
            return await query
                .Select(ExpressionExtension.PullByLimit<TModel, TMoqModel>())
                .FirstOrDefaultAsync();
        }
        
        public static async IAsyncEnumerable<TMoqModel> RecoveryListMoqModels<TModel, TMoqModel>(
            this IQueryable<TModel> query)
            where TModel : class
            where TMoqModel : IMoqModel<TModel>
        {
            await foreach (var item in query
                .Select(ExpressionExtension.PullByLimit<TModel, TMoqModel>())
                .AsAsyncEnumerable())
            {
                yield return item;
            }
        }
    }
}

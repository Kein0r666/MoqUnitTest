using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using MoqUnitTest.Moq.Models;
using MoqUnitTest.Moq.Models.Generator;
using MoqUnitTest.Moq.Models.Interface;
using MoqUnitTest.Moq.MoqDB.EfCore.Extension;
using UserServiceTest.MoqDB.MoqModels;

namespace MoqUnitTest.Moq.MoqDB.EfCore
{
    public abstract class MoqDataContext<TContext> : IDisposable
        where TContext : DbContext
    {
        public TContext Context { get; set; }
        public MoqData MoqData { get; set; }

        public MoqDataContext()
        {
            MoqData = new MoqData();
        }

        /// <summary>
        /// Создание контекста базы данных в памяти
        /// </summary>
        /// <returns>Контекст базы данных</returns>
        public static TContext CreateDataBase()
        {
            var options = new DbContextOptionsBuilder<TContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            var DataContext = (TContext)Activator.CreateInstance(typeof(TContext), new object[] { options });

            return DataContext;
        }
        /// <summary>
        /// Создание контекста базы данных в памяти
        /// </summary>
        /// <returns>Контекст базы данных</returns>
        public TContext CreateDb()
        {
            var options = new DbContextOptionsBuilder<TContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            Context = (TContext)Activator.CreateInstance(typeof(TContext), new object[] { options });

            return Context;
        }
        /// <summary>
        /// Метода для заполнения базы данных. Всегда следует переопределять
        /// </summary>
        /// <param name="context"></param>
        public virtual void Seed(TContext context)
        {
            if (Context == null)
                Context = context;
        }
        /// <summary>
        /// Создаёт мок экземпляр, и заполняет его в список MoqData. Создаёт запись в базе данных
        /// </summary>
        /// <typeparam name="TModel">Модель базы данных</typeparam>
        /// <param name="moqModel">Мок генератор</param>
        public virtual IMoqModel<TModel> Create<TModel>(IMoqGenerator<TModel> moqModel)
            where TModel : class
        {
            var item = MoqData.CreateItem(moqModel);
            Context.Append(item);

            return item;
        }
        /// <summary>
        /// Создаёт мок экземпляр, и заполняет его в список MoqData. Создаёт запись в базе данных ассинхронно 
        /// </summary>
        /// <typeparam name="TModel">Модель базы данных</typeparam>
        /// <param name="moqModel">Мок генератор</param>
        public virtual async Task CreateAsync<TModel>(IMoqGenerator<TModel> moqModel)
            where TModel : class
        {
            var item = MoqData.CreateItem((MoqGenerator<TModel>)moqModel);
            await Context.AppendAsync(item);
        }
        /// <summary>
        /// Создаёт список мок экземпляров, и заполняет их в список MoqData. Создаёт записи в базе данных 
        /// </summary>
        /// <typeparam name="TModel">Модель базы данных</typeparam>
        /// <param name="moqModel">Мок генератор</param>
        public virtual IEnumerable<IMoqModel<TModel>> CreateItems<TModel>(IMoqGenerator<TModel> moqModel, int count)
            where TModel : class
        {
            var items = MoqData.CreateListItems(moqModel, count);
            Context.AppendRange(items);

            return items;
        }
        /// <summary>
        /// Создаёт список мок экземпляров, и заполняет их в список MoqData. Создаёт записи в базе данных ассинхронно
        /// </summary>
        /// <typeparam name="TModel">Модель базы данных</typeparam>
        /// <param name="moqModel">Мок генератор</param>
        public virtual async Task CreateItemsAsync<TModel>(IMoqGenerator<TModel> moqModel, int count)
           where TModel : class
        {
            var item = MoqData.CreateListItems(moqModel, count);
            await Context.AppendRangeAsync(item);
        }
        /// <summary>
        /// Создаёт список мок экземпляров, и заполняет их в список MoqData. Создаёт записи в базе данных 
        /// </summary>
        /// <typeparam name="TModel">Модель базы данных</typeparam>
        /// <param name="moqModel">Мок генератор</param>
        public virtual IEnumerable<IMoqModel<TModel>> CreateList<TModel>(IMoqGenerator<TModel>[] moqModel)
            where TModel : class
        {
            var items = MoqData.CreateListItems(moqModel);
            Context.AppendRange(items);

            return items;
        }
        /// <summary>
        /// Создаёт список мок экземпляров, и заполняет их в список MoqData. Создаёт записи в базе данных ассинхронно 
        /// </summary>
        /// <typeparam name="TModel">Модель базы данных</typeparam>
        /// <param name="moqModel">Мок генератор</param>
        public virtual async Task CreateListAsync<TModel>(MoqGenerator<TModel>[] moqModel)
            where TModel : class
        {
            var item = MoqData.CreateListItems(moqModel);
            await Context.AppendRangeAsync(item);
        }

        public void Dispose()
        {
            if (Context.Database.IsInMemory())
                Context.Database.EnsureDeleted();

            Context.Dispose();
            MoqData.Dispose();

            GC.Collect();
        }
    }
}

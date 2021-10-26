using Microsoft.Extensions.Configuration;
using MoqUnitTest.Moq.Models;
using MoqUnitTest.Moq.Models.Generator;
using MoqUnitTest.Moq.Models.Interface;
using MoqUnitTest.Moq.MoqDB.EF6.Extension;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserServiceTest.MoqDB.MoqModels;

namespace MoqUnitTest.Moq.MoqDB.EF6
{
    //TODO Восстановить прошлую версию DataContexta для работы с мок объектами
    public abstract class MoqDataContext<TContext> : IDisposable
        where TContext : System.Data.Entity.DbContext
    {
        public bool IsDisposed { get; private set; }

        public TContext Context { get; set; }
        public IConfiguration Configuration { get; private set; }
        public MoqData MoqData { get; set; }

        public MoqDataContext(IConfiguration configuration = null)
        {
            IsDisposed = false;
            MoqData = new MoqData();
            Configuration = configuration;
        }

        /// <summary>
        /// Создание контекста базы данных в памяти
        /// </summary>
        /// <returns>Контекст базы данных</returns>
        public static TContext CreateDataBase()
        {
            var connection = Effort.DbConnectionFactory.CreateTransient();
            var DataContext = (TContext)Activator.CreateInstance(typeof(TContext), new object[] { connection });
            return DataContext;
        }
        /// <summary>
        /// Создание контекста базы данных в памяти
        /// </summary>
        /// <returns>Контекст базы данных</returns>
        public TContext CreateDb()
        {
            var connection = Effort.DbConnectionFactory.CreateTransient();
            Context = (TContext)Activator.CreateInstance(typeof(TContext), connection);
            return Context;
        }
        /// <summary>
        /// Создание контекста базы данных
        /// </summary>
        /// <param name="connectionName"></param>
        /// <returns>Контекст базы данных</returns>
        public TContext CreateDb(string connectionName)
        {
            var connection = Configuration.GetConnectionString(connectionName);
            Context = (TContext)Activator.CreateInstance(typeof(TContext), connection);
            return Context;
        }
        /// <summary>
        /// Копирование данных с другой базы данных
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="func"></param>
        /// <param name="connectionName"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        public async Task CopyData<TEntity>(Func<Task<TEntity>, TEntity> func, string connectionName, int skip = 0, int take = 100)
            where TEntity : class
        {
            if (Configuration == null)
                throw new NullReferenceException(nameof(Configuration));

            using var db = CreateDb(connectionName);
        }
        /// <summary>
        /// Заполнение базы данными 
        /// </summary>
        /// <param name="context"></param>
        public virtual void Seed(TContext context)
        {
            if (Context == null)
                Context = context;
        }
        /// <summary>
        /// Заполнение базы асинхронно
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public virtual async Task SeedAsync(TContext context)
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
        public virtual async Task<IMoqModel<TModel>> CreateAsync<TModel>(IMoqGenerator<TModel> moqModel)
            where TModel : class
        {
            var item = MoqData.CreateItem(moqModel);
            await Context.AppendAsync(item);
            return item;
        }
        /// <summary>
        /// Создаёт список мок экземпляров, и заполняет их в список MoqData. Создаёт записи в базе данных 
        /// </summary>
        /// <typeparam name="TModel">Модель базы данных</typeparam>
        /// <param name="moqModel">Мок генератор</param>
        public virtual List<IMoqModel<TModel>> CreateItems<TModel>(IMoqGenerator<TModel> moqModel, int count)
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
        public virtual async Task<List<IMoqModel<TModel>>> CreateItemsAsync<TModel>(IMoqGenerator<TModel> moqModel, int count)
           where TModel : class
        {
            var item = MoqData.CreateListItems(moqModel, count);
            await Context.AppendRangeAsync(item);
            return item;
        }
        /// <summary>
        /// Создаёт список мок экземпляров и заполняет их в список MoqData. Создаёт записи в базе данных 
        /// </summary>
        /// <typeparam name="TModel">Модель базы данных</typeparam>
        /// <param name="moqModel">Мок генератор</param>
        public virtual List<IMoqModel<TModel>> CreateList<TModel>(IMoqGenerator<TModel>[] moqModel)
            where TModel : class
        {
            var items = MoqData.CreateListItems(moqModel);
            Context.AppendRange(items);
            return items;
        }
        /// <summary>
        /// Создаёт мок объект и заполняет их в список MoqData. Обновляет записи в базе данных
        /// </summary>
        /// <typeparam name="TModel">Модель базы данных</typeparam>
        /// <param name="moqModel">Мок генератор</param>
        public virtual IMoqModel<TModel> Update<TModel>(IMoqGenerator<TModel> moqModel)
            where TModel : class
        {
            var item = MoqData.CreateItem(moqModel);
            Context.Update(item);
            return item;
        }
        /// <summary>
        /// Создаёт мок объект и заполняет их в список MoqData. Обновляет записи в базе данных асинхронно
        /// </summary>
        /// <typeparam name="TModel">Модель базы данных</typeparam>
        /// <param name="moqModel">Мок генератор</param>
        public virtual List<IMoqModel<TModel>> UpdateRange<TModel>(IMoqGenerator<TModel>[] moqModels)
            where TModel : class
        {
            var items = MoqData.CreateListItems(moqModels);
            Context.UpdateRange(items);
            return items;
        }
        /// <summary>
        /// Создаёт список мок экземпляров и заполняет их в список MoqData. Обновляет записи в базе данных 
        /// </summary>
        /// <typeparam name="TModel">Модель базы данных</typeparam>
        /// <param name="moqModel">Мок генератор</param>
        public virtual async Task<IMoqModel<TModel>> UpdateAsync<TModel>(IMoqGenerator<TModel> moqModel)
            where TModel : class
        {
            var item = MoqData.CreateItem(moqModel);
            await Context.UpdateAsync(item);
            return item;
        }
        /// <summary>
        /// Создаёт список мок экземпляров и заполняет их в список MoqData. Обновляет записи в базе данных асинхронно
        /// </summary>
        /// <typeparam name="TModel">Модель базы данных</typeparam>
        /// <param name="moqModel">Мок генератор</param>
        public virtual async Task<List<IMoqModel<TModel>>> UpdateRangeAsync<TModel>(IMoqGenerator<TModel>[] moqModels)
            where TModel : class
        {
            var items = MoqData.CreateListItems(moqModels);
            await Context.UpdateRangeAsync(items);
            return items;
        }

        /// <summary>
        /// Создаёт список мок экземпляров, и заполняет их в список MoqData. Создаёт записи в базе данных ассинхронно 
        /// </summary>
        /// <typeparam name="TModel">Модель базы данных</typeparam>
        /// <param name="moqModel">Мок генератор</param>
        public virtual async Task<List<IMoqModel<TModel>>> CreateListAsync<TModel>(MoqGenerator<TModel>[] moqModel)
            where TModel : class
        {
            var item = MoqData.CreateListItems(moqModel);
            await Context.AppendRangeAsync(item);
            return item;
        }

        public void Dispose()
        {
            if (!IsDisposed)
            {
                IsDisposed = true;
                Context.Dispose();
                MoqData.Dispose();
            }

            GC.Collect();
        }
    }
}

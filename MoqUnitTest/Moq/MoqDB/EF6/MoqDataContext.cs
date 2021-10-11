using Microsoft.Extensions.Configuration;
using MoqUnitTest.Moq.Models;
using MoqUnitTest.Moq.Models.Generator;
using MoqUnitTest.Moq.Models.Interface;
using MoqUnitTest.Moq.MoqDB.EF6.Extension;
using MoqUnitTest.Moq.MoqDB.Interface;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;
using UserServiceTest.MoqDB.MoqModels;

namespace MoqUnitTest.Moq.MoqDB.EF6
{
    //TODO Восстановить прошлую версию DataContexta для работы с мок объектами
    public abstract class MoqDataContext<TContext> : IDisposable
        where TContext : DbContext
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

            Context.Entry(item.Create());
            Context.SaveChanges();

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
            var item = MoqData.CreateItem(moqModel);

            Context.Entry(item.Create()).State = EntityState.Added;
            await Context.SaveChangesAsync();

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

            //foreach (var item in items)
            //    Context.Entry(item.Create()).State = EntityState.Added;

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

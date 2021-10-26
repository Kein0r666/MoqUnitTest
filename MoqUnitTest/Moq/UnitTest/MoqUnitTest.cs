using MoqUnitTest.Moq.MoqDB;
using EFCore = MoqUnitTest.Moq.MoqDB.EfCore;
using EF6 = MoqUnitTest.Moq.MoqDB.EF6;
using MoqUnitTest.Moq.UnitTest.Injector;
using System;
using Microsoft.Extensions.Configuration;
using MoqUnitTest.Moq.MoqDB.EF6.Extension;
using MoqUnitTest.Moq.MoqDB.EfCore.Extension;

namespace MoqUnitTest.Moq.UnitTest
{
    public abstract class MoqUnitTest<TService, TInjector> : IDisposable
        where TService : class
        where TInjector : DependencyInjector
    {
        public bool IsRealDb { get; private set; }
        public bool RunOnRelease { get; private set; }
        public bool IsEF6 { get; private set; }
        public bool IsEFCore { get; private set; }

        protected TInjector Injector { get; set; }
        public TService Service { get; set; }

        public MoqUnitTest(TInjector dependencyInjector, bool? isRealDb = null, bool? runOnRelease = null)
        {
            Injector = dependencyInjector;
            this.IsRealDb = isRealDb.HasValue ? isRealDb.Value : false;
            this.RunOnRelease = runOnRelease ?? false;
        }
        public MoqUnitTest(TInjector dependencyInjector, bool? isRealDb = null, string variableRunOnRelease = "")
        {
            Injector = dependencyInjector;
            this.IsRealDb = isRealDb.HasValue ? isRealDb.Value : false;
            if (bool.TryParse(Environment.GetEnvironmentVariable(variableRunOnRelease), out var runOnRelease))
                RunOnRelease = runOnRelease;
        }

        /// <summary>
        /// Prepare current service
        /// </summary>
        /// <returns></returns>
        public virtual TService PrepareService()
        {
            Service = Injector.GetService<TService>();
            return Service;
        }
        /// <summary>
        /// Prepare EFCore moq database
        /// </summary>
        /// <typeparam name="TMoqContext">Moq database</typeparam>
        /// <typeparam name="TContext">Database</typeparam>
        /// <param name="connectionName">Connection name if this Moq database is real</param>
        /// <param name="moqDataContext">Moq database</param>
        /// <param name="seedRealDb">if true seed database, otherwise<code>false</code></param>
        /// <returns>Moq database</returns>
        public virtual TMoqContext PrepareEFCoreDb<TMoqContext, TContext>(string connectionName = "", TMoqContext moqDataContext = default, bool seedRealDb = false)
            where TMoqContext : EFCore.MoqDataContext<TContext>
            where TContext : Microsoft.EntityFrameworkCore.DbContext
        {
            if (IsRealDb && !string.IsNullOrWhiteSpace(connectionName))
            {
                Injector.CreateEFCoreDbContext<TContext>(connectionName);
                moqDataContext.Context = Injector.GetService<TContext>();
                if(seedRealDb)
                    moqDataContext.Seed(null);

            }
            else if (!IsRealDb && moqDataContext != null)
            {
                Injector.CreateEFCoreDbContext(moqDataContext.CreateDb());
                moqDataContext.Seed(null);
            }

            IsEFCore = true;
            return moqDataContext;
        }
        /// <summary>
        /// Prepare EF6 moq database
        /// </summary>
        /// <typeparam name="TMoqContext">Moq database</typeparam>
        /// <typeparam name="TContext">Database</typeparam>
        /// <param name="connectionName">Connection name if this Moq database is real</param>
        /// <param name="moqDataContext">Moq database</param>
        /// <param name="seedRealDb">if true seed database, otherwise<code>false</code></param>
        /// <returns>Moq database</returns>
        public virtual TMoqContext PrepareEF6Db<TMoqContext, TContext>(string connectionName = "", TMoqContext moqDataContext = default, bool seedRealDb = false)
            where TMoqContext : EF6.MoqDataContext<TContext>
            where TContext : System.Data.Entity.DbContext
        {
            if (IsRealDb && !string.IsNullOrWhiteSpace(connectionName))
            {
                Injector.CreateEF6DbContext<TContext>(connectionName);
                moqDataContext.Context = Injector.GetService<TContext>();
                if (seedRealDb)
                    moqDataContext.Seed(null);
            }
            else if (moqDataContext != null)
            {
                Injector.CreateEF6DbContext(moqDataContext.CreateDb());
                moqDataContext.Seed(null);
            }

            IsEF6 = true;
            return moqDataContext;
        }

        /// <summary>
        /// Create dependency for unit test
        /// </summary>
        protected internal abstract void CreateDependency();

        public abstract void Dispose();
    }
}

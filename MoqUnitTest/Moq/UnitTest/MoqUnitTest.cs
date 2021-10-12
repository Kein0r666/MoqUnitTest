using MoqUnitTest.Moq.MoqDB;
using EFCore = MoqUnitTest.Moq.MoqDB.EfCore;
using EF6 = MoqUnitTest.Moq.MoqDB.EF6;
using MoqUnitTest.Moq.UnitTest.Injector;
using System;

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

        public virtual TService PrepareService()
        {
            Service = Injector.GetService<TService>();
            return Service;
        }
        public virtual TMoqContext PrepareDb<TMoqContext, TContext>(string connectionName = "", TMoqContext moqDataContext = null)
            where TMoqContext : EFCore.MoqDataContext<TContext>
            where TContext : Microsoft.EntityFrameworkCore.DbContext
        {
            if (IsRealDb && !string.IsNullOrWhiteSpace(connectionName))
                Injector.CreateDbContext<TContext>(connectionName);
            else if (moqDataContext != null)
            {
                Injector.CreateDbContext(moqDataContext.CreateDb());
            }

            var moqDb = Injector.GetService<TMoqContext>();

            if (!IsRealDb)
                moqDb.Seed(moqDb.Context);

            IsEFCore = true;
            return moqDb;
        }
        public virtual TMoqContext PrepareEF6Db<TMoqContext, TContext>(string connectionName = "", TMoqContext moqDataContext = null)
            where TMoqContext : EF6.MoqDataContext<TContext>
            where TContext : System.Data.Entity.DbContext
        {
            if (IsRealDb && !string.IsNullOrWhiteSpace(connectionName))
                Injector.CreateEF6DbContext<TContext>(connectionName);
            else if (moqDataContext != null)
            {
                Injector.CreateEF6DbContext(moqDataContext.CreateDb());
            }

            var moqDb = Injector.GetService<TMoqContext>();

            if(!IsRealDb)
                moqDb.Seed(moqDb.Context);

            IsEF6 = true;
            return moqDb;

        }

        public abstract void Dispose();
    }
}

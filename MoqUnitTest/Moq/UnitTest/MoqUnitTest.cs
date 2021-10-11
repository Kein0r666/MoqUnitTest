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
        public virtual void PrepareDb<TContext>(string connectionName = "", EFCore.MoqDataContext<TContext> moqDataContext = null)
            where TContext : Microsoft.EntityFrameworkCore.DbContext
        {
            if (IsRealDb && !string.IsNullOrWhiteSpace(connectionName))
                Injector.CreateDbContext<TContext>(connectionName);
            else if (moqDataContext != null)
                Injector.CreateDbContext(moqDataContext.CreateDb());

            IsEFCore = true;
        }
        public virtual void PrepareDb<TContext>(string connectionName = "", EF6.MoqDataContext<TContext> moqDataContext = null)
            where TContext : System.Data.Entity.DbContext
        {
            if (IsRealDb && !string.IsNullOrWhiteSpace(connectionName))
                Injector.CreateEF6DbContext<TContext>(connectionName);
            else if (moqDataContext != null)
            {
                Injector.CreateEF6DbContext(moqDataContext.CreateDb());
            }
                
            IsEF6 = true;
        }

        public abstract void Dispose();
    }
}

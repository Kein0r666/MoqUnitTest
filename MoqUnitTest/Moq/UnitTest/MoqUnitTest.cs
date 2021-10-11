using Microsoft.EntityFrameworkCore;
using MoqUnitTest.Moq.MoqDB;
using MoqUnitTest.Moq.UnitTest.Injector;
using System;

namespace MoqUnitTest.Moq.UnitTest
{
    public abstract class MoqUnitTest<TService, TInjector> : IDisposable
        where TService : class
        where TInjector : DependencyInjector
    {
        protected TInjector Injector { get; set; }
        public TService service { get; set; }

        public MoqUnitTest(TInjector dependencyInjector)
        {
            Injector = dependencyInjector;   
        }


        public abstract void Dispose();
    }
}

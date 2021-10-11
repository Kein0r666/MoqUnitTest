using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserServiceTest.MoqDB.MoqModels;

namespace MoqUnitTest.Moq.Models.Interface
{
    public interface IMoqGenerator<T>
        where T : class
    {
        public IMoqModel<T> Generate();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace UserServiceTest.MoqDB.MoqModels
{
    public interface IMoqModel<T>
        where T : class
    {
        public T Create();
    }
}

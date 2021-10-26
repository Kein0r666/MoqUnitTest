using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserServiceTest.MoqDB.MoqModels;

namespace MoqUnitTest.Moq.Models
{
    public class MoqItems : Dictionary<Type, List<object>>, IDisposable
    {

        public void Up(Type type, List<object> obj)
        {
            if (Get(type) == null)
                Add(type, obj);
            else
                this[type].AddRange(obj);
        }

        public List<object> Get(Type obj)
        {
            var enumerator = GetEnumerator();

            while (enumerator.MoveNext())
            {
                if (enumerator.Current.Key == obj)
                    return enumerator.Current.Value;
            }

            return null;
        }

        public void Dispose()
        {
            var enumerator = GetEnumerator();
            while(enumerator.MoveNext())
            {
                enumerator.Dispose();
            }
        }
    }
}

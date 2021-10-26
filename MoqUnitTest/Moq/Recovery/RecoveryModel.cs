using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MoqUnitTest.Moq.Recovery
{
    public class RecoveryModel<TModel>
        where TModel : class
    {
        public RecoveryModel(TModel model)
        {
            Model = model;
            Recursive = false;
        }
        public RecoveryModel(TModel model, bool recursive)
        {
            Model = model;
            Recursive = recursive;
        }

        public TModel Model { get; set; }
        public bool Recursive { get; set; }
    }
}

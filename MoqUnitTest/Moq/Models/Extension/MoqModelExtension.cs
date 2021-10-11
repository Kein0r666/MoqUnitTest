using Microsoft.EntityFrameworkCore;
using MoqUnitTest.Moq.Models.Generator;
using System;
using System.Threading.Tasks;
using UserServiceTest.MoqDB.MoqModels;

namespace MoqUnitTest.Moq.Models.Extension
{
    public static class MoqModelExtension
    {
        public static MoqGenerator<TModel> PreConfiure<TMoq, TModel>(this TMoq model, Func<TMoq, MoqGenerator<TModel>> func)
            where TMoq : IMoqModel<TModel>
            where TModel : class
        {
            return func.Invoke(model);
        }
    }
}

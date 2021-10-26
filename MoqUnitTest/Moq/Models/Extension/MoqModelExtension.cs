using Microsoft.EntityFrameworkCore;
using MoqUnitTest.Moq.Models.Generator;
using System;
using System.Linq;
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
        public static TModel FillInnerModel<TModel>(this IMoqModel<TModel> outerModel)
            where TModel : class
        {
            var outerProps = outerModel.GetType().GetProperties();
            var innerProps = typeof(TModel).GetProperties();

            var innerModel = (TModel)Activator.CreateInstance(typeof(TModel));

            foreach (var outerProp in outerProps)
            {
                foreach (var innerProp in innerProps)
                {
                    if(innerProp.Name == outerProp.Name)
                        innerProp.SetValue(innerModel, outerProp.GetValue(outerModel));
                }
            }
            return innerModel;
        }
        public static IMoqModel<TModel> FillOuterModel<TModel>(this TModel innerModel, IMoqModel<TModel> outerModel = null)
            where TModel : class
        {
            var outerProps = outerModel.GetType().GetProperties();
            var innerProps = typeof(TModel).GetProperties();

            if(outerModel == null)
                outerModel = (IMoqModel<TModel>)Activator.CreateInstance(typeof(IMoqModel<TModel>));

            foreach (var outerProp in outerProps)
            {
                foreach (var innerProp in innerProps)
                {
                    if (innerProp.Name == outerProp.Name)
                        outerProp.SetValue(outerModel, innerProp.GetValue(innerModel));
                }
            }
            return outerModel;
        }
    }
}

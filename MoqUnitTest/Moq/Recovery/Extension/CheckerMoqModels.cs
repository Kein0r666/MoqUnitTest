using System;
using MoqUnitTest.Moq.Models.Generator;

namespace MoqUnitTest.Moq.Recovery.Extension
{
    public static class CheckerMoqModels
    {
        public static bool CheckIsRecovered<TModel>(this RecoveryModel<TModel> model)
            where TModel : class
        {
            return model?.Model != null;
        }

        public static RecoveryGenerator<TModel> IfIsNotRecovered<TModel>(this RecoveryGenerator<TModel> recoveredGenerator, params object?[] recoveryParams)
            where TModel : class
        {
            if (recoveredGenerator == null)
                return (RecoveryGenerator<TModel>)Activator.CreateInstance(recoveredGenerator.GetType(), recoveryParams);

            recoveredGenerator.IsRecovered = true;
            return recoveredGenerator;
        }
    }
}

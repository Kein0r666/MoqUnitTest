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

        public static TGenerator IfIsNotRecovered<TGenerator, TModel>(this TGenerator recoveredGenerator, params object?[] recoveryParams)
            where TGenerator : RecoveryGenerator<TModel>
            where TModel : class
        {
            if (recoveredGenerator == null)
                return (TGenerator)Activator.CreateInstance(typeof(TGenerator), recoveryParams);

            recoveredGenerator.IsRecovered = true;
            return recoveredGenerator;
        }
    }
}

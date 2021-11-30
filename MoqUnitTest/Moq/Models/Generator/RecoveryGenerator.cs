using System;
using System.Collections.Generic;
using System.Text;
using MoqUnitTest.Moq.Attributes;
using MoqUnitTest.Moq.Models.Extension;
using MoqUnitTest.Moq.Recovery;
using MoqUnitTest.Moq.Recovery.Attirbute;
using MoqUnitTest.Moq.Recovery.Extension;
using UserServiceTest.MoqDB.MoqModels;

namespace MoqUnitTest.Moq.Models.Generator
{
    public abstract class RecoveryGenerator<T> : MoqGenerator<T>, IRecoveryMoqModel<T>
        where T : class
    {
        public RecoveryGenerator()
        {
            
        }
        public RecoveryGenerator(RecoveryModel<T> recoveryModel)
        {
            RecoveryModel = recoveryModel;
            IsRecovered = RecoveryModel.CheckIsRecovered();
        }

        [NonGenerable]
        [NonPull]
        /// <summary>
        /// Model for recovery this moq model
        /// </summary>
        protected internal RecoveryModel<T> RecoveryModel { get; set; }

        /// <summary>
        /// Recovery this model
        /// </summary>
        /// <returns></returns>
        public override IMoqModel<T> Generate()
            => Recovery(RecoveryModel);

        /// <summary>
        /// Recovery model
        /// </summary>
        /// <param name="recoveryModel"></param>
        /// <returns></returns>
        public virtual IMoqModel<T> Recovery(RecoveryModel<T> recoveryModel)
        {
            if (IsRecovered)
                return (IMoqModel<T>)this;

            if (!recoveryModel?.Recursive ?? false)
                return (IMoqModel<T>)this;

            var buyerProfile = recoveryModel?.Model;
            if (buyerProfile == null)
            {
                return Recreate();
            }
            else
            {
                return buyerProfile.FillOuterModel((IMoqModel<T>)this);
            }
        }


        /// <summary>
        /// Recreate current model
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public abstract IMoqModel<T> Recreate();
        /// <summary>
        /// Create new model
        /// </summary>
        /// <returns></returns>
        public abstract T Create();

        [NonGenerable]
        [NonPull]
        public bool IsRecovered { get; set; }
        [NonGenerable]
        [NonPull]
        public bool IsComposed { get; set; }
        [NonGenerable]
        [NonPull]
        public bool IsUpdated { get; set; }

    }
}

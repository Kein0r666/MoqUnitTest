using MoqUnitTest.Moq.Attributes;
using UserServiceTest.MoqDB.MoqModels;

namespace MoqUnitTest.Moq.Recovery
{
    public interface IRecoveryMoqModel<T> : IMoqModel<T>
        where T : class
    {
        public IMoqModel<T> Recovery(RecoveryModel<T> recoveryModel);
        public IMoqModel<T> Recreate();

        [NonGenerable]
        public bool IsRecovered { get; set; }
        [NonGenerable]
        public bool IsComposed { get; set; }
        [NonGenerable]
        public bool IsUpdated { get; set; }
    }
}

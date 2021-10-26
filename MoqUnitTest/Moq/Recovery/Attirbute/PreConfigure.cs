using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Evoshare.MoqTesting.Extension
{

    public class PreConfigureRecoveryModel : Attribute
    {
        private readonly string[] _param;
        public PreConfigureRecoveryModel(params string[] param)
        {
            _param = param;
        }
        public async Task PrepareAsync(ListPreconfigures preConfigures)
        {
            foreach (var param in _param)
            {
                await preConfigures.InvokeAsync(param);
            }
        }
        public void Prepare(ListPreconfigures preConfigures)
        {
            foreach (var param in _param)
            {
                preConfigures.Invoke(param);
            }
        }
    }

    public class ListPreconfigures : List<PreConfigures>
    {
        public Task InvokeAsync(string name)
        {
            using var enumerator = this.GetEnumerator();

            if(enumerator.MoveNext())
                if (enumerator.Current == null)
                    throw new NullReferenceException(nameof(ListPreconfigures));
            do
            {
                if (enumerator.Current.Name == name)
                {
                    return enumerator.Current.RecoveryFuncAsync();
                }

            } while (enumerator.MoveNext());

            throw new Exception("No recovery function found");
        }
        public void Invoke(string name)
        {
            using var enumerator = this.GetEnumerator();
            if (enumerator.MoveNext())
                if (enumerator.Current == null)
                    throw new NullReferenceException(nameof(ListPreconfigures));

            do
            {
                if (enumerator.Current.Name == name)
                    enumerator.Current.RecoveryFunc();
                
            } while (enumerator.MoveNext());

            throw new Exception("No recovery function found");
        }
    }

    public class PreConfigures
    {
        public PreConfigures(Func<Task> recoveryFunc)
        {
            Name = recoveryFunc.Method.Name;
            RecoveryFuncAsync = recoveryFunc;
        }
        public PreConfigures(Action recoveryFunc)
        {
            Name = recoveryFunc.Method.Name;
            RecoveryFunc = recoveryFunc;
        }
        public string Name { get; }
        public Func<Task> RecoveryFuncAsync { get; set; }
        public Action RecoveryFunc { get; set; }
    
    }
}

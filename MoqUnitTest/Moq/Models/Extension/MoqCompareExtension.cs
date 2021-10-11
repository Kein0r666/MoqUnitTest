using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoqUnitTest.Moq.Models.Extension
{
    public static class MoqCompareExtension
    {
        public static bool CompareSimpleTypes<T>(this T expected, T actual)
        {
            var props = expected.GetType().GetProperties();

            object test = null;

            for (var i = 0; i < props.Length; i++)
            {
                test = props[i].GetValue(actual);

                if (props[i].GetValue(actual).CheckIsSimpleType())
                    if (props[i].GetValue(expected) != props[i].GetValue(actual))
                        return false;
            }

            return true;
        }

        public static bool CheckIsSimpleType<T>(this T type)
        {
            if (type is string)
                return true;
            if (type is decimal)
                return true;
            if (type is float)
                return true;
            if (type is bool)
                return true;
            if (type is short)
                return true;
            if (type is int)
                return true;
            if (type is double)
                return true;
            if (type is char)
                return true;

            return false;
        }
    }
}

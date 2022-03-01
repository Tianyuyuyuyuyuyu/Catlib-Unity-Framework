using System;

namespace Framework.Utility.Runtime
{
    public static partial class RuntimeUtilities
    {
        /// <summary>
        /// 通用扩展
        /// </summary>
        public static class Compare
        {
            /// <summary>
            /// 计算毕竟结果
            /// </summary>
            /// <param name="a">A</param>
            /// <param name="compareOperator">符号</param>
            /// <param name="b">B</param>
            /// <returns>结果</returns>
            public static bool Calc(IComparable a, CompareOperator compareOperator, IComparable b)
            {
                var compareTo = a.CompareTo(b);
                switch (compareOperator)
                {
                    case CompareOperator.Equals:
                        return compareTo == 0;
                    case CompareOperator.NotEquals:
                        return compareTo != 0;
                    case CompareOperator.LessThan:
                        return compareTo < 0;
                    case CompareOperator.GreaterThan:
                        return compareTo > 0;
                    case CompareOperator.LessThanOrEquals:
                        return compareTo <= 0;
                    case CompareOperator.GreaterThanOrEquals:
                        return compareTo >= 0;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(compareOperator), compareOperator, null);
                }

                return false;
            }
        }

        /// <summary>
        /// Standard comparison operators.
        /// </summary>
        public enum CompareOperator
        {
            /// <summary> == mathematical operator.</summary>
            Equals,

            /// <summary> != mathematical operator.</summary>
            NotEquals,

            /// <summary> < mathematical operator.</summary>
            LessThan,

            /// <summary> > mathematical operator.</summary>
            GreaterThan,

            /// <summary> <= mathematical operator.</summary>
            LessThanOrEquals,

            /// <summary> >= mathematical operator.</summary>
            GreaterThanOrEquals
        }
    }
}
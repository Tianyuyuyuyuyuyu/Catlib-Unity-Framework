using System;
using System.Text;
using UnityEngine;

namespace Framework.Utility.Runtime
{
    public static partial class RuntimeUtilities
    {
        public static class Math
        {
            /// <summary>
            /// 字母后缀
            /// </summary>
            private static readonly string[] numberSuffixes;
            /// <summary>
            /// stringbuilder
            /// </summary>
            [ThreadStatic]
            private static StringBuilder stringBuilder = new StringBuilder(1024);
            
            static Math()
            {
                numberSuffixes = new string[110];
                int i = 0;
                numberSuffixes[i++] = string.Empty;
                //本地化
                // numberSuffixes[i++] = "NUMBER_K";
                // numberSuffixes[i++] = "NUMBER_M";
                // numberSuffixes[i++] = "NUMBER_B";
                // numberSuffixes[i++] = "NUMBER_T";

                numberSuffixes[i++] = "K";
                numberSuffixes[i++] = "M";
                numberSuffixes[i++] = "B";
                numberSuffixes[i++] = "T";

                for (char c = 'A'; c <= 'Z'; c = (char)(c + 1))
                {
                    numberSuffixes[i++] = new string(c, 2);
                }
                for (char c2 = 'A'; c2 <= 'Z'; c2 = (char)(c2 + 1))
                {
                    numberSuffixes[i++] = new string(c2, 3);
                }
                for (char c3 = 'A'; c3 <= 'Z'; c3 = (char)(c3 + 1))
                {
                    numberSuffixes[i++] = new string(new char[2]
                    {
                        c3,
                        '4'
                    });
                }
                for (char c4 = 'A'; c4 <= 'Z'; c4 = (char)(c4 + 1))
                {
                    numberSuffixes[i++] = new string(new char[2]
                    {
                        c4,
                        '5'
                    });
                }
                for (; i < 110; i++)
                {
                    numberSuffixes[i] = "e" + i * 3;
                }
            }


            public static float Vector2ToAngle(Vector2 v)
            {
                float num = Mathf.Atan2(v.y, v.x);
                return (num * 57.29578f - 90f + 360f) % 360f;
            }

            public static int MinMaxControl(int value, int min, int max)
            {
                if (value < min)
                {
                    return min;
                }
                else if (value > max)
                {
                    return max;
                }
                return value;
            }

            public static float MinMaxControl(float value, float min, float max)
            {
                if (value < min)
                {
                    return min;
                }
                else if (value > max)
                {
                    return max;
                }
                return value;
            }

            /// <summary>
            /// 冒泡升序排列
            /// </summary>
            /// <param name="arr"></param>
            /// <returns></returns>
            public static int[] BubbleSort(int[] arr)
            {
                // 外层for循环控制循环次数
                for (int i = 0; i < arr.Length; i++)
                {
                    int temp = 0;
                    for (int j = i + 1; j < arr.Length; j++)
                    {
                        if (arr[i] > arr[j])
                        {
                            temp = arr[j];
                            arr[j] = arr[i];
                            arr[i] = temp;
                        }
                    }
                }
                return arr;
            }

            /// <summary>
            /// 根据值获取字符串缩写
            /// </summary>
            /// <param name="x">值</param>
            /// <returns>字符串缩写</returns>
            public static string GetDoubleString(double x)
            {
                return GetDoubleStringBuilder(x).ToString();
            }

            /// <summary>
            /// 根据值获取字符串缩写
            /// </summary>
            /// <param name="x">值</param>
            /// <returns>字符串缩写</returns>
            public static StringBuilder GetDoubleStringBuilder(double x)
            {
                stringBuilder.Length = 0;
                if (double.IsInfinity(x))
                {
                    return stringBuilder.Append("INF");
                }
                if (double.IsNaN(x))
                {
                    return stringBuilder.Append("NaN");
                }
                if (x < 0.0)
                {
                    if (-1.0 < x)
                    {
                        return stringBuilder.Append("0");
                    }
                    stringBuilder.Append("-");
                    double num = x - 1E-05;
                    int num2 = (int)x;
                    if (x > -1000.0 && (int)num < num2)
                    {
                        x = num;
                    }
                    x = 0.0 - x;
                }
                if (x < 1000.0)
                {
                    stringBuilder.Append((int)x);
                    return stringBuilder;
                }

                ClassicNotation(x, stringBuilder);
                return stringBuilder;

                // switch (NOTATION_STYLE)
                // {
                //     case NotationStyle.CLASSIC:
                //         ClassicNotation(x, stringBuilder);
                //         return stringBuilder;
                //     case NotationStyle.SCIENTIFIC:
                //     {
                //         if (x < 1000.0)
                //         {
                //             ClassicNotation(x, stringBuilder);
                //             return stringBuilder;
                //         }
                //         int num3 = Mathf.FloorToInt((float)Math.Log10(x));
                //         double num4 = x;
                //         for (int i = 0; i < num3; i++)
                //         {
                //             num4 /= 10.0;
                //         }
                //         return stringBuilder.Append(num4.ToString("F2")).Append("E").Append(num3);
                //     }
                //     default:
                //         throw new Exception(NOTATION_STYLE.ToString());
                // }
            }

            /// <summary>
            /// 根据值获取字符串缩写 流行版
            /// </summary>
            /// <param name="x">值</param>
            /// <param name="sb">字符串构建器</param>
            /// <returns>字符串缩写</returns>
            public static void ClassicNotation(double x, StringBuilder sb)
            {
                int num = 0;
                while (x >= 1000.0)
                {
                    x /= 1000.0;
                    num++;
                }
                if (x >= 100.0)
                {
                    sb.Append((int)x);
                }
                else if (x >= 10.0)
                {
                    sb.Append(x.ToString("F1"));
                }
                else
                {
                    sb.Append(x.ToString("F2"));
                }
                sb.Append(numberSuffixes[num]);
            }
        }
    }
}

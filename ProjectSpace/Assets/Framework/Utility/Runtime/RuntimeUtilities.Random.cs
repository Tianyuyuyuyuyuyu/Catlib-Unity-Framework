//------------------------------------------------------------
// Game FrameworkMono v3.x
// Copyright © 2013-2018 Jiang Yin. All rights reserved.
// Homepage: http://gameframework.cn/
// Feedback: mailto:jiangyin@gameframework.cn
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

namespace Framework.Utility.Runtime
{
    public static partial class RuntimeUtilities
    {
        /// <summary>
        /// 随机相关的实用函数。
        /// </summary>
        public static class Random
        {
            private static System.Random s_Random = new System.Random((int) System.DateTime.Now.Ticks);

            /// <summary>
            /// 0-100数列
            /// </summary>
            public static List<int> Sequence = new List<int>(100);

            /// <summary>
            /// 设置随机数种子。
            /// </summary>
            /// <param name="seed">随机数种子。</param>
            public static void SetSeed(int seed)
            {
                s_Random = new System.Random(seed);
            }

            /// <summary>
            /// 返回非负随机数。
            /// </summary>
            /// <returns>大于等于零且小于 System.Int32.MaxValue 的 32 位带符号整数。</returns>
            public static int GetRandom()
            {
                return s_Random.Next();
            }

            /// <summary>
            /// 返回一个小于所指定最大值的非负随机数。
            /// </summary>
            /// <param name="maxValue">要生成的随机数的上界（随机数不能取该上界值）。maxValue 必须大于等于零。</param>
            /// <returns>大于等于零且小于 maxValue 的 32 位带符号整数，即：返回值的范围通常包括零但不包括 maxValue。不过，如果 maxValue 等于零，则返回 maxValue。</returns>
            public static int GetRandom(int maxValue)
            {
                return s_Random.Next(maxValue);
            }

            /// <summary>
            /// 返回一个小于所指定最大值的非负随机数。
            /// </summary>
            /// <param name="maxValue">要生成的随机数的上界（随机数不能取该上界值）。maxValue 必须大于等于零。</param>
            /// <returns>大于等于零且小于 maxValue 的 64 位带符号整数，即：返回值的范围通常包括零但不包括 maxValue。不过，如果 maxValue 等于零，则返回 maxValue。</returns>
            public static long GetRandom(long maxValue)
            {
                return (long)(maxValue * s_Random.NextDouble());
            }

            /// <summary>
            /// 返回一个指定范围内的随机数。
            /// </summary>
            /// <param name="minValue">返回的随机数的下界（随机数可取该下界值）。</param>
            /// <param name="maxValue">返回的随机数的上界（随机数不能取该上界值）。maxValue 必须大于等于 minValue。</param>
            /// <returns>一个大于等于 minValue 且小于 maxValue 的 32 位带符号整数，即：返回的值范围包括 minValue 但不包括 maxValue。如果 minValue 等于 maxValue，则返回 minValue。</returns>
            public static int GetRandom(int minValue, int maxValue)
            {
                return s_Random.Next(minValue, maxValue);
            }

            /// <summary>
            /// 返回一个指定范围内的随机数。
            /// </summary>
            /// <param name="minValue">返回的随机数的下界（随机数可取该下界值）</param>
            /// <param name="maxValue">返回的随机数的上界（随机数可取该下界值）</param>
            /// <returns>值</returns>
            public static float GetRandom(float minValue, float maxValue)
            {
                return UnityEngine.Random.Range(minValue, maxValue);
            }

            /// <summary>
            /// 返回一个指定范围内的随机数(包含上下界)。
            /// </summary>
            /// <param name="minValue">返回的随机数的下界（随机数可取该下界值）。</param>
            /// <param name="maxValue">返回的随机数的上界（随机数可取该上界值）。maxValue 必须大于等于 minValue。</param>
            /// <returns>一个大于等于 minValue 且小于 maxValue 的 32 位带符号整数。如果 minValue 等于 maxValue，则返回 minValue。</returns>
            public static int GetRandomInclusive(int minValue, int maxValue)
            {
                return s_Random.Next(minValue, maxValue + 1);
            }

            /// <summary>
            /// 返回一个指定范围内的随机数(包含上下界)。
            /// </summary>
            /// <param name="minValue">返回的随机数的下界（随机数可取该下界值）。</param>
            /// <param name="maxValue">返回的随机数的上界（随机数可取该上界值）。maxValue 必须大于等于 minValue。</param>
            /// <returns>一个大于等于 minValue 且小于 maxValue 的 32 位带符号整数。如果 minValue 等于 maxValue，则返回 minValue。</returns>
            public static long GetRandomInclusive(long minValue, long maxValue)
            {
                return minValue + (long) ((maxValue - minValue) * s_Random.NextDouble());
            }

            /// <summary>
            /// 返回一个指定范围内的随机数。
            /// </summary>
            /// <param name="value1">值1。</param>
            /// <param name="value2">值2</param>
            /// <returns>如果 value1 等于 value2，则返回 value1。</returns>
            public static int GetRandomInRange(int value1, int value2)
            {
                if (value1 > value2)
                {
                    return s_Random.Next(value2, value1);
                }
                else
                {
                    return s_Random.Next(value1, value2);
                }
            }

            /// <summary>
            /// 返回一个指定范围内的随机数。
            /// </summary>
            /// <param name="value1">值1。</param>
            /// <param name="value2">值2</param>
            /// <returns>如果 value1 等于 value2，则返回 value1。</returns>
            public static float GetRandomInRange(float value1, float value2)
            {
                if (value1 > value2)
                {
                    return UnityEngine.Random.Range(value2, value1);
                }
                else
                {
                    return UnityEngine.Random.Range(value1, value2);
                }
            }

            /// <summary>
            /// 返回一个介于 0.0 和 1.0 之间的随机数。
            /// </summary>
            /// <returns>大于等于 0.0 并且小于 1.0 的双精度浮点数。</returns>
            public static double GetRandomDouble()
            {
                return s_Random.NextDouble();
            }

            /// <summary>
            /// 用随机数填充指定字节数组的元素。
            /// </summary>
            /// <param name="buffer">包含随机数的字节数组。</param>
            public static void GetRandomBytes(byte[] buffer)
            {
                s_Random.NextBytes(buffer);
            }

            /// <summary>
            /// 洗牌算法
            /// </summary>
            /// <typeparam name="T">泛型T</typeparam>
            /// <param name="array">数组</param>
            /// <param name="length">数组长度</param>
            public static void RealRandomArray<T>(T[] array, int length)
            {
                for (int i = length - 1; i > 0; i--)
                {
                    var index = s_Random.Next(i + 1); //(int)(MT19937.Real2() * (i + 1));

                    var value = array[i];
                    array[i] = array[index];
                    array[index] = value;
                }
            }

            /// <summary>
            /// 洗牌算法
            /// </summary>
            /// <typeparam name="T">泛型T</typeparam>
            /// <param name="list">数组</param>
            public static void RealRandomList<T>(List<T> list)
            {
                for (int i = list.Count - 1; i > 0; i--)
                {
                    var index = s_Random.Next(i + 1); //(int)(MT19937.Real2() * (i + 1));

                    var value = list[i];
                    list[i] = list[index];
                    list[index] = value;
                }
            }

            /// <summary>
            /// 根据填充方法随机获得填充起点
            /// </summary>
            /// <param name="fillMethod"></param>
            /// <returns></returns>
            public static int GetRandomFillOrigin(Image.FillMethod fillMethod)
            {
                switch (fillMethod)
                {
                    case Image.FillMethod.Horizontal:
                        var o1 = Enum.GetValues(typeof(Image.OriginHorizontal));
                        return (int) o1.GetValue(GetRandom(o1.Length));
                    case Image.FillMethod.Vertical:
                        var o2 = Enum.GetValues(typeof(Image.OriginVertical));
                        return (int) o2.GetValue(GetRandom(o2.Length));
                    case Image.FillMethod.Radial90:
                        var o3 = Enum.GetValues(typeof(Image.Origin90));
                        return (int) o3.GetValue(GetRandom(o3.Length));
                    case Image.FillMethod.Radial180:
                        var o4 = Enum.GetValues(typeof(Image.Origin180));
                        return (int) o4.GetValue(GetRandom(o4.Length));
                    case Image.FillMethod.Radial360:
                        var o5 = Enum.GetValues(typeof(Image.Origin180));
                        return (int) o5.GetValue(GetRandom(o5.Length));
                }

                return 0;
            }

            /// <summary>
            /// 获取枚举随机值
            /// </summary>
            /// <typeparam name="T">枚举类型</typeparam>
            /// <returns>枚举随机值</returns>
            public static T GetRandom<T>()
            {
                var values = Enum.GetValues(typeof(T));

                return (T)values.GetValue(GetRandom(values.Length));
            }

            /// <summary>
            /// 是否满足概率，百分制
            /// </summary>
            /// <param name="value">概率[0-100]</param>
            /// <returns>是否满足</returns>
            public static bool Probability(int value)
            {
                var random = GetRandom(100);

                if (random < value)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            /// <summary>
            /// 是否满足概率，百分制
            /// </summary>
            /// <param name="value">概率[0,1]</param>
            /// <returns>是否满足</returns>
            public static bool Probability(double value)
            {
                var random = GetRandomDouble();

                if (random < value)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            /// <summary>
            /// 重新随机
            /// </summary>
            public static void RandomSequence()
            {
                var l = Sequence.Capacity - Sequence.Count;
                for (int i = 0; i < l; i++)
                {
                    Sequence.Add(0);
                }
                for (var index = 0; index < Sequence.Count; index++)
                {
                    Sequence[index] = GetRandom(100);
                }
            }

            /// <summary>
            /// 取随机数值
            /// </summary>
            /// <param name="index">索引</param>
            /// <returns>随机数</returns>
            public static int GetSequenceValue(int index)
            {
                if (Sequence.Count == 0)
                {
                    RandomSequence();
                }
                return Sequence[index % Sequence.Count];
            }

            /// <summary>
            /// 根据权重获取列表随机值
            /// </summary>
            /// <typeparam name="T">值类型</typeparam>
            /// <param name="list"></param>
            /// <param name="getWeight"></param>
            /// <returns></returns>
            public static T GetRandom<T>(List<T> list, Func<T, int> getWeight)
            {
                var sum = list.Sum((getWeight));
                var random = GetRandom(sum);

                foreach (var t in list)
                {
                    var weight = getWeight.Invoke(t);
                    if (random < weight)
                    {
                        return t;
                    }
                }

                return list.Last();
            }

            /// <summary>
            /// 不重复的一串随机数
            /// </summary>
            readonly static List<int> randomList = new List<int>();

            /// <summary>
            /// 获得一串不重复的随机数
            /// </summary>
            /// <param name="maxValue">最大值 - 取不到</param>
            /// <param name="count">要求数目</param>
            /// <returns>随机数队列</returns>
            public static List<int> GetRandomList(int maxValue, int count)
            {
                randomList.Clear();
                if (count > maxValue || count < 0) return randomList;

                while (randomList.Count != count)
                {
                    var rand = GetRandom(maxValue);
                    if (randomList.Contains(rand) == false)
                    {
                        randomList.Add(rand);
                    }
                }
                return randomList;
            }

            /// <summary>
            /// 获得符合概率的ID
            /// </summary>
            /// <param name="array">权重队列</param>
            /// <param name="rateIdx">随机ID队列</param>
            /// <param name="total">总权重</param>
            /// <returns></returns>
            public static int Probability(Dictionary<int, float> array, List<int> rateIdx, float total)
            {
                var rant = GetRandom(0.0f, total);  //所有权重和
                var temp = 0.0f;
                for (int i = 0; i < rateIdx.Count; i++)
                {
                    var idx = rateIdx[i];    //洗牌后的ID队列
                    var rate = array[idx];  //获得对应权重
                    temp += rate;           //计算概率
                    if (rant < temp)
                    {
                        return idx;
                    }
                }
                return 0;
            }
        }
    }
}
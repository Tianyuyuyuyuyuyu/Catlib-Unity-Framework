using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using Unity.Jobs;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework.Utility.Runtime
{
    /// <summary>
    /// 通用扩展
    /// </summary>
    public static class Extension
    {
        /// <summary>
        /// d2是否是d1的昨天
        /// </summary>
        /// <param name="d1">时间1</param>
        /// <param name="d2">时间2</param>
        /// <returns>是否</returns>
        public static bool IsYesterday(this DateTime d1, DateTime d2)
        {
            if (d1.Year > d2.Year || (d1.Year == d2.Year && d1.DayOfYear > d2.DayOfYear))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// IphoneX 适配
        /// </summary>
        /// <param name="rectTransform">RectTransform</param>
        /// <param name="offset">偏移</param>
        public static void IphoneXAdapt(this RectTransform rectTransform, float offset)
        {
            var rectTransformAnchoredPosition = rectTransform.anchoredPosition;
            rectTransformAnchoredPosition.x += offset;
            rectTransform.anchoredPosition = rectTransformAnchoredPosition;
        }

        /// <summary>
        /// 转16进制
        /// </summary>
        /// <param name="color">颜色</param>
        /// <returns>16进制</returns>
        public static string ToHex(this Color color)
        {
            Color32 color32 = color;
            string r = Convert.ToString(color32.r, 16);
            if (r == "0")
                r = "00";
            string g = Convert.ToString(color32.g, 16);
            if (g == "0")
                g = "00";
            string b = Convert.ToString(color32.b, 16);
            if (b == "0")
                b = "00";
            string a = Convert.ToString(color32.a, 16);
            if (a == "0")
                a = "00";

            return "#" + r.PadLeft(2, '0') + g.PadLeft(2, '0') + b.PadLeft(2, '0') + a.PadLeft(2, '0');
        }

        /// <summary>
        /// 返回随机数
        /// </summary>
        /// <param name="vector2Int">范围</param>
        /// <returns>值</returns>
        public static int Random(this Vector2Int vector2Int)
        {
            return RuntimeUtilities.Random.GetRandom(vector2Int.x, vector2Int.y + 1);
        }

        /// <summary>
        /// 返回随机数
        /// </summary>
        /// <param name="vector2Int">范围</param>
        /// <param name="random">随机值</param>
        /// <returns>值</returns>
        public static int Random(this Vector2Int vector2Int, int random)
        {
            int i = (int) ((vector2Int.y - vector2Int.x) * (random / 100f));
            return vector2Int.x + i;
        }

        /// <summary>
        /// 返回随机数
        /// </summary>
        /// <param name="vector2">范围</param>
        /// <returns>值</returns>
        public static float Random(this Vector2 vector2)
        {
            return RuntimeUtilities.Random.GetRandom(vector2.x, vector2.y);
        }

        /// <summary>
        /// 返回随机数
        /// </summary>
        /// <param name="vector2">范围</param>
        /// <param name="random">随机值</param>
        /// <returns>值</returns>
        public static float Random(this Vector2 vector2, int random)
        {
            var i = (vector2.y - vector2.x) * (random / 100f);
            return vector2.x + i;
        }

        /// <summary>
        /// 追加值
        /// </summary>
        /// <typeparam name="TKey">键值</typeparam>
        /// <param name="dictionary">字典</param>
        /// <param name="key">键值</param>
        /// <param name="value">值</param>
        public static void Append<TKey>(this Dictionary<TKey, long> dictionary, TKey key, long value)
        {
            if (dictionary.ContainsKey(key))
            {
                dictionary[key] += value;
            }
            else
            {
                dictionary.Add(key, value);
            }
        }

        /// <summary>
        /// 追加值
        /// </summary>
        /// <typeparam name="TKey">键值</typeparam>
        /// <param name="dictionary">字典</param>
        /// <param name="key">键值</param>
        /// <param name="value">值</param>
        public static void Append<TKey>(this Dictionary<TKey, int> dictionary, TKey key, int value)
        {
            if (dictionary.ContainsKey(key))
            {
                dictionary[key] += value;
            }
            else
            {
                dictionary.Add(key, value);
            }
        }

        /// <summary>
        /// 追加值
        /// </summary>
        /// <typeparam name="TKey">键值</typeparam>
        /// <typeparam name="TListValue">值</typeparam>
        /// <param name="dictionary">字典</param>
        /// <param name="key">键值</param>
        /// <param name="value">值</param>
        public static void Append<TKey, TListValue>(this Dictionary<TKey, IList<TListValue>> dictionary, TKey key, TListValue value)
        {
            if (dictionary.ContainsKey(key))
            {
                dictionary[key].Add(value);
            }
            else
            {
                dictionary.Add(key, new List<TListValue>() {value});
            }
        }

        /// <summary>
        /// 获取值
        /// </summary>
        /// <typeparam name="TKey">键值</typeparam>
        /// <typeparam name="TValue">值</typeparam>
        /// <param name="dictionary">字典</param>
        /// <param name="key">键值</param>
        public static TValue Get<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
        {
            if (dictionary.ContainsKey(key))
            {
                return dictionary[key];
            }
            else
            {
                return default;
            }
        }

        /// <summary>
        /// 获取列表里的随机值
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="list">列表</param>
        /// <returns>随机值</returns>
        public static T GetRandom<T>(this IList<T> list)
        {
            return list[RuntimeUtilities.Random.GetRandom(list.Count)];
        }

        /// <summary>
        /// 获取列表里的随机值
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="list">列表</param>
        /// <param name="randomValue">随机值</param>
        /// <returns>随机值</returns>
        public static T GetRandom<T>(this IList<T> list, int randomValue)
        {
            return list[randomValue % list.Count];
        }

        /// <summary>
        /// 是否在范围内
        /// </summary>
        /// <param name="range">范围</param>
        /// <param name="value">值</param>
        /// <returns>是否</returns>
        public static bool IsInRange(this Vector2Int range, int value)
        {
            return value >= range.x && value <= range.y;
        }

        /// <summary>
        /// 是否在范围内
        /// </summary>
        /// <param name="range">范围</param>
        /// <param name="value">值</param>
        /// <returns>是否</returns>
        public static bool IsInRange(this Vector2Int range, float value)
        {
            return value >= range.x && value <= range.y;
        }

        /// <summary>
        /// 是否在范围内
        /// </summary>
        /// <param name="range">范围</param>
        /// <param name="value">值</param>
        /// <returns>是否</returns>
        public static bool IsInRange(this Vector2 range, float value)
        {
            return value >= range.x && value <= range.y;
        }

#if ODIN_INSPECTOR
        /// <summary>
        /// 获取枚举的LabelText特性值
        /// </summary>
        /// <param name="value">枚举</param>
        /// <returns>LabelText特性值</returns>
        public static string GetEnumLabel(this Enum value)
        {
            return value.GetType().GetField(value.ToString()).GetCustomAttribute<LabelTextAttribute>()?.Text;
        }
#endif


        public static IEnumerable<TResult> TakeWhileType<TSource, TResult>(this IEnumerable<TSource> source)
        {
            if (source == null)
                return null;
            var takeWhileIterator = TakeWhileIterator<TSource, TResult>(source);
            return takeWhileIterator;
        }

        public static IEnumerable<TResult> TakeWhileIterator<TSource, TResult>(IEnumerable<TSource> source)
        {
            foreach (TSource source1 in source)
            {
                if (source1 is TResult tResult)
                {
                    yield return tResult;
                }
            }
        }


        public static JobHandle Combine(this JobHandle jobHandle, JobHandle otherJobHandle)
        {
            return JobHandle.CombineDependencies(jobHandle, otherJobHandle);
        }

        /// <summary>
        /// 时间转换
        /// </summary>
        /// <param name="value">字符串</param>
        /// <returns>时间</returns>
        public static DateTime DateTimeParseYYYY_MM_DD(this string value)
        {
            return DateTime.ParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Helpful method by Stack Overflow user ata
        /// https://stackoverflow.com/questions/3210393/how-do-i-remove-all-non-alphanumeric-characters-from-a-string-except-dash
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ConvertToAlphanumeric(this string input)
        {
            char[] arr = input.ToCharArray();

            arr = System.Array.FindAll<char>(arr, (c => (char.IsLetterOrDigit(c)
                                                         || c == '_')));

            // If the first index is a number
            while (char.IsDigit(arr[0]))
            {
                List<char> newArray = new List<char>();
                newArray = new List<char>(arr);
                newArray.RemoveAt(0);
                arr = newArray.ToArray();
                if (arr.Length == 0) break; // No valid characters to use, returning empty
            }

            return new string(arr);
        }

        /// <summary>
        /// 转换颜色富文本
        /// </summary>
        /// <param name="value">对象</param>
        /// <param name="color">颜色</param>
        /// <returns>颜色富文本</returns>
        public static string ToColorRichText(this object value, Color color)
        {
            return $"<color={color.ToHex()}>{value}</color>";
        }

#if UNITY_EDITOR
        
        /// <summary>
        /// 获取资源Guid
        /// </summary>
        /// <param name="o">资源</param>
        /// <returns>Guid</returns>
        public static string GetAssetGuid(this Object o)
        {
            return AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(o));
        }
        
        /// <summary>
        /// 将颜色设置进入编辑器Prefs
        /// </summary>
        /// <param name="key">键值</param>
        /// <param name="color">颜色</param>
        public static void SetColor(string key, Color color)
        {
            EditorPrefs.SetFloat(key + ".r", color.r);
            EditorPrefs.SetFloat(key + ".g", color.g);
            EditorPrefs.SetFloat(key + ".b", color.b);
            EditorPrefs.SetFloat(key + ".a", color.a);
        }

        /// <summary>
        /// 将颜色设置进入编辑器Prefs
        /// </summary>
        /// <param name="key">键值</param>
        /// <param name="defaultColor">默认颜色</param>
        /// <returns>颜色</returns>
        public static Color GetColor(string key, Color defaultColor)
        {
            float r = EditorPrefs.GetFloat(key + ".r", defaultColor.r);
            float g = EditorPrefs.GetFloat(key + ".g", defaultColor.g);
            float b = EditorPrefs.GetFloat(key + ".b", defaultColor.b);
            float a = EditorPrefs.GetFloat(key + ".a", defaultColor.a);
            return new Color(r, g, b, a);
        }
#endif
    }
}
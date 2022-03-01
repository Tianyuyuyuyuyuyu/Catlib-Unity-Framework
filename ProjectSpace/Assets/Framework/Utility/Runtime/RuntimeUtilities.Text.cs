using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Framework.Utility.Runtime
{
    public static partial class RuntimeUtilities
    {
        /// <summary>
        /// 字符相关的实用函数。
        /// </summary>
        public static class Text
        {
            [ThreadStatic]
            private static StringBuilder s_CachedStringBuilder = new StringBuilder(1024);

            private static Regex regex2 = new Regex("^(13[0-9]|14[01456879]|15[0-35-9]|16[2567]|17[0-8]|18[0-9]|19[0-35-9])\\d{8}$");
            private static Regex regex1 = new Regex("^((13[0-9])|(14[5,7])|(15[0-3,5-9])|(17[0,3,5-8])|(18[0-9])|166|198|199|(147))\\d{8}$");
            private static Regex regex_18 = new Regex("^([1-6][1-9]|50)\\d{4}(18|19|20)\\d{2}((0[1-9])|10|11|12)(([0-2][1-9])|10|20|30|31)\\d{3}[0-9Xx]$");
            private static Regex regex_15 = new Regex("^([1-6][1-9]|50)\\d{4}\\d{2}((0[1-9])|10|11|12)(([0-2][1-9])|10|20|30|31)\\d{3}$");

            /// <summary>
            /// 获取格式化字符串。
            /// </summary>
            /// <param name="format">字符串格式。</param>
            /// <param name="arg0">字符串参数 0。</param>
            /// <returns>格式化后的字符串。</returns>
            public static string Format(string format, object arg0)
            {
                if (format == null)
                {
                    throw new Exception("Format is invalid.");
                }

                if (s_CachedStringBuilder == null) s_CachedStringBuilder = new StringBuilder(1024);
                s_CachedStringBuilder.Length = 0;
                s_CachedStringBuilder.AppendFormat(format, arg0);
                return s_CachedStringBuilder.ToString();
            }

            /// <summary>
            /// 获取格式化字符串。
            /// </summary>
            /// <param name="format">字符串格式。</param>
            /// <param name="arg0">字符串参数 0。</param>
            /// <param name="arg1">字符串参数 1。</param>
            /// <returns>格式化后的字符串。</returns>
            public static string Format(string format, object arg0, object arg1)
            {
                if (format == null)
                {
                    throw new Exception("Format is invalid.");
                }

                if (s_CachedStringBuilder == null) s_CachedStringBuilder = new StringBuilder(1024);
                s_CachedStringBuilder.Length = 0;
                s_CachedStringBuilder.AppendFormat(format, arg0, arg1);
                return s_CachedStringBuilder.ToString();
            }

            /// <summary>
            /// 获取格式化字符串。
            /// </summary>
            /// <param name="format">字符串格式。</param>
            /// <param name="arg0">字符串参数 0。</param>
            /// <param name="arg1">字符串参数 1。</param>
            /// <param name="arg2">字符串参数 2。</param>
            /// <returns>格式化后的字符串。</returns>
            public static string Format(string format, object arg0, object arg1, object arg2)
            {
                if (format == null)
                {
                    throw new Exception("Format is invalid.");
                }

                if (s_CachedStringBuilder == null) s_CachedStringBuilder = new StringBuilder(1024);
                s_CachedStringBuilder.Length = 0;
                s_CachedStringBuilder.AppendFormat(format, arg0, arg1, arg2);
                return s_CachedStringBuilder.ToString();
            }

            /// <summary>
            /// 获取格式化字符串。
            /// </summary>
            /// <param name="format">字符串格式。</param>
            /// <param name="args">字符串参数。</param>
            /// <returns>格式化后的字符串。</returns>
            public static string Format(string format, params object[] args)
            {
                if (format == null)
                {
                    throw new Exception("Format is invalid.");
                }

                if (args == null)
                {
                    throw new Exception("Args is invalid.");
                }

                if (s_CachedStringBuilder == null) s_CachedStringBuilder = new StringBuilder(1024);
                s_CachedStringBuilder.Length = 0;
                s_CachedStringBuilder.AppendFormat(format, args);
                return s_CachedStringBuilder.ToString();
            }

            /// <summary>
            /// 将文本按行切分。
            /// </summary>
            /// <param name="text">要切分的文本。</param>
            /// <returns>按行切分后的文本。</returns>
            public static string[] SplitToLines(string text)
            {
                List<string> texts = new List<string>();
                int position = 0;
                string rowText;
                while ((rowText = ReadLine(text, ref position)) != null)
                {
                    texts.Add(rowText);
                }

                return texts.ToArray();
            }

            /// <summary>
            /// 根据类型和名称获取完整名称。
            /// </summary>
            /// <typeparam name="T">类型。</typeparam>
            /// <param name="name">名称。</param>
            /// <returns>完整名称。</returns>
            public static string GetFullName<T>(string name)
            {
                return GetFullName(typeof(T), name);
            }

            /// <summary>
            /// 根据类型和名称获取完整名称。
            /// </summary>
            /// <param name="type">类型。</param>
            /// <param name="name">名称。</param>
            /// <returns>完整名称。</returns>
            public static string GetFullName(Type type, string name)
            {
                if (type == null)
                {
                    throw new Exception("Type is invalid.");
                }

                string typeName = type.FullName;
                return string.IsNullOrEmpty(name) ? typeName : RuntimeUtilities.Text.Format("{0}.{1}", typeName, name);
            }

            /// <summary>
            /// 获取用于编辑器显示的名称。
            /// </summary>
            /// <param name="fieldName">字段名称。</param>
            /// <returns>编辑器显示名称。</returns>
            public static string FieldNameForDisplay(string fieldName)
            {
                if (string.IsNullOrEmpty(fieldName))
                {
                    return string.Empty;
                }

                string str = Regex.Replace(fieldName, @"^m_", string.Empty);
                str = Regex.Replace(str, @"((?<=[a-z])[A-Z]|[A-Z](?=[a-z]))", @" $1").TrimStart();
                return str;
            }

            /// <summary>
            /// 格式化数字
            /// </summary>
            /// <param name="arg0">字符串参数 0。</param>
            /// <returns>格式化后字符串</returns>
            public static string FormatNumber(long? arg0)
            {
                return arg0?.ToString("N0");
            }

            /// <summary>
            /// 格式化数字
            /// </summary>
            /// <param name="format">字符串格式。</param>
            /// <param name="arg0">字符串参数 0。</param>
            /// <returns>格式化后字符串</returns>
            public static string FormatNumber(string format, long? arg0)
            {
                return Format(format, arg0?.ToString("N0"));
            }

            private static string ReadLine(string text, ref int position)
            {
                if (text == null)
                {
                    return null;
                }

                int length = text.Length;
                int offset = position;
                while (offset < length)
                {
                    char ch = text[offset];
                    switch (ch)
                    {
                        case '\r':
                        case '\n':
                            string str = text.Substring(position, offset - position);
                            position = offset + 1;
                            if (((ch == '\r') && (position < length)) && (text[position] == '\n'))
                            {
                                position++;
                            }

                            return str;
                        default:
                            offset++;
                            break;
                    }
                }

                if (offset > position)
                {
                    string str = text.Substring(position, offset - position);
                    position = offset;
                    return str;
                }

                return null;
            }

            /// <summary>
            /// 验证手机格式 - 第二版
            /// </summary>
            /// <param name="telephone"></param>
            /// <returns></returns>
            public static bool RegexPhone(string telephone)
            {
                return regex2.IsMatch(telephone);
            }

            /// <summary>
            /// 手机号验证 - 第一版
            /// </summary>
            /// <param name="telephone"></param>
            /// <returns></returns>
            public static bool RegexPhone_(string telephone)
            {
                return regex1.IsMatch(telephone);
            }

            /// <summary>
            /// 身份证验证
            /// </summary>
            /// <param name="idCard"></param>
            /// <returns></returns>
            public static bool RegexIdCard(string idCard)
            {
                return regex_15.IsMatch(idCard) || regex_18.IsMatch(idCard);
            }
        }
    }
}

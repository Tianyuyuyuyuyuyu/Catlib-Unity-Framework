using System;
using System.Security.Cryptography;
using System.Text;

namespace Framework.Utility.Runtime
{
    public static partial class RuntimeUtilities
    {
        public static class MD5
        {
            /// <summary>
            /// MD5
            /// </summary>
            private static readonly System.Security.Cryptography.MD5 Md5;

            /// <summary>
            ///   初始化 <see cref="T:System.Object" /> 类的新实例。
            /// </summary>
            static MD5()
            {
                if (Md5 == null)
                {
                    Md5 = new MD5CryptoServiceProvider();
                }
            }

            /// <summary>
            /// 加密
            /// </summary>
            /// <param name="content">内容</param>
            /// <returns>密文</returns>
            public static string Encrypt(string content)
            {
                var computeHash = Md5.ComputeHash(Encoding.UTF8.GetBytes(content));

                return BitConverter.ToString(computeHash).Replace("-", "").ToLower();
            }

            /// <summary>
            /// 加密 - 32位
            /// </summary>
            /// <param name="content">内容</param>
            /// <returns></returns>
            public static string Encrypt32(string content)
            {
                var computeHash = Md5.ComputeHash(Encoding.UTF8.GetBytes(content));

                string hashString = "";
                for (int i = 0; i < computeHash.Length; i++)
                {
                    hashString += Convert.ToString(computeHash[i], 16).PadLeft(2, '0');
                }
                return hashString.PadLeft(32, '0');
            }
        }
    }
}

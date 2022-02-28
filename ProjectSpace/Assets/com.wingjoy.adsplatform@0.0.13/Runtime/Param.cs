using System.Collections.Generic;

namespace AdsPlatform.Runtime
{
    public class Param
    {
        /// <summary>
        /// 参数
        /// </summary>
        private Dictionary<string, object> Params;

        public Param()
        {
            Params = new Dictionary<string, object>();
        }

        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="key">参数键值</param>
        /// <param name="value">参数</param>
        public void SetPara(string key, object value)
        {
            Params.Add(key, value);
        }

        /// <summary>
        /// 获取参数
        /// </summary>
        /// <typeparam name="T">参数类型</typeparam>
        /// <param name="key">参数键值</param>
        /// <returns>参数</returns>
        public T GetPara<T>(string key)
        {
            if (Params.TryGetValue(key, out var value))
            {
                return (T) value;
            }
            else
            {
                return default;
            }
        }

        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="key">键值</param>
        /// <returns>是否存在</returns>
        public bool Exist(string key)
        {
            return Params.ContainsKey(key);
        }

        /// <summary>
        /// 尝试获取参数
        /// </summary>
        /// <typeparam name="T">参数类型</typeparam>
        /// <param name="key">键值</param>
        /// <param name="value">参数</param>
        /// <returns>是否存在</returns>
        public bool TryGetPara<T>(string key, out T value)
        {
            if (Params.TryGetValue(key, out var para))
            {
                value = (T) para;
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }
    }
}
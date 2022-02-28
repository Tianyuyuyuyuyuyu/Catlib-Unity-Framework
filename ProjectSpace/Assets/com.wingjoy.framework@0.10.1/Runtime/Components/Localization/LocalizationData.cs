using System;
using System.Collections.Generic;

namespace Wingjoy.Framework.Runtime.Localization
{
    public class LocalizationData<T>
    {
        /// <summary>
        /// 子项数据
        /// </summary>
        public List<LocalizationSubData<T>> LocalizationSubDataList;

        public LocalizationData()
        {
            LocalizationSubDataList = new List<LocalizationSubData<T>>();
        }
    }

    [Serializable]
    public class LocalizationSubData<T>
    {
        /// <summary>
        /// 语言
        /// </summary>
        public Language Language;
        /// <summary>
        /// 内容
        /// </summary>
        public T Value;
    }
}
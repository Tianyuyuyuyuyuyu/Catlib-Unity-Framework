using System.Collections.Generic;
using UnityEngine;

namespace Wingjoy.Framework.Runtime
{
    [SerializeField]
    public class BuiltInBundle
    {
        /// <summary>
        /// Bundle路径
        /// </summary>
        [SerializeField]
        private List<string> m_BundleNames = new List<string>();
        
        //
        // /// <summary>
        // /// 模拟模式下Bundle路径
        // /// </summary>
        // [SerializeField]
        // private List<string> m_SimGroupBundleNames = new List<string>();

        public List<string> BundleNames
        {
            get => m_BundleNames;
            set => m_BundleNames = value;
        }

        // public List<string> SimGroupBundleNames
        // {
        //     get => m_SimGroupBundleNames;
        //     set => m_SimGroupBundleNames = value;
        // }
    }
}
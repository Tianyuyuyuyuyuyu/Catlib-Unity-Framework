using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Wingjoy.Framework.Runtime.UI
{
    public class UIInChildrenField : UIField
    {
        /// <summary>
        /// 获取组件列表
        /// </summary>
        /// <returns>组件列表</returns>
        public override List<Component> GetList()
        {
            return GetComponentsInChildren<Component>(true).ToList();
        }
    }
}
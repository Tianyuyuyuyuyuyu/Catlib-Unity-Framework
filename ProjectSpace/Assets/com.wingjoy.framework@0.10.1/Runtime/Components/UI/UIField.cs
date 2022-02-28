using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using UnityEngine;

namespace Wingjoy.Framework.Runtime.UI
{
    public class UIField : MonoBehaviour
    {
        /// <summary>
        /// 忽略RectTransform
        /// </summary>
        [LabelText("忽略检索RectTransform")]
        public bool IgnoreRectTransform;
        /// <summary>
        /// 目标组件
        /// </summary>
        [ValueDropdown("GetComponentsList", IsUniqueList = true)]
        public List<Component> Components;

        /// <summary>
        /// 获取组件列表
        /// </summary>
        /// <returns>组件列表</returns>
        public virtual List<Component> GetList()
        {
            return GetComponents<Component>().ToList();
        }

        /// <summary>
        /// 获取当前物体所挂载的可本地化组件
        /// </summary>
        /// <returns>可本地化组件列表</returns>
        public IEnumerable GetComponentsList()
        {
            var components = GetList();
            
            var enumerable = components
                .Where((component => component.GetType() != typeof(UIField) && component.GetType() != typeof(UIInChildrenField)))
                .Where((component => component.GetType() != typeof(CanvasRenderer)));
            
            if (IgnoreRectTransform)
            {
                enumerable = enumerable.Where((component => component.GetType() != typeof(RectTransform)));
            }

            var path = Utility.Path.GetPath(transform).Replace(transform.name, "");
            return enumerable.Select((component => new ValueDropdownItem($"{Utility.Path.GetPath(component.transform).Replace(path, "")} {component.GetType().GetNiceName()} ID:{component.GetInstanceID()}", component)));
        }
    }
}
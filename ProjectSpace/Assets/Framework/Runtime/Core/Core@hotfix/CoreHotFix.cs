using System;
using UnityEngine;
using Framework.Runtime.Base;
using Framework.Runtime.Core;
using Framework.Runtime.HotFix.UI;
using Framework.Utility.Runtime;

namespace Framework.Runtime.HotFix
{
    public class CoreHotFix
    {
        /// <summary>
        /// UI组件
        /// </summary>
        public static UIComponent UI { get; private set; }

        /// <summary>
        /// 整个游戏的启动器
        /// </summary>
        /// <param name="frameworkMono">框架</param>
        /// <param name="coreType">核心类型</param>
        /// <param name="mainProjectTypes">游戏逻辑域传过来的所有type</param>
        /// <param name="hotfixTypes">热更所有type</param>
        public static void StartGame(FrameworkMono frameworkMono, Type coreType, Type[] mainProjectTypes = null, Type[] hotfixTypes = null)
        {
            Debug.Log("StartGame");
        
            foreach (var hotfixType in hotfixTypes)
            {
                if (hotfixType != null)
                {
                    if (hotfixType.BaseType != null && hotfixType.BaseType.FullName != null)
                    {
                        if (hotfixType.BaseType.FullName.Contains(".FrameworkHotFixComponent"))
                        {
                            var componentConfigAttribute = hotfixType.GetAttributeInILRuntime<ComponentConfigAttribute>();
                            ComponentConfig componentConfig = null;
                            if (componentConfigAttribute != null)
                            {
                                Debug.Log("找到ComponentConfig特性，获取配置" + componentConfigAttribute.ConfigType);
                                componentConfig = ComponentConfig.GetComponentConfig(componentConfigAttribute.ConfigType.ToString());
                            }
        
                            if (componentConfig != null)
                            {
                                Debug.Log("创建带参组件" + hotfixType.ToString());
                                Activator.CreateInstance(hotfixType, new object[] {componentConfig});
                            }
                            else
                            {
                                Debug.Log("创建无参组件" + hotfixType.ToString());
                                Activator.CreateInstance(hotfixType);
                            }
                        }
                    }
        
                    if (hotfixType.FullName == coreType.FullName)
                    {
                        coreType = hotfixType;
                    }
                }
            }
        
            Debug.Log(coreType.Name);
        
            var o = Activator.CreateInstance(coreType);
            if (o != null)
            {
                coreType.GetMethod("InitComponents").Invoke(o, null);
            }
        }

        /// <summary>
        /// 初始化内置组件
        /// </summary>
        public virtual void InitComponents()
        {
            UI = (UIComponent) FrameworkHotFixComponent.GetFrameworkComponent(typeof(UIComponent));
            CoreMain.RegisterSendUIMessage(UI.SendUIMessage);
        }
    }
}
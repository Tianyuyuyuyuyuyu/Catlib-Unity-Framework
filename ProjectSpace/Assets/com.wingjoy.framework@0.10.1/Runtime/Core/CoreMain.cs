using System;
using Wingjoy.Framework.Runtime.Ads;
using Wingjoy.Framework.Runtime.Audio;
using Wingjoy.Framework.Runtime.Localization;
using Wingjoy.Framework.Runtime.Save;
using Wingjoy.Framework.Runtime.Setting;
using Wingjoy.Framework.Runtime.Splash;

namespace Wingjoy.Framework.Runtime
{
    public class CoreMain
    {
        /// <summary>
        /// 广告组件
        /// </summary>
        public static AdsComponent Ads { get; private set; }
        
        /// <summary>
        /// 音频组件
        /// </summary>
        public static AudioComponent Audio { get; private set; }

        /// <summary>
        /// 闪屏
        /// </summary>
        public static SplashComponent Splash { get; private set; }

        /// <summary>
        /// 本地化
        /// </summary>
        public static LocalizationComponent Loc { get; private set; }

        /// <summary>
        /// 存档
        /// </summary>
        public static SaveComponent Save { get; private set; }

        /// <summary>
        /// 设置
        /// </summary>
        public static SettingComponent Setting { get; private set; }

        /// <summary>
        /// 发送UI信息
        /// </summary>
        private static Action<string, object> m_SendUIMessage;

        /// <summary>
        /// 初始化内置组件
        /// </summary>
        public virtual void InitComponents()
        {
            Ads = (AdsComponent) WingjoyFrameworkComponent.GetFrameworkComponent(typeof(AdsComponent));
            Audio = (AudioComponent) WingjoyFrameworkComponent.GetFrameworkComponent(typeof(AudioComponent));
            Splash = (SplashComponent) WingjoyFrameworkComponent.GetFrameworkComponent(typeof(SplashComponent));
            Loc = (LocalizationComponent) WingjoyFrameworkComponent.GetFrameworkComponent(typeof(LocalizationComponent));
            Save = (SaveComponent) WingjoyFrameworkComponent.GetFrameworkComponent(typeof(SaveComponent));
            Setting = (SettingComponent) WingjoyFrameworkComponent.GetFrameworkComponent(typeof(SettingComponent));
        }

        /// <summary>
        /// 发送UI信息
        /// </summary>
        /// <param name="messageKey">消息键值</param>
        /// <param name="userData">自定义数据</param>
        public static void SendUIMessage(string messageKey, object userData = null)
        {
            m_SendUIMessage?.Invoke(messageKey, userData);
        }

        /// <summary>
        /// 注册发送UI信息逻辑
        /// </summary>
        /// <param name="action">逻辑</param>
        public static void RegisterSendUIMessage(Action<string, object> action)
        {
            m_SendUIMessage += action;
        }
    }
}
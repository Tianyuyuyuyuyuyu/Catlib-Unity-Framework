using System;
using Framework.Runtime.Ads;
using Framework.Runtime.Audio;
using Framework.Runtime.Localization;
using Framework.Runtime.Save;
using Framework.Runtime.Setting;
using Framework.Runtime.Splash;

namespace Framework.Runtime.Core
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
            Ads = (AdsComponent) FrameworkComponent.GetFrameworkComponent(typeof(AdsComponent));
            Audio = (AudioComponent) FrameworkComponent.GetFrameworkComponent(typeof(AudioComponent));
            Splash = (SplashComponent) FrameworkComponent.GetFrameworkComponent(typeof(SplashComponent));
            Loc = (LocalizationComponent) FrameworkComponent.GetFrameworkComponent(typeof(LocalizationComponent));
            Save = (SaveComponent) FrameworkComponent.GetFrameworkComponent(typeof(SaveComponent));
            Setting = (SettingComponent) FrameworkComponent.GetFrameworkComponent(typeof(SettingComponent));
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
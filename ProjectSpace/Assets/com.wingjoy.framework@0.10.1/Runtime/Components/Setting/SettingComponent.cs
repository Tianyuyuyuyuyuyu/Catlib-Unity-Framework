using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Wingjoy.Framework.Runtime;
using WingjoyUtility.Runtime;

namespace Wingjoy.Framework.Runtime.Setting
{
    [DisallowMultipleComponent]
    public class SettingComponent : WingjoyFrameworkComponent
    {
        /// <summary>
        /// 游戏暂停前的速度
        /// </summary>
        private float m_GameSpeedBeforePause = 1f;

        [SerializeField, Range(1, 120), OnValueChanged("OnFrameRateChanged")]
        private int m_FrameRate = 30;

        [SerializeField, Range(0, 8), OnValueChanged("OnGameSpeedChanged")]
        private float m_GameSpeed = 1f;

        [SerializeField]
        private bool m_RunInBackground = true;

        [SerializeField]
        private bool m_NeverSleep = true;

        [SerializeField]
        private int m_SyncCount = 0;

        [SerializeField]
        private int m_AntiAliasing = 4;

        [SerializeField, EnumToggleButtons]
        private ShadowQuality m_ShadowQuality = ShadowQuality.All;

        /// <summary>
        /// 获取或设置游戏帧率。
        /// </summary>
        public int FrameRate
        {
            get
            {
                return m_FrameRate;
            }
            set
            {
                Application.targetFrameRate = m_FrameRate = value;
            }
        }

        /// <summary>
        /// 获取或设置游戏速度。
        /// </summary>
        public float GameSpeed
        {
            get
            {
                return m_GameSpeed;
            }
            set
            {
                Time.timeScale = m_GameSpeed = (value >= 0f ? value : 0f);
            }
        }

        /// <summary>
        /// 获取游戏是否暂停。
        /// </summary>
        public bool IsGamePaused
        {
            get
            {
                return m_GameSpeed <= 0f;
            }
        }

        /// <summary>
        /// 获取是否正常游戏速度。
        /// </summary>
        public bool IsNormalGameSpeed
        {
            get
            {
                return m_GameSpeed == 1f;
            }
        }

        /// <summary>
        /// 获取或设置是否允许后台运行。
        /// </summary>
        public bool RunInBackground
        {
            get
            {
                return m_RunInBackground;
            }
            set
            {
                Application.runInBackground = m_RunInBackground = value;
            }
        }

        /// <summary>
        /// 获取或设置是否禁止休眠。
        /// </summary>
        public bool NeverSleep
        {
            get
            {
                return m_NeverSleep;
            }
            set
            {
                m_NeverSleep = value;
                Screen.sleepTimeout = value ? SleepTimeout.NeverSleep : SleepTimeout.SystemSetting;
            }
        }

        /// <summary>
        /// 获取或设置垂直同步
        /// </summary>
        public int SyncCount
        {
            get
            {
                return m_SyncCount;
            }
            set
            {
                QualitySettings.vSyncCount = m_SyncCount = value;
            }
        }

        /// <summary>
        /// 获取或设置抗锯齿
        /// </summary>
        public int AntiAliasing
        {
            get
            {
                return m_AntiAliasing;
            }
            set
            {
                QualitySettings.antiAliasing = m_AntiAliasing = value;
            }
        }

        /// <summary>
        /// 获取或设置阴影质量
        /// </summary>
        public ShadowQuality ShadowQuality
        {
            get
            {
                return m_ShadowQuality;
            }
            set
            {
                QualitySettings.shadows = m_ShadowQuality = value;
            }
        }

        /// <summary>
        /// 游戏框架组件抽象类。
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            //基础设置
            Application.targetFrameRate = m_FrameRate;
            Time.timeScale = m_GameSpeed;
            Application.runInBackground = m_RunInBackground;
            Screen.sleepTimeout = m_NeverSleep ? SleepTimeout.NeverSleep : SleepTimeout.SystemSetting;
            QualitySettings.antiAliasing = m_AntiAliasing;
            QualitySettings.vSyncCount = m_SyncCount;
            QualitySettings.shadows = m_ShadowQuality;
        }

        /// <summary>
        /// 暂停游戏。
        /// </summary>
        public void PauseGame()
        {
            if (IsGamePaused)
            {
                return;
            }

            m_GameSpeedBeforePause = GameSpeed;
            GameSpeed = 0f;
        }

        /// <summary>
        /// 恢复游戏。
        /// </summary>
        public void ResumeGame()
        {
            if (!IsGamePaused)
            {
                return;
            }

            GameSpeed = m_GameSpeedBeforePause;
        }

        /// <summary>
        /// 重置为正常游戏速度。
        /// </summary>
        public void ResetNormalGameSpeed()
        {
            if (IsNormalGameSpeed)
            {
                return;
            }

            GameSpeed = 1f;
        }

        /// <summary>
        /// 游戏目标帧率改变
        /// </summary>
        public void OnFrameRateChanged()
        {
            Application.targetFrameRate = m_FrameRate;
        }

        /// <summary>
        /// 游戏速度改变
        /// </summary>
        public void OnGameSpeedChanged()
        {
            Time.timeScale = m_GameSpeed = (m_GameSpeed >= 0f ? m_GameSpeed : 0f);
        }

        /// <summary>
        /// 保存配置。
        /// </summary>
        /// <returns>是否保存配置成功。</returns>
        public bool Save()
        {
            PlayerPrefs.Save();
            return true;
        }

        /// <summary>
        /// 检查是否存在指定配置项。
        /// </summary>
        /// <param name="settingName">要检查配置项的名称。</param>
        /// <returns>指定的配置项是否存在。</returns>
        public bool HasSetting(string settingName)
        {
            return PlayerPrefs.HasKey(settingName);
        }

        /// <summary>
        /// 移除指定配置项。
        /// </summary>
        /// <param name="settingName">要移除配置项的名称。</param>
        public void RemoveSetting(string settingName)
        {
            PlayerPrefs.DeleteKey(settingName);
        }

        /// <summary>
        /// 清空所有配置项。
        /// </summary>
        [Button(ButtonSizes.Medium)]
        public void RemoveAllSettings()
        {
            PlayerPrefs.DeleteAll();
        }

        /// <summary>
        /// 从指定配置项中读取布尔值。
        /// </summary>
        /// <param name="settingName">要获取配置项的名称。</param>
        /// <returns>读取的布尔值。</returns>
        public bool GetBool(string settingName)
        {
            return PlayerPrefs.GetInt(settingName) != 0;
        }

        /// <summary>
        /// 从指定配置项中读取布尔值。
        /// </summary>
        /// <param name="settingName">要获取配置项的名称。</param>
        /// <param name="defaultValue">当指定的配置项不存在时，返回此默认值。</param>
        /// <returns>读取的布尔值。</returns>
        public bool GetBool(string settingName, bool defaultValue)
        {
            return PlayerPrefs.GetInt(settingName, defaultValue ? 1 : 0) != 0;
        }

        /// <summary>
        /// 向指定配置项写入布尔值。
        /// </summary>
        /// <param name="settingName">要写入配置项的名称。</param>
        /// <param name="value">要写入的布尔值。</param>
        public void SetBool(string settingName, bool value)
        {
            PlayerPrefs.SetInt(settingName, value ? 1 : 0);
        }

        /// <summary>
        /// 从指定配置项中读取整数值。
        /// </summary>
        /// <param name="settingName">要获取配置项的名称。</param>
        /// <returns>读取的整数值。</returns>
        public int GetInt(string settingName)
        {
            return PlayerPrefs.GetInt(settingName);
        }

        /// <summary>
        /// 从指定配置项中读取整数值。
        /// </summary>
        /// <param name="settingName">要获取配置项的名称。</param>
        /// <param name="defaultValue">当指定的配置项不存在时，返回此默认值。</param>
        /// <returns>读取的整数值。</returns>
        public int GetInt(string settingName, int defaultValue)
        {
            return PlayerPrefs.GetInt(settingName, defaultValue);
        }

        /// <summary>
        /// 向指定配置项写入整数值。
        /// </summary>
        /// <param name="settingName">要写入配置项的名称。</param>
        /// <param name="value">要写入的整数值。</param>
        public void SetInt(string settingName, int value)
        {
            PlayerPrefs.SetInt(settingName, value);
        }

        /// <summary>
        /// 从指定配置项中读取浮点数值。
        /// </summary>
        /// <param name="settingName">要获取配置项的名称。</param>
        /// <returns>读取的浮点数值。</returns>
        public float GetFloat(string settingName)
        {
            return PlayerPrefs.GetFloat(settingName);
        }

        /// <summary>
        /// 从指定配置项中读取浮点数值。
        /// </summary>
        /// <param name="settingName">要获取配置项的名称。</param>
        /// <param name="defaultValue">当指定的配置项不存在时，返回此默认值。</param>
        /// <returns>读取的浮点数值。</returns>
        public float GetFloat(string settingName, float defaultValue)
        {
            return PlayerPrefs.GetFloat(settingName, defaultValue);
        }

        /// <summary>
        /// 向指定配置项写入浮点数值。
        /// </summary>
        /// <param name="settingName">要写入配置项的名称。</param>
        /// <param name="value">要写入的浮点数值。</param>
        public void SetFloat(string settingName, float value)
        {
            PlayerPrefs.SetFloat(settingName, value);
        }

        /// <summary>
        /// 从指定配置项中读取字符串值。
        /// </summary>
        /// <param name="settingName">要获取配置项的名称。</param>
        /// <returns>读取的字符串值。</returns>
        public string GetString(string settingName)
        {
            return PlayerPrefs.GetString(settingName);
        }

        /// <summary>
        /// 从指定配置项中读取字符串值。
        /// </summary>
        /// <param name="settingName">要获取配置项的名称。</param>
        /// <param name="defaultValue">当指定的配置项不存在时，返回此默认值。</param>
        /// <returns>读取的字符串值。</returns>
        public string GetString(string settingName, string defaultValue)
        {
            return PlayerPrefs.GetString(settingName, defaultValue);
        }

        /// <summary>
        /// 向指定配置项写入字符串值。
        /// </summary>
        /// <param name="settingName">要写入配置项的名称。</param>
        /// <param name="value">要写入的字符串值。</param>
        public void SetString(string settingName, string value)
        {
            PlayerPrefs.SetString(settingName, value);
        }

        /// <summary>
        /// 从指定配置项中读取对象。
        /// </summary>
        /// <typeparam name="T">要读取对象的类型。</typeparam>
        /// <param name="settingName">要获取配置项的名称。</param>
        /// <returns>读取的对象。</returns>
        public T GetObject<T>(string settingName)
        {
            return RuntimeUtilities.Json.ToObject<T>(PlayerPrefs.GetString(settingName));
        }

        /// <summary>
        /// 从指定配置项中读取对象。
        /// </summary>
        /// <param name="objectType">要读取对象的类型。</param>
        /// <param name="settingName">要获取配置项的名称。</param>
        /// <returns></returns>
        public object GetObject(Type objectType, string settingName)
        {
            return RuntimeUtilities.Json.ToObject(objectType, PlayerPrefs.GetString(settingName));
        }

        /// <summary>
        /// 从指定配置项中读取对象。
        /// </summary>
        /// <typeparam name="T">要读取对象的类型。</typeparam>
        /// <param name="settingName">要获取配置项的名称。</param>
        /// <param name="defaultObj">当指定的配置项不存在时，返回此默认对象。</param>
        /// <returns>读取的对象。</returns>
        public T GetObject<T>(string settingName, T defaultObj)
        {
            string json = PlayerPrefs.GetString(settingName, null);
            if (json == null)
            {
                return defaultObj;
            }

            return RuntimeUtilities.Json.ToObject<T>(json);
        }

        /// <summary>
        /// 从指定配置项中读取对象。
        /// </summary>
        /// <param name="objectType">要读取对象的类型。</param>
        /// <param name="settingName">要获取配置项的名称。</param>
        /// <param name="defaultObj">当指定的配置项不存在时，返回此默认对象。</param>
        /// <returns></returns>
        public object GetObject(Type objectType, string settingName, object defaultObj)
        {
            string json = PlayerPrefs.GetString(settingName, null);
            if (json == null)
            {
                return defaultObj;
            }

            return RuntimeUtilities.Json.ToObject(objectType, json);
        }

        /// <summary>
        /// 向指定配置项写入对象。
        /// </summary>
        /// <typeparam name="T">要写入对象的类型。</typeparam>
        /// <param name="settingName">要写入配置项的名称。</param>
        /// <param name="obj">要写入的对象。</param>
        public void SetObject<T>(string settingName, T obj)
        {
            PlayerPrefs.SetString(settingName, RuntimeUtilities.Json.ToJson(obj));
        }

        /// <summary>
        /// 向指定配置项写入对象。
        /// </summary>
        /// <param name="settingName">要写入配置项的名称。</param>
        /// <param name="obj">要写入的对象。</param>
        public void SetObject(string settingName, object obj)
        {
            PlayerPrefs.SetString(settingName, RuntimeUtilities.Json.ToJson(obj));
        }
    }
}
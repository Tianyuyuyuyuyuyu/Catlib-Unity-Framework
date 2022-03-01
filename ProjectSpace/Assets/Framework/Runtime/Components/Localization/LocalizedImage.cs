using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace Framework.Runtime.Localization
{
    public class LocalizedImage : Image, ILocalizedCom
    {
        /// <summary>
        /// 本地化使能
        /// </summary>
        [SerializeField]
        public bool EnableImage = true;
        /// <summary>
        /// 本地化键值
        /// </summary>
        [SerializeField]
        public string LocalizationKey;
        /// <summary>
        /// 选中要显示的语言
        /// </summary>
        [SerializeField]
        private Language m_SelectDisplayLanguage;
        /// <summary>
        /// 原始数据
        /// </summary>
        private ImageData m_OriginalImageData;
        /// <summary>
        /// 文本本地化数据
        /// </summary>
        [SerializeField]
        public List<ImageData> LocalizationData = new List<ImageData>();

        protected override void Awake()
        {
            m_OriginalImageData = new ImageData();
            m_OriginalImageData.Color = color;
            
            if (Application.isPlaying)
            {
                FrameworkComponent.GetFrameworkComponent<LocalizationComponent>().RegisterLocalizedCom(this);
                DoLocalize();
            }
        }

        protected override void OnDestroy()
        {
            if (Application.isPlaying)
            {
                FrameworkComponent.GetFrameworkComponent<LocalizationComponent>().RemoveLocalizedCom(this);
            }
        }

        /// <summary>
        /// 生成本地化KEY值
        /// </summary>
        [Button(ButtonSizes.Large)]
        public void GenerateLocalizationKey()
        {
            LocalizationKey = GetPath(transform).Replace("/Canvas (Environment)", "").TrimStart('/');
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }

        /// <summary>
        /// 获取Transform路径
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <returns>Transform路径</returns>
        private static string GetPath(Transform transform)
        {
            return transform ? GetPath(transform.parent) + "/" + transform.gameObject.name : "";
        }

        /// <summary>
        /// 是否使能
        /// </summary>
        public bool EnableLocalization => EnableImage;

        /// <summary>
        /// 本地化键值
        /// </summary>
        public string GetLocalizationKey()
        {
            return LocalizationKey;
        }

        /// <summary>
        /// 本地化内容
        /// </summary>
        public string GetContent()
        {
#if UNITY_EDITOR
            if (sprite != null)
            {
                return AssetDatabase.GetAssetPath(sprite);
            }
            else
#endif
            {
                return "";
            }
        }

        /// <summary>
        /// 本地化
        /// </summary>
        public void DoLocalize()
        {
            if (EnableImage)
            {
                var localizationComponent = FrameworkComponent.GetFrameworkComponent<LocalizationComponent>();
                Addressables.LoadAssetAsync<Sprite>(localizationComponent.GS(LocalizationKey)).Completed += handle =>
                {
                    sprite = handle.Result;
                };
            }
        }

        [Serializable]
        public struct ImageData
        {
            /// <summary>
            /// 语言
            /// </summary>
            public Language Language;
            /// <summary>
            /// 使能字体大小
            /// </summary>
            public Color Color;
        }
    }
}

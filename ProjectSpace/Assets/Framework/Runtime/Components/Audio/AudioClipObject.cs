using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Framework.Utility.Runtime;

namespace Framework.Runtime.Audio
{
    public class AudioClipObject : ScriptableObject
    {
        /// <summary>
        /// 音频资源引用
        /// </summary>
        [SerializeField]
        private AssetReferenceAudioClip m_ReferenceAudioClip;


        public AssetReferenceAudioClip ReferenceAudioClip
        {
            get => m_ReferenceAudioClip;
            set => m_ReferenceAudioClip = value;
        }

        public string SafeName
        {
            get
            {
                return name.ConvertToAlphanumeric();
            }
        }

        /// <summary>
        /// 获取音频资源
        /// </summary>
        /// <param name="onLoad">加载完成回调</param>
        public AsyncOperationHandle<AudioClip> LoadClip(Action<AudioClip> onLoad)
        {
            AsyncOperationHandle<AudioClip> asyncOperationHandle = Addressables.LoadAssetAsync<AudioClip>(m_ReferenceAudioClip);
            asyncOperationHandle.Completed += handle =>
            {
                onLoad?.Invoke(handle.Result);
            };
            return asyncOperationHandle;
            // if (m_ReferenceAudioClip.IsValid())
            // {
            //     if (m_ReferenceAudioClip.OperationHandle.IsDone)
            //     {
            //         onLoad?.Invoke(m_ReferenceAudioClip.Asset as AudioClip);    
            //     }
            //     else
            //     {
            //         m_ReferenceAudioClip.OperationHandle.Completed += handle =>
            //         {
            //             onLoad?.Invoke(handle.Result as AudioClip);
            //         };
            //     }
            // }
            // else
            // {
            //     m_ReferenceAudioClip.LoadAssetAsync<AudioClip>().Completed += handle =>
            //     {
            //         onLoad?.Invoke(handle.Result);
            //     };
            // }
        }
    }
}
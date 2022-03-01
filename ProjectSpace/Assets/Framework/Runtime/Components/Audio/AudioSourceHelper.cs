using System;
using System.Collections;
using Framework.Runtime.Core;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Framework.Runtime.Audio
{
    public class AudioSourceHelper : MonoBehaviour
    {
        /// <summary>
        /// 音频文件
        /// </summary>
        private AudioClip m_AudioClip;

        /// <summary>
        /// 音频文件对象
        /// </summary>
        private AudioClipObject m_AudioClipObject;

        /// <summary>
        /// 音频源
        /// </summary>
        private AudioSource m_AudioSource;

        /// <summary>
        /// 加载音频资源句柄
        /// </summary>
        private AsyncOperationHandle<AudioClip> m_LoadClipHandle;
        public AudioClipObject AudioClipObject => m_AudioClipObject;

        private AudioSource AudioSource
        {
            get
            {
                if (m_AudioSource == null)
                {
                    m_AudioSource = GetComponent<AudioSource>();
                }

                return m_AudioSource;
            }
        }

        /// <summary>
        /// 是否正在播放
        /// </summary>
        public bool IsPlaying
        {
            get
            {
                //正在播放或者正在加载还没完成都属于没有闲置
                return AudioSource.isPlaying || (m_LoadClipHandle.IsValid() && !m_LoadClipHandle.IsDone);
            }
        }

        /// <summary>
        /// 音量
        /// </summary>
        public virtual float SettingVolume
        {
            get { return CoreMain.Audio.SoundVolume; }
        }

        /// <summary>
        /// 播放
        /// </summary>
        /// <param name="audioClipObject">音频资源</param>
        /// <param name="fadeInSeconds">淡入时长</param>
        /// <param name="delay">延迟</param>
        public void Play(AudioClipObject audioClipObject, float fadeInSeconds = 0, float delay = 0)
        {
            void PlayAudio()
            {
                StopAllCoroutines();
                AudioSource.Stop();
                AudioSource.clip = m_AudioClip;
                if (fadeInSeconds == 0)
                {
                    ApplyVolume();
                }
                else
                {
                    StartCoroutine(FadeIn(fadeInSeconds));    
                }
                AudioSource.PlayDelayed(delay);
            }
            
            if (m_AudioClipObject == audioClipObject)
            {
                PlayAudio();
            }
            else
            {
                //释放上一个句柄
                if (m_AudioClipObject != null)
                {
                    if (m_LoadClipHandle.IsValid())
                    {
                        Addressables.ReleaseInstance(m_LoadClipHandle);
                    }
                }

                m_AudioClipObject = audioClipObject;
                m_LoadClipHandle = audioClipObject.LoadClip((clip =>
                {
                    m_AudioClip = clip;
                    if (m_AudioClip == null)
                    {
                        Debug.LogError($"Load {audioClipObject.SafeName} clip failed");
                        return;
                    }

                    PlayAudio();
                }));
            }
        }

        /// <summary>
        /// 停止播放
        /// </summary>
        /// <param name="fadeOutSeconds">淡出时间</param>
        public void Stop(float fadeOutSeconds = 0)
        {
            StopAllCoroutines();
            if (fadeOutSeconds == 0)
            {
                AudioSource.Stop();
            }
            else
            {
                StartCoroutine(FadeOut(fadeOutSeconds,(() =>
                {
                    AudioSource.Stop();
                })));    
            }
        }

#if UNITY_EDITOR
        public void PlayDebug(AudioClipObject file, bool dontReset)
        {
            m_AudioClipObject = file;
            if (!dontReset)
            {
                AudioSource.Stop();
            }

            AudioSource.clip = file.ReferenceAudioClip.editorAsset;
            AudioSource.volume = 1;
            AudioSource.Play();
        }
#endif

        /// <summary>
        /// 淡入
        /// </summary>
        /// <param name="fadeInSeconds">淡入时间</param>
        IEnumerator FadeIn(float fadeInSeconds)
        {
            float timer = 0;
            while (timer < fadeInSeconds)
            {
                timer += Time.deltaTime;
                AudioSource.volume = Mathf.Lerp(0, SettingVolume, timer / fadeInSeconds);
                yield return null;
            }
        }

        /// <summary>
        /// 淡出
        /// </summary>
        /// <param name="fadeOutSeconds">淡出时间</param>
        /// <param name="fadeOutComplete">淡出完成</param>
        IEnumerator FadeOut(float fadeOutSeconds, Action fadeOutComplete)
        {
            float timer = 0;
            while (timer < fadeOutSeconds)
            {
                timer += Time.deltaTime;
                AudioSource.volume = Mathf.Lerp(0, SettingVolume, (fadeOutSeconds - timer) / fadeOutSeconds);
                yield return null;
            }
            
            fadeOutComplete?.Invoke();
        }

        /// <summary>
        /// 应用音量
        /// </summary>
        public void ApplyVolume()
        {
            AudioSource.volume = SettingVolume;
        }
    }
}
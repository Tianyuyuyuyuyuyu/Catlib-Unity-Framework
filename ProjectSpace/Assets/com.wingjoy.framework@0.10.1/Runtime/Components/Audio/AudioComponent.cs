using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using Wingjoy.Framework.Runtime.Definition;
using WingjoyUtility.Runtime;

namespace Wingjoy.Framework.Runtime.Audio
{
    public class AudioComponent : WingjoyFrameworkComponent
    {
        /// <summary>
        /// 主音量
        /// </summary>
        [Range(0, 1)]
        [SerializeField]
        [OnValueChanged("ApplyMasterVolume")]
        [TitleGroup("Volume")]
        [HorizontalGroup("Volume/Master"), LabelText("Master")]
        private float m_MasterVolume = 1;

        /// <summary>
        /// 主音量静音
        /// </summary>
        [SerializeField]
        [OnValueChanged("ApplyMasterVolume")]
        [HorizontalGroup("Volume/Master", Width = 60), LabelText("Mute"), LabelWidth(40)]
        private bool m_MasterMuted;

        /// <summary>
        /// 音效音量
        /// </summary>
        [Range(0, 1)]
        [SerializeField]
        [OnValueChanged("ApplySoundVolume")]
        [HorizontalGroup("Volume/Sound"), LabelText("Sound")]
        private float m_SoundVolume = 1;

        /// <summary>
        /// 音效静音
        /// </summary>
        [SerializeField]
        [OnValueChanged("ApplySoundVolume")]
        [HorizontalGroup("Volume/Sound", Width = 60), LabelText("Mute"), LabelWidth(40)]
        private bool m_SoundMuted;

        /// <summary>
        /// 音乐音量
        /// </summary>
        [Range(0, 1)]
        [SerializeField]
        [OnValueChanged("ApplyMusicVolume")]
        [HorizontalGroup("Volume/Music"), LabelText("Music")]
        private float m_MusicVolume = 1;

        /// <summary>
        /// 音乐静音
        /// </summary>
        [SerializeField]
        [OnValueChanged("ApplyMusicVolume")]
        [HorizontalGroup("Volume/Music", Width = 60), LabelText("Mute"), LabelWidth(40)]
        private bool m_MusicMuted;

        /// <summary>
        /// 音频设置
        /// </summary>
        [TitleGroup("Settings")]
        [SerializeField]
        [OpenOrCreateButton]
        private AudioComponentSettings m_AudioSettings;

        [SerializeField]
        [TitleGroup("AudioLibraryReference")]
        [OpenOrCreateButton]
        //[DrawWithUnity]
        //[InlineButton("Awake")]
        private AssetReferenceAudioLibrary m_LibraryReference;

        /// <summary>
        /// 音频助理
        /// </summary>
        private List<SoundHelper> m_SoundHelpers;

        /// <summary>
        /// 音效根节点
        /// </summary>
        private GameObject m_SoundSourceRoot;

        /// <summary>
        /// 背景音乐助理
        /// </summary>
        private MusicHelper m_MusicHelper;

        public float SoundVolume
        {
            get { return m_SoundVolume * m_MasterVolume * Convert.ToInt32(!m_MasterMuted) * Convert.ToInt32(!m_SoundMuted); }
            set
            {
                m_SoundVolume = value;
                ApplySoundVolume();
            }
        }

        public float MusicVolume
        {
            get { return m_MusicVolume * m_MasterVolume * Convert.ToInt32(!m_MasterMuted) * Convert.ToInt32(!m_MusicMuted); }
            set
            {
                m_MusicVolume = value;
                ApplyMusicVolume();
            }
        }

        public bool SoundMute
        {
            get { return m_SoundMuted; }
            set
            {
                m_SoundMuted = value;
                ApplySoundVolume();
            }
        }

        public bool MusicMute
        {
            get { return m_MusicMuted; }
            set
            {
                m_MusicMuted = value;
                ApplyMusicVolume();
            }
        }

        public List<SoundHelper> SoundHelpers => m_SoundHelpers;

        public MusicHelper MusicHelper => m_MusicHelper;

#if UNITY_EDITOR
        public AudioLibrary Library => m_LibraryReference.editorAsset;
#endif

        protected override async void Awake()
        {
            base.Awake();
            m_SoundHelpers = new List<SoundHelper>();

            m_SoundSourceRoot = new GameObject("SoundSource");
            m_SoundSourceRoot.transform.SetParent(transform);

            for (int i = 0; i < m_AudioSettings.InitialAudioSourceHelperCount; i++)
            {
                CreateNewSourceHelper();
            }

            var musicSource = new GameObject("MusicSource");
            musicSource.transform.SetParent(transform);
            musicSource.AddComponent<AudioSource>().loop = true;
            m_MusicHelper = musicSource.AddComponent<MusicHelper>();
            await LoadAudioLibrary();
        }

        public override async UniTask Launcher()
        {
            if (CoreMain.Setting != null)
            {
                MusicVolume = CoreMain.Setting.GetFloat(Constant.Setting.MusicVolume, 1);
                SoundVolume = CoreMain.Setting.GetFloat(Constant.Setting.SoundVolume, 1);
                MusicMute = CoreMain.Setting.GetBool(Constant.Setting.MusicMuted, false);
                SoundMute = CoreMain.Setting.GetBool(Constant.Setting.SoundMuted, false);    
            }
        }

        /// <summary>
        /// 加载音频库
        /// </summary>
        /// <returns>音频库</returns>
        public async UniTask<AudioLibrary> LoadAudioLibrary()
        {
            if (m_LibraryReference.IsValid())
            {
                if (m_LibraryReference.IsDone)
                {
                    return m_LibraryReference.Asset as AudioLibrary;
                }
                else
                {
                    await m_LibraryReference.OperationHandle;
                    return m_LibraryReference.OperationHandle.Result as AudioLibrary;
                }
            }
            else
            {
                var asyncOperationHandle = m_LibraryReference.LoadAssetAsync();
                await asyncOperationHandle;
                return asyncOperationHandle.Result;
            }
        }

        /// <summary>
        /// 播放音效
        /// </summary>
        /// <param name="audioId">音频ID</param>
        /// <param name="fadeInSeconds">淡入时长</param>
        /// <param name="delay">延迟</param>
        public async void PlaySound(int audioId, float fadeInSeconds = 0, float delay = 0)
        {
            var loadAudioLibrary = await LoadAudioLibrary();
            var soundClipObject = loadAudioLibrary.LoadSoundClipObject(audioId);
            PlaySound(soundClipObject);
        }

        /// <summary>
        /// 播放音效
        /// </summary>
        /// <param name="audioClipObject">音频资源</param>
        /// <param name="fadeInSeconds">淡入时长</param>
        /// <param name="delay">延迟</param>
        public void PlaySound(AudioClipObject audioClipObject, float fadeInSeconds = 0, float delay = 0)
        {
            if (audioClipObject == null)
            {
                Debug.LogError("Invalid sound clip object");
                return;
            }

            var availableAudioSource = GetAvailableAudioSource();
            if (availableAudioSource != null)
            {
                availableAudioSource.Play(audioClipObject, fadeInSeconds, delay);
            }
        }

        /// <summary>
        /// 播放音乐
        /// </summary>
        /// <param name="audioId">音频ID</param>
        /// <param name="fadeOutSeconds">上一首淡出时长</param>
        /// <param name="fadeInSeconds">淡入时长</param>
        /// <param name="delay">延迟</param>
        public async void PlayMusic(int audioId, float fadeOutSeconds = 0.5f, float fadeInSeconds = 0.5f, float delay = 0)
        {
            StopMusic(fadeOutSeconds);
            await UniTask.Delay((int) (fadeOutSeconds * 1000));
            var loadAudioLibrary = await LoadAudioLibrary();
            var musicClipObject = loadAudioLibrary.LoadMusicClipObject(audioId);
            PlayMusic(musicClipObject, fadeInSeconds, delay);
        }

        /// <summary>
        /// 播放音乐
        /// </summary>
        /// <param name="audioClipObject">音频资源</param>
        /// <param name="fadeInSeconds">淡入时长</param>
        /// <param name="delay">延迟</param>
        public void PlayMusic(AudioClipObject audioClipObject, float fadeInSeconds = 0, float delay = 0)
        {
            if (audioClipObject == null)
            {
                Debug.LogError("Invalid music clip object");
                return;
            }

            m_MusicHelper.Play(audioClipObject, fadeInSeconds, delay);
        }

        /// <summary>
        /// 按照音乐列表播放音乐
        /// </summary>
        /// <param name="audioPlayList">音乐列表</param>
        /// <param name="fadeOutSeconds">淡出时间</param>
        IEnumerator PlayMusic(AudioPlayList audioPlayList, float fadeOutSeconds = 0.5f)
        {
            //停止当前音乐
            StopMusic(fadeOutSeconds);

            yield return new WaitForSeconds(fadeOutSeconds);

            var loadAudioLibrary = LoadAudioLibrary();
            yield return loadAudioLibrary;
            foreach (var value in audioPlayList.Values)
            {
                var musicClipObject = loadAudioLibrary.GetAwaiter().GetResult().LoadMusicClipObject(value);
                PlayMusic(musicClipObject);
                yield return new WaitWhile((() => m_MusicHelper.IsPlaying && m_MusicHelper.AudioClipObject == musicClipObject));
            }
        }

        /// <summary>
        /// 停止播放音乐
        /// </summary>
        /// <param name="fadeOutSeconds">淡出时间</param>
        public void StopMusic(float fadeOutSeconds = 0)
        {
            if (m_MusicHelper.IsPlaying)
            {
                m_MusicHelper.Stop(fadeOutSeconds);
            }
        }

        /// <summary>
        /// 停止播放音效
        /// </summary>
        /// <param name="fadeOutSeconds">淡出时间</param>
        public void StopAllSound(float fadeOutSeconds)
        {
            foreach (var soundHelper in m_SoundHelpers)
            {
                if (soundHelper.IsPlaying)
                {
                    soundHelper.Stop(fadeOutSeconds);
                }
            }
        }

        /// <summary>
        /// 应用音乐音量
        /// </summary>
        public void ApplyMusicVolume()
        {
            m_MusicHelper?.ApplyVolume();
        }

        /// <summary>
        /// 应用音效音量
        /// </summary>
        public void ApplySoundVolume()
        {
            foreach (var sourceHelper in m_SoundHelpers)
            {
                sourceHelper.ApplyVolume();
            }
        }

        /// <summary>
        /// 应用主音量
        /// </summary>
        public void ApplyMasterVolume()
        {
            ApplySoundVolume();
            ApplyMusicVolume();
        }

        /// <summary>
        /// 获取可用的音频助理
        /// </summary>
        public AudioSourceHelper GetAvailableAudioSource()
        {
            foreach (var audioSourceHelper in m_SoundHelpers)
            {
                if (!audioSourceHelper.IsPlaying)
                {
                    return audioSourceHelper;
                }
            }

            return CreateNewSourceHelper();
        }

        /// <summary>
        /// 创建全新的音频助理
        /// </summary>
        /// <returns>音频助理</returns>
        private AudioSourceHelper CreateNewSourceHelper()
        {
            AudioSource newSource = Instantiate(m_AudioSettings.SourcePrefab, m_SoundSourceRoot.transform).GetComponent<AudioSource>();
            newSource.name = "AudioSource " + m_SoundHelpers.Count;
            var sourceHelper = newSource.GetComponent<SoundHelper>();
            m_SoundHelpers.Add(sourceHelper);
            return sourceHelper;
        }
    }
}
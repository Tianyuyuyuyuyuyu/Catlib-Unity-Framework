using Sirenix.OdinInspector;
using UnityEngine;

namespace Wingjoy.Framework.Runtime.Audio
{
    [SerializeField]
    public class AudioComponentSettings : ScriptableObject
    {
        /// <summary>
        /// 初始音频助理
        /// </summary>
        [SerializeField]
        private int m_InitialAudioSourceHelperCount = 10;

        /// <summary>
        /// 音频源预制件
        /// </summary>
        [SerializeField]
        private AudioSourceHelper m_SourcePrefab;

        public int InitialAudioSourceHelperCount => m_InitialAudioSourceHelperCount;

        public AudioSourceHelper SourcePrefab => m_SourcePrefab;
    }
}
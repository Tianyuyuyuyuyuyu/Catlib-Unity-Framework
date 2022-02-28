using System;
using System.Collections.Generic;
using UnityEngine;

namespace Wingjoy.Framework.Runtime.Audio
{
    [Serializable]
    public class AudioGroup : ScriptableObject
    {
        /// <summary>
        /// 组名称
        /// </summary>
        [SerializeField]
        private string m_GroupName;

        /// <summary>
        /// 资源
        /// </summary>
        [SerializeField]
        private List<AudioClipObject> m_AudioClipObjects = new List<AudioClipObject>();

        /// <summary>
        /// 是否收起
        /// </summary>
        [SerializeField]
        private bool m_IsFoldOut;


        public string Name
        {
            get => m_GroupName;
            set => m_GroupName = value;
        }

        public List<AudioClipObject> AudioClipObjects => m_AudioClipObjects;

        public bool IsFoldOut
        {
            get => m_IsFoldOut;
            set => m_IsFoldOut = value;
        }
    }
}
using System;
using System.Collections.Generic;

namespace Wingjoy.Framework.Runtime.Audio
{
    [Serializable]
    public class AudioPlayList
    {
        /// <summary>
        /// 播放列表
        /// </summary>
        private List<int> m_Values;

        public List<int> Values
        {
            get => m_Values;
            set => m_Values = value;
        }

        public AudioPlayList()
        {
            m_Values = new List<int>();
        }

        /// <summary>
        /// 添加音频ID
        /// </summary>
        /// <param name="audioId">音频ID</param>
        public void Add(int audioId)
        {
            m_Values.Add(audioId);
        }

        /// <summary>
        /// 清空列表
        /// </summary>
        public void Clear()
        {
            m_Values.Clear();
        }
    }
}
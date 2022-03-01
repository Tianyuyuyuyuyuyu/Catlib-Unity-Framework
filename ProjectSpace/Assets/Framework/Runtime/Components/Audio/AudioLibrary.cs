using System;
using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;
using Framework.Utility.Runtime;

namespace Framework.Runtime.Audio
{
    public class AudioLibrary : ScriptableObject
    {
        /// <summary>
        /// 库名称
        /// </summary>
        [SerializeField]
        private string m_LibraryName;

        /// <summary>
        /// 路径
        /// </summary>
        [SerializeField]
        [FolderPath]
        private string m_RootPath = "Assets/Audio";

        /// <summary>
        /// 音效组
        /// </summary>
        [SerializeField]
        [HideInInspector]
        private List<AudioGroup> m_SoundGroups = new List<AudioGroup>();

        /// <summary>
        /// 音乐组
        /// </summary>
        [SerializeField]
        [HideInInspector]
        private List<AudioGroup> m_MusicGroups = new List<AudioGroup>();


        public string LibraryName => m_LibraryName;

        public string RootPath => m_RootPath;

        public List<AudioGroup> SoundGroups => m_SoundGroups;

        public List<AudioGroup> MusicGroups => m_MusicGroups;

        /// <summary>
        /// 加载音效资源
        /// </summary>
        /// <param name="audioId">音频ID</param>
        /// <returns>音效资源</returns>
        public AudioClipObject LoadSoundClipObject(int audioId)
        {
            return GetClipObjectFromGroup(m_SoundGroups, audioId);
        }

        /// <summary>
        /// 加载音乐资源
        /// </summary>
        /// <param name="audioId">音频ID</param>
        /// <returns>音乐资源</returns>
        public AudioClipObject LoadMusicClipObject(int audioId)
        {
            return GetClipObjectFromGroup(m_MusicGroups, audioId);
        }

        /// <summary>
        /// 根据audioId从Group中获取音频
        /// </summary>
        /// <param name="audioGroups">音频组</param>
        /// <param name="audioId">ID</param>
        /// <returns>音频</returns>
        private AudioClipObject GetClipObjectFromGroup(List<AudioGroup> audioGroups, int audioId)
        {
            var groupIndex = audioId / 1000;
            var audioIndex = audioId % 1000;
            if (groupIndex < 0 || groupIndex >= audioGroups.Count)
            {
                Debug.LogError($"Invalid sound group Id (AudioId:{audioId})");
            }
            else
            {
                var audioGroup = audioGroups[groupIndex];
                if (audioGroup != null)
                {
                    if (audioIndex < 0 || audioIndex >= audioGroup.AudioClipObjects.Count)
                    {
                        Debug.LogError($"Invalid audio Id (AudioId:{audioId})");
                    }
                    else
                    {
                        return audioGroup.AudioClipObjects[audioIndex];
                    }
                }
                else
                {
                    Debug.LogError($"Missing sound group: (groupId:{groupIndex},audioId:{audioId})");
                }
            }

            return null;
        }

        /// <summary>
        /// 计算音频ID
        /// </summary>
        /// <param name="audioClipObject">音频对象</param>
        /// <returns>ID</returns>
        public int CalcAudioIds(AudioClipObject audioClipObject)
        {
            for (var groupIndex = 0; groupIndex < m_SoundGroups.Count; groupIndex++)
            {
                var soundGroup = m_SoundGroups[groupIndex];
                for (var audioIndex = 0; audioIndex < soundGroup.AudioClipObjects.Count; audioIndex++)
                {
                    var clipObject = soundGroup.AudioClipObjects[audioIndex];
                    if (clipObject == audioClipObject)
                    {
                        return AudioIdFormula(groupIndex, audioIndex);
                    }
                }
            }

            return -1;
        }

        /// <summary>
        /// 音频ID计算公式
        /// </summary>
        /// <param name="groupIndex">组索引</param>
        /// <param name="audioIndex">音频索引</param>
        /// <returns>ID</returns>
        public int AudioIdFormula(int groupIndex, int audioIndex)
        {
            return groupIndex * 1000 + audioIndex;
        }

        /// <summary>
        /// 获取音频组
        /// </summary>
        /// <param name="audioGroupType">音频组类型</param>
        /// <returns>音频组列表</returns>
        public List<AudioGroup> GetAudioGroups(AudioGroupType audioGroupType)
        {
            if (audioGroupType == AudioGroupType.Music)
            {
                return m_MusicGroups;
            }
            else
            {
                return m_SoundGroups;
            }
        }

        /// <summary>
        /// 验证音频ID
        /// </summary>
        /// <param name="audioName">音频名</param>
        /// <param name="audioId">音频Id</param>
        /// <param name="audioGroupType">音频组类型</param>
        /// <returns>是否正确</returns>
        public bool ValidAudioId(string audioName, int audioId, AudioGroupType audioGroupType)
        {
            var groupIndex = audioId / 1000;
            var audioIndex = audioId % 1000;

            var audioGroups = GetAudioGroups(audioGroupType);
            if (groupIndex < audioGroups.Count)
            {
                var audioGroup = audioGroups[groupIndex];
                if (audioIndex < audioGroup.AudioClipObjects.Count)
                {
                    var clipObject = audioGroup.AudioClipObjects[audioIndex];
                    return clipObject.SafeName == audioName;
                }
            }

            return false;
        }

#if UNITY_EDITOR
        private const string nameSpaceName = "Wingjoy.FrameworkMono.Runtime.Audio";

        public void GenerateAudioLibrary()
        {
            string fileName = $"AudioId - {m_LibraryName}.cs";
            string filePath = $"{RuntimeUtilities.Path.FrameworkDataPath}/Framework/Audio/Script/{fileName}";

            string directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(filePath, string.Empty);
            StreamWriter writer = new StreamWriter(filePath, true);
            writer.WriteLine("namespace " + nameSpaceName + "{");

            var values = Enum.GetValues(typeof(AudioGroupType));

            foreach (AudioGroupType value in values)
            {
                var audioGroups = GetAudioGroups(value);

                writer.WriteLine("    public static class " + m_LibraryName + value + "s");
                writer.WriteLine("    {");

                for (var groupIndex = 0; groupIndex < audioGroups.Count; groupIndex++)
                {
                    var soundGroup = audioGroups[groupIndex];
                    for (var audioIndex = 0; audioIndex < soundGroup.AudioClipObjects.Count; audioIndex++)
                    {
                        var clipObject = soundGroup.AudioClipObjects[audioIndex];
                        int id = AudioIdFormula(groupIndex, audioIndex);
                        writer.WriteLine($"        public const int {clipObject.SafeName} = {id};");
                    }
                }

                writer.WriteLine("    }");

                writer.WriteLine();
            }
            writer.WriteLine("}");
            writer.Close();

            AssetDatabase.ImportAsset(filePath);
            AssetDatabase.Refresh();
        }


        /// <summary>
        /// 读取本地音频Id
        /// </summary>
        /// <param name="audioClipObject">音频对象</param>
        /// <param name="audioGroupType">音频组类型</param>
        /// <returns>Id</returns>
        public int GetAudioIds(AudioClipObject audioClipObject, AudioGroupType audioGroupType)
        {
            string suffix = audioGroupType.ToString();
            var className = $"Wingjoy.FrameworkMono.Runtime.Audio.{m_LibraryName}{suffix}s";
            var type = AssemblyUtilities.GetTypeByCachedFullName(className);
            if (type == null)
                return -1;
            var fieldInfo = type.GetField(audioClipObject.SafeName);
            if (fieldInfo == null)
                return -1;
            int value = (int) fieldInfo.GetValue(null);
            return value;
        }

        /// <summary>
        /// 是否已经注册
        /// </summary>
        /// <param name="audioClipObject">字段名称</param>
        /// <param name="id">ID</param>
        /// <param name="audioGroupType">音频组类型</param>
        /// <returns>是否</returns>
        public bool HasRegister(AudioClipObject audioClipObject, int id, AudioGroupType audioGroupType)
        {
            var audioIds = GetAudioIds(audioClipObject, audioGroupType);
            return audioIds == id;
        }

#endif
    }
}
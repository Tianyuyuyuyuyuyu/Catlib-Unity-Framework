using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace Framework.Editor.Localization
{
    [GlobalConfig("FrameworkData/Framework/Localization")]
    public class GenerateExcel : GlobalConfig<GenerateExcel>
    {
        /// <summary>
        /// 文本文件
        /// </summary>
        public TextAsset File;

        /// <summary>
        /// 生成路径
        /// </summary>
        [FolderPath]
        public string GenerateFolder;


        [Button(ButtonSizes.Large)]
        public void Generate()
        {

        }
    }
}
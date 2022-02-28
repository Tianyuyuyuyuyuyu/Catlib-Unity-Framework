using System.IO;

namespace Wingjoy.Framework.Editor.Localization
{
    public static class LocalizationHelper
    {
        /// <summary>
        /// 将语言缩写加入文件名中
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="suffix">语言缩写</param>
        /// <returns>新的文件路径</returns>
        public static string AddSuffixToFileName(string path, string suffix)
        {
            return Path.GetDirectoryName(path) + Path.AltDirectorySeparatorChar +
                   Path.GetFileNameWithoutExtension(path) + suffix + Path.GetExtension(path);
        }

        /// <summary>
        /// 将语言缩写加入文件名中
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="suffix">语言缩写</param>
        /// <param name="extend">扩展路径</param>
        /// <returns>新的文件路径</returns>
        public static string GetPathBySuffix(string path, string suffix,string extend)
        {
            return Path.GetDirectoryName(path) + Path.AltDirectorySeparatorChar + suffix +
                   Path.AltDirectorySeparatorChar + extend + Path.AltDirectorySeparatorChar +
                   Path.GetFileNameWithoutExtension(path) + Path.GetExtension(path);
        }
    }
}

using UnityEngine;

namespace Wingjoy.Framework.Runtime
{
    public partial class Utility
    {
        public class Path
        {
            /// <summary>
            /// 获取Transform路径
            /// </summary>
            /// <param name="transform">Transform</param>
            /// <returns>Transform路径</returns>
            public static string GetPath(Transform transform)
            {
                return transform ? GetPath(transform.parent) + "/" + transform.gameObject.name : "";
            }

            /// <summary>
            /// 获取Transform路径移除Canvas（Environment）
            /// </summary>
            /// <param name="transform">Transform</param>
            /// <returns>Transform路径</returns>
            public static string GetPathWithoutCanvasEnvironment(Transform transform)
            {
                return GetPath(transform).Replace("/Canvas (Environment)", "").TrimStart('/');
            }
        }
    }
}
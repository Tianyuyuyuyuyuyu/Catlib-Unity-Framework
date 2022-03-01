using System.IO;
using UnityEngine;

namespace Framework.Utility.Runtime
{
    public partial class RuntimeUtilities
    {
        public static class Path
        {
            private static string m_FrameworkDataPath = "Assets/FrameworkData";

            /// <summary>
            /// 框架数据默认路径
            /// </summary>
            public static string FrameworkDataPath
            {
                get
                {
                    if (!Directory.Exists(m_FrameworkDataPath))
                    {
                        Directory.CreateDirectory(m_FrameworkDataPath);
                    }
                    
                    return m_FrameworkDataPath;
                }
            }

            public static string GetProtectPath
            {
                get
                {
                    string path = string.Empty;
#if UNITY_EDITOR
                    path = Application.persistentDataPath;
#elif UNITY_IOS
                    path = Application.persistentDataPath;
#elif UNITY_ANDROID
                    using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                    {
                        using (AndroidJavaObject currentActivity = jc.GetStatic<AndroidJavaObject>("currentActivity"))
                        {
                            path = currentActivity.Call<AndroidJavaObject>("getFilesDir").Call<string>("getCanonicalPath");
                            Debug.Log("path : " + path);
                        }
                    }
#endif


                    return path;
                }
            }

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

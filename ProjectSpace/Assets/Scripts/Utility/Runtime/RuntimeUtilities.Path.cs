using System.IO;
using UnityEngine;

namespace WingjoyUtility.Runtime
{
    public partial class RuntimeUtilities
    {
        public static class Path
        {
            private static string m_WingjoyDataPath = "Assets/WingjoyData";

            /// <summary>
            /// 公司数据默认路径
            /// </summary>
            public static string WingjoyDataPath
            {
                get
                {
                    if (!Directory.Exists(m_WingjoyDataPath))
                    {
                        Directory.CreateDirectory(m_WingjoyDataPath);
                    }
                    
                    return m_WingjoyDataPath;
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
        }
    }
}

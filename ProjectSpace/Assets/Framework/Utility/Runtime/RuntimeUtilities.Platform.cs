using System;
using UnityEngine;

namespace Framework.Utility.Runtime
{
    public static partial class RuntimeUtilities
    {
        public static class Platform
        {
            public static bool IsAndroid
            {
                get
                {
                    bool retValue = false;
#if UNITY_ANDROID
                    retValue = true;
#endif
                    return retValue;
                }
            }

            public static bool IsStandaloneWin
            {
                get
                {
                    bool retValue = false;
#if UNITY_STANDALONE_WIN
            retValue = true;
#endif
                    return retValue;
                }
            }

            public static bool IsEditor
            {
                get
                {
                    bool retValue = false;
#if UNITY_EDITOR
                    retValue = true;
#endif
                    return retValue;
                }
            }

            public static bool IsiOS
            {
                get
                {
                    bool retValue = false;
#if UNITY_IOS
                retValue = true;    
#endif
                    return retValue;
                }
            }

            /// <summary>
            /// 获取平台字符串
            /// </summary>
            /// <returns>平台字符串</returns>
            public static string GetPlatform()
            {
                switch (Application.platform)
                {
                    case RuntimePlatform.OSXEditor:
                        break;
                    case RuntimePlatform.OSXPlayer:
                        break;
                    case RuntimePlatform.WindowsPlayer:
                        break;
                    case RuntimePlatform.WindowsEditor:
                        break;
                    case RuntimePlatform.IPhonePlayer:
                        return "Ios";
                    case RuntimePlatform.Android:
                        break;
                    case RuntimePlatform.LinuxPlayer:
                        break;
                    case RuntimePlatform.LinuxEditor:
                        break;
                    case RuntimePlatform.WebGLPlayer:
                        break;
                    case RuntimePlatform.PS4:
                        break;
                    case RuntimePlatform.XboxOne:
                        break;
                    case RuntimePlatform.tvOS:
                        break;
                    case RuntimePlatform.Switch:
                        break;
                    case RuntimePlatform.Lumin:
                        break;
#if UNITY_2019_4_OR_NEWER
                    case RuntimePlatform.Stadia:
                        break;
#endif
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                return Application.platform.ToString();
            }
        }
    }
}
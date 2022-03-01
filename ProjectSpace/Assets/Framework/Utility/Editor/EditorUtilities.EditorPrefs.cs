using UnityEditor;

namespace Framework.Utility.Editor
{
    public static partial class EditorUtilities
    {
        public static class EditorPrefs
        {
            /// <summary>
            /// 项目键值
            /// </summary>
            private static string s_ProjectKey;

            /// <summary>
            /// 初始化
            /// </summary>
            [InitializeOnLoadMethod]
            public static void Init()
            {
                s_ProjectKey = $"{PlayerSettings.companyName}{PlayerSettings.productName}";
            }

            /// <summary>
            ///   <para>Sets the value of the preference identified by key as an integer.</para>
            /// </summary>
            /// <param name="key">Name of key to write integer to.</param>
            /// <param name="value">Value of the integer to write into the storage.</param>
            public static void SetInt(string key, int value)
            {
                UnityEditor.EditorPrefs.SetInt(s_ProjectKey + key, value);
            }

            /// <summary>
            ///   <para>Returns the value corresponding to key in the preference file if it exists.</para>
            /// </summary>
            /// <param name="key">Name of key to read integer from.</param>
            /// <param name="defaultValue">Integer value to return if the key is not in the storage.</param>
            /// <returns>
            ///   <para>The value stored in the preference file.</para>
            /// </returns>
            public static int GetInt(string key, int defaultValue)
            {
                return UnityEditor.EditorPrefs.GetInt(s_ProjectKey + key, defaultValue);
            }

            /// <summary>
            ///   <para>Returns the value corresponding to key in the preference file if it exists.</para>
            /// </summary>
            /// <param name="key">Name of key to read integer from.</param>
            /// <returns>
            ///   <para>The value stored in the preference file.</para>
            /// </returns>
            public static int GetInt(string key) => UnityEditor.EditorPrefs.GetInt(s_ProjectKey + key, 0);


            /// <summary>
            ///   <para>Sets the float value of the preference identified by key.</para>
            /// </summary>
            /// <param name="key">Name of key to write float into.</param>
            /// <param name="value">Float value to write into the storage.</param>
            public static void SetFloat(string key, float value)
            {
                UnityEditor.EditorPrefs.SetFloat(s_ProjectKey + key, value);
            }

            /// <summary>
            ///   <para>Returns the float value corresponding to key if it exists in the preference file.</para>
            /// </summary>
            /// <param name="key">Name of key to read float from.</param>
            /// <param name="defaultValue">Float value to return if the key is not in the storage.</param>
            /// <returns>
            ///   <para>The float value stored in the preference file or the defaultValue id the
            ///   requested float does not exist.</para>
            /// </returns>
            public static float GetFloat(string key, float defaultValue)
            {
                return UnityEditor.EditorPrefs.GetFloat(s_ProjectKey + key, defaultValue);
            }

            /// <summary>
            ///   <para>Returns the float value corresponding to key if it exists in the preference file.</para>
            /// </summary>
            /// <param name="key">Name of key to read float from.</param>
            /// <returns>
            ///   <para>The float value stored in the preference file or the defaultValue id the
            ///   requested float does not exist.</para>
            /// </returns>
            public static float GetFloat(string key) => UnityEditor.EditorPrefs.GetFloat(s_ProjectKey + key, 0.0f);

            /// <summary>
            ///   <para>Sets the value of the preference identified by key. Note that EditorPrefs does not support null strings and will store an empty string instead.</para>
            /// </summary>
            /// <param name="key"></param>
            /// <param name="value"></param>
            public static void SetString(string key, string value)
            {
                UnityEditor.EditorPrefs.SetString(s_ProjectKey + key, value);
            }

            /// <summary>
            ///   <para>Returns the value corresponding to key in the preference file if it exists.</para>
            /// </summary>
            /// <param name="key"></param>
            /// <param name="defaultValue"></param>
            public static string GetString(string key, string defaultValue)
            {
                return UnityEditor.EditorPrefs.GetString(s_ProjectKey + key, defaultValue);
            }

            /// <summary>
            ///   <para>Returns the value corresponding to key in the preference file if it exists.</para>
            /// </summary>
            /// <param name="key"></param>
            public static string GetString(string key) => UnityEditor.EditorPrefs.GetString(s_ProjectKey + key, "");

            /// <summary>
            ///   <para>Sets the value of the preference identified by key.</para>
            /// </summary>
            /// <param name="key"></param>
            /// <param name="value"></param>
            public static void SetBool(string key, bool value)
            {
                UnityEditor.EditorPrefs.SetBool(s_ProjectKey + key, value);
            }

            /// <summary>
            ///   <para>Returns the value corresponding to key in the preference file if it exists.</para>
            /// </summary>
            /// <param name="key"></param>
            /// <param name="defaultValue"></param>
            public static bool GetBool(string key, bool defaultValue)
            {
                return UnityEditor.EditorPrefs.GetBool(s_ProjectKey + key, defaultValue);
            }

            /// <summary>
            ///   <para>Returns the value corresponding to key in the preference file if it exists.</para>
            /// </summary>
            /// <param name="key"></param>
            public static bool GetBool(string key) => UnityEditor.EditorPrefs.GetBool(s_ProjectKey + key, false);

            /// <summary>
            ///   <para>Returns true if key exists in the preferences file.</para>
            /// </summary>
            /// <param name="key">Name of key to check for.</param>
            /// <returns>
            ///   <para>The existence or not of the key.</para>
            /// </returns>
            public static bool HasKey(string key)
            {
                return UnityEditor.EditorPrefs.HasKey(s_ProjectKey + key);
            }

            /// <summary>
            ///   <para>Removes key and its corresponding value from the preferences.</para>
            /// </summary>
            /// <param name="key"></param>
            public static void DeleteKey(string key)
            {
                UnityEditor.EditorPrefs.DeleteKey(s_ProjectKey + key);
            }

            /// <summary>
            ///   <para>Removes all keys and values from the preferences. Use with caution.</para>
            /// </summary>
            public static void DeleteAll()
            {
                UnityEditor.EditorPrefs.DeleteAll();
            }
        }
    }
}
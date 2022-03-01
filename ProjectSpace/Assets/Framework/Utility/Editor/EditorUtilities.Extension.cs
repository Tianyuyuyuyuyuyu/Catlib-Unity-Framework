using UnityEditor;
using UnityEditor.Presets;
using UnityEngine;

namespace Framework.Utility.Editor
{
    public static partial class EditorUtilities
    {
        /// <summary>
        /// Adds a new element to the end of the array and returns the new element
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        public static SerializedProperty AddNewArrayElement(this SerializedProperty prop)
        {
            int index = prop.arraySize;
            prop.InsertArrayElementAtIndex(index);
            return prop.GetArrayElementAtIndex(index);
        }

        public static PropertyModification FindProp(this Preset preset, string propName)
        {
            for (int i = 0; i < preset.PropertyModifications.Length; i++)
            {
                if (preset.PropertyModifications[i].propertyPath.Equals(propName))
                {
                    return preset.PropertyModifications[i];
                }
            }

            return null;
        }
    }
}
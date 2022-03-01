using System;
using Framework.Utility.Runtime;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
#if ADDRESSABLES
using UnityEngine.AddressableAssets;
#endif

namespace Framework.Utility.Editor.Drawers
{
    public class OpenOrCreateButtonDrawer : OdinAttributeDrawer<OpenOrCreateButton>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            this.CallNextDrawer(label);
            EditorGUILayout.EndVertical();

            Type type = null;
            bool notExist = false;
            object value = null;
            var typeOfValue = Property.ValueEntry.TypeOfValue;
#if ADDRESSABLES
            if (typeOfValue.IsSubclassOf(typeof(AssetReference)))
            {
                if (typeOfValue.BaseType != null)
                {
                    if (typeOfValue.BaseType.GenericTypeArguments.Length > 0)
                    {
                        type = typeOfValue.BaseType.GenericTypeArguments[0];
                        if (Property.ValueEntry.WeakSmartValue is AssetReference t)
                        {
                            value = t.editorAsset;
                            notExist = t.editorAsset == null;
                        }
                    }
                }
            }
            else
#endif
            {
                type = typeOfValue;
                notExist = Property.ValueEntry.WeakSmartValue == null || Property.ValueEntry.ValueState == PropertyValueState.NullReference;
                value = Property.ValueEntry.WeakSmartValue;
            }

            if (type == null)
                return;

            if (notExist)
            {
                if (GUILayout.Button("Create"))
                {
                    ClassSelector classSelector = new ClassSelector(type, (o =>
                    {
#if ADDRESSABLES
                        if (Property.ValueEntry.WeakSmartValue is AssetReference t)
                        {
                            t.SetEditorAsset(o);
                        }
                        else
#endif
                        {
                            Property.ValueEntry.WeakSmartValue = o;
                        }
                    }));
                    classSelector.ShowInPopup();
                }
            }
            else
            {
                if (GUILayout.Button("Open"))
                {
                    var editorWindow = OdinEditorWindow.CreateOdinEditorWindowInstanceForObject(value);
                    editorWindow.position = GUIHelper.GetEditorWindowRect().AlignCenter(700f, 800f);
                    editorWindow.Show();
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        public override bool CanDrawTypeFilter(Type type)
        {
#if ADDRESSABLES
            if (type.IsSubclassOf(typeof(AssetReference)))
            {
                if (type.BaseType != null)
                {
                    if (type.BaseType.GenericTypeArguments.Length > 0)
                    {
                        var genericArguments = type.BaseType.GenericTypeArguments[0];
                        return genericArguments != null && genericArguments.IsSubclassOf(typeof(ScriptableObject));
                    }
                }
            }
            else
#endif
            {
                return type.IsSubclassOf(typeof(ScriptableObject));
            }

            return false;
        }
    }
}
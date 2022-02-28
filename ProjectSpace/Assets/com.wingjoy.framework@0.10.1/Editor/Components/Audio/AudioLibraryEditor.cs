using System.Collections.Generic;
using System.IO;
using System.Linq;
using JSAM.JSAMEditor;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Wingjoy.Framework.Runtime.Audio;
using WingjoyUtility.Editor;
using WingjoyUtility.Runtime;

namespace Wingjoy.Framework.Editor.Audio
{
    [CustomEditor(typeof(AudioLibrary))]
    public class AudioLibraryEditor : OdinEditor
    {
        private GUITabGroup m_AnimatedTabGroup;
        private GUITabPage m_SoundPage;
        private GUITabPage m_MusicPage;
        private AudioLibrary m_AudioLibrary;
        private Dictionary<object, AudioList> m_AudioListDic = new Dictionary<object, AudioList>();

        private SerializedProperty m_SoundGroupsSerializedProperty;
        private SerializedProperty m_MusicGroupsSerializedProperty;

        private int m_DragSelectedIndex = -1;

        protected override void OnEnable()
        {
            base.OnEnable();
            Undo.undoRedoPerformed += OnUndoRedoPerformed;

            m_AnimatedTabGroup = SirenixEditorGUI.CreateAnimatedTabGroup("Library");
            m_SoundPage = m_AnimatedTabGroup.RegisterTab("Sound");
            m_MusicPage = m_AnimatedTabGroup.RegisterTab("Music");
            m_AudioLibrary = target as AudioLibrary;

            m_SoundGroupsSerializedProperty = serializedObject.FindProperty("m_SoundGroups");
            m_MusicGroupsSerializedProperty = serializedObject.FindProperty("m_MusicGroups");
            InitData();
        }

        private void OnUndoRedoPerformed()
        {
            InitData();
            Repaint();
        }

        public void InitData()
        {
            m_AudioListDic.Clear();

            for (int i = 0; i < m_SoundGroupsSerializedProperty.arraySize; i++)
            {
                var element = m_SoundGroupsSerializedProperty.GetArrayElementAtIndex(i);
                if (element.objectReferenceValue == null)
                    continue;
                SerializedObject group = new SerializedObject(element.objectReferenceValue);
                var objects = group.FindProperty("m_AudioClipObjects");
                m_AudioListDic.Add(element.objectReferenceValue, new AudioList(i, m_AudioLibrary, group, objects, AudioGroupType.Sound));
            }

            for (int i = 0; i < m_MusicGroupsSerializedProperty.arraySize; i++)
            {
                var element = m_MusicGroupsSerializedProperty.GetArrayElementAtIndex(i);
                if (element.objectReferenceValue == null)
                    continue;
                SerializedObject group = new SerializedObject(element.objectReferenceValue);
                var objects = group.FindProperty("m_AudioClipObjects");
                m_AudioListDic.Add(element.objectReferenceValue, new AudioList(i, m_AudioLibrary, group, objects, AudioGroupType.Music));
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            Undo.undoRedoPerformed -= OnUndoRedoPerformed;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            m_AnimatedTabGroup.BeginGroup();

            if (m_SoundPage.BeginPage())
            {
                DrawLibrary(m_AudioLibrary.SoundGroups, m_SoundGroupsSerializedProperty);
            }

            m_SoundPage.EndPage();

            if (m_MusicPage.BeginPage())
            {
                DrawLibrary(m_AudioLibrary.MusicGroups, m_MusicGroupsSerializedProperty);
            }

            m_MusicPage.EndPage();

            m_AnimatedTabGroup.EndGroup();

            if (GUILayout.Button("Generate Audio Id", (GUILayoutOption[]) GUILayoutOptions.Height(31)))
            {
                m_AudioLibrary.GenerateAudioLibrary();
            }
        }

        public void DrawLibrary(List<AudioGroup> audioGroups, SerializedProperty groupProperty)
        {
            string title = string.Empty;
            title += "Total Files: " + audioGroups.Where((group => group != null)).Sum((group => group.AudioClipObjects.Count));
            var blontent = new GUIContent(title);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(blontent);

            blontent = new GUIContent("New Group", "Add a new audio Group");
            if (GUILayout.Button(blontent, new GUILayoutOption[] {GUILayout.ExpandWidth(false)}))
            {
                var utility = InputDialogEditor.Init("Enter Group Name", true, true);
                utility.AddField(new GUIContent("Group Name"), "New Group");
                InputDialogEditor.onSubmitField += (strings =>
                {
                    OnSubmitField(strings, groupProperty);
                });
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginVertical();
            List<int> dragHistory = new List<int>();
            for (var index = 0; index < audioGroups.Count; index++)
            {
                var audioGroup = audioGroups[index];
                if (audioGroup == null)
                {
                    EditorGUILayout.BeginHorizontal();
                    var deleteTip = new GUIContent("Missing Group Should be Delete", "Remove this group and any Audio Files inside it from the library");
                    if (GUILayout.Button(deleteTip, new GUILayoutOption[] {GUILayout.ExpandWidth(false)}))
                    {
                        groupProperty.DeleteArrayElementAtIndex(index);
                    }

                    EditorGUILayout.EndHorizontal();
                    continue;
                }

                Rect dropRect = EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                int dragIndex = HandleDragAndDrop(dropRect, index, audioGroup);
                if (dragIndex > -2)
                {
                    dragHistory.Add(dragIndex);
                    if (dragIndex > -1)
                    {
                        m_DragSelectedIndex = dragIndex;
                    }
                }

                bool dragging = m_DragSelectedIndex == index;

                audioGroup.IsFoldOut = EditorGUILayout.BeginFoldoutHeaderGroup(audioGroup.IsFoldOut, audioGroup.Name);
                if (audioGroup.IsFoldOut)
                {
                    if (m_AudioListDic.TryGetValue(audioGroup, out var list))
                    {
                        list.Draw(index, groupProperty, audioGroup);
                    }
                }

                if (dragging)
                {
                    GUIStyle style = JSAMEditorHelper.ApplyTextAnchorToStyle(GUI.skin.box, TextAnchor.MiddleCenter);
                    style = JSAMEditorHelper.ApplyFontSizeToStyle(style, 15);
                    style = JSAMEditorHelper.ApplyBoldTextToStyle(style);
                    style = JSAMEditorHelper.ApplyTextColorToStyle(style, Color.white);
                    JSAMEditorHelper.BeginColourChange(Color.white);
                    GUI.Box(dropRect, "Drop to Add Audio File(s)", style);
                    JSAMEditorHelper.EndColourChange();
                }

                EditorGUILayout.EndFoldoutHeaderGroup();
                EditorGUILayout.EndVertical();
            }

            // Un-mark drag-hovered lists
            if (dragHistory.Count > 0)
            {
                bool blankHover = true;
                for (int i = 0; i < dragHistory.Count; i++)
                {
                    if (dragHistory[i] > -1)
                    {
                        blankHover = false;
                        break;
                    }
                }

                if (blankHover == true) m_DragSelectedIndex = -1;
            }

            EditorGUILayout.EndVertical();
        }

        private void OnSubmitField(string[] input, SerializedProperty groupProperty)
        {
            string inputName = input[0];
            if (string.IsNullOrEmpty(inputName))
            {
                EditorUtility.DisplayDialog("名称错误",
                    "名称请不要留空.", "OK");
                GUIUtility.ExitGUI();
            }
            else if (m_AudioLibrary.SoundGroups.Exists((group => group.Name == inputName)))
            {
                EditorUtility.DisplayDialog("重复音频组！",
                    "已经包含同样名称的音频组", "OK");
                GUIUtility.ExitGUI();
                return;
            }

            var addNewArrayElement = groupProperty.AddNewArrayElement();
            AudioGroup audioGroup = CreateInstance<AudioGroup>();
            audioGroup.Name = inputName;
            audioGroup.name = inputName;
            AssetDatabase.AddObjectToAsset(audioGroup, target);
            addNewArrayElement.objectReferenceValue = audioGroup;

            InitData();
            Repaint();
        }

        public int HandleDragAndDrop(Rect dragRect, int index, AudioGroup audioGroup)
        {
            if (dragRect.Contains(Event.current.mousePosition))
            {
                if (Event.current.type == EventType.DragUpdated)
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    Event.current.Use();
                    return index;
                }
                else if (Event.current.type == EventType.DragPerform)
                {
                    foreach (var objectReference in DragAndDrop.objectReferences)
                    {
                        string filePath = AssetDatabase.GetAssetPath(objectReference);
                        var clipObjects = ImportAssetsOrFoldersAtPath(m_AudioLibrary, audioGroup, filePath);
                        foreach (var audioClipObject in clipObjects)
                        {
                            if (audioGroup.AudioClipObjects.Contains(audioClipObject))
                            {
                                continue;
                            }

                            audioGroup.AudioClipObjects.Add(audioClipObject);
                        }
                    }
                    
                    EditorUtility.SetDirty(m_AudioLibrary);
                    Event.current.Use();
                    m_DragSelectedIndex = -1;
                    InitData();
                }
            }
            else if (Event.current.type == EventType.DragUpdated)
            {
                return -1;
            }

            return -2;
        }

        public static List<AudioClipObject> ImportAssetsOrFoldersAtPath(AudioLibrary audioLibrary, AudioGroup audioGroup, string filePath)
        {
            var asset = AssetDatabase.LoadAssetAtPath<Object>(filePath);
            if (!AssetDatabase.IsValidFolder(filePath))
            {
                if (asset != null)
                {
                    if (asset is AudioClipObject audioClipObject)
                    {
                        return new List<AudioClipObject> {audioClipObject};
                    }
                    else if (asset is AudioClip audioClip)
                    {
                        var path = $"{audioLibrary.RootPath}/ClipObjects/{audioGroup.Name}/{audioClip.name}.asset";
                        var loadAssetAtPath = AssetDatabase.LoadAssetAtPath<AudioClipObject>(path);
                        if (loadAssetAtPath != null)
                        {
                            audioClipObject = loadAssetAtPath;
                        }
                        else
                        {
                            audioClipObject = CreateInstance<AudioClipObject>();
                            audioClipObject.ReferenceAudioClip = new AssetReferenceAudioClip(audioClip);
                            RuntimeUtilities.AssetDatabaseHelper.CreateAssetSafe(audioClipObject, path);
                        }

                        return new List<AudioClipObject> {audioClipObject};
                    }
                    else
                    {
                        Debug.LogError("Invalid file");
                    }
                }
            }
            else
            {
                List<AudioClipObject> imports = new List<AudioClipObject>();
                List<string> importTarget = new List<string>(Directory.GetDirectories(filePath));
                importTarget.AddRange(Directory.GetFiles(filePath));
                for (int i = 0; i < importTarget.Count; i++)
                {
                    imports.AddRange(ImportAssetsOrFoldersAtPath(audioLibrary, audioGroup, importTarget[i]));
                }

                return imports;
            }

            return new List<AudioClipObject>();
        }
    }

    public class AudioList
    {
        private AudioLibrary m_AudioLibrary;
        private ReorderableList m_List;
        private int m_GroupIndex;
        private AudioGroupType m_AudioGroupType;

        public AudioList(int groupIndex, AudioLibrary audioLibrary, SerializedObject obj, SerializedProperty prop, AudioGroupType audioGroupType)
        {
            m_GroupIndex = groupIndex;
            m_AudioGroupType = audioGroupType;
            m_AudioLibrary = audioLibrary;
            m_List = new ReorderableList(obj, prop, true, false, false, false);
            m_List.drawElementCallback += DrawElementCallback;
        }

        private void DrawElementCallback(Rect rect, int index, bool isactive, bool isfocused)
        {
            var element = m_List.serializedProperty.GetArrayElementAtIndex(index);
            var file = element.objectReferenceValue as AudioClipObject;

            Rect prevRect = new Rect(rect);
            Rect currentRect = new Rect(prevRect);

            currentRect.xMax = currentRect.xMin + 20;
            GUIContent blontent = new GUIContent("T", "Change the group of this Audio clip object");

            var id = m_GroupIndex * 1000 + index;
            bool hasRegister = false;
            if (file != null)
            {
                hasRegister = m_AudioLibrary.HasRegister(file, id, m_AudioGroupType);
            }

            using (new EditorGUI.DisabledScope(true))
            {
                prevRect = new Rect(currentRect);
                currentRect.xMin = prevRect.xMax + 5;
                currentRect.xMax = rect.xMax - 110;

                if (file == null)
                {
                    blontent = new GUIContent($"{id}.Missing");
                }
                else
                {
                    if (hasRegister)
                    {
                        blontent = new GUIContent($"{id}.{file.SafeName}");
                    }
                    else
                    {
                        blontent = new GUIContent($"{id}.{file.SafeName}*", "需要重新生成AudioId");
                    }
                }
            }

            Rect decoyRect = EditorGUI.PrefixLabel(currentRect, blontent);
            using (new EditorGUI.DisabledScope(true))
            {
                if (file != null)
                {
                    EditorGUI.PropertyField(decoyRect, element, GUIContent.none);
                }
            }

            using (new EditorGUI.DisabledScope(!true))
            {
                prevRect = new Rect(currentRect);

                GUIContent copyCode;
                if (hasRegister)
                {
                    blontent = new GUIContent("Name", "点击拷贝对应文件名称");
                    copyCode = new GUIContent("Code", "点击拷贝对应文件播放代码");
                }
                else
                {
                    blontent = new GUIContent("Name", "需要重新生成AudioId才能拷贝到正确的ID");
                    copyCode = new GUIContent("Code", "点击拷贝对应文件播放代码");
                }

                currentRect.xMin = prevRect.xMax + 2;
                currentRect.xMax = currentRect.xMax + 45;

                if (file != null)
                {
                    if (GUI.Button(currentRect, blontent))
                    {
                        EditorGUIUtility.systemCopyBuffer = $"{m_AudioLibrary.LibraryName}{m_AudioGroupType.ToString()}s.{file.SafeName}";
                    }

                    currentRect.xMin = currentRect.xMax + 2;
                    currentRect.xMax = currentRect.xMax + 45;
                    if (GUI.Button(currentRect, copyCode))
                    {
                        EditorGUIUtility.systemCopyBuffer = $"CoreMain.Audio.Play{m_AudioGroupType.ToString()}({m_AudioLibrary.LibraryName}{m_AudioGroupType.ToString()}s.{file.SafeName});";
                    }
                }
            }

            //关闭按钮
            prevRect = new Rect(currentRect);
            currentRect.xMin = prevRect.xMax + 2;
            currentRect.xMax = currentRect.xMax + 25;
            blontent = new GUIContent("X");
            if (GUI.Button(currentRect, blontent))
            {
                var arrayElementAtIndex = m_List.serializedProperty.GetArrayElementAtIndex(index);
                arrayElementAtIndex.objectReferenceValue = null;
                m_List.serializedProperty.DeleteArrayElementAtIndex(index);

                m_List.serializedProperty.serializedObject.ApplyModifiedProperties();
                GUIUtility.ExitGUI();
            }
        }

        public void Draw(int index, SerializedProperty groupProperty, AudioGroup audioGroup)
        {
            m_List.DoLayoutList();

            Rect rect = EditorGUILayout.BeginHorizontal();
            var blontent = new GUIContent("Delete", "Remove this group and any Audio clip objects inside it from the library");
            if (GUILayout.Button(blontent, new GUILayoutOption[] {GUILayout.ExpandWidth(false)}))
            {
                groupProperty.GetArrayElementAtIndex(index).DeleteCommand();
                groupProperty.DeleteArrayElementAtIndex(index);
                Object.DestroyImmediate(audioGroup, true);
            }

            rect.xMin = rect.xMax - 100;
            GUI.Label(rect, "File Count: " + audioGroup.AudioClipObjects.Count,
                JSAMEditorHelper.ApplyTextAnchorToStyle(EditorStyles.label, TextAnchor.MiddleRight));
            EditorGUILayout.EndHorizontal();
        }
    }
}
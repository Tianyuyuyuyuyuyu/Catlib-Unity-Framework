using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Framework.Editor.Audio
{
    public class InputDialogEditor : EditorWindow
    {
        public class CustomField
        {
            public GUIContent content;
            public string text;
            public bool useTextArea;
        }

        protected static InputDialogEditor window;
        bool allowEnterKey;
        bool allowEscapeKey;
        bool focused = false;

        List<CustomField> fields = new List<CustomField>();

        public static System.Action<string[]> onSubmitField;

        public static InputDialogEditor Init(string windowName, bool allowEnterKey, bool allowEscapeKey)
        {
            window = CreateInstance<InputDialogEditor>();
            window.position = new Rect(window.position.position, new Vector2(300, 300));
            window.Show();
            window.titleContent = new GUIContent(windowName);
            window.allowEnterKey = allowEnterKey;
            window.allowEscapeKey = allowEscapeKey;
            onSubmitField = null;
            return window;
        }

        private void OnDisable()
        {
            onSubmitField = null;
        }

        public void AddField(GUIContent content, string startingText = "", bool useTextArea = false)
        {
            var newField = new CustomField();
            newField.content = content;
            newField.text = startingText;
            newField.useTextArea = useTextArea;
            fields.Add(newField);
        }

        private void OnGUI()
        {
            for (int i = 0; i < fields.Count; i++)
            {
                if (fields[i].content != GUIContent.none)
                {
                    EditorGUILayout.LabelField(fields[i].content);
                }

                if (!focused) GUI.SetNextControlName("TextBox");
                if (!fields[i].useTextArea)
                {
                    fields[i].text = EditorGUILayout.TextField(fields[i].text);
                }
                else
                {
                    fields[i].text = EditorGUILayout.TextArea(fields[i].text);
                }

                if (!focused)
                {
                    GUI.FocusControl("TextBox");
                    focused = true;
                }

                EditorGUILayout.Space();
            }

            if (Event.current.type == EventType.KeyDown)
            {
                if (Event.current.keyCode == KeyCode.Escape)
                {
                    if (allowEscapeKey)
                    {
                        window.Close();
                    }
                }

                if (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter)
                {
                    if (allowEnterKey)
                    {
                        SubmitText();
                    }
                }
            }

            if (GUILayout.Button("Submit"))
            {
                SubmitText();
            }
        }

        void SubmitText()
        {
            string[] text = new string[fields.Count];
            for (int i = 0; i < fields.Count; i++)
            {
                text[i] = fields[i].text;
            }

            onSubmitField?.Invoke(text);
            window.Close();
        }
    }
}
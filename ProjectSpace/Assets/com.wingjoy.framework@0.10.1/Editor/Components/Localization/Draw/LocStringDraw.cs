// using System;
// using Sirenix.OdinInspector.Editor;
// using Sirenix.Utilities.Editor;
// using UnityEditor;
// using UnityEngine;
// using Wingjoy.Framework.Runtime.Localization;
//
// namespace Wingjoy.Framework.Editor.Localization.Draw
// {
//     public class LocStringDraw : OdinValueDrawer<LocString>
//     {
//         /// <summary>Draws the property with GUILayout support.</summary>
//         /// <param name="label">The label. This can be null, so make sure your drawer supports that.</param>
//         protected override void DrawPropertyLayout(GUIContent label)
//         {
//             GUILayout.BeginHorizontal();
//             if (label != null)
//             {
//                 GUILayout.Label(label);
//             }
//             foreach (var inspectorProperty in ValueEntry.Property.Children)
//             {
//                 if (inspectorProperty.Name == "m_EntryKey")
//                 {
//                 }
//                 else if (inspectorProperty.Name == "Value")
//                 {
//                     inspectorProperty.Draw(label);
//                 }
//             }
//             GUILayout.EndHorizontal();
//         }
//     }
//     
//     public class MultiLinePropertyAttributeDrawer : OdinAttributeDrawer<MultiLineLocStringAttribute, LocString>
//     {
//         /// <summary>
//         /// Draws the property.
//         /// </summary>
//         protected override void DrawPropertyLayout(GUIContent label)
//         {
//             var entry = this.ValueEntry;
//             var attribute = this.Attribute;
//
//             var position = EditorGUILayout.GetControlRect(label != null, EditorGUIUtility.singleLineHeight * attribute.Lines);
//             position.height -= 2;
//
//             if (label == null)
//             {
//                 entry.SmartValue.Value = EditorGUI.TextArea(position, entry.SmartValue.Value, EditorStyles.textArea);
//             }
//             else
//             {
//                 var controlID = GUIUtility.GetControlID(label, FocusType.Keyboard, position);
//                 var areaPosition = EditorGUI.PrefixLabel(position, controlID, label, EditorStyles.label);
//
//                 entry.SmartValue.Value = EditorGUI.TextArea(areaPosition, entry.SmartValue.Value, EditorStyles.textArea);
//             }
//         }
//     }
// }
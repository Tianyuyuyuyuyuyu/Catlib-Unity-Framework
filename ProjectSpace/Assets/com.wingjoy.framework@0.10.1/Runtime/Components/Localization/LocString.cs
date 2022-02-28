// using System;
// using Sirenix.OdinInspector;
// using UnityEngine;
//
// namespace Wingjoy.Framework.Runtime.Localization
// {
//     [Serializable]
//     public class LocString
//     {
//         /// <summary>
//         /// 实例键值
//         /// </summary>
//         [SerializeField]
//         private string m_TableGuid;
//
//         /// <summary>
//         /// 实例键值
//         /// </summary>
//         [SerializeField]
//         private string m_EntryKey;
//
//         /// <summary>
//         /// 内容
//         /// </summary>
//         [HideLabel]
//         [NonSerialized]
//         public string Value;
//
//         /// <summary>
//         /// 获取本地化文本
//         /// </summary>
//         /// <returns>本地化文本</returns>
//         public string GetValue()
//         {
//             return CoreMain.Loc.GS(m_EntryKey);
//         }
//     }
// }
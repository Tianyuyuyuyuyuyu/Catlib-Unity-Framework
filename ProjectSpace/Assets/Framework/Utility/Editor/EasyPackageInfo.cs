using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Framework.Utility.Editor
{
    [Serializable]
    public class EasyPackageInfo
    {
        public string name;

        public string version;

        public string displayName;

        public string description;

        public string unity;

        /// <summary>返回表示当前对象的字符串。</summary>
        /// <returns>表示当前对象的字符串。</returns>
        public override string ToString()
        {
            List<string> lines = new List<string>();
            
            AppendMember(lines, nameof(name), name);
            AppendMember(lines, nameof(version), version);
            AppendMember(lines, nameof(displayName), displayName);
            AppendMember(lines, nameof(description), description);
            AppendMember(lines, nameof(unity), unity);

            
            var max = lines.Max((s => EditorStyles.label.CalcSize(new GUIContent(s)).x));
            var row = new string('-', Mathf.CeilToInt(max/4.5f));
            lines.Insert(0, row);
            lines.Add(row);

            string value = string.Empty;
            foreach (var line in lines)
            {
                value += line + "\n";
            }

            return value;
        }

        /// <summary>
        /// 新增成员
        /// </summary>
        /// <param name="lines">行列表</param>
        /// <param name="name">成员名称</param>
        /// <param name="o">成员</param>
        private void AppendMember(List<string> lines, string name, object o)
        {
            lines.Add($"{name} : {o}");
        }
    }
}

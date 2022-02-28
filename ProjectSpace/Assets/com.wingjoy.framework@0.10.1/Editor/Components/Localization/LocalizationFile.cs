using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System.IO;
using System.Text;
using System.Threading;
using UnityEditor;
using UnityEngine;
using Wingjoy.Framework.Runtime.Localization;

namespace Wingjoy.Framework.Editor.Localization
{
    [GlobalConfig("WingjoyData/Framework/Localization")]
    public class LocalizationFile : GlobalConfig<LocalizationFile>
    {
        private Thread m_TranslateThread;

        [HideLabel, PreviewField(120, ObjectFieldAlignment.Left), HorizontalGroup("Split",120)]
        public TextAsset TextAsset;
        
        [VerticalGroup("Split/Meta"),MinValue(100000)]
        public int IntervalTime;
        
        [VerticalGroup("Split/Meta")]
        public Language SourceLanguage;

        [VerticalGroup("Split/Meta")]
        public Language TargetLanguage;

        public char Separator;

        [ProgressBar(0, 100), ReadOnly]
        public float Progress; 

        [VerticalGroup("Split/Meta"),Button(ButtonSizes.Large, Name = "开始翻译")]
        public void StartTranslate()
        {
            if (m_TranslateThread != null)
            {
                Debug.Log(m_TranslateThread.ThreadState);
                if (m_TranslateThread.IsAlive)
                {
                    EditorUtility.DisplayDialog("", "正在翻译中", "确定");
                    return;
                }
                m_TranslateThread.Abort();
            }

            if (IntervalTime < 1000)
            {
                EditorUtility.DisplayDialog("警告", "间隔时间太短，易被google封禁，间隔时间必须大于1秒", "确定");
                return;
            }

//            if (string.IsNullOrEmpty(SourceLanguage))
//            {
//                EditorUtility.DisplayDialog("警告", "源语言未选定", "确定");
//                return;
//            }
//            if (string.IsNullOrEmpty(TargetLanguage))
//            {
//                EditorUtility.DisplayDialog("警告", "目标语言未选定", "确定");
//                return;
//            }
            if (TextAsset == null)
            {
                EditorUtility.DisplayDialog("警告", "未选择文件", "确定");
                return;
            }
            
            string filePath = Application.dataPath.Replace("Assets",string.Empty) + AssetDatabase.GetAssetPath(TextAsset);
            
            m_TranslateThread = new Thread(() =>
            {
                StringBuilder outPut = new StringBuilder(1024);
                var allLines = File.ReadAllLines(filePath);
                long totalLineCount = allLines.Length;
                long currentLineCount = 0;
                Progress = 0;
                foreach (var line in allLines)
                {
                    var strs = line.Split(Separator);
                    string translate;
                    if (strs.Length == 2)
                    {
                        translate = TranslatorOverview.Instance.DefaultTranslator.Translate(SourceLanguage, TargetLanguage, strs[0]);
                        var result = strs[0] + Separator+ translate;
                        outPut.Append(result);
                        Debug.Log(strs[0]+"->"+translate);
                    }
                    else
                    {
                        translate = TranslatorOverview.Instance.DefaultTranslator.Translate(SourceLanguage, TargetLanguage, line);
                        outPut.Append(line);
                        Debug.Log(line + "->" + translate);
                    }
                    
                    outPut.AppendLine();

                    currentLineCount++;
                    Progress = currentLineCount *100f/ totalLineCount;

                    Thread.Sleep(IntervalTime);
                }
                File.WriteAllText(LocalizationHelper.AddSuffixToFileName(filePath, "_" + TargetLanguage), outPut.ToString());
            });
            m_TranslateThread.Start();
        }

        [VerticalGroup("Split/Meta"),Button(ButtonSizes.Large, Name = "停止翻译")]
        public void AbortTranslate()
        {
            if (m_TranslateThread != null)
            {
                if (m_TranslateThread.IsAlive)
                {
                    m_TranslateThread.Abort();
                }
            }
        }


        /*[HideLabel, PreviewField(120, ObjectFieldAlignment.Left), HorizontalGroup("Split", 120)]
        public TextAsset TextAsset;*/
    }
}

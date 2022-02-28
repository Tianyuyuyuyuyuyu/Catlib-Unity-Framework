using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BDFramework.Core.Tools;
using BDFramework.Editor;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;

namespace BDFramework.Editor.EditorLife
{
    /// <summary>
    /// 这个类用以编辑器环境下辅助BD生命周期的开发
    /// </summary>
    [InitializeOnLoad]
    static public class WingjoyFrameEditorLife
    {
        static WingjoyFrameEditorLife()
        {
            EditorApplication.playModeStateChanged += OnPlayExit;
        }

        /// <summary>
        /// 代码编译完成后
        /// </summary>
        [UnityEditor.Callbacks.DidReloadScripts(0)]
        static void OnScriptReload()
        {
            OnCodeBuildComplete();
        }

        /// <summary>
        /// 退出播放模式
        /// </summary>
        /// <param name="state"></param>
        static private void OnPlayExit(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                InitFrameEditor();
            }
        }

        /// <summary>
        /// Editor代码刷新后执行
        /// </summary>
        static public void OnCodeBuildComplete()
        {
            if (EditorApplication.isPlaying)
            {
                return;
            }

            InitFrameEditor();
        }

        /// <summary>
        /// 初始化框架编辑器
        /// </summary>
        static public void InitFrameEditor()
        {
            //BD生命周期启动
            BDApplication.Init();
        }
    }
}
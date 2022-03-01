using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Utilities;

namespace Framework.Editor.ILRuntime
{
    [GlobalConfig("Assets/FrameworkData/FrameworkMono")]
    public class BuildDllConfig : GlobalConfig<BuildDllConfig>
    {
        /// <summary>
        /// 项目
        /// </summary>
        public List<CsProject> CsProjects;

        [Serializable]
        public class CsProject
        {
            /// <summary>
            /// 项目路径
            /// </summary>
            [FilePath(Extensions = ".csproj", RequireExistingPath = true)]
            public string Path;

            /// <summary>
            /// 集成Base
            /// </summary>
            public bool Base;
            
            /// <summary>
            /// 集成HotFix
            /// </summary>
            public bool HotFix;

            /// <summary>
            /// 集成DLL
            /// </summary>
            public bool Dll;
            
            /// <summary>
            /// 黑名单
            /// </summary>
            [FilePath]
            public List<string> BlackCsProject;
        }
    }
}
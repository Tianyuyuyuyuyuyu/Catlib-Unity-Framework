using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using Wingjoy.Framework.Runtime;
using Wingjoy.Framework.Runtime.HotFix.UI;
using Wingjoy.Framework.Runtime.UI;
using Object = UnityEngine.Object;

namespace Wingjoy.Framework.Editor
{
    [GlobalConfig("Assets/WingjoyData/Framework")]
    public class UIFormCreator : GlobalConfig<UIFormCreator>
    {
        /// <summary>
        /// 命名空间
        /// </summary>
        public string NameSpace = "UIForm";

        /// <summary>
        /// 生成路径
        /// </summary>
        [FolderPath]
        public string ScriptPath = "Assets/Script/UIForm";

        /// <summary>
        /// 窗体数据
        /// </summary>
        public List<UIFormData> UiFormDataList;

        /// <summary>
        /// 生成所有窗体
        /// </summary>
        [Button(ButtonSizes.Large), LabelText("生成全部脚本")]
        public void Generate()
        {
            foreach (var uiFormData in UiFormDataList)
            {
                uiFormData.CheckData();
                uiFormData.CreateScriptPlaceHolder();
            }

            AssetDatabase.Refresh();

            if (!EditorApplication.isCompiling)
            {
                OnScriptReload();
            }
        }

        /// <summary>
        /// 监听哪些脚本需要实装
        /// </summary>
        [DidReloadScripts]
        public static void OnScriptReload()
        {
            LoadInstanceIfAssetExists();
            if (HasInstanceLoaded)
            {
                var instanceUiFormDataList = Instance.UiFormDataList;
                int count = instanceUiFormDataList.Count;
                foreach (var uiFormData in instanceUiFormDataList)
                {
                    if (uiFormData.Status == CreateStatus.CreateScriptPlaceHolder)
                    {
                        uiFormData.HandleReplacePlaceHolder();
                    }
                    else if (uiFormData.Status == CreateStatus.HandleReplacePlaceHolder)
                    {
                        uiFormData.CreateScript();
                    }
                    // else if (uiFormData.Status == CreateStatus.CreateScript)
                    // {
                    //     //uiFormData.Install();
                    // }
                    else if (uiFormData.Status == CreateStatus.CreateScript)
                    {
                        Debug.Log($"{uiFormData.UiFormPrefab?.name}完毕");
                        count--;
                        uiFormData.Status = CreateStatus.Complete;
                    }
                    else if (uiFormData.Status == CreateStatus.Complete)
                    {
                        count--;
                    }
                }

                EditorUtility.SetDirty(Instance);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                if (!EditorApplication.isCompiling && count > 0)
                {
                    OnScriptReload();
                }
            }
        }

        [Serializable]
        public class UIFormData
        {
            /// <summary>
            /// 窗体
            /// </summary>
            [HorizontalGroup("Meta"), HideLabel]
            public GameObject UiFormPrefab;

            /// <summary>
            /// 基础类型
            /// </summary>
            [ValueDropdown("GetBaseTypeList")]
            public string BaseType = typeof(UIFormBase).FullName;

            /// <summary>
            /// UIGroup关系
            /// </summary>
            private Dictionary<UIGroupPlaceholder, UIPlaceholder> uiGroupBaseRelation = new Dictionary<UIGroupPlaceholder, UIPlaceholder>();

            /// <summary>
            /// UIField关系
            /// </summary>
            private Dictionary<UIField, UIPlaceholder> uiFieldRelation = new Dictionary<UIField, UIPlaceholder>();

            /// <summary>
            /// 创建状态
            /// </summary>
            [ReadOnly]
            public CreateStatus Status;

            /// <summary>
            /// 获取所有满足条件的类型
            /// </summary>
            /// <returns>类型列表</returns>
            public IEnumerable GetBaseTypeList()
            {
                return AssemblyUtilities.GetTypes(AssemblyTypeFlags.CustomTypes)
                    .Where((type => type.InheritsFrom<UIFormBase>() && type.GetCustomAttribute<UIFormBaseAttribute>() != null))
                    .Select((type => new ValueDropdownItem(type.Name, type.FullName)));
            }

            /// <summary>
            /// 验证数据是否可行
            /// </summary>
            public void CheckData()
            {
                if (!AssemblyUtilities.GetTypes(AssemblyTypeFlags.CustomTypes).Any((type => type.FullName == BaseType)))
                {
                    throw new Exception($"{BaseType} 不存在，无法创建窗体");
                }
            }

            /// <summary>
            /// 计算关系
            /// </summary>
            [HorizontalGroup("Meta"), Button(Name = "分析关系")]
            public void CalcRelationship()
            {
                uiGroupBaseRelation.Clear();
                var uiGroupBases = UiFormPrefab.GetComponentsInChildren<UIGroupPlaceholder>(true);
                foreach (var uiGroupBase in uiGroupBases)
                {
                    var findValidParent = FindValidParent(uiGroupBase.gameObject);
                    if (findValidParent != null)
                    {
                        uiGroupBaseRelation.Add(uiGroupBase, findValidParent);
                    }
                }

                uiFieldRelation.Clear();
                var uiFields = UiFormPrefab.GetComponentsInChildren<UIField>(true);
                foreach (var uiField in uiFields)
                {
                    var uiGroupBase = uiField.GetComponent<UIGroupPlaceholder>();
                    if (uiGroupBase != null)
                    {
                        uiFieldRelation.Add(uiField, uiGroupBase);
                        continue;
                    }

                    var uiFormBase = uiField.GetComponent<UIFormBasePlaceholder>();
                    if (uiFormBase != null)
                    {
                        uiFieldRelation.Add(uiField, uiFormBase);
                        continue;
                    }

                    var findValidParent = FindValidParent(uiField.gameObject);
                    if (findValidParent != null)
                    {
                        uiFieldRelation.Add(uiField, findValidParent);
                        continue;
                    }
                }
            }

            /// <summary>
            /// 寻找到带有UIGroupBase或者UIFormBase的有效物体
            /// </summary>
            /// <returns>有效物体</returns>
            public UIPlaceholder FindValidParent(GameObject go)
            {
                if (go == null)
                    return null;

                var transformParent = go.transform.parent;
                if (transformParent != null)
                {
                    var parentGameObject = transformParent.gameObject;
                    var uiGroupBase = parentGameObject.GetComponent<UIGroupPlaceholder>();
                    if (uiGroupBase != null)
                    {
                        return uiGroupBase;
                    }

                    var uiFormBase = parentGameObject.GetComponent<UIFormBasePlaceholder>();
                    if (uiFormBase != null)
                    {
                        return uiFormBase;
                    }

                    return FindValidParent(parentGameObject);
                }
                else
                {
                    return null;
                }
            }

            /// <summary>
            /// 获取Transform.Find路径
            /// </summary>
            /// <param name="child">子</param>
            /// <param name="parent">父</param>
            /// <returns>Transform.Find路径</returns>
            public string GetTransformFindPath(Transform child, Transform parent)
            {
                List<string> names = new List<string>();
                Transform current = child;
                while (current != null)
                {
                    if (current == parent)
                    {
                        break;
                    }
                    else
                    {
                        names.Add(current.name);
                        current = current.parent;
                    }
                }

                string path = string.Empty;
                for (var index = names.Count - 1; index >= 0; index--)
                {
                    var s = names[index];
                    path += s;
                    if (index > 0)
                    {
                        path += "/";
                    }
                }

                return path;
            }

            [Serializable]
            private class ReplacePlaceHolder
            {
                /// <summary>
                /// 实例ID
                /// </summary>
                public string Guid;

                /// <summary>
                /// 类名
                /// </summary>
                public string ClassName;

                /// <summary>
                ///   初始化 <see cref="T:System.Object" /> 类的新实例。
                /// </summary>
                public ReplacePlaceHolder(string guid, string className)
                {
                    Guid = guid;
                    ClassName = className;
                }
            }

            /// <summary>
            /// 替换
            /// </summary>
            [SerializeField]
            [HideInInspector]
            private List<ReplacePlaceHolder> m_ReplacePlaceHolders = new List<ReplacePlaceHolder>();

            /// <summary>
            /// 创建脚本占位
            /// </summary>
            [HorizontalGroup("Meta"), Button(Name = "替换脚本占位")]
            public void CreateScriptPlaceHolder()
            {
                Status = CreateStatus.CreateScriptPlaceHolder;
                if (UiFormPrefab != null)
                {
                    var folder = Instance.ScriptPath;

                    var enumerable = AssemblyUtilities.GetTypes(AssemblyTypeFlags.CustomTypes).ToList();
                    m_ReplacePlaceHolders.Clear();
                    var uiPlaceholders = UiFormPrefab.GetComponentsInChildren<UIPlaceholder>(true);
                    foreach (var placeholder in uiPlaceholders)
                    {
                        if (string.IsNullOrEmpty(placeholder.RuntimeUiGroupType) || !enumerable.Any((type => type.Name == placeholder.RuntimeUiGroupType)))
                        {
                            string className = string.Empty;
                            if (placeholder is UIGroupPlaceholder uiGroupPlaceholder)
                            {
                                if (uiGroupPlaceholder.Prefab)
                                {
                                    var type = Type.GetType(uiGroupPlaceholder.RuntimeUiGroupType);
                                    if (type != null)
                                    {
                                        continue;
                                    }

                                    CreateScriptPlaceHolder($"{folder}/Prefab", placeholder.RuntimeUiGroupType, Instance.NameSpace, typeof(UIGroupBase));
                                }
                                else
                                {
                                    className = $"{placeholder.name}UIGroup";
                                    string nameSpace = $"{Instance.NameSpace}.{UiFormPrefab.name}NS";
                                    CreateScriptPlaceHolder($"{folder}/{UiFormPrefab.name}", className, nameSpace, typeof(UIGroupBase));
                                }
                            }
                            else
                            {
                                className = $"{placeholder.name}";
                                CreateScriptPlaceHolder($"{folder}/{UiFormPrefab.name}", className, Instance.NameSpace, AssemblyUtilities.GetTypeByCachedFullName(BaseType));
                            }

                            m_ReplacePlaceHolders.Add(new ReplacePlaceHolder(placeholder.Guid, className));
                        }
                    }

                    EditorUtility.SetDirty(Instance);
                    AssetDatabase.SaveAssets();
                }
            }

            /// <summary>
            /// 处理替换脚本
            /// </summary>
            public void HandleReplacePlaceHolder()
            {
                Status = CreateStatus.HandleReplacePlaceHolder;
                //var instantiate = Object.Instantiate(UiFormPrefab);
                var enumerable = AssemblyUtilities.GetTypes(AssemblyTypeFlags.CustomTypes).ToList();
                var uiGroupPlaceholders = UiFormPrefab.GetComponentsInChildren<UIPlaceholder>(true);
                foreach (var replacePlaceHolder in m_ReplacePlaceHolders)
                {
                    var uiGroupPlaceholder = uiGroupPlaceholders.FirstOrDefault((placeholder => placeholder.Guid == replacePlaceHolder.Guid));
                    if (uiGroupPlaceholder != null)
                    {
                        var first = enumerable.FirstOrDefault((type => type.Name == replacePlaceHolder.ClassName));
                        if (first != null)
                        {
                            uiGroupPlaceholder.SetRuntimeUIGroupType(first.Name);
                        }
                    }
                }

                m_ReplacePlaceHolders.Clear();
                EditorUtility.SetDirty(UiFormPrefab);
                //PrefabUtility.SaveAsPrefabAssetAndConnect(instantiate, AssetDatabase.GetAssetPath(UiFormPrefab), InteractionMode.AutomatedAction);
                //DestroyImmediate(instantiate);
            }

            private static CodeCompileUnit CodeCompileUnit(string className, string nameSpace, Type baseType, out CodeTypeDeclaration fieldClass)
            {
                CodeCompileUnit unit = new CodeCompileUnit();
                CodeNamespace sampleNamespace = new CodeNamespace(nameSpace);
                sampleNamespace.Imports.Add(new CodeNamespaceImport("System"));
                fieldClass = new CodeTypeDeclaration(className);
                fieldClass.BaseTypes.Add(new CodeTypeReference(baseType));
                fieldClass.TypeAttributes = TypeAttributes.Public;
                fieldClass.IsClass = true;
                fieldClass.IsPartial = true;

                sampleNamespace.Types.Add(fieldClass);
                unit.Namespaces.Add(sampleNamespace);
                return unit;
            }

            /// <summary>
            /// 创建脚本占位符
            /// </summary>
            /// <param name="folder">文件夹</param>
            /// <param name="className">类名</param>
            /// <param name="nameSpace">命名空间</param>
            /// <param name="baseType">基类</param>
            public void CreateScriptPlaceHolder(string folder, string className, string nameSpace, Type baseType)
            {
                string fieldScript = $"{folder}/{className}.cs";
                string logicScript = $"{folder}/{className}Logic.cs";

                var unit = CodeCompileUnit(className, nameSpace, baseType, out var fieldClass);

                CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");

                if (!File.Exists(logicScript))
                {
                    var logicUnit = CodeCompileUnit(className, nameSpace, baseType, out var logicFieldClass);
                    if (baseType != typeof(UIGroupBase))
                    {
                        logicFieldClass.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(UIPrefabName)), new CodeAttributeArgument("Name", new CodePrimitiveExpression(className))));
                    }

                    CodeGeneratorOptions autoScriptOptions = new CodeGeneratorOptions();
                    autoScriptOptions.BracingStyle = "C";
                    autoScriptOptions.BlankLinesBetweenMembers = true;

                    var directoryName = Path.GetDirectoryName(logicScript);
                    if (!Directory.Exists(directoryName))
                    {
                        Directory.CreateDirectory(directoryName);
                    }

                    using (System.IO.StreamWriter sw = new System.IO.StreamWriter(logicScript))
                    {
                        provider.GenerateCodeFromCompileUnit(logicUnit, sw, autoScriptOptions);
                    }
                }

                //生成字段
                CodeGeneratorOptions logicScriptOptions = new CodeGeneratorOptions();
                logicScriptOptions.BracingStyle = "C";
                fieldClass.Comments.Add(new CodeCommentStatement("当前文件自动生成，禁止修改", true));
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(fieldScript))
                {
                    provider.GenerateCodeFromCompileUnit(unit, sw, logicScriptOptions);
                }
            }

            /// <summary>
            /// 创建脚本
            /// </summary>
            [HorizontalGroup("Meta"), Button(Name = "构建脚本")]
            public void CreateScript()
            {
                Status = CreateStatus.CreateScript;
                CalcRelationship();
                var folder = Instance.ScriptPath;
                string fieldScript = $"{folder}/{UiFormPrefab.name}/{UiFormPrefab.name}.cs";
                var directoryName = Path.GetDirectoryName(fieldScript);
                if (!Directory.Exists(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }

                int reallyUIGroupCount = 0;
                foreach (var keyValuePair in uiGroupBaseRelation)
                {
                    string tempFolder;

                    if (keyValuePair.Key.Prefab)
                    {
                        tempFolder = $"{folder}/Prefab";
                    }
                    else
                    {
                        tempFolder = $"{folder}/{UiFormPrefab.name}";
                        reallyUIGroupCount++;
                    }

                    CreateUIGroup(tempFolder, keyValuePair.Key, UiFormPrefab);
                }

                CreateUIForm(folder, UiFormPrefab, reallyUIGroupCount > 0);
            }

            #region AutoCodeCreate

            /// <summary>
            /// 创建重写方法
            /// </summary>
            /// <param name="codeTypeDeclaration">类</param>
            /// <param name="methodName">方法名</param>
            /// <param name="args">方法参数</param>
            /// <returns>方法</returns>
            public CodeMemberMethod CreateOverrideMethod(CodeTypeDeclaration codeTypeDeclaration, string methodName, params CodeParameterDeclarationExpression[] args)
            {
                var updateSubUIGroupMethod = new CodeMemberMethod();
                updateSubUIGroupMethod.Name = methodName;
                updateSubUIGroupMethod.Attributes = MemberAttributes.Override | MemberAttributes.Public;
                updateSubUIGroupMethod.Parameters.AddRange(args);
                updateSubUIGroupMethod.ReturnType = new CodeTypeReference(typeof(void));
                codeTypeDeclaration.Members.Add(updateSubUIGroupMethod);
                updateSubUIGroupMethod.Statements.Add(CreateInvokeBaseMethodExpression(updateSubUIGroupMethod));

                return updateSubUIGroupMethod;
            }

            /// <summary>
            /// 创建UIGroup字段
            /// </summary>
            /// <param name="uiGroupBase">uiGroup</param>
            /// <returns>字段</returns>
            public CodeMemberField CreateUIGroupMember(UIGroupPlaceholder uiGroupBase)
            {
                var name = $"{uiGroupBase.name}UIGroup";
                CodeMemberField field = new CodeMemberField(uiGroupBase.RuntimeUiGroupType, $"m_{name}");
                field.Comments.Add(new CodeCommentStatement(Utility.Path.GetPathWithoutCanvasEnvironment(uiGroupBase.transform), true));
                field.Attributes = MemberAttributes.Private;
                field.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(SerializeField))));
                var boxGroupAttributeType = new CodeTypeReference(typeof(BoxGroupAttribute));
                var boxGroupAttribute = new CodeAttributeDeclaration(boxGroupAttributeType);
                boxGroupAttribute.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression("窗体字段")));
                field.CustomAttributes.Add(boxGroupAttribute);
                return field;
            }

            /// <summary>
            /// 创建组件字段
            /// </summary>
            /// <param name="component">组件</param>
            /// <param name="specifyName">指定名称</param>
            /// <returns>字段</returns>
            public CodeMemberField CreateFieldMember(Component component, string specifyName = "")
            {
                string name;
                if (string.IsNullOrEmpty(specifyName))
                {
                    name = $"{component.name}{component.GetType().GetNiceName()}";
                }
                else
                {
                    name = $"{specifyName}{component.GetType().GetNiceName()}";
                }

                CodeMemberField field = new CodeMemberField(component.GetType(), $"m_{name}");
                field.Comments.Add(new CodeCommentStatement(Utility.Path.GetPathWithoutCanvasEnvironment(component.transform), true));
                field.Attributes = MemberAttributes.Private;
                field.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(SerializeField))));
                var boxGroupAttributeType = new CodeTypeReference(typeof(BoxGroupAttribute));
                var boxGroupAttribute = new CodeAttributeDeclaration(boxGroupAttributeType);
                boxGroupAttribute.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression("窗体字段")));
                field.CustomAttributes.Add(boxGroupAttribute);
                return field;
            }

            /// <summary>
            /// 创建组件字段
            /// </summary>
            /// <param name="transform">Transform</param>
            /// <param name="type">类型</param>
            /// <returns>字段</returns>
            public CodeMemberField CreateParentFieldMember(Transform transform, string type)
            {
                CodeMemberField parentField = new CodeMemberField(type, $"m_Parent");
                parentField.Comments.Add(new CodeCommentStatement(Utility.Path.GetPathWithoutCanvasEnvironment(transform), true));
                parentField.Attributes = MemberAttributes.Private;
                parentField.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(SerializeField))));
                var boxGroupAttributeType = new CodeTypeReference(typeof(BoxGroupAttribute));
                var boxGroupAttribute = new CodeAttributeDeclaration(boxGroupAttributeType);
                boxGroupAttribute.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression("窗体字段")));
                parentField.CustomAttributes.Add(boxGroupAttribute);
                return parentField;
            }

            /// <summary>
            /// 创建属性
            /// </summary>
            /// <param name="codeMemberField">字段</param>
            /// <returns>属性</returns>
            public CodeMemberProperty CreateProperty(CodeMemberField codeMemberField)
            {
                CodeMemberProperty property = new CodeMemberProperty();
                property.HasGet = true;
                property.HasSet = true;
                property.Type = codeMemberField.Type;
                property.Name = codeMemberField.Name.Substring(0, 2);
                return property;
            }

            /// <summary>
            /// 执行基函数逻辑
            /// </summary>
            /// <param name="codeMemberMethod">函数</param>
            /// <returns>语句</returns>
            public CodeExpression CreateInvokeBaseMethodExpression(CodeMemberMethod codeMemberMethod)
            {
                CodeMethodReferenceExpression invokeBase = new CodeMethodReferenceExpression(new CodeBaseReferenceExpression(), codeMemberMethod.Name);

                List<CodeExpression> expressions = new List<CodeExpression>();
                foreach (CodeParameterDeclarationExpression parameter in codeMemberMethod.Parameters)
                {
                    expressions.Add(new CodeVariableReferenceExpression(parameter.Name));
                }

                return new CodeMethodInvokeExpression(invokeBase, expressions.ToArray());
            }

            /// <summary>
            /// 创建组件赋值语句
            /// </summary>
            /// <param name="codeMemberField">字段</param>
            /// <param name="path">路径</param>
            /// <returns>赋值语句</returns>
            public CodeAssignStatement CreateComponentAssignStatement(CodeMemberField codeMemberField, string path)
            {
                CodeExpression target = null;
                if (string.IsNullOrEmpty(path))
                {
                    target = new CodeVariableReferenceExpression("Transform");
                }
                else
                {
                    CodeMethodReferenceExpression find = new CodeMethodReferenceExpression(new CodeVariableReferenceExpression("Transform"), "Find");
                    target = new CodeMethodInvokeExpression(find, new CodePrimitiveExpression(path));
                }

                CodeMethodReferenceExpression get = new CodeMethodReferenceExpression(target, "GetComponent", new CodeTypeReference(codeMemberField.Type.BaseType));
                CodeMethodInvokeExpression invokeGet = new CodeMethodInvokeExpression(get, new CodeExpression[0]);
                CodeAssignStatement codeAssignStatement = new CodeAssignStatement(new CodeArgumentReferenceExpression(codeMemberField.Name), invokeGet);
                return codeAssignStatement;
            }

            /// <summary>
            /// 创建UIGroup赋值语句
            /// </summary>
            /// <param name="field">字段</param>
            /// <returns>赋值语句</returns>
            public CodeStatement CreateCreateUIGroupAssignStatement(CodeMemberField field)
            {
                CodeMethodReferenceExpression createInstance = new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(typeof(Activator)), "CreateInstance", new CodeTypeReference(field.Type.BaseType));
                CodeMethodInvokeExpression invokeCreateInstance = new CodeMethodInvokeExpression(createInstance, new CodeExpression[0]);
                CodeAssignStatement codeAssignStatement = new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), field.Name), invokeCreateInstance);
                return codeAssignStatement;
            }

            /// <summary>
            /// 创建调用BindGameObject语句
            /// </summary>
            /// <param name="field">字段</param>
            /// <param name="path">路径</param>
            /// <returns>语句</returns>
            public CodeExpression CreateUiGroupBindGameObjectExpression(CodeMemberField field, string path)
            {
                CodeExpression target = null;
                if (string.IsNullOrEmpty(path))
                {
                    target = new CodeTypeReferenceExpression("Transform");
                }
                else
                {
                    CodeMethodReferenceExpression find = new CodeMethodReferenceExpression(new CodeTypeReferenceExpression("Transform"), "Find");
                    target = new CodeMethodInvokeExpression(find, new CodePrimitiveExpression(path));
                }

                CodeMethodReferenceExpression bindGameObject = new CodeMethodReferenceExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), field.Name), "BindGameObject");
                CodeMethodInvokeExpression invokeBindGameObject = new CodeMethodInvokeExpression(bindGameObject, new CodeMethodReferenceExpression(target, "gameObject"));
                return invokeBindGameObject;
            }

            /// <summary>
            /// 创建绑定父级语句
            /// </summary>
            /// <param name="field">字段</param>
            /// <returns>语句</returns>
            public CodeExpression CreateBindParentCodeExpression(CodeMemberField field)
            {
                CodeMethodReferenceExpression bindParent = new CodeMethodReferenceExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), field.Name), "BindParent");
                CodeMethodInvokeExpression invokeBindGameObject = new CodeMethodInvokeExpression(bindParent, new CodeThisReferenceExpression());
                return invokeBindGameObject;
            }

            /// <summary>
            /// 创建初始化语句
            /// </summary>
            /// <param name="field">字段</param>
            /// <param name="methodName"></param>
            /// <param name="parameterName"></param>
            /// <returns>语句</returns>
            public CodeExpression CreateCodeExpression(CodeMemberField field, string methodName, params string[] parameterName)
            {
                CodeMethodReferenceExpression expression = new CodeMethodReferenceExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), field.Name), methodName);

                CodeArgumentReferenceExpression[] argumentReferenceExpressions = new CodeArgumentReferenceExpression[parameterName.Length];
                for (var index = 0; index < parameterName.Length; index++)
                {
                    var s = parameterName[index];
                    argumentReferenceExpressions[index] = new CodeArgumentReferenceExpression(s);
                }

                CodeMethodInvokeExpression invokeExpression = new CodeMethodInvokeExpression(expression, argumentReferenceExpressions);
                return invokeExpression;
            }

            #endregion

            /// <summary>
            /// 创建UIGroup
            /// </summary>
            /// <param name="folder">文件夹</param>
            /// <param name="uiFormPrefab">占位符</param>
            /// <param name="importSubNs">是否需要引用子命名空间</param>
            public void CreateUIForm(string folder, GameObject uiFormPrefab, bool importSubNs)
            {
                string className = $"{uiFormPrefab.name}";
                string fieldScript = $"{folder}/{UiFormPrefab.name}/{className}.cs";
                string logicScript = $"{folder}/{UiFormPrefab.name}/{className}Logic.cs";

                CodeCompileUnit unit = new CodeCompileUnit();
                CodeNamespace sampleNamespace = new CodeNamespace(Instance.NameSpace);
                sampleNamespace.Imports.Add(new CodeNamespaceImport("System"));
                if (importSubNs)
                {
                    sampleNamespace.Imports.Add(new CodeNamespaceImport($"{Instance.NameSpace}.{uiFormPrefab.name}NS"));
                }

                CodeTypeDeclaration fieldClass = new CodeTypeDeclaration(className);
                fieldClass.BaseTypes.Add(new CodeTypeReference(AssemblyUtilities.GetTypeByCachedFullName(BaseType)));
                fieldClass.TypeAttributes = TypeAttributes.Public;
                fieldClass.IsClass = true;
                fieldClass.IsPartial = true;

                sampleNamespace.Types.Add(fieldClass);
                unit.Namespaces.Add(sampleNamespace);

                //生成代码
                CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
                if (!File.Exists(logicScript))
                {
                    CodeGeneratorOptions autoScriptOptions = new CodeGeneratorOptions();
                    autoScriptOptions.BracingStyle = "C";
                    autoScriptOptions.BlankLinesBetweenMembers = true;
                    using (System.IO.StreamWriter sw = new System.IO.StreamWriter(logicScript))
                    {
                        provider.GenerateCodeFromCompileUnit(unit, sw, autoScriptOptions);
                    }
                }

                //生成字段
                CodeGeneratorOptions logicScriptOptions = new CodeGeneratorOptions();
                logicScriptOptions.BracingStyle = "C";
                fieldClass.Comments.Add(new CodeCommentStatement("当前文件自动生成，禁止修改", true));

                var installFieldMethod = CreateOverrideMethod(fieldClass, "InstallField");
                var initSubUIGroupMethod = CreateOverrideMethod(fieldClass, "InitSubUIGroup", new CodeParameterDeclarationExpression(typeof(object), "userData"));
                var updateSubUIGroupMethod = CreateOverrideMethod(fieldClass, "UpdateSubUIGroup");
                var openSubUIGroupMethod = CreateOverrideMethod(fieldClass, "OpenSubUIGroup", new CodeParameterDeclarationExpression(typeof(object), "userData"));
                var disposeSubUIGroupMethod = CreateOverrideMethod(fieldClass, "DisposeSubUIGroup");

                var uiFormPrefabGameObject = uiFormPrefab.gameObject;
                var uiGroups = uiGroupBaseRelation.Where((pair => pair.Value.gameObject == uiFormPrefabGameObject));
                foreach (var group in uiGroups)
                {
                    var codeMemberField = CreateUIGroupMember(@group.Key);
                    fieldClass.Members.Add(codeMemberField);
                    installFieldMethod.Statements.Add(CreateCreateUIGroupAssignStatement(codeMemberField));
                    installFieldMethod.Statements.Add(CreateUiGroupBindGameObjectExpression(codeMemberField, GetTransformFindPath(group.Key.transform, group.Value.transform)));
                    installFieldMethod.Statements.Add(CreateBindParentCodeExpression(codeMemberField));

                    initSubUIGroupMethod.Statements.Add(CreateCodeExpression(codeMemberField, "OnInit", "userData"));
                    updateSubUIGroupMethod.Statements.Add(CreateCodeExpression(codeMemberField, "OnUpdate"));
                    openSubUIGroupMethod.Statements.Add(CreateCodeExpression(codeMemberField, "OnFormOpen", "userData"));
                    disposeSubUIGroupMethod.Statements.Add(CreateCodeExpression(codeMemberField, "Release"));
                }

                var uiFields = uiFieldRelation.Where((pair => pair.Value.gameObject == uiFormPrefabGameObject));

                foreach (var uiField in uiFields)
                {
                    foreach (var component in uiField.Key.Components)
                    {
                        var codeMemberField = CreateFieldMember(component);
                        fieldClass.Members.Add(codeMemberField);
                        installFieldMethod.Statements.Add(CreateComponentAssignStatement(codeMemberField, GetTransformFindPath(component.transform, uiField.Value.transform)));
                    }
                }

                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(fieldScript))
                {
                    provider.GenerateCodeFromCompileUnit(unit, sw, logicScriptOptions);
                }
            }

            /// <summary>
            /// 创建UIGroup
            /// </summary>
            /// <param name="folder">文件夹</param>
            /// <param name="uiGroupBase">占位符</param>
            /// <param name="uiFormPrefab">uiForm预制件</param>
            public void CreateUIGroup(string folder, UIGroupPlaceholder uiGroupBase, GameObject uiFormPrefab)
            {
                string className = uiGroupBase.RuntimeUiGroupType;
                string fieldScript = $"{folder}/{className}.cs";
                string logicScript = $"{folder}/{className}Logic.cs";

                CodeCompileUnit unit = new CodeCompileUnit();
                string nameSpace = $"{Instance.NameSpace}.{uiFormPrefab.name}NS";
                if (uiGroupBase.Prefab)
                {
                    nameSpace = Instance.NameSpace;
                }

                CodeNamespace sampleNamespace = new CodeNamespace(nameSpace);
                sampleNamespace.Imports.Add(new CodeNamespaceImport("System"));
                CodeTypeDeclaration fieldClass = new CodeTypeDeclaration(className);
                fieldClass.BaseTypes.Add(new CodeTypeReference(typeof(UIGroupBase)));
                fieldClass.TypeAttributes = TypeAttributes.Public;
                fieldClass.IsClass = true;
                fieldClass.IsPartial = true;

                sampleNamespace.Types.Add(fieldClass);
                unit.Namespaces.Add(sampleNamespace);

                //生成代码
                CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
                if (!File.Exists(logicScript))
                {
                    CodeGeneratorOptions autoScriptOptions = new CodeGeneratorOptions();
                    autoScriptOptions.BracingStyle = "C";
                    autoScriptOptions.BlankLinesBetweenMembers = true;
                    using (System.IO.StreamWriter sw = new System.IO.StreamWriter(logicScript))
                    {
                        provider.GenerateCodeFromCompileUnit(unit, sw, autoScriptOptions);
                    }
                }

                //生成字段
                CodeGeneratorOptions logicScriptOptions = new CodeGeneratorOptions();
                logicScriptOptions.BracingStyle = "C";
                fieldClass.Comments.Add(new CodeCommentStatement("当前文件自动生成，禁止修改", true));

                var installFieldMethod = CreateOverrideMethod(fieldClass, "InstallField");
                var initSubUIGroupMethod = CreateOverrideMethod(fieldClass, "InitSubUIGroup", new CodeParameterDeclarationExpression(typeof(object), "userData"));
                var updateSubUIGroupMethod = CreateOverrideMethod(fieldClass, "UpdateSubUIGroup");
                var openSubUIGroupMethod = CreateOverrideMethod(fieldClass, "OpenSubUIGroup", new CodeParameterDeclarationExpression(typeof(object), "userData"));
                var disposeSubUIGroupMethod = CreateOverrideMethod(fieldClass, "DisposeSubUIGroup");

                //创建父级
                if (uiGroupBaseRelation.TryGetValue(uiGroupBase, out var parent))
                {
                    var parentType = parent.RuntimeUiGroupType;
                    if (uiGroupBase.Prefab)
                    {
                        parentType = typeof(UIBase).ToString();
                    }

                    var parentField = CreateParentFieldMember(parent.transform, parentType);
                    fieldClass.Members.Add(parentField);

                    var bindParent = new CodeMemberMethod();
                    bindParent.Name = "BindParent";
                    bindParent.Attributes = MemberAttributes.Public;
                    bindParent.ReturnType = new CodeTypeReference(typeof(void));
                    var codeParameterDeclarationExpression = new CodeParameterDeclarationExpression(parentType, "parent");
                    bindParent.Parameters.Add(codeParameterDeclarationExpression);

                    CodeAssignStatement parentAssignStatement = new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), parentField.Name), new CodeVariableReferenceExpression("parent"));
                    bindParent.Statements.Add(parentAssignStatement);
                    fieldClass.Members.Add(bindParent);
                }

                var uiGroupGameObject = uiGroupBase.gameObject;
                var uiGroups = uiGroupBaseRelation.Where((pair => pair.Value.gameObject == uiGroupGameObject));
                foreach (var group in uiGroups)
                {
                    var codeMemberField = CreateUIGroupMember(@group.Key);
                    fieldClass.Members.Add(codeMemberField);
                    installFieldMethod.Statements.Add(CreateCreateUIGroupAssignStatement(codeMemberField));
                    installFieldMethod.Statements.Add(CreateUiGroupBindGameObjectExpression(codeMemberField, GetTransformFindPath(group.Key.transform, group.Value.transform)));
                    installFieldMethod.Statements.Add(CreateBindParentCodeExpression(codeMemberField));

                    initSubUIGroupMethod.Statements.Add(CreateCodeExpression(codeMemberField, "OnInit", "userData"));
                    updateSubUIGroupMethod.Statements.Add(CreateCodeExpression(codeMemberField, "OnUpdate"));
                    openSubUIGroupMethod.Statements.Add(CreateCodeExpression(codeMemberField, "OnFormOpen", "userData"));
                    disposeSubUIGroupMethod.Statements.Add(CreateCodeExpression(codeMemberField, "Release"));
                }

                var uiFields = uiFieldRelation.Where((pair => pair.Value.gameObject == uiGroupGameObject));

                foreach (var uiField in uiFields)
                {
                    foreach (var component in uiField.Key.Components)
                    {
                        CodeMemberField codeMemberField;

                        if (component == null)
                        {
                            Debug.LogError($"{uiFormPrefab} 中的component包含null");
                            continue;
                        }
                        
                        //如果是预制件，并且要被创建的组件是当前预制件的根节点，则使用预制件的名称，防止每个引用该预制件的名称不一致导致生成代码有问题
                        if (component.transform == uiGroupBase.transform && uiGroupBase.Prefab)
                        {
                            codeMemberField = CreateFieldMember(component, uiGroupBase.RuntimeUiGroupType);
                        }
                        else
                        {
                            codeMemberField = CreateFieldMember(component);
                        }

                        fieldClass.Members.Add(codeMemberField);
                        installFieldMethod.Statements.Add(CreateComponentAssignStatement(codeMemberField, GetTransformFindPath(component.transform, uiField.Value.transform)));
                    }
                }

                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(fieldScript))
                {
                    provider.GenerateCodeFromCompileUnit(unit, sw, logicScriptOptions);
                }
            }
        }

        public enum CreateStatus
        {
            CreateScriptPlaceHolder,
            HandleReplacePlaceHolder,
            CreateScript,

            //Install,
            Complete
        }
    }
}
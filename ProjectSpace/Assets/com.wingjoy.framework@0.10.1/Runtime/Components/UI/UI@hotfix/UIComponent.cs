using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Wingjoy.Framework.Runtime.UI;
using WingjoyUtility.Runtime;
using WinjoyFramework.Runtime.HotFix;

namespace Wingjoy.Framework.Runtime.HotFix.UI
{
    [ComponentConfig(typeof(UIConfig))]
    public class UIComponent : WingjoyFrameworkHotFixComponent
    {
        /// <summary>
        /// UI窗体池
        /// </summary>
        private Dictionary<Type, HandleUIForm> m_UIFormsPool;

        /// <summary>
        /// 正在打开的UI列表
        /// </summary>
        private List<HandleUIForm> m_UIFormsOpenPool;

        /// <summary>
        /// 背景遮罩
        /// </summary>
        private HandleUIForm m_MaskUIFormData;

        /// <summary>
        /// UI组件配置
        /// </summary>
        private UIConfig m_UiConfig;

        /// <summary>
        /// 消息委托字典
        /// </summary>
        private Dictionary<string, MessageReceiver> m_MessageReceivers;

        /// <summary>
        /// 被切换的界面
        /// </summary>
        private List<HandleUIForm> m_SwitchedUIForm;

        public UIComponent(UIConfig uiConfig)
        {
            m_UiConfig = uiConfig;
            m_UIFormsPool = new Dictionary<Type, HandleUIForm>();
            m_UIFormsOpenPool = new List<HandleUIForm>();
            m_MessageReceivers = new Dictionary<string, MessageReceiver>();
            m_SwitchedUIForm = new List<HandleUIForm>();
            var asyncOperationHandle = m_UiConfig.MaskUiFormPrefab.InstantiateAsync(m_UiConfig.StandBy);
            asyncOperationHandle.Completed += handle =>
            {
                HandleUIForm handleUiForm = new HandleUIForm();
                handleUiForm.AsyncOperationHandle = asyncOperationHandle;
                handleUiForm.Complete += ui =>
                {
                    handleUiForm.Status = UIFromStatus.Close;
                    handleUiForm.OpenMode = OpenMode.Cover;
                    m_MaskUIFormData = handleUiForm;
                };
                MaskUIForm maskUiForm = new MaskUIForm
                {
                    Lock = true
                };
                maskUiForm.BindGameObject(handle.Result);
                handleUiForm.OnComplete(maskUiForm, null);
                m_UIFormsPool.Add(typeof(MaskUIForm), handleUiForm);
            };
            Debug.Log("Create UIConfig");
        }

        public override void Update()
        {
            for (var index = m_UIFormsOpenPool.Count - 1; index >= 0; index--)
            {
                var openUIFormData = m_UIFormsOpenPool[index];
                openUIFormData.UIFormBase.OnUpdate();
            }
        }

        /// <summary>
        /// 添加窗体到正在打开的UI列表中
        /// </summary>
        /// <param name="handleUIForm">窗体数据</param>
        /// <param name="isPassive">是否被动</param>
        private void AppendUIFormToOpenPool(HandleUIForm handleUIForm, bool isPassive)
        {
            if (!isPassive)
            {
                if (handleUIForm.OpenMode == OpenMode.CloseOther)
                {
                    for (var i = m_UIFormsOpenPool.Count - 1; i >= 0; i--)
                    {
                        var data = m_UIFormsOpenPool[i];
                        if (data.UIFormBase.Lock)
                            continue;

                        Close(data.UIFormBase);
                    }
                }
                else if (handleUIForm.OpenMode == OpenMode.TopSwitch)
                {
                    var lastOrDefault = m_UIFormsOpenPool.LastOrDefault();
                    if (lastOrDefault != null)
                    {
                        //记录被切换的界面
                        handleUIForm.SwitchHandleUIForm = lastOrDefault;
                        m_SwitchedUIForm.Add(lastOrDefault);

                        Close(lastOrDefault.UIFormBase, ignoreOpenMode: true);
                    }
                }
            }

            m_UIFormsOpenPool.Add(handleUIForm);

            handleUIForm.UIFormBase.Transform.SetParent(m_UiConfig.Root);

            if (handleUIForm.UIFormBase.NeedMask)
            {
                m_UIFormsOpenPool.Insert(m_UIFormsOpenPool.Count - 1, m_MaskUIFormData);
                m_MaskUIFormData.UIFormBase.Transform.SetParent(m_UiConfig.Root);
                m_MaskUIFormData.UIFormBase.OnReopen(null);
            }

            OnUIFormFormOpenPoolChanged();
        }

        /// <summary>
        /// 将窗体从正在打开的UI列表中移除
        /// </summary>
        /// <param name="handleUIForm">窗体数据</param>
        /// <param name="ignoreOpenMode">忽略打开模式</param>
        private void RemoveUIFormFormOpenPool(HandleUIForm handleUIForm, bool ignoreOpenMode)
        {
            m_UIFormsOpenPool.Remove(handleUIForm);
            handleUIForm.UIFormBase.Transform.SetParent(m_UiConfig.StandBy);

            if (!ignoreOpenMode)
            {
                if (handleUIForm.OpenMode == OpenMode.TopSwitch)
                {
                    if (handleUIForm.SwitchHandleUIForm != null)
                    {
                        //使用上次数据
                        PassiveOpen(handleUIForm.SwitchHandleUIForm);
                        handleUIForm.SwitchHandleUIForm.UIFormBase.OnReopen(handleUIForm.SwitchHandleUIForm.UserData);
                        m_SwitchedUIForm.RemoveAll(form => form == handleUIForm);
                    }
                }
            }
        }

        /// <summary>
        /// 正在打开的UI列表变动
        /// </summary>
        private void OnUIFormFormOpenPoolChanged()
        {
            for (var index = 0; index < m_UIFormsOpenPool.Count; index++)
            {
                var openUIFormData = m_UIFormsOpenPool[index];
                openUIFormData.UIFormBase.SortOrderChange(index);
            }
        }

        /// <summary>
        /// 获取打开中的界面数量
        /// </summary>
        /// <returns>界面数量</returns>
        public int GetOpeningFormCount()
        {
            return m_UIFormsOpenPool.Count;
        }

        #region Operate

        /// <summary>
        /// 设置界面外边距
        /// </summary>
        /// <param name="top">上外边距</param>
        /// <param name="bottom">下外边距</param>
        /// <param name="left">左外边距</param>
        /// <param name="right">右外边距</param>
        public void SetRectMargin(float top, float bottom, float left, float right)
        {
            var root = m_UiConfig.Root.RectTransform();
            var standBy = m_UiConfig.StandBy.RectTransform();

            root.offsetMin = standBy.offsetMin = new Vector2(left, bottom);
            root.offsetMax = standBy.offsetMax = new Vector2(-right, -top);
        }

        /// <summary>
        /// 加载窗体
        /// </summary>
        /// <typeparam name="T">窗体类型</typeparam>
        /// <param name="userData">用户自定义数据</param>
        /// <param name="onLoad">加载成功</param>
        public void Load<T>(object userData = null, Action<T> onLoad = null) where T : UIFormBase
        {
            var type = typeof(T);
            m_UIFormsPool.TryGetValue(type, out var handleUIForm);
            if (handleUIForm == null || handleUIForm.Status == UIFromStatus.None)
            {
                var newHandle = CreateHandleUIForm(m_UiConfig.StandBy, type, userData);
                newHandle.Complete += ui =>
                {
                    onLoad?.Invoke(ui as T);
                    newHandle.Status = UIFromStatus.Close;
                };
            }
            else if (handleUIForm.Status == UIFromStatus.Loading)
            {
                handleUIForm.Complete += ui =>
                {
                    onLoad?.Invoke(ui as T);
                };
            }
            else
            {
                onLoad?.Invoke(handleUIForm.UIFormBase as T);
            }
        }

        /// <summary>
        /// 获取指定界面
        /// </summary>
        /// <typeparam name="T">界面类型</typeparam>
        /// <returns>界面</returns>
        public T Get<T>() where T : UIFormBase
        {
            var type = typeof(T);
            var handle = m_UIFormsOpenPool.Find((x => x.UIFormBase.GetType() == type));
            if (handle != null)
            {
                return handle.UIFormBase as T;
            }

            return null;
        }

        /// <summary>
        /// 获取全部界面
        /// </summary>
        /// <returns>全部界面</returns>
        public List<UIFormBase> GetAll()
        {
            return m_UIFormsOpenPool.Select((handleUIForm => handleUIForm.UIFormBase)).ToList();
        }

        /// <summary>
        /// 存在界面
        /// </summary>
        /// <typeparam name="T">界面类型</typeparam>
        /// <returns>是否存在</returns>
        public bool Has<T>() where T : UIFormBase
        {
            var type = typeof(T);
            return m_UIFormsOpenPool.Exists((x => x.UIFormBase.GetType() == type));
        }

        /// <summary>
        /// 打开界面
        /// </summary>
        /// <typeparam name="T">界面类型</typeparam>
        /// <param name="openMode">打开方式</param>
        /// <param name="userData">用户自定义数据</param>
        /// <param name="onOpen">打开回调</param>
        public void Open<T>(OpenMode openMode = OpenMode.Cover, object userData = null, Action<T> onOpen = null) where T : UIFormBase
        {
            var type = typeof(T);
            Open(type, openMode, userData, (ui => onOpen?.Invoke((T) ui)));
        }

        /// <summary>
        /// 打开界面
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="openMode">打开方式</param>
        /// <param name="userData">用户自定义数据</param>
        /// <param name="onOpen">打开回调</param>
        public void Open(Type type, OpenMode openMode = OpenMode.Cover, object userData = null, Action<UIFormBase> onOpen = null)
        {
            if (m_UIFormsPool.TryGetValue(type, out var handleUIForm))
            {
                if (handleUIForm.Status == UIFromStatus.None)
                {
                    return;
                }

                if (handleUIForm.Status == UIFromStatus.Open)
                {
                    Debug.LogWarning("界面已经被打开");
                    onOpen?.Invoke(handleUIForm.UIFormBase);
                    return;
                }

                OpenMode beforeMode = handleUIForm.OpenMode;
                handleUIForm.OpenMode = openMode;
                handleUIForm.UserData = userData;
                switch (handleUIForm.Status)
                {
                    case UIFromStatus.None:
                        break;
                    case UIFromStatus.Loading:
                        handleUIForm.Complete += ui =>
                        {
                            onOpen?.Invoke(ui);
                            ui.OnOpen(userData);
                        };
                        break;
                    case UIFromStatus.Close:
                        var findIndex = m_SwitchedUIForm.FindIndex((form => form == handleUIForm));
                        if (findIndex == -1)
                        {
                            handleUIForm.Status = UIFromStatus.Open;
                            AppendUIFormToOpenPool(handleUIForm, false);
                            onOpen?.Invoke(handleUIForm.UIFormBase);
                            handleUIForm.UIFormBase.OnReopen(userData);
                        }
                        else
                        {
                            m_SwitchedUIForm.RemoveRange(findIndex, m_SwitchedUIForm.Count - findIndex);

                            var lastOrDefault = m_UIFormsOpenPool.LastOrDefault();
                            if (lastOrDefault != null)
                            {
                                lastOrDefault.SwitchHandleUIForm = null;
                                Close(lastOrDefault.UIFormBase);
                            }

                            handleUIForm.OpenMode = beforeMode;
                            handleUIForm.Status = UIFromStatus.Open;
                            AppendUIFormToOpenPool(handleUIForm, true);
                            onOpen?.Invoke(handleUIForm.UIFormBase);
                            handleUIForm.UIFormBase.OnReopen(userData);
                        }

                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                var newHandleUIForm = CreateHandleUIForm(m_UiConfig.Root, type, userData);
                newHandleUIForm.OpenMode = openMode;
                newHandleUIForm.UserData = userData;
                newHandleUIForm.Complete += ui =>
                {
                    newHandleUIForm.Status = UIFromStatus.Open;
                    AppendUIFormToOpenPool(newHandleUIForm, false);
                    onOpen?.Invoke(ui);
                    ui.OnOpen(userData);
                };
            }
        }

        /// <summary>
        /// 创建加载UI句柄
        /// </summary>
        /// <param name="parent">父级</param>
        /// <param name="type">UI类型</param>
        /// <param name="userData">自定义数据</param>
        /// <returns>句柄</returns>
        private HandleUIForm CreateHandleUIForm(Transform parent, Type type, object userData)
        {
            HandleUIForm newHandleUIForm = new HandleUIForm();
            string addressableName;
            var uiPrefabName = type.GetAttribute<UIPrefabName>();
            var typeNiceName = type.GetNiceName();
            if (uiPrefabName != null)
            {
                addressableName = uiPrefabName.Name;
            }
            else
            {
                addressableName = typeNiceName;
            }

            var asyncOperationHandle = Addressables.InstantiateAsync($"{m_UiConfig.PrefabPath}/{addressableName}.prefab", parent);
            newHandleUIForm.AsyncOperationHandle = asyncOperationHandle;
            asyncOperationHandle.Completed += handle =>
            {
                if (addressableName != typeNiceName)
                {
                    handle.Result.name = $"{typeNiceName}({addressableName})";
                }
                else
                {
                    handle.Result.name = typeNiceName;
                }

                UIFormBase uiFormBase = (UIFormBase) Activator.CreateInstance(type);
                uiFormBase.BindGameObject(handle.Result);
                newHandleUIForm.OnComplete(uiFormBase, userData);
            };
            m_UIFormsPool.Add(type, newHandleUIForm);
            return newHandleUIForm;
        }

        /// <summary>
        /// 被动打开，如Switch
        /// </summary>
        /// <param name="handleUIForm">UIFormHandle</param>
        private void PassiveOpen(HandleUIForm handleUIForm)
        {
            handleUIForm.Status = UIFromStatus.Open;
            AppendUIFormToOpenPool(handleUIForm, true);
        }

        /// <summary>
        /// 关闭窗体
        /// </summary>
        /// <typeparam name="T">窗体类型</typeparam>
        /// <param name="userData">用户自定义数据</param>
        /// <param name="onClosed">关闭完成</param>
        /// <param name="ignoreOpenMode">忽略打开模式</param>
        /// <param name="ignoreLock">忽略锁定</param>
        public void Close<T>(object userData = null, Action onClosed = null, bool ignoreOpenMode = false, bool ignoreLock = false) where T : UIFormBase
        {
            var type = typeof(T);
            Close(type, userData, onClosed, ignoreOpenMode, ignoreLock);
        }

        /// <summary>
        /// 关闭窗体
        /// </summary>
        /// <param name="type">窗体类型</param>
        /// <param name="userData">用户自定义数据</param>
        /// <param name="onClosed">关闭完成</param>
        /// <param name="ignoreOpenMode">忽略打开模式</param>
        /// <param name="ignoreLock">忽略锁定</param>
        public void Close(Type type, object userData = null, Action onClosed = null, bool ignoreOpenMode = false, bool ignoreLock = false)
        {
            if (m_UIFormsPool.TryGetValue(type, out var handleUiForm))
            {
                switch (handleUiForm.Status)
                {
                    case UIFromStatus.None:
                        handleUiForm.Status = UIFromStatus.Close;
                        onClosed?.Invoke();
                        break;
                    case UIFromStatus.Loading:
                        Addressables.ReleaseInstance(handleUiForm.AsyncOperationHandle);
                        m_UIFormsPool.Remove(type);
                        handleUiForm.Status = UIFromStatus.Close;
                        onClosed?.Invoke();
                        break;
                    case UIFromStatus.Open:
                        if (!ignoreLock && handleUiForm.UIFormBase.Lock)
                        {
                            onClosed?.Invoke();
                            return;
                        }

                        handleUiForm.UIFormBase.OnClose(userData, null);
                        RemoveUIFormFormOpenPool(handleUiForm, ignoreOpenMode);
                        handleUiForm.Status = UIFromStatus.Close;
                        onClosed?.Invoke();
                        break;
                    case UIFromStatus.Close:
                        onClosed?.Invoke();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// 关闭窗体
        /// </summary>
        /// <param name="formBase">窗体</param>
        /// <param name="userData">用户自定义数据</param>
        /// <param name="onClosed">关闭完成</param>
        /// <param name="ignoreOpenMode">忽略打开模式</param>
        /// <param name="ignoreLock">忽略锁定</param>
        public void Close(UIFormBase formBase, object userData = null, Action onClosed = null, bool ignoreOpenMode = false, bool ignoreLock = false)
        {
            var type = formBase.GetType();
            Close(type, userData, onClosed, ignoreOpenMode, ignoreLock);
        }

        /// <summary>
        /// 释放窗体
        /// </summary>
        /// <typeparam name="T">窗体类型</typeparam>
        /// <param name="userData">用户自定义数据</param>
        public void Release<T>(object userData = null) where T : UIFormBase
        {
            var type = typeof(T);
            Release(type);
        }

        /// <summary>
        /// 释放窗体
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="userData">用户自定义数据</param>
        public void Release(Type type, object userData = null)
        {
            if (m_UIFormsPool.TryGetValue(type, out var handleUIForm))
            {
                switch (handleUIForm.Status)
                {
                    case UIFromStatus.None:
                        break;
                    case UIFromStatus.Loading:
                        Addressables.ReleaseInstance(handleUIForm.AsyncOperationHandle);
                        break;
                    case UIFromStatus.Open:
                    case UIFromStatus.Close:
                        handleUIForm.UIFormBase.Dispose();
                        Addressables.ReleaseInstance(handleUIForm.AsyncOperationHandle);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                m_UIFormsPool.Remove(type);
            }
        }

        /// <summary>
        /// 释放窗体
        /// </summary>
        /// <param name="uiFormBase">窗体</param>
        /// <param name="userData">用户自定义数据</param>
        public void Release(UIFormBase uiFormBase, object userData = null)
        {
            var type = uiFormBase.GetType();
            Release(type, userData);
        }

        /// <summary>
        /// 释放所有界面
        /// </summary>
        /// <param name="ignoreLock">忽略锁定</param>
        public void ReleaseAll(bool ignoreLock = false)
        {
            var types = m_UIFormsPool.Keys.ToList();
            for (var index = types.Count - 1; index >= 0; index--)
            {
                var type = types[index];
                if (m_UIFormsPool.TryGetValue(type, out var handleUIForm))
                {
                    if (!ignoreLock && handleUIForm.UIFormBase.Lock)
                    {
                        continue;
                    }

                    if (handleUIForm == m_MaskUIFormData)
                    {
                        continue;
                    }

                    Release(type);

                    m_SwitchedUIForm.Remove(handleUIForm);
                }
            }
        }

        /// <summary>
        /// 关闭所有界面
        /// </summary>
        /// <param name="ignoreLock">忽略锁定</param>
        public void CloseAll(bool ignoreLock = false)
        {
            var types = m_UIFormsPool.Keys.ToList();
            for (var index = types.Count - 1; index >= 0; index--)
            {
                var type = types[index];
                if (m_UIFormsPool.TryGetValue(type, out var handleUIForm))
                {
                    if (!ignoreLock && handleUIForm.UIFormBase.Lock)
                    {
                        continue;
                    }

                    //忽略打开方式，避免有些界面因为Switch而被重新打开
                    Close(type, ignoreOpenMode: true, ignoreLock: ignoreLock);
                    //清除Switch，避免下次打开后，残留Switch记录的数据
                    handleUIForm.SwitchHandleUIForm = null;
                }
            }
        }

        #endregion

        #region Message

        /// <summary>
        /// 注册消息接收委托
        /// </summary>
        /// <param name="messageKey">消息键值</param>
        /// <param name="messageReceiver">消息接收委托</param>
        public void RegisterUIMessageReceiver(string messageKey, MessageReceiver messageReceiver)
        {
            if (!m_MessageReceivers.ContainsKey(messageKey))
            {
                m_MessageReceivers.Add(messageKey, null);
            }

            m_MessageReceivers[messageKey] += messageReceiver;
        }

        /// <summary>
        /// 移除消息接收委托
        /// </summary>
        /// <param name="messageKey"></param>
        /// <param name="messageReceiver"></param>
        public void RemoveUIMessageReceiver(string messageKey, MessageReceiver messageReceiver)
        {
            if (m_MessageReceivers.ContainsKey(messageKey))
            {
                m_MessageReceivers[messageKey] -= messageReceiver;
            }
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="messageKey">消息键值</param>
        /// <param name="userData">自定义数据</param>
        public void SendUIMessage(string messageKey, object userData = null)
        {
            if (m_MessageReceivers.TryGetValue(messageKey, out var receiver))
            {
                receiver?.Invoke(userData);
            }
        }

        #endregion
    }
}
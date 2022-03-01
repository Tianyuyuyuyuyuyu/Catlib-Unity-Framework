using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Framework.Runtime.HotFix.UI
{
    public class UIBase
    {
        /// <summary>
        /// 游戏物体
        /// </summary>
        private GameObject m_GameObject;

        /// <summary>
        /// 游戏物体
        /// </summary>
        private Transform m_Transform;

        /// <summary>
        /// 异步资源句柄列表
        /// </summary>
        private List<AsyncOperationHandle> m_Handles = new List<AsyncOperationHandle>();

        /// <summary>
        /// 待销毁的对象列表
        /// </summary>
        private List<IDisposable> m_Disposables = new List<IDisposable>();
        
        /// <summary>
        /// 已注册的消息委托
        /// </summary>
        private Dictionary<string, List<MessageReceiver>> m_RegisteredUIMessageReceivers;

        /// <summary>
        /// 游戏物体
        /// </summary>
        public GameObject GameObject => m_GameObject;

        /// <summary>
        /// 游戏物体
        /// </summary>
        public Transform Transform => m_Transform;
        
        /// <summary>
        /// 绑定游戏物体
        /// </summary>
        /// <param name="gameObject">游戏物体</param>
        public void BindGameObject(GameObject gameObject)
        {
            m_GameObject = gameObject;
            m_Transform = gameObject.transform;
            InstallField();
        }

        /// <summary>
        /// 实装字段
        /// </summary>
        public virtual void InstallField()
        {

        }
        
        /// <summary>
        /// 初始化子UIGroup
        /// </summary>
        /// <param name="userData">自定义数据</param>
        public virtual void InitSubUIGroup(object userData)
        {
            
        }
        
        /// <summary>
        /// 每帧更新UIGroup
        /// </summary>
        public virtual void UpdateSubUIGroup()
        {
            
        }
        
        /// <summary>
        /// 打开子组
        /// </summary>
        /// <param name="userData">自定义数据</param>
        public virtual void OpenSubUIGroup(object userData)
        {
            
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="userData">自定义数据</param>
        public virtual void OnInit(object userData)
        {
            m_RegisteredUIMessageReceivers = new Dictionary<string, List<MessageReceiver>>();
        }
        
        /// <summary>
        /// 每帧刷新
        /// </summary>
        public virtual void OnUpdate()
        {
            
        }
        
        /// <summary>
        /// 注册消息接收
        /// </summary>
        /// <param name="messageKey">信息键值</param>
        /// <param name="messageReceiver">消息接收</param>
        public void RegisterUIMessageReceiver(string messageKey, MessageReceiver messageReceiver)
        {
            CoreHotFix.UI.RegisterUIMessageReceiver(messageKey, messageReceiver);

            if (m_RegisteredUIMessageReceivers.TryGetValue(messageKey, out var messageReceivers))
            {
                messageReceivers.Add(messageReceiver);
            }
            else
            {
                m_RegisteredUIMessageReceivers.Add(messageKey, new List<MessageReceiver>() {messageReceiver});
            }
        }

        /// <summary>
        /// 注册消息接收
        /// </summary>
        /// <param name="messageKey">信息键值</param>
        /// <param name="messageReceiver">消息接收</param>
        public void RemoveUIMessageReceiver(string messageKey, MessageReceiver messageReceiver)
        {
            CoreHotFix.UI.RemoveUIMessageReceiver(messageKey, messageReceiver);

            if (m_RegisteredUIMessageReceivers.TryGetValue(messageKey, out var messageReceivers))
            {
                if (messageReceivers.Contains(messageReceiver))
                {
                    messageReceivers.Remove(messageReceiver);
                }
            }
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="messageKey">消息键值</param>
        /// <param name="userData">消息数据</param>
        public void SendUIMessage(string messageKey, object userData)
        {
            CoreHotFix.UI.SendUIMessage(messageKey, userData);
        }

        /// <summary>
        /// 添加依赖
        /// </summary>
        /// <param name="asyncOperation">句柄</param>
        public void AddDependency(AsyncOperationHandle asyncOperation)
        {
            m_Handles.Add(asyncOperation);
        }
        
        /// <summary>
        /// 添加依赖
        /// </summary>
        /// <param name="iDisposable">可销毁的对象</param>
        public void AddDependency(IDisposable iDisposable)
        {
            m_Disposables.Add(iDisposable);
        }
        
        /// <summary>
        /// 销毁子UIGroup 自动生成使用，无法在正常逻辑中被重写
        /// </summary>
        public virtual void DisposeSubUIGroup()
        {
            
        }

        /// <summary>
        /// 销毁时
        /// </summary>
        public virtual void OnRelease()
        {
            
        }

        /// <summary>
        /// 释放所有句柄
        /// </summary>
        public void ReleaseAllHandle()
        {
            foreach (var asyncOperationHandle in m_Handles)
            {
                if (asyncOperationHandle.IsValid())
                {
                    Addressables.ReleaseInstance(asyncOperationHandle);    
                }
            }
            m_Handles.Clear();
        }

        /// <summary>
        /// 删除所有消息接收
        /// </summary>
        public void RemoveAllUIMessageReceiver()
        {
            foreach (var registeredMessageReceiver in m_RegisteredUIMessageReceivers)
            {
                foreach (var messageReceiver in registeredMessageReceiver.Value)
                {
                    CoreHotFix.UI.RemoveUIMessageReceiver(registeredMessageReceiver.Key, messageReceiver);
                }
            }
            m_RegisteredUIMessageReceivers.Clear();
        }

        /// <summary>
        /// 释放
        /// </summary>
        public void Release()
        {
            DisposeSubUIGroup();
            RemoveAllUIMessageReceiver();

            ReleaseAllHandle();
            
            foreach (var disposable in m_Disposables)
            {
                disposable?.Dispose();
            }
            m_Disposables.Clear();
            OnRelease();
        }
    }
}

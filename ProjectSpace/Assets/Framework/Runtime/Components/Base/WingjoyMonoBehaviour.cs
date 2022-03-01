using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Framework.Runtime
{
    public class WingjoyMonoBehaviour : MonoBehaviour
    {
        /// <summary>
        /// 异步资源句柄列表
        /// </summary>
        public List<AsyncOperationHandle> Handles = new List<AsyncOperationHandle>();

        /// <summary>
        /// 添加依赖
        /// </summary>
        /// <param name="asyncOperation">句柄</param>
        public void AddDependency(AsyncOperationHandle asyncOperation)
        {
            Handles.Add(asyncOperation);
        }

        /// <summary>
        /// 释放
        /// </summary>
        public virtual void Release()
        {
            foreach (var asyncOperationHandle in Handles)
            {
                if (asyncOperationHandle.IsValid())
                {
                    Addressables.ReleaseInstance(asyncOperationHandle);    
                }
            }
            Handles.Clear();
        }
    }
}
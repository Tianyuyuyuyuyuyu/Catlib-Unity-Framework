using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using WingjoyUtility.Runtime;
using ReadOnlyAttribute = Sirenix.OdinInspector.ReadOnlyAttribute;

namespace Wingjoy.Framework.Runtime.Save
{
    public class SaveComponent : WingjoyFrameworkComponent
    {
        /// <summary>
        /// 定义存档路径
        /// </summary>
        private string m_DirPath;

        /// <summary>
        /// 定义存档路径
        /// </summary>
        public string DirPath => m_DirPath;
        
        /// <summary>
        /// 存档文件签名
        /// </summary>
        public int SaveFileSignature = 741365263;

        /// <summary>
        /// 存档文件主版本号
        /// </summary>
        public int SaveFileMainVersion = 0;
        /// <summary>
        /// 存档文件子版本号
        /// </summary>
        public int SaveFileSubVersion = 0;

        /// <summary>
        /// 保存机制
        /// </summary>
        private Action m_SaveAction;

        /// <summary>
        /// 自动保存使能
        /// </summary>
        [ShowInInspector]
        private bool m_AutoSaveEnable;

        /// <summary>
        /// 自动保存时间间隔
        /// </summary>
        [ShowInInspector]
        private float m_AutoSaveInterval;

        /// <summary>
        /// 自从上次自动保存过后经过的时间
        /// </summary>
        public float TimeSinceLastAutoSave;

        protected override void Awake()
        {
            base.Awake();
            m_DirPath = "/Save/";
#if UNITY_EDITOR
            m_DirPath = Application.persistentDataPath + m_DirPath;
#elif UNITY_ANDROID
            m_DirPath = RuntimeUtilities.Path.GetProtectPath + m_DirPath;
#elif UNITY_IOS
            m_DirPath = Application.persistentDataPath + m_DirPath;
#endif

        }

        /// <summary>
        /// 二进制存储
        /// </summary>
        /// <typeparam name="T">存储对象类型</typeparam>
        /// <param name="fileName">文件名</param>
        /// <param name="saveObject">存储对象</param>
        public void BinarySave<T>(string fileName, T saveObject)
        {
            try
            {
                string path = m_DirPath + fileName;
                var directoryName = Path.GetDirectoryName(path);

                if (!Directory.Exists(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }

                BinaryFormatter bf = new BinaryFormatter();
                MemoryStream memoryStream = new MemoryStream();
                bf.Serialize(memoryStream, saveObject);
                
                using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
                {
                    using (BinaryWriter binaryWriter = new BinaryWriter(fs))
                    {
                        //处理签名
                        binaryWriter.Write(SaveFileSignature);
                        //处理版本号
                        binaryWriter.Write(SaveFileMainVersion);
                        binaryWriter.Write(SaveFileSubVersion);
                        //处理游戏内容
                        byte[] gameBuffer = RuntimeUtilities.Zip.Compress(memoryStream.ToArray());
                        binaryWriter.Write(gameBuffer.Length);

                        Debug.LogFormat("存档长度:{0}", gameBuffer.Length);
                        fs.Write(gameBuffer,0,gameBuffer.Length);
                    }
                }

                memoryStream.Close();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        /// <summary>
        /// 异步二进制存储
        /// </summary>
        /// <typeparam name="T">存储对象类型</typeparam>
        /// <param name="fileName">文件名</param>
        /// <param name="saveObject">存储对象</param>
        public async UniTask BinarySaveAsync<T>(string fileName, T saveObject)
        {
            string path = m_DirPath + fileName;
            var directoryName = Path.GetDirectoryName(path);

            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }

            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream memoryStream = new MemoryStream();
            bf.Serialize(memoryStream, saveObject);

            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                using (BinaryWriter binaryWriter = new BinaryWriter(fs))
                {
                    //处理签名
                    binaryWriter.Write(SaveFileSignature);
                    //处理版本号
                    binaryWriter.Write(SaveFileMainVersion);
                    binaryWriter.Write(SaveFileSubVersion);
                    //处理游戏内容
                    byte[] gameBuffer = await RuntimeUtilities.Zip.CompressAsync(memoryStream.ToArray());
                    binaryWriter.Write(gameBuffer.Length);

                    Debug.LogFormat("存档长度:{0}", gameBuffer.Length);
                    await fs.WriteAsync(gameBuffer,0,gameBuffer.Length);
                }
            }

            memoryStream.Close();
        }

        /// <summary>
        /// 二进制加载
        /// </summary>
        /// <typeparam name="T">加载类型</typeparam>
        /// <param name="fileName">文件名</param>
        /// <param name="saveUpdateLogic">需要升级</param>
        /// <returns>加载结果</returns>
        public T BinaryLoad<T>(string fileName, Action<T, int> saveUpdateLogic) where T : class
        {
            string path = m_DirPath + fileName;
            if (!File.Exists(path))
            {
                return null;
            }

            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    using (BinaryReader binaryReader = new BinaryReader(fs))
                    {
                        var signature = binaryReader.ReadInt32();
                        if (signature != SaveFileSignature)
                        {
                            Debug.LogError("存档签名不正确");
                        }

                        var mainVer = binaryReader.ReadInt32();
                        var subVer = binaryReader.ReadInt32();
                        var gameBufferLength = binaryReader.ReadInt32();

                        if (mainVer < SaveFileMainVersion)
                        {//本地的存档主版本号小于游戏的存档主版本号，则销毁存档文件
                            Debug.LogError("本地存档太老，需要重新构建");
                            return null;
                        }

                        MemoryStream memoryStream = new MemoryStream();
                        var readBytes = binaryReader.ReadBytes(gameBufferLength);
                        if (RuntimeUtilities.Zip.IsGZipHeader(readBytes))
                        {
                            readBytes = RuntimeUtilities.Zip.Decompress(readBytes);
                        }
                        memoryStream.Write(readBytes, 0, readBytes.Length);
                        memoryStream.Flush();
                        memoryStream.Position = 0;

                        BinaryFormatter bf = new BinaryFormatter();
                        var saveObject = bf.Deserialize(memoryStream) as T;

                        //如果本地存档子版本号小于游戏的存档子版本号，则根据设置进行升级
                        if (subVer < SaveFileSubVersion)
                        {
                            Debug.LogWarning("本地存档需要更新");
                            saveUpdateLogic?.Invoke(saveObject, subVer);
                        }


                        return saveObject;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return null;
            }   
        }

        /// <summary>
        /// Json保存
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="saveObject">对象</param>
        public void JsonSave(string fileName, object saveObject)
        {
            string path = m_DirPath + fileName + ".json";
            if (!Directory.Exists(m_DirPath))
            {
                Directory.CreateDirectory(m_DirPath);
            }
            //将对象序列化为字符串
            string toSave = JsonUtility.ToJson(saveObject);
            StreamWriter streamWriter = File.CreateText(path);
            streamWriter.Write(toSave);
            streamWriter.Close();
        }

        /// <summary>
        /// Json 加载
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="pType">对象类型</param>
        /// <returns>对象</returns>
        public object JsonLoad(string fileName, Type pType)
        {
            string path = m_DirPath + fileName + ".json";
            if (!File.Exists(path))
            {
                return null;
            }
            StreamReader streamReader = File.OpenText(path);
            string data = streamReader.ReadToEnd();
            streamReader.Close();
            return JsonUtility.FromJson(data, pType);
        }

        private void Update()
        {
            if (m_AutoSaveEnable)
            {
                TimeSinceLastAutoSave += Time.unscaledDeltaTime;
                if(TimeSinceLastAutoSave > m_AutoSaveInterval)
                {
                    var realtimeSinceStartup = Time.realtimeSinceStartup;
                    Debug.Log("开始自动保存");
                    m_SaveAction?.Invoke();
                    Debug.LogFormat("自动保存结束，耗时{0}秒", (Time.realtimeSinceStartup - realtimeSinceStartup).ToString("F1"));
                    TimeSinceLastAutoSave = 0;
                }
            }
        }

        /// <summary>
        /// 使能自动保存
        /// </summary>
        /// <param name="saveAction">保存机制</param>
        /// <param name="autoSaveInterval">自动保存间隔时间</param>
        public void EnableAutoSave(Action saveAction, float autoSaveInterval)
        {
            if (saveAction == null)
            {
                return;
            }

            if (autoSaveInterval < 10)
            {
                return;
            }

            Debug.Log("开启自动保存");

            m_SaveAction = saveAction;
            m_AutoSaveInterval = autoSaveInterval;
            m_AutoSaveEnable = true;
        }
    }
}
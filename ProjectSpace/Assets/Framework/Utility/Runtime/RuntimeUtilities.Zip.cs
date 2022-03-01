using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace Framework.Utility.Runtime
{
    public static partial class RuntimeUtilities
    {
        public static class Zip
        {
            public static bool IsGZipHeader(byte[] arr)
            {
                return arr.Length >= 2 && arr[0] == 31 && arr[1] == 139;
            }

            public static byte[] Compress(byte[] uncompressed)
            {
                byte[] result;
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (GZipStream gZipStream = new GZipStream(memoryStream, CompressionMode.Compress))
                    {
                        gZipStream.Write(uncompressed, 0, uncompressed.Length);
                        gZipStream.Close();
                        result = memoryStream.ToArray();
                    }
                }

                return result;
            }
            
            public static async Task<byte[]> CompressAsync(byte[] uncompressed)
            {
                byte[] result;
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (GZipStream gZipStream = new GZipStream(memoryStream, CompressionMode.Compress))
                    {
                        await gZipStream.WriteAsync(uncompressed, 0, uncompressed.Length);
                        gZipStream.Close();
                        result = memoryStream.ToArray();
                    }
                }

                return result;
            }

            public static byte[] Decompress(byte[] compressed)
            {
                byte[] array = new byte[4096];
                byte[] result;
                using (MemoryStream memoryStream = new MemoryStream(compressed))
                {
                    using (GZipStream gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                    {
                        using (MemoryStream memoryStream2 = new MemoryStream())
                        {
                            for (int num = -1; num != 0; num = gZipStream.Read(array, 0, array.Length))
                            {
                                if (num > 0)
                                {
                                    memoryStream2.Write(array, 0, num);
                                }
                            }

                            result = memoryStream2.ToArray();
                        }
                    }
                }

                return result;
            }

            public static byte[] ToByteArray(string hexString)
            {
                hexString = hexString.Replace("-", string.Empty);
                int length = hexString.Length;
                byte[] array = new byte[length / 2];
                for (int i = 0; i < length; i += 2)
                {
                    array[i / 2] = Convert.ToByte(hexString.Substring(i, 2), 16);
                }

                return array;
            }

            /// <summary>
            /// 对目标文件夹进行压缩，将压缩结果保存为指定文件
            /// </summary>
            /// <param name="dirPath">目标文件夹</param>
            /// <param name="fileName">压缩文件</param>
            public static void Compress(string dirPath, string fileName)
            {
                List<SerializeFileInfo> list = new List<SerializeFileInfo>();

                GenerateSerializeFileInfoList(list, dirPath);

                list.ForEach((info => info.FileName = info.FileName.Replace("\\", "/").Replace(dirPath, "")));

                BinaryFormatter formatter = new BinaryFormatter();
                using (Stream s = new MemoryStream())
                {
                    formatter.Serialize(s, list);
                    s.Position = 0;
                    CreateCompressFile(s, fileName);
                }
            }

            public static void GenerateSerializeFileInfoList(List<SerializeFileInfo> list, string path)
            {
                var directoryInfo = new DirectoryInfo(path);
                var files = directoryInfo.GetFiles();
                var directories = directoryInfo.GetDirectories();
                foreach (var directory in directories)
                {
                    list.Add(new SerializeFileInfo(directory.FullName, null, false));
                    GenerateSerializeFileInfoList(list, directory.FullName);
                }

                foreach (var file in files)
                {
                    list.Add(new SerializeFileInfo(file.FullName, System.IO.File.ReadAllBytes(file.FullName), true));
                }
            }

            /// <summary>
            /// 对目标压缩文件解压缩，将内容解压缩到指定文件夹
            /// </summary>
            /// <param name="fileName">压缩文件</param>
            /// <param name="dirPath">解压缩目录</param>
            public static void DeCompress(string fileName, string dirPath)
            {
                using (Stream source = System.IO.File.OpenRead(fileName))
                {
                    using (Stream destination = new MemoryStream())
                    {
                        using (GZipStream input = new GZipStream(source, CompressionMode.Decompress, true))
                        {
                            byte[] bytes = new byte[4096];
                            int n;
                            while ((n = input.Read(bytes, 0, bytes.Length)) != 0)
                            {
                                destination.Write(bytes, 0, n);
                            }
                        }

                        destination.Flush();
                        destination.Position = 0;
                        DeSerializeFiles(destination, dirPath);
                    }
                }
            }

            private static void DeSerializeFiles(Stream s, string dirPath)
            {
                BinaryFormatter b = new BinaryFormatter();
                List<SerializeFileInfo> list = (List<SerializeFileInfo>) b.Deserialize(s);

                foreach (SerializeFileInfo f in list)
                {
                    if (f.IsFile)
                    {
                        string newName = dirPath + "/" + f.FileName;
                        using (FileStream fs = new FileStream(newName, FileMode.Create, FileAccess.Write))
                        {
                            fs.Write(f.FileBuffer, 0, f.FileBuffer.Length);
                            fs.Close();
                        }
                    }
                    else
                    {
                        string newName = dirPath + "/" + f.FileName;
                        Directory.CreateDirectory(newName);
                    }
                }
            }

            private static void CreateCompressFile(Stream source, string destinationName)
            {
                using (Stream destination = new FileStream(destinationName, FileMode.Create, FileAccess.Write))
                {
                    using (GZipStream output = new GZipStream(destination, CompressionMode.Compress))
                    {
                        byte[] bytes = new byte[4096];
                        int n;
                        while ((n = source.Read(bytes, 0, bytes.Length)) != 0)
                        {
                            output.Write(bytes, 0, n);
                        }
                    }
                }
            }

            [Serializable]
            public class SerializeFileInfo
            {
                public SerializeFileInfo(string name, byte[] buffer, bool isFile)
                {
                    FileName = name;
                    FileBuffer = buffer;
                    IsFile = isFile;
                }

                public string FileName;

                public byte[] FileBuffer;

                public bool IsFile;
            }
        }
    }
}
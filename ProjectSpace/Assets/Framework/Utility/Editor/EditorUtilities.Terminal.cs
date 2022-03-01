using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Debug = UnityEngine.Debug;

namespace Framework.Utility.Editor
{
    public static partial class EditorUtilities
    {
        public class Terminal
        {
            /// <summary>
            /// 是否输出日志
            /// </summary>
            private static bool m_LogEnable = false;

            /// <summary>
            /// 设置日志使能
            /// </summary>
            /// <param name="enable">使能</param>
            public static void SetLogEnable(bool enable)
            {
                m_LogEnable = enable;
            }

            /// <summary>
            /// 输出日志
            /// </summary>
            /// <param name="value">日志</param>
            private static void Log(object value)
            {
                if(!m_LogEnable)
                    return;
                
                Log(value);
            }
                
            /// <summary>
            /// 调用命令
            /// </summary>
            /// <param name="command">命令</param>
            /// <param name="argument">参数</param>
            public static void ProcessCommand(string command, string argument)
            {
                ProcessStartInfo info = new ProcessStartInfo(command);
                info.Arguments = argument;
                info.CreateNoWindow = true;
                info.ErrorDialog = true;
                info.UseShellExecute = true;

                if (info.UseShellExecute)
                {
                    info.RedirectStandardOutput = false;
                    info.RedirectStandardError = false;
                    info.RedirectStandardInput = false;
                }
                else
                {
                    info.RedirectStandardOutput = true;
                    info.RedirectStandardError = true;
                    info.RedirectStandardInput = true;
                    info.StandardOutputEncoding = System.Text.Encoding.UTF8;
                    info.StandardErrorEncoding = System.Text.Encoding.UTF8;
                }

                Process process = Process.Start(info);

                if (!info.UseShellExecute)
                {
                    Log(process.StandardOutput);
                    Log(process.StandardError);
                }

                process.WaitForExit();
            }

            /// <summary>
            /// 异步调用命令
            /// </summary>
            /// <param name="command">命令</param>
            /// <param name="argument">参数</param>
            public static async Task ProcessCommandAsync(string command, string argument)
            {
                await Task.Run((() =>
                {
                    ProcessStartInfo info = new ProcessStartInfo(command);
                    info.Arguments = argument;
                    info.CreateNoWindow = true;
                    info.ErrorDialog = true;
                    info.UseShellExecute = true;

                    if (info.UseShellExecute)
                    {
                        info.RedirectStandardOutput = false;
                        info.RedirectStandardError = false;
                        info.RedirectStandardInput = false;
                    }
                    else
                    {
                        info.RedirectStandardOutput = true;
                        info.RedirectStandardError = true;
                        info.RedirectStandardInput = true;
                        info.StandardOutputEncoding = System.Text.Encoding.UTF8;
                        info.StandardErrorEncoding = System.Text.Encoding.UTF8;
                    }

                    Process process = Process.Start(info);

                    if (!info.UseShellExecute)
                    {
                        Log(process.StandardOutput);
                        Log(process.StandardError);
                    }

                    process.WaitForExit();

                }));
            }

            /// <summary>
            /// 异步调用命令
            /// </summary>
            /// <param name="writeCommand">命令（附带参数）</param>
            /// <returns>返回结果</returns>
            public static Task<string> ProcessCommandAsync(string writeCommand)
            {
                var task = Task.Run((async () =>
                {
                    ProcessStartInfo info = new ProcessStartInfo("cmd.exe");
                    info.CreateNoWindow = true;
                    info.ErrorDialog = true;
                    info.UseShellExecute = false;

                    if (info.UseShellExecute)
                    {
                        info.RedirectStandardOutput = false;
                        info.RedirectStandardError = false;
                        info.RedirectStandardInput = false;
                    }
                    else
                    {
                        info.RedirectStandardOutput = true;
                        info.RedirectStandardError = true;
                        info.RedirectStandardInput = true;
                        info.StandardOutputEncoding = System.Text.Encoding.UTF8;
                        info.StandardErrorEncoding = System.Text.Encoding.UTF8;
                    }

                    Process process = Process.Start(info);
                    await ReadToEndAsync(process.StandardOutput);
                    await process.StandardInput.WriteLineAsync(writeCommand + " &exit");
                    await process.StandardOutput.ReadLineAsync();
                    var s = await ReadToEndAsync(process.StandardOutput);
                    return s;
                }));

                return task;
            }

            /// <summary>
            /// 调用命令
            /// </summary>
            /// <param name="writeCommand">命令（附带参数）</param>
            /// <returns>返回结果</returns>
            public static string ProcessCommand(string writeCommand)
            {
                ProcessStartInfo info = new ProcessStartInfo("cmd.exe");
                info.CreateNoWindow = true;
                info.ErrorDialog = true;
                info.UseShellExecute = false;

                if (info.UseShellExecute)
                {
                    info.RedirectStandardOutput = false;
                    info.RedirectStandardError = false;
                    info.RedirectStandardInput = false;
                }
                else
                {
                    info.RedirectStandardOutput = true;
                    info.RedirectStandardError = true;
                    info.RedirectStandardInput = true;
                    info.StandardOutputEncoding = System.Text.Encoding.UTF8;
                    info.StandardErrorEncoding = System.Text.Encoding.UTF8;
                }

                Process process = Process.Start(info);

                var readToEnd = ReadToEnd(process.StandardOutput);
                Log(readToEnd);

                process.StandardInput.WriteLine(writeCommand + " &exit");

                var readLine = process.StandardOutput.ReadLine();
                Log(readLine);

                return process.StandardOutput.ReadToEnd();
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="command"></param>
            /// <param name="argument"></param>
            public static void LightProcessCommand(string command, string argument)
            {
                Process p = new Process
                {
                    StartInfo =
                {
                    CreateNoWindow = true, UseShellExecute = false, FileName = command, Arguments = argument
                }
                };
                p.Start();
                p.WaitForExit();
            }

            /// <summary>
            /// 读到最后
            /// </summary>
            /// <param name="reader">读取器</param>
            /// <returns>结果</returns>
            public static string ReadToEnd(StreamReader reader)
            {
                StringBuilder stringBuilder = new StringBuilder(1024);
                string outPut;
                do
                {
                    outPut = reader.ReadLine();
                    stringBuilder.Append(outPut);
                } while (!string.IsNullOrEmpty(outPut));

                return stringBuilder.ToString();
            }

            /// <summary>
            /// 读到最后
            /// </summary>
            /// <param name="reader">读取器</param>
            /// <returns>结果</returns>
            public static async Task<string> ReadToEndAsync(StreamReader reader)
            {
                StringBuilder stringBuilder = new StringBuilder(1024);
                string outPut;
                do
                {
                    outPut = await reader.ReadLineAsync();
                    stringBuilder.Append(outPut);
                    Thread.Sleep(1);
                } while (!string.IsNullOrEmpty(outPut));

                return stringBuilder.ToString();
            }
        }
    }
}

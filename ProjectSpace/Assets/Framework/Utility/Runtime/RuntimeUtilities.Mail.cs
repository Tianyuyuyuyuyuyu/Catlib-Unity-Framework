using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Framework.Utility.Runtime
{
    public static partial class RuntimeUtilities
    {
        public class Mail
        {
            /// <summary>
            /// 邮件HOST
            /// </summary>
            private static string s_MailHost = "smtp.qq.com";
            
            /// <summary>
            /// 用户名
            /// </summary>
            private static string s_UserName;
            
            /// <summary>
            /// 密码
            /// </summary>
            private static string s_Password;

            /// <summary>
            /// 收件人
            /// </summary>
            private static List<string> s_Addressee = new List<string>();

            /// <summary>
            /// 设置用户名
            /// </summary>
            /// <param name="userName">用户名</param>
            public static void SetUserName(string userName)
            {
                s_UserName = userName;
            }

            /// <summary>
            /// 设置密码
            /// </summary>
            /// <param name="password">密码</param>
            public static void SetPassword(string password)
            {
                s_Password = password;
            }

            /// <summary>
            /// 发送邮件
            /// </summary>
            /// <param name="subject">主题</param>
            /// <param name="body">内容</param>
            /// <param name="callback">回调</param>
            public static void SendEmail(string subject, string body = "", Action callback = null)
            {
                SendEmail(subject, null, body, callback);
            }

            /// <summary>
            /// 添加收件人地址
            /// </summary>
            /// <param name="address">地址</param>
            public static void AddAddressee(string address)
            {
                s_Addressee.Add(address);
            }
            
            /// <summary>
            /// 清除收件人地址
            /// </summary>
            public static void ClearAddAddressee()
            {
                s_Addressee.Clear();
            }

            /// <summary>
            /// 邮件发送
            /// </summary>
            /// <param name="subject">主题</param>
            /// <param name="filePath">文件路径</param>
            /// <param name="body">内容</param>
            /// <param name="callback">回调</param>
            public static async void SendEmail(string subject, List<string> filePath, string body = "", Action callback = null)
            {
                MailMessage mail = new MailMessage();
                //发送邮箱的地址
                mail.From = new MailAddress(s_UserName);
                //收件人邮箱地址 如需发送多个人添加多个Add即可
                foreach (var addresses in s_Addressee)
                {
                    mail.To.Add(addresses);
                }
                
                //标题
                mail.Subject = subject;
                //正文
                mail.Body = body;

                if (filePath?.Count > 0)
                {
                    foreach (var s in filePath)
                    {
                        //添加一个本地附件 
                        mail.Attachments.Add(new Attachment(s));
                    }
                }

                //所使用邮箱的SMTP服务器
                SmtpClient smtpServer = new SmtpClient(s_MailHost);
                //SMTP端口
                smtpServer.Port = 587;
                //账号密码 一般邮箱会提供一串字符来代替密码
                smtpServer.Credentials = new System.Net.NetworkCredential(s_UserName, s_Password) as ICredentialsByHost;
                smtpServer.EnableSsl = true;
                ServicePointManager.ServerCertificateValidationCallback =
                    delegate(object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
                    {
                        return true;
                    };

                await smtpServer.SendMailAsync(mail);
                
                callback?.Invoke();
                smtpServer.Dispose();
                foreach (var mailAttachment in mail.Attachments)
                {
                    mailAttachment.Dispose();
                }
                mail.Dispose();
            }
        }
    }
}

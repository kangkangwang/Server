using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace Collection.Base
{
    /// <summary>
    ///     邮件基类
    /// </summary>
    public class SmtpBaseClass
    {
        #region 发送邮件

        /// <summary>
        ///     发邮件
        /// </summary>
        /// <param name="info">邮件信息</param>
        /// <returns></returns>
        public string SendMail(MailInfo info)
        {
            try
            {
                if (info == null)
                {
                    return "未发送"; // 为空 返回1 发送失败
                }
                //
                var sc = new SmtpClient(info.MailFromServerPath)
                {
                    Credentials = new NetworkCredential(info.MailFrom, info.MailFromPassword)
                };
                //
                var mm = new MailMessage(info.MailFrom, info.MailTo,
                    info.MailSubject, info.MailBody) {Priority = MailPriority.Normal};
                //附件
                if (info.FileList != null)
                {
                    Attachment att;
                    //添加附件
                    foreach (var file in info.FileList)
                    {
                        att = new Attachment(file);
                        mm.Attachments.Add(att);
                    }
                }
                if (info.StreamDic != null)
                {
                    Attachment att;
                    //添加附件
                    foreach (var dic in info.StreamDic)
                    {
                        att = new Attachment(dic.Value, dic.Key);
                        mm.Attachments.Add(att);
                    }
                }
                //发送
                mm.IsBodyHtml = true;
                sc.Send(mm);
                return "发送成功";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        #endregion

        #region 变量声明

        #endregion

        #region 构造函数

        #endregion
    }

    public class MailInfo
    {
        public string MailFromServerPath { get; set; }
        public string MailFrom { get; set; }
        public string MailFromPassword { get; set; }
        public string MailTo { get; set; }
        public string MailSubject { get; set; }
        public string MailBody { get; set; }
        public List<string> FileList { get; set; }
        public Dictionary<string, Stream> StreamDic { get; set; }
    }
}
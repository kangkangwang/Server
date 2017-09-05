using System;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using Collection.Base;
using DeerMonitor.EquipmentManage.Core.BLL;
using DeerMonitor.Framework.ControlUtil;
using Timer = DeerMonitor.Framework.Commons.Threading.Timer;

namespace Collection.Excutes
{
    /// <summary>
    ///     异常中心
    /// </summary>
    public class ExceptionCenter
    {
        #region 构造函数

        #endregion

        #region 启动函数

        /// <summary>
        ///     启动函数
        /// </summary>
        public void Start()
        {
            timerSend = new Timer(SendSmsPeriod);
            timerSend.Elapsed += TimerSend_Elapsed;
            timerSend.Start();
        }

        #endregion

        #region 变量申明

        private Timer timerSend;


        public int SendSmsPeriod
        {
            get
            {
                var a = 60000;
                if (ConfigurationManager.AppSettings.AllKeys.Contains("SendSmtpPeriod"))
                {
                    int.TryParse(ConfigurationManager.AppSettings["SendSmtpPeriod"], out a);
                }
                return a;
            }
        }

        #endregion

        #region 发送短信

        /// <summary>
        ///     发送短信定时器
        /// </summary>
        private void TimerSend_Elapsed(object sender, EventArgs e)
        {
            SendMail();
        }

        /// <summary>
        ///     查询报警短信信息
        /// </summary>
        public void SendMail()
        {
            var mailFromServerPath = "";
            var mailFrom = "";
            var mailFromPassword = "";
            if (ConfigurationManager.AppSettings.AllKeys.Contains("MailFromServerPath"))
            {
                mailFromServerPath = ConfigurationManager.AppSettings["MailFromServerPath"];
            }
            else
            {
                return;
            }
            if (ConfigurationManager.AppSettings.AllKeys.Contains("MailFrom"))
            {
                mailFrom = ConfigurationManager.AppSettings["MailFrom"];
            }
            else
            {
                return;
            }
            if (ConfigurationManager.AppSettings.AllKeys.Contains("MailFromPassword"))
            {
                mailFromPassword = ConfigurationManager.AppSettings["MailFromPassword"];
            }
            else
            {
                return;
            }

            var exceplist = BLLFactory<B_EP_Exceptions>.Instance.GetExceptionsUnSended();
            if (exceplist == null || exceplist.Count == 0)
            {
            }
            else
            {
                var info = new MailInfo();
                var smtpBaseClass = new SmtpBaseClass();
                //找出有多少个收件人
                var emails = (from u in exceplist select u.Email).Distinct().ToList();
                foreach (var email in emails)
                {
                    var exceps = exceplist.Where(u => u.Email == email).ToList();
                    var subject = "用量异常";
                    var body = new StringBuilder();
                    var idList = (from u in exceplist select u.ID).ToList();
                    var ids = string.Join(",", idList);

                    body.AppendLine("以下为设备用量异常,请及时处理!");
                    //创建表格
                    body.AppendLine("<!DOCTYPE html>");
                    body.AppendLine("   <html>");
                    body.AppendLine(
                        "<head>\n\t<style>\n\t\tbody,td{font:9pt 宋体}\n\t\t.b1{background:#ffffff;color:#000000;font-weight:bold;FONT-SIZE: 12pt}\n\t\t.b0{color:#000000}\n\t\t.TDClass2{background:#ffffff;color:#000000;BORDER-BOTTOM: #ccccff 1px solid}\n\t\t.TDClass3{background:#ccccff;color:#000000;width:100px;height:25px}\n\t</style>\n</head>");
                    body.AppendLine("   <body>");
                    body.AppendLine("<table cellSpacing=0 cellPadding=0  border=\"1\">");
                    body.AppendLine("   <tr>");
                    body.AppendLine("       <th width=\"300\">Define定义</th>  ");
                    body.AppendLine("       <th width=\"300\">Meaure度量</th>  ");
                    body.AppendLine("       <th width=\"100\">Analyze分析</th>");
                    body.AppendLine("       <th width=\"100\">Improve改善</th> ");
                    body.AppendLine("       <th width=\"100\">Contral控制</th> ");
                    body.AppendLine("   </tr>");

                    foreach (var item in exceps)
                    {
                        body.AppendLine("   <tr>");
                        body.AppendLine("       <td>" + item.Define + "</td>  ");
                        body.AppendLine("       <td>" + item.Measue + "</td>  ");
                        body.AppendLine("       <td> </td>");
                        body.AppendLine("       <td> </td> ");
                        body.AppendLine("       <td> </td> ");
                        body.AppendLine("   </tr>");
                    }
                    body.AppendLine("</table>");
                    body.AppendLine("   </html>");
                    body.AppendLine("   </body>");
                    try
                    {
                        info = new MailInfo();
                        info.MailFromServerPath = mailFromServerPath;
                        info.MailFrom = mailFrom;
                        info.MailFromPassword = mailFromPassword;
                        info.MailTo = email;
                        info.MailSubject = subject;
                        info.MailBody = body.ToString();
                        var result = smtpBaseClass.SendMail(info);
                        BLLFactory<B_EP_Exceptions>.Instance.UpdateExceptionState(ids, result);
                        Thread.Sleep(5000);
                    }
                    catch (Exception e)
                    {
                        var msg = string.Format("{0}  <{1}> {2}", DateTime.Now.ToString(BasePublic.strTimeFormat),
                            email + "邮件发送失败！", e.Message);
                        Console.WriteLine(string.Format(msg));
                        BasePublic.ExceptionDeal(BaseEnum.Msg, msg);
                    }
                }
            }
        }

        #endregion
    }
}
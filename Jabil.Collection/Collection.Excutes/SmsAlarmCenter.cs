using System;
using System.Configuration;
using System.Linq;
using System.Threading;
using Collection.Base;
using DeerMonitor.Framework.ControlUtil;
using DeerMonitor.SYS.Core.BLL;
using Timer = DeerMonitor.Framework.Commons.Threading.Timer;

namespace Collection.Excutes
{
    /// <summary>
    ///     短信中心
    /// </summary>
    public class SmsAlarmCenter
    {
        #region 构造函数

        #endregion

        #region 启动函数

        /// <summary>
        ///     启动函数
        /// </summary>
        public void Start()
        {
            fSmsBaseClass = new SmsBaseClass();
            try
            {
                fSmsBaseClass.OpenPort(ComPort);
            }
            catch (Exception e)
            {
                var msg = string.Format("{0}  {1} {2}", DateTime.Now.ToString(BasePublic.strTimeFormat),
                    ComPort + "串口打开失败！", e.Message);
                Console.WriteLine(string.Format(msg));
                BasePublic.ExceptionDeal(BaseEnum.Msg, msg);
            }
            timerSend = new Timer(SendSmsPeriod);
            timerSend.Elapsed += TimerSend_Elapsed;
            timerSend.Start();
        }

        #endregion

        #region 变量申明

        private SmsBaseClass fSmsBaseClass;
        private Timer timerSend;

        public string ComPort
        {
            get
            {
                var str = "Com2";
                if (ConfigurationManager.AppSettings.AllKeys.Contains("ComPort"))
                {
                    str = ConfigurationManager.AppSettings["ComPort"];
                }
                return str;
            }
        }

        public int SendSmsPeriod
        {
            get
            {
                var a = 60000;
                if (ConfigurationManager.AppSettings.AllKeys.Contains("SendSmsPeriod"))
                {
                    int.TryParse(ConfigurationManager.AppSettings["SendSmsPeriod"], out a);
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
            SendToPhone();
        }

        /// <summary>
        ///     查询报警短信信息
        /// </summary>
        public void SendToPhone()
        {
            var messagrlist = BLLFactory<B_SYS_MessegeSend>.Instance.Find("(1=1) and Status='1'");
            if (messagrlist == null || messagrlist.Count == 0)
            {
            }
            else
            {
                foreach (var item in messagrlist)
                {
                    try
                    {
                        fSmsBaseClass.SendDTU(item.SendTo, 0, item.Title + item.Content);
                        item.Status = 2;
                        BLLFactory<B_SYS_MessegeSend>.Instance.Update(item, item.ID);
                        Thread.Sleep(5000);
                    }
                    catch (Exception e)
                    {
                        var msg = string.Format("{0}  <{1}> {2}", DateTime.Now.ToString(BasePublic.strTimeFormat),
                            item.ID + "短信发送失败！", e.Message);
                        Console.WriteLine(string.Format(msg));
                        BasePublic.ExceptionDeal(BaseEnum.Msg, msg);
                    }
                }
            }
        }

        #endregion
    }
}
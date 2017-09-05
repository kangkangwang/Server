using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Collection.Base;
using Collection.Excutes;

namespace Jabil.Collection
{
    /// <summary>
    ///     程序主入口
    /// </summary>
    internal class Program
    {
        #region 变量申明

        private static CollectCenter_Jabil collectCenter;
        private static SocketCenter socketCenter;
        private static SmsAlarmCenter smsAlarmCenter;
        private static ListenAdobeCenter listenAdobeCenter;
        private static ExceptionCenter exceptionCenter;

        private static CollectCenter_Jabil collectCenter_NQ;

        /// <summary>
        ///     是否数据采集
        /// </summary>
        private static int isCollect = 0;

        /// <summary>
        ///     是否发送socket
        /// </summary>
        private static int isSocket = 0;

        /// <summary>
        ///     是否发送报警短信
        /// </summary>
        private static int isAlarmSys = 0;

        /// <summary>
        ///     是否监听Adobe
        /// </summary>
        private static int isListenAdobe = 0;

        /// <summary>
        ///     是否监听异常发邮件
        /// </summary>
        private static int isListenExcep = 0;

        /// <summary>
        ///     是否采集耐奇数据
        /// </summary>
        private static int isNQ = 0;

        #endregion

        #region 入口函数

        /// <summary>
        ///     入口函数
        /// </summary>
        public static void Main()
        {
            //应用程序出错处理,调试时关闭
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            BasePublic.Start();
            Thread.Sleep(4000);

            #region 进程及线程控制

            var processes = Process.GetProcesses();
            //获取当前进程名
            var proName = Process.GetCurrentProcess().ProcessName;

            var a = (from li in processes where li.ProcessName == proName select li).Count();

            if (a > 1)
            {
                Console.WriteLine("程序已运行");
                return;
            }

            #region 关闭SocketServer未正常关闭的线程

            var processSockets =
                (from li in processes where li.ProcessName.Contains("SocketServer") select li).ToList();
            foreach (var item in processSockets)
            {
                Console.WriteLine(string.Format("上次遗留Socket进程未关闭"));
                try
                {
                    item.Kill();
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
                Console.WriteLine(string.Format("Socket进程关闭成功"));
                Thread.Sleep(1000);
            }

            #endregion

            #endregion

            #region 读取配置文件及打开相应线程

            if (ConfigurationManager.AppSettings.AllKeys.Contains("IsCollect"))
            {
                int.TryParse(ConfigurationManager.AppSettings["IsCollect"], out isCollect);
            }
            if (ConfigurationManager.AppSettings.AllKeys.Contains("IsSocket"))
            {
                int.TryParse(ConfigurationManager.AppSettings["IsSocket"], out isSocket);
            }
            if (ConfigurationManager.AppSettings.AllKeys.Contains("IsAlarmSys"))
            {
                int.TryParse(ConfigurationManager.AppSettings["IsAlarmSys"], out isAlarmSys);
            }
            if (ConfigurationManager.AppSettings.AllKeys.Contains("IsListenAdobe"))
            {
                int.TryParse(ConfigurationManager.AppSettings["IsListenAdobe"], out isListenAdobe);
            }
            if (ConfigurationManager.AppSettings.AllKeys.Contains("IsListenExcep"))
            {
                int.TryParse(ConfigurationManager.AppSettings["IsListenExcep"], out isListenExcep);
            }
            if (ConfigurationManager.AppSettings.AllKeys.Contains("IsNQ"))
            {
                int.TryParse(ConfigurationManager.AppSettings["IsNQ"], out isNQ);
            }
            
            if (isNQ == 1)
            {
                var threadListenExcep = new Thread(CreateCollect_NQ);
                threadListenExcep.Start();
            }
            #endregion
        }

        #endregion

        #region 应用程序错误处理

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var error = e.ExceptionObject as Exception;
            BasePublic.ExceptionDeal(BaseEnum.Main, error.Message);
            Console.WriteLine("应用程序出错,5s后关闭");
            Thread.Sleep(5000);
            Environment.Exit(-1);
        }

        #endregion

       

        #region 创建NQ采集程序

        /// <summary>
        ///     创建NQ采集程序
        /// </summary>
        private static void CreateCollect_NQ()
        {
            collectCenter_NQ = new CollectCenter_Jabil("NQ");
            collectCenter_NQ.Start();
        }

        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Collection.Core.Model;
using DeerMonitor.Base.Core.BLL;
using DeerMonitor.Base.Core.Entity;
using DeerMonitor.EnergyManage.Core.BLL;
using DeerMonitor.EnergyManage.Core.Entity;
using DeerMonitor.Framework.ControlUtil;
using MySql.Data.MySqlClient;
using Timer = DeerMonitor.Framework.Commons.Threading.Timer;

namespace Collection.Base
{
    /// <summary>
    ///     公共基类
    /// </summary>
    public class BasePublic
    {
        #region 变量申明

        /// <summary>
        ///     数据库链接（主要的）
        /// </summary>
        public static SqlConnection sqlConnection_Main;
        /// <summary>
        ///     数据库链接（主要的2）
        /// </summary>
        public static SqlConnection sqlConnection_Main2;

        public static string strCon
        {
            get
            {
                var str = ConfigurationManager.ConnectionStrings["sqlserver"].ToString();
                return str;
            }
        }
        /// <summary>
        ///     配置信息定时器
        /// </summary>
        private static Timer timerCheck;

        /// <summary>
        ///     所有PLC
        /// </summary>
        public static List<M_MT_TagDefine> lstPLC = new List<M_MT_TagDefine>();

        /// <summary>
        ///     耐奇系统的所有分组
        /// </summary>
        public static List<M_MT_TagDefine> lstNQ = new List<M_MT_TagDefine>();
        /// <summary>
        ///     耐奇系统的所有分组
        /// </summary>
        public static List<M_MT_TagDefine> lstNQ2 = new List<M_MT_TagDefine>();

        /// <summary>
        ///     所有位号
        /// </summary>
        public static List<M_BS_TagDefine> lstTagsAll = new List<M_BS_TagDefine>();
        /// <summary>
        ///     耐奇所有位号
        /// </summary>
        public static List<M_BS_TagDefine> lstTagsAllNQ = new List<M_BS_TagDefine>();

        /// <summary>
        ///     所有能源位号
        /// </summary>
        public static List<M_EM_EnergyTag> lstTagsEnergy = new List<M_EM_EnergyTag>();

        /// <summary>
        ///     添加一个对象作为锁(更新配置信息用)
        /// </summary>
        public static object lockerConfig = new object();

        /// <summary>
        ///     添加一个对象作为锁(日志文件用)
        /// </summary>
        public static object lockerLog = new object();

        /// <summary>
        ///     时间格式化字符串
        /// </summary>
        public static string strTimeFormat = "yyyy-MM-dd HH:mm:ss";


        /// <summary>
        ///     无信号的处理周期
        /// </summary>
        public static int NoDataCount
        {
            get
            {
                var a = 3;
                if (ConfigurationManager.AppSettings.AllKeys.Contains("NoDataCount"))
                {
                    int.TryParse(ConfigurationManager.AppSettings["NoDataCount"], out a);
                }
                return a;
            }
        }

        /// <summary>
        ///     无信号数据时的默认值
        /// </summary>
        public static double NoDataValue
        {
            get
            {
                var a = -0.1;
                if (ConfigurationManager.AppSettings.AllKeys.Contains("NoDataValue"))
                {
                    double.TryParse(ConfigurationManager.AppSettings["NoDataValue"], out a);
                }
                return a;
            }
        }

        /// <summary>
        ///     一个数据的周期
        /// </summary>
        public static int OneDataCount
        {
            get
            {
                var a = 20;
                if (ConfigurationManager.AppSettings.AllKeys.Contains("OneDataCount"))
                {
                    int.TryParse(ConfigurationManager.AppSettings["OneDataCount"], out a);
                }
                return a;
            }
        }

        /// <summary>
        ///     单位时间内允许的报警次数
        /// </summary>
        public static int LimitAlarmCount
        {
            get
            {
                var a = 5;
                if (ConfigurationManager.AppSettings.AllKeys.Contains("LimitAlarmCount"))
                {
                    int.TryParse(ConfigurationManager.AppSettings["LimitAlarmCount"], out a);
                }
                return a;
            }
        }

        /// <summary>
        ///     报警发送的周期
        /// </summary>
        public static int AlarmPeriod
        {
            get
            {
                var a = 900;
                if (ConfigurationManager.AppSettings.AllKeys.Contains("AlarmPeriod"))
                {
                    int.TryParse(ConfigurationManager.AppSettings["AlarmPeriod"], out a);
                }
                return a;
            }
        }

        /// <summary>
        ///     报警发送的周期0
        /// </summary>
        public static int AlarmCount
        {
            get
            {
                var a = 10;
                if (ConfigurationManager.AppSettings.AllKeys.Contains("AlarmCount"))
                {
                    int.TryParse(ConfigurationManager.AppSettings["AlarmCount"], out a);
                }
                return a;
            }
        }

        /// <summary>
        ///     更新配置周期
        /// </summary>
        public static int CheckPeriod
        {
            get
            {
                var a = 60000;
                if (ConfigurationManager.AppSettings.AllKeys.Contains("CheckPeriod"))
                {
                    int.TryParse(ConfigurationManager.AppSettings["CheckPeriod"], out a);
                }
                return a;
            }
        }


        /// <summary>
        ///     日志记录周期
        /// </summary>
        public static int LogPeriod
        {
            get
            {
                var a = 10000;
                if (ConfigurationManager.AppSettings.AllKeys.Contains("LogPeriod"))
                {
                    int.TryParse(ConfigurationManager.AppSettings["LogPeriod"], out a);
                }
                return a;
            }
        }

        /// <summary>
        ///     主进程日志路径
        /// </summary>
        public static string LogDir_Main
        {
            get
            {
                var str = "Log\\Main";
                if (ConfigurationManager.AppSettings.AllKeys.Contains("LogDir_Main"))
                {
                    str = ConfigurationManager.AppSettings["LogDir_Main"];
                }
                return str;
            }
        }

        /// <summary>
        ///     主进程的日志队列
        /// </summary>
        private static readonly List<string> lstLogs_Main = new List<string>();

        /// <summary>
        ///     数采线程日志路径
        /// </summary>
        public static string LogDir_Collect
        {
            get
            {
                var str = "Log\\Collect";
                if (ConfigurationManager.AppSettings.AllKeys.Contains("LogDir_Collect"))
                {
                    str = ConfigurationManager.AppSettings["LogDir_Collect"];
                }
                return str;
            }
        }

        /// <summary>
        ///     数采线程的日志队列
        /// </summary>
        private static readonly List<string> lstLogs_Collect = new List<string>();

        /// <summary>
        ///     Socket线程日志路径
        /// </summary>
        public static string LogDir_Socket
        {
            get
            {
                var str = "Log\\Socket";
                if (ConfigurationManager.AppSettings.AllKeys.Contains("LogDir_Socket"))
                {
                    str = ConfigurationManager.AppSettings["LogDir_Socket"];
                }
                return str;
            }
        }


        /// <summary>
        ///     NQ线程的日志队列
        /// </summary>
        private static readonly List<string> lstLogs_NQ = new List<string>();
        /// <summary>
        ///     耐奇线程日志路径
        /// </summary>
        public static string LogDir_NQ
        {
            get
            {
                var str = "Log\\Socket";
                if (ConfigurationManager.AppSettings.AllKeys.Contains("LogDir_NQ"))
                {
                    str = ConfigurationManager.AppSettings["LogDir_NQ"];
                }
                return str;
            }
        }

        /// <summary>
        ///     Socket线程的日志队列
        /// </summary>
        private static readonly List<string> lstLogs_Socket = new List<string>();

        /// <summary>
        ///     短信线程日志路径
        /// </summary>
        public static string LogDir_Msg
        {
            get
            {
                var str = "Log\\Msg";
                if (ConfigurationManager.AppSettings.AllKeys.Contains("LogDir_Msg"))
                {
                    str = ConfigurationManager.AppSettings["LogDir_Msg"];
                }
                return str;
            }
        }

        /// <summary>
        ///     短信线程的日志队列
        /// </summary>
        private static readonly List<string> lstLogs_Msg = new List<string>();

        /// <summary>
        ///     Adobe验证日志路径
        /// </summary>
        public static string LogDir_Adobe
        {
            get
            {
                var str = "Log\\Adobe";
                if (ConfigurationManager.AppSettings.AllKeys.Contains("LogDir_Adobe"))
                {
                    str = ConfigurationManager.AppSettings["LogDir_Adobe"];
                }
                return str;
            }
        }

        /// <summary>
        ///     Adobe验证线程的日志队列
        /// </summary>
        private static readonly List<string> lstLogs_Adobe = new List<string>();

        /// <summary>
        ///     日志线程定时器
        /// </summary>
        private static Timer timerLog;

        /// <summary>
        ///     允许同时保留日志数
        /// </summary>
        public static int AllowLogCount
        {
            get
            {
                var a = 10;
                if (ConfigurationManager.AppSettings.AllKeys.Contains("AllowLogCount"))
                {
                    int.TryParse(ConfigurationManager.AppSettings["AllowLogCount"], out a);
                }
                return a;
            }
        }

        /// <summary>
        ///     服务器IP地址
        /// </summary>
        public static string ServerIP
        {
            get
            {
                var str = "127.0.0.1";
                if (ConfigurationManager.AppSettings.AllKeys.Contains("ServerIP"))
                {
                    str = ConfigurationManager.AppSettings["ServerIP"];
                }
                return str;
            }
        }

        /// <summary>
        ///     服务器端口
        /// </summary>
        public static int ServerPort
        {
            get
            {
                var a = 8885;
                if (ConfigurationManager.AppSettings.AllKeys.Contains("ServerPort"))
                {
                    int.TryParse(ConfigurationManager.AppSettings["ServerPort"], out a);
                }
                return a;
            }
        }

        public static MySqlConnection mySqlConnection;

        public static string strConMySql
        {
            get
            {
                var str = ConfigurationManager.ConnectionStrings["mysql_NQ"].ToString();
                return str;
            }
        }

        #endregion

        #region 构造函数

        /// <summary>
        ///     构造函数
        /// </summary>
        public BasePublic()
        {

        }

        #endregion

        #region 启动函数

        public static void Start()
        {
            lstNQ = new List<M_MT_TagDefine>();
            lstPLC = new List<M_MT_TagDefine>();
            #region 初始化数据链接

            try
            {
                sqlConnection_Main = new SqlConnection(strCon);
                sqlConnection_Main.Open();
            }
            catch (Exception ex)
            {
            }

            try
            {
                sqlConnection_Main2 = new SqlConnection(strCon);
                sqlConnection_Main2.Open();
            }
            catch (Exception ex)
            {
            }


            try
            {
                mySqlConnection = new MySqlConnection(strConMySql);
                mySqlConnection.Open();
            }
            catch
            {
            }
            Thread.Sleep(1000);

            #endregion

            CheckAndUpdateConfig();

            //初始化配置文件检查线程
            timerCheck = new Timer(CheckPeriod);
            timerCheck.Elapsed += timerCheck_Elapsed;
            timerCheck.Start();

            //初始化日志线程
            timerLog = new Timer(LogPeriod);
            timerLog.Elapsed += timerLog_Elapsed;
            timerLog.Start();
        }

        #endregion

        #region 获取日志文件路径

        /// <summary>
        ///     获取日志文件名称
        /// </summary>
        private static string GetLogFileName(BaseEnum mtype)
        {
            var fileName = "";
            var logFileDir = "";
            switch (mtype)
            {
                case BaseEnum.Adobe:
                    logFileDir = LogDir_Adobe;
                    break;
                case BaseEnum.Collect:
                    logFileDir = LogDir_Collect;
                    break;
                case BaseEnum.Main:
                    logFileDir = LogDir_Main;
                    break;
                case BaseEnum.Msg:
                    logFileDir = LogDir_Msg;
                    break;
                case BaseEnum.Socket:
                    logFileDir = LogDir_Socket;
                    break;
                case BaseEnum.NQ:
                    logFileDir = LogDir_NQ;
                    break;
                default:
                    break;
            }
            if (!Directory.Exists(logFileDir))
            {
                try
                {
                    //创建路径
                    Directory.CreateDirectory(logFileDir);
                    MangeFiles(logFileDir);
                }
                catch (Exception e)
                {
                    fileName = null;
                    ExceptionDeal(BaseEnum.Main,
                        string.Format("{0}  {1}  {2}", DateTime.Now.ToString(strTimeFormat),
                            mtype + "." + "GetLogFileName", e.Message));
                    return null;
                }
            }
            fileName = Application.StartupPath + "\\" + logFileDir + "\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".log";

            return fileName;
        }

        #endregion

        #region 异常处理

        /// <summary>
        ///     异常处理
        /// </summary>
        public static void ExceptionDeal(BaseEnum baseEnum, string msg)
        {
            switch (baseEnum)
            {
                case BaseEnum.Adobe:
                    lock (lstLogs_Adobe)
                    {
                        lstLogs_Adobe.Add(msg);
                    }
                    break;
                case BaseEnum.Collect:
                    lock (lstLogs_Collect)
                    {
                        lstLogs_Collect.Add(msg);
                    }
                    break;
                case BaseEnum.Main:
                    lock (lstLogs_Main)
                    {
                        lstLogs_Main.Add(msg);
                    }
                    break;
                case BaseEnum.Msg:
                    lock (lstLogs_Msg)
                    {
                        lstLogs_Msg.Add(msg);
                    }
                    break;
                case BaseEnum.Socket:
                    lock (lstLogs_Socket)
                    {
                        lstLogs_Socket.Add(msg);
                    }
                    break;
                case BaseEnum.NQ:
                    lock (lstLogs_NQ)
                    {
                        lstLogs_NQ.Add(msg);
                    }
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region 更新配置信息

        /// <summary>
        ///     更新配置信息线程
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void timerCheck_Elapsed(object sender, EventArgs e)
        {
            lock (lockerConfig)
            {
                CheckAndUpdateConfig();
            }
        }

        /// <summary>
        ///     更新配置信息
        /// </summary>
        private static void CheckAndUpdateConfig()
        {
            lock (lockerConfig)
            {
                Console.WriteLine(string.Format("获取配置信息中..."));
                lstPLC.Clear();
                lstNQ.Clear();
                //查找所有PLC
                var lst =
                    BLLFactory<B_BS_TagDefine>.Instance.Find("1=1 and IsUse=1 and TagGroupID='-1' and TagType<>'NQ' ")
                        .OrderBy(p => p.ID)
                        .ToList();
                var lst2 = BLLFactory<B_BS_TagDefine>.Instance.Find("1=1 and IsUse=1 and TagGroupID='-1' and TagType='NQ' ")
                            .OrderBy(p => p.ID)
                            .ToList();
                //查找所有位号
                lstTagsAll =
                    BLLFactory<B_BS_TagDefine>.Instance.Find(
                        string.Format("1=1 and TagGroupID<>'-1' and IsVirtual<1 and TagGroupID<>'NQ001' "), "order by ID");
                //查找NQ所有位号
                 lstTagsAllNQ =
                    BLLFactory<B_BS_TagDefine>.Instance.Find(
                        string.Format("1=1 and TagGroupID<>'-1' and IsVirtual<1 and TagGroupID='NQ001' "), "order by ID");
                if (lst == null || lst.Count == 0)
                {
                    var msg = string.Format("{0}    查询到的PLC数量为0，请检查数据", DateTime.Now.ToString(strTimeFormat));
                    //ExceptionDeal(msg);
                    return;
                }
                //把PLC和其下的所有位号整合都在一起
                foreach (var item in lst)
                {
                    var model = new M_MT_TagDefine();
                    model.ID = item.ID;
                    model.TagAddr = item.TagAddr;
                    model.TagName = item.TagName;
                    model.SendOrder = item.SendOrder;
                    model.lstTags =
                        (from li in lstTagsAll where li.TagGroupID == item.ID select li).OrderBy(p => p.DataIndex)
                            .ToList();
                    model.TagType = item.TagType;
                    lstPLC.Add(model);
                }
                //将NQ位号分组
                var lstStr = (from li in lstTagsAllNQ select li.TagAddr).Distinct().ToList();
                var modelNQ = new M_MT_TagDefine();
                modelNQ.ID = "NQ001";
                modelNQ.TagType = "NQ";

                foreach (var item in lstStr)
                {
                    M_MT_TagDefine model = new M_MT_TagDefine();
                    model.ID = item;
                    model.TagAddr = item;
                    model.TagType = "NQ";
                    var lst00 = (from li in lstTagsAllNQ where li.TagAddr == item select li).OrderBy(p => p.ID).ToList();
                    model.lstTags = lst00;
                    lstNQ.Add(model);
                }
                //foreach (var item in lst2)
                //{
                //    var model = new M_MT_TagDefine();
                //    model.ID = item.ID;
                //    model.TagAddr = item.TagAddr;
                //    model.TagName = item.TagName;
                //    model.SendOrder = item.SendOrder;
                //    model.lstTags =
                //        (from li in lstTagsAll_NQ where li.TagGroupID == item.ID select li).OrderBy(p => p.DataIndex)
                //            .ToList();
                //    model.TagType = item.TagType;
                //    model.CollectPeriod = item.CollectPeriod;
                //    lstNQ2.Add(model);
                //}
                lstTagsEnergy = BLLFactory<B_EM_EnergyTag>.Instance.GetEnergyTagList(null);
                Console.WriteLine("{0}    共查询到共有{1}个PLC", DateTime.Now.ToString(strTimeFormat), lstPLC.Count);
            }
        }

        #endregion

        #region 写日志

        private static void timerLog_Elapsed(object sender, EventArgs e)
        {
            WriteLog(BaseEnum.Adobe);
            WriteLog(BaseEnum.Collect);
            WriteLog(BaseEnum.Main);
            WriteLog(BaseEnum.Msg);
            WriteLog(BaseEnum.Socket);
        }

        /// <summary>
        ///     写日志
        /// </summary>
        private static void WriteLog(BaseEnum mtype)
        {
            var fileName = GetLogFileName(mtype);
            var IsWriteToLog = File.Exists(fileName);
            if (!IsWriteToLog)
            {
                try
                {
                    //须close掉，不然无法写入
                    File.Create(fileName).Close();
                    IsWriteToLog = true;
                }
                catch (Exception e)
                {
                    IsWriteToLog = false;
                    ExceptionDeal(BaseEnum.Main,
                        string.Format("{0}  {1}  {2}", DateTime.Now.ToString(strTimeFormat),
                            mtype + "." + "WriteLog", e.Message));
                }
            }
            if (IsWriteToLog)
            {
                WriteLog2(mtype, fileName);
            }
        }

        private static void WriteLog2(BaseEnum mtype, string fileName)
        {
            lock (lockerLog)
            {
                var file = new FileInfo(fileName);
                if (file.Exists)
                {
                    if (file.Length > 30 * 1024 * 1024)
                    {
                        file.Delete();
                    }
                }
                var sw = new StreamWriter(fileName, true);
                switch (mtype)
                {
                    case BaseEnum.Adobe:

                        #region MyRegion

                        lock (lstLogs_Adobe)
                        {
                            foreach (var item in lstLogs_Adobe)
                            {
                                try
                                {
                                    sw.WriteLine(item);
                                }
                                catch (Exception e)
                                {
                                    ExceptionDeal(mtype,
                                        string.Format("{0}  {1}  {2}", DateTime.Now.ToString(strTimeFormat),
                                            mtype + "." + "WriteLog2", e.Message));
                                }
                            }
                            lstLogs_Adobe.Clear();
                        }

                        #endregion

                        break;
                    case BaseEnum.Collect:

                        #region MyRegion

                        lock (lstLogs_Collect)
                        {
                            foreach (var item in lstLogs_Collect)
                            {
                                try
                                {
                                    sw.WriteLine(item);
                                }
                                catch (Exception e)
                                {
                                    ExceptionDeal(BaseEnum.Main,
                                        string.Format("{0}  {1}  {2}", DateTime.Now.ToString(strTimeFormat),
                                            mtype + "." + "WriteLog2", e.Message));
                                }
                            }
                            lstLogs_Collect.Clear();
                        }
                        break;

                        #endregion

                    case BaseEnum.Main:

                        #region MyRegion

                        lock (lstLogs_Main)
                        {
                            foreach (var item in lstLogs_Main)
                            {
                                try
                                {
                                    sw.WriteLine(item);
                                }
                                catch (Exception e)
                                {
                                    ExceptionDeal(BaseEnum.Main,
                                        string.Format("{0}  {1}  {2}", DateTime.Now.ToString(strTimeFormat),
                                            mtype + "." + "WriteLog2", e.Message));
                                }
                            }
                            lstLogs_Main.Clear();
                        }

                        #endregion

                        break;
                    case BaseEnum.Msg:

                        #region MyRegion

                        lock (lstLogs_Msg)
                        {
                            foreach (var item in lstLogs_Msg)
                            {
                                try
                                {
                                    sw.WriteLine(item);
                                }
                                catch (Exception e)
                                {
                                    ExceptionDeal(BaseEnum.Main,
                                        string.Format("{0}  {1}  {2}", DateTime.Now.ToString(strTimeFormat),
                                            mtype + "." + "WriteLog2", e.Message));
                                }
                            }
                            lstLogs_Msg.Clear();
                        }

                        #endregion

                        break;
                    case BaseEnum.Socket:

                        #region MyRegion

                        lock (lstLogs_Socket)
                        {
                            foreach (var item in lstLogs_Socket)
                            {
                                try
                                {
                                    sw.WriteLine(item);
                                }
                                catch (Exception e)
                                {
                                    ExceptionDeal(BaseEnum.Main,
                                        string.Format("{0}  {1}  {2}", DateTime.Now.ToString(strTimeFormat),
                                            mtype + "." + "WriteLog2", e.Message));
                                }
                            }
                            lstLogs_Socket.Clear();
                        }

                        #endregion

                        break;
                    default:
                        break;
                }
                sw.Close();
                sw.Dispose();
            }
        }

        /// <summary>
        ///     管理日志文件个数
        /// </summary>
        /// <param name="LogDir"></param>
        private static void MangeFiles(string LogDir)
        {
            var files = Directory.GetFiles(LogDir).OrderBy(p => p);
            var count = AllowLogCount;
            if (files.Count() < AllowLogCount)
            {
                count = files.Count();
            }
            var files_Delete = files.Take(count).ToList();
            foreach (var item in files_Delete)
            {
                if (File.Exists(item))
                {
                    File.Delete(item);
                }
            }
        }

        #endregion
    }
}
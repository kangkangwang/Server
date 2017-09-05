//using System;
//using System.Collections.Generic;
//using System.Configuration;
//using System.Data;
//using System.Data.SqlClient;
//using System.IO;
//using System.Linq;
//using System.Net;
//using System.Net.Sockets;
//using System.Threading;
//using DeerMonitor.Base.Core.BLL;
//using DeerMonitor.Base.Core.Entity;
//using DeerMonitor.Framework.ControlUtil;
//using Collection.Base.Helper;
//using Collection.Core.Model;
//using Timer = DeerMonitor.Framework.Commons.Threading.Timer;

//namespace Collection.Base.Excute
//{
//    /// <summary>
//    ///     数据采集中心
//    /// </summary>
//    public class CollectCenter1
//    {
//        #region 变量申明

//        /// <summary>
//        /// 添加一个对象作为锁(日志文件用)
//        /// </summary>
//        public static object locker = new object();

//        /// <summary>
//        /// 添加一个对象作为锁(更新配置信息用)
//        /// </summary>
//        public static object lockerConfig = new object();

//        /// <summary>
//        ///     线程信号灯
//        /// </summary>
//        public static ManualResetEvent eventX = new ManualResetEvent(false);

//        /// <summary>
//        ///     完成的线程数
//        /// </summary>
//        public static int iCount = 0;

//        /// <summary>
//        ///     最大线程数
//        /// </summary>
//        public static int iMaxCount = 0;

//        /// <summary>
//        ///     保存数据的变量
//        /// </summary>
//        public static DataTable dtTable = new DataTable();

//        /// <summary>
//        ///     日志路径
//        /// </summary>
//        private readonly string LogDir = ConfigurationManager.AppSettings["LogPath"]; //创建写入文件

//        /// <summary>
//        /// 日志线程的队列
//        /// </summary>
//        private readonly List<string> lstLogs = new List<string>();
//        /// <summary>
//        /// socket服务端
//        /// </summary>
//        //private readonly SocketServer socketServer = new SocketServer();

//        /// <summary>
//        ///     结束时间
//        /// </summary>
//        private DateTime dtE = DateTime.Now;

//        /// <summary>
//        ///     开始时间
//        /// </summary>
//        private DateTime dtS = DateTime.Now;

//        /// <summary>
//        ///     所有PLC
//        /// </summary>
//        public List<M_MT_TagDefine> lstPLC = new List<M_MT_TagDefine>();

//        /// <summary>
//        ///     所有位号
//        /// </summary>
//        public List<M_BS_TagDefine> lstTagsAll = new List<M_BS_TagDefine>();

//        /// <summary>
//        ///     时间格式化字符串
//        /// </summary>
//        private string strTimeFormat = "yyyy-MM-dd HH:mm:ss";

//        /// <summary>
//        /// 日志线程定时器
//        /// </summary>
//        private Timer timerLog = null;

//        /// <summary>
//        /// 配置信息定时器
//        /// </summary>
//        private Timer timerCheck = null;

//        /// <summary>
//        /// 检查数据定时器
//        /// </summary>
//        private Timer timerUpdate = null;
//        #endregion

//        string failedPLC = "";

//        #region 启动方法

//        /// <summary>
//        ///     启动方法
//        /// </summary>
//        public void Start()
//        {
//            #region 创建datatable格式

//            dtTable = new DataTable();
//            dtTable.Columns.Add(new DataColumn("ID"));
//            dtTable.Columns.Add(new DataColumn("TagID"));
//            dtTable.Columns.Add(new DataColumn("RealValue"));
//            dtTable.Columns.Add(new DataColumn("RealState"));
//            dtTable.Columns.Add(new DataColumn("RealTime"));
//            dtTable.Columns.Add(new DataColumn("RealMinVal"));
//            dtTable.Columns.Add(new DataColumn("RealMaxVal"));
//            dtTable.Columns.Add(new DataColumn("RealVarVal"));
//            dtTable.Columns.Add(new DataColumn("RecordTime"));

//            #endregion


//            CheckAndUpdateConfig();

//            //启动socketServer
//            //socketServer.StartTCPServer2();

//            //初始化日志线程
//            timerLog = new Timer(2000);
//            timerLog.Elapsed += timerLog_Elapsed;
//            timerLog.Start();
//            //初始化配置文件检查线程
//            timerCheck = new Timer(1000 * 60);
//            timerCheck.Elapsed += timerCheck_Elapsed;
//            timerCheck.Start();


//            while (true)
//            {
//                lock (lockerConfig)
//                {
//                    //初始化线程池
//                    ThreadPool.SetMaxThreads(lstPLC.Count, lstPLC.Count);
//                    dtS = DateTime.Now;
//                    iMaxCount = lstPLC.Count;
//                    iCount = 0;
//                    dtS = DateTime.Now;
//                    if (true)
//                    {
//                        Console.Write("上次失败的PLC有:" + failedPLC);
//                        failedPLC = "";
//                        for (int i = 0; i < lstPLC.Count; i++)
//                        {
//                            ThreadPool.QueueUserWorkItem(ThreadRun, lstPLC[i]);
//                        }
//                        //等待事件的完成，即线程调用ManualResetEvent.Set()方法
//                        //eventX.WaitOne  阻止当前线程，直到当前 WaitHandle 收到信号为止。 
//                        eventX.WaitOne(Timeout.Infinite, true); //3000); //, true);
//                        Thread.Sleep(2000);
//                    }
//                }
//            }
//        }

//        #endregion

//        #region 更新配置信息
//        /// <summary>
//        /// 更新配置信息
//        /// </summary>
//        private void CheckAndUpdateConfig()
//        {
//            Console.WriteLine(string.Format("获取配置信息中..."));
//            lstPLC = new List<M_MT_TagDefine>();
//            //查找所有PLC
//            List<M_BS_TagDefine> lst =
//                BLLFactory<B_BS_TagDefine>.Instance.Find("1=1 and IsVirtual=1 and IsUse=1 and TagGroupID='-1'")
//                    .OrderBy(p => p.ID)
//                    .ToList();
//            //查找所有位号
//            lstTagsAll = BLLFactory<B_BS_TagDefine>.Instance.Find(string.Format("1=1 and TagGroupID<>'-1' "), "order by ID");
//            if (lst == null || lst.Count == 0)
//            {
//                string msg = string.Format("{0}    查询到的PLC数量为0，请检查数据", DateTime.Now.ToString(strTimeFormat));
//                ExceptionDeal(msg);
//                return;
//            }
//            //把PLC和其下的所有位号整合都在一起
//            foreach (M_BS_TagDefine item in lst)
//            {
//                var model = new M_MT_TagDefine();
//                model.ID = item.ID;
//                model.TagAddr = item.TagAddr;
//                model.TagName = item.TagName;
//                model.SendOrder = item.SendOrder;
//                model.lstTags =
//                    (from li in lstTagsAll where li.TagGroupID == item.ID select li).OrderBy(p => p.ID).ToList();
//                lstPLC.Add(model);
//            }
//            Console.WriteLine("{0}    共查询到共有{1}个PLC", DateTime.Now.ToString(strTimeFormat), lstPLC.Count);
//        }
//        #endregion

//        #region 启动线程,获取数据
//        private static ManualResetEvent TimeoutObject = new ManualResetEvent(false);
//        private static void CallBackMethod(IAsyncResult asyncresult)
//        {
//            object[] obs = (object[])asyncresult.AsyncState;
//            Socket so = (Socket)obs[0];
//            ManualResetEvent timer = (ManualResetEvent)obs[1];
//            try
//            {

//                //IsConnectionSuccessful = false;
//                //TcpClient tcpclien0t = asyncresult.AsyncState as TcpClient;
//                //var tcpclient = (Socket)asyncresult.AsyncState;
//                if (so != null)
//                {
//                    so.EndConnect(asyncresult);
//                    //IsConnectionSuccessful = true;
//                }
//                //client.EndConnect(asyncresult);
//            }
//            catch (Exception ex)
//            {
//                //IsConnectionSuccessful = false;
//                //socketexception = ex;
//            }
//            finally
//            {
//                timer.Set();
//            }
//        }

//        /// <summary>
//        ///     读取tagdefine表中的IP信息及指令并循环创建连接
//        /// </summary>
//        /// <param name="item"></param>
//        public void ThreadRun(object item)
//        {
//            #region 创建socket连接

//            var info = item as M_MT_TagDefine;
//            if (info == null)
//            {
//                return;
//            }
//            string socketIP = info.TagAddr;
//            string[] arrSendOrder = info.SendOrder.Split(',');
//            var barrySendOrder = new byte[arrSendOrder.Length];
//            for (int i = 0; i < arrSendOrder.Length; i++)
//            {
//                barrySendOrder[i] = Convert.ToByte(arrSendOrder[i], 16);
//                string a = string.Format("0x{0:X}", barrySendOrder[i]);
//            }
//            int threadID = Thread.CurrentThread.ManagedThreadId; //获取当前线程的ID标识
//            DateTime dt = DateTime.Now;
//            int Port = 502; //访问的端口号
//            IPAddress IP = IPAddress.Parse(socketIP);
//            var ipe = new IPEndPoint(IP, Port); //把ip和端口转化为IPEndPoint的实例
//            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); //创建socket实例
//                                                                                                      //socket.SendTimeout = 1000;
//                                                                                                      //socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.NoDelay, true);
//                                                                                                      // socket.BeginConnect(ipe, AcceptCallback, socket);
//            #endregion


//            //socket连接建立及连接异常处理
//            try
//            {
//                object[] obs = new object[3];
//                TimeoutObject = new ManualResetEvent(false);
//                obs[0] = socket;
//                obs[1] = TimeoutObject;
//                //socket.Connect(ipe);

//                socket.BeginConnect(ipe, new AsyncCallback(CallBackMethod), obs);
//                //socket.ConnectTimeOut(ipe, 5000);
//                if (TimeoutObject.WaitOne(5000, false))
//                {

//                    //if (IsConnectionSuccessful)
//                    //{
//                    //    rs = true;
//                    //}
//                    //else
//                    //{
//                    //    rs = false;
//                    //    throw socketexception;
//                    //}
//                }
//                else
//                {
//                    failedPLC += info.ID + ",";
//                    //rs = false;
//                    //socket.Close();
//                    throw new TimeoutException("TimeOut Exception");
//                }
//            }
//            catch (Exception e)
//            {
//                string msg = string.Format("{0}  模块<{1}>连接失败,{2},{3}", dt, info.ID, threadID,
//                    e.Message);
//                Console.WriteLine(string.Format(msg));
//                ExceptionDeal(msg);
//                return;
//            }
//            // Console.WriteLine(string.Format("{0}  模块<{1}({2})>连接成功,{3}", dt, info.TagName, info.ID, threadID));
//            try
//            {
//                socket.Send(barrySendOrder, barrySendOrder.Length, SocketFlags.None); //modbusTCP套接字命令发送
//            }
//            catch (Exception e)
//            {
//                string msg = string.Format("{0}  模块<{1}>通讯失败,{2},{3}", dt, info.ID, threadID,
//                    e.Message);
//                Console.WriteLine(string.Format(msg));
//                ExceptionDeal(msg);
//                return;
//            }
//            //Console.WriteLine(string.Format("{0}  模块<{1}({2})>通讯成功,{3}", dt, info.TagName, info.ID, threadID));
//            int n = barrySendOrder[11];
//            int m = 10 + 2 * n - 1;
//            var recData = new byte[m];
//            try
//            {
//                socket.Receive(recData, recData.Length, 0); //套接字读取
//                //socket.BeginReceive(recData, 0, recData.Length, 0, new AsyncCallback(ReadCallback), socket);
//            }
//            catch (Exception e)
//            {
//                string msg = string.Format("{0}   模块<{1}>接收失败,{2},{3}", dt, info.ID, threadID,
//                    e.Message);
//                Console.WriteLine(string.Format(msg));
//                ExceptionDeal(msg);
//                return;
//            }
//            //Console.WriteLine(string.Format("{0}  模块<{1}({2})>接收成功,{3}", dt, info.TagName, info.ID, threadID));

//            string st = DataConvert.ByteArrayToHexStr(recData);
//            socket.Close();
//            socket.Dispose(); //释放socket连接
//            //Console.WriteLine(string.Format("{0}  模块<{1}({2})>关闭,开始转化接收到的数据,{3}", dt, info.TagName, info.ID, threadID));

//            #region 数据转化到datatable

//            if (!string.IsNullOrEmpty(st))
//            {
//                st = st.Substring(18, 2 * m - 18);
//                var sd = new string[n];
//                int t = 0;
//                string msgValue = "";
//                try
//                {
//                    for (int i = 0; i < n; i++)
//                    {
//                        sd[i] = st.Substring(t, 4);
//                        sd[i] = DataConvert.realValue(sd[i]);
//                        t += 4;
//                    }
//                }
//                catch (Exception e)
//                {
//                    throw new Exception(e.Message);
//                }
//                lock (dtTable)
//                {
//                    foreach (M_BS_TagDefine model in info.lstTags)
//                    {
//                        DataRow dr = dtTable.NewRow();
//                        decimal dValue = DataConvert.GetOneData2(model, sd);
//                        dr["TagID"] = model.ID;
//                        dr["RealValue"] = dValue;
//                        dr["RealTime"] = DateTime.Parse(dtS.ToString(strTimeFormat));
//                        dr["RecordTime"] = DateTime.Parse(dtS.ToString(strTimeFormat));
//                        if (model.Formule != "10")
//                        {
//                            dtTable.Rows.Add(dr);
//                        }
//                        msgValue += string.Format("{0},", dr["RealValue"]);
//                    }

//                    Console.WriteLine("{0}  模块<{1}({2})>数据：{3}", dt, info.TagName, info.ID, msgValue);
//                }
//            }

//            #endregion

//            Interlocked.Increment(ref iCount);
//            if (iCount == iMaxCount)
//            {
//                Thread.Sleep(100);
//                Console.WriteLine("发出结束信号!");
//                dtE = DateTime.Now;
//                var ts1 = new TimeSpan(dtS.Ticks);
//                var ts2 = new TimeSpan(dtE.Ticks);
//                TimeSpan ts3 = ts1.Subtract(ts2).Duration();
//                Console.WriteLine("执行一次轮询话费时间为:{0}秒", ts3.TotalMilliseconds / 1000);
//                InsertToServer();
//            }
//        }
//        #endregion

//        #region 异常处理
//        /// <summary>
//        ///     异常处理
//        /// </summary>
//        public void ExceptionDeal(string msg)
//        {
//            Interlocked.Increment(ref iCount);
//            lock (lstLogs)
//            {
//                lstLogs.Add(msg);
//            }
//            if (iCount == iMaxCount)
//            {
//                Thread.Sleep(100);
//                Console.WriteLine("发出结束信号!");
//                dtE = DateTime.Now;
//                var ts1 = new TimeSpan(dtS.Ticks);
//                var ts2 = new TimeSpan(dtE.Ticks);
//                TimeSpan ts3 = ts1.Subtract(ts2).Duration();

//                Console.WriteLine("执行一次轮询话费时间为:{0}秒", ts3.TotalMilliseconds / 1000);
//                InsertToServer();
//            }
//        }
//        #endregion

//        #region 写入数据库

//        /// <summary>
//        ///     写入数据库
//        /// </summary>
//        public void InsertToServer()
//        {
//            string sqlCon = ConfigurationManager.ConnectionStrings["sqlserver"].ConnectionString;
//            var sqlConnection = new SqlConnection(sqlCon);
//            //BulkWriteToServer(sqlConnection, "T_BS_Realdata");
//            BulkWriteToServer(sqlConnection, "T_BS_Realdata");
//        }


//        /// <summary>
//        ///     大数据写入数据库
//        /// </summary>
//        /// <param name="con"></param>
//        /// <param name="destinationtablename"></param>
//        private void BulkWriteToServer(SqlConnection con, string destinationtablename)
//        {
//            try
//            {
//                lock (dtTable)
//                {
//                    //socketServer.SendToClient(dtTable, dtS);
//                    if (con.State == ConnectionState.Closed)
//                    {
//                        con.Open();
//                    }
//                    var topbranddtcopy = new SqlBulkCopy(con, SqlBulkCopyOptions.CheckConstraints | SqlBulkCopyOptions.FireTriggers, null);
//                    topbranddtcopy.DestinationTableName = destinationtablename;
//                    //var dt = DataTableHelper.ListToDataTable(lstData);
//                    topbranddtcopy.WriteToServer(dtTable);

//                    dtE = DateTime.Now;
//                    var ts1 = new TimeSpan(dtS.Ticks);
//                    var ts2 = new TimeSpan(dtE.Ticks);
//                    TimeSpan ts3 = ts1.Subtract(ts2).Duration();
//                    Console.WriteLine("写入{0}条数据,花费了{1}秒", dtTable.Rows.Count, ts3.TotalMilliseconds / 1000);
//                    dtTable.Rows.Clear();
//                }
//                con.Close();
//                con.Dispose();
//                eventX.Set();
//            }
//            catch (Exception e)
//            {
//                eventX.Set();
//                throw;
//            }
//        }

//        #endregion

//        #region 写日志

//        private void timerLog_Elapsed(object sender, EventArgs e)
//        {
//            WriteLog();
//        }

//        /// <summary>
//        ///     写日志
//        /// </summary>
//        private void WriteLog()
//        {
//            #region 判断日志文件是否存在

//            if (!Directory.Exists(LogDir))
//            {
//                //创建路径
//                Directory.CreateDirectory(LogDir);
//            }
//            string fileName = LogDir + "/" + DateTime.Now.ToString("yyyy-MM-dd") + ".log";

//            bool IsWriteToLog = File.Exists(fileName);
//            if (!IsWriteToLog)
//            {
//                try
//                {
//                    //须close掉，不然无法写入
//                    File.Create(fileName).Close();

//                    IsWriteToLog = true;
//                }
//                catch
//                {
//                    IsWriteToLog = false;
//                }
//            }

//            #endregion

//            lock (locker)
//            {
//                var file = new FileInfo(fileName);
//                if (file.Exists)
//                {
//                    if (file.Length > 20 * 1024 * 1024)
//                    {
//                        file.Delete();
//                    }
//                }
//                var sw = new StreamWriter(fileName, true);
//                lock (lstLogs)
//                {
//                    foreach (string item in lstLogs)
//                    {
//                        sw.WriteLine(item);
//                    }
//                    //开始写入值
//                    sw.Close();
//                    sw.Dispose();
//                }
//            }
//        }

//        #endregion

//        #region 更新线程
//        /// <summary>
//        /// 更新线程
//        /// </summary>
//        /// <param name="sender"></param>
//        /// <param name="e"></param>
//        private void timerCheck_Elapsed(object sender, EventArgs e)
//        {
//            lock (lockerConfig)
//            {
//                CheckAndUpdateConfig();
//                //socketServer.CheckAndUpdateConfig(lstTagsAll);
//            }
//        }
//        #endregion
//    }
//}


//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.SqlClient;
//using System.Linq;
//using System.Net;
//using System.Net.Sockets;
//using System.Threading;
//using DeerMonitor.Base.Core.Entity;
//using DeerMonitor.Framework.ControlUtil;
//using Collection.Base.Base;
//using Collection.Base.Helper;
//using Collection.Core.Model;
//using DeerMonitor.EquipmentManage.Core.BLL;
//using DeerMonitor.EquipmentManage.Core.Entity;

//namespace Collection.Base.Excute
//{
//    /// <summary>
//    ///     数据采集中心
//    /// </summary>
//    public class CollectCenter
//    {
//        #region 变量申明


//        /// <summary>
//        ///     开始时间
//        /// </summary>
//        private DateTime dtS = DateTime.Now;

//        /// <summary>
//        /// Tags的数据集
//        /// </summary>
//        private DataSet dsTags = null;

//        /// <summary>
//        /// T_BS_TagDefine适配器
//        /// </summary>
//        private SqlDataAdapter sqlDataAdapter = null;

//        /// <summary>
//        /// T_BS_Raldata_Spt适配器
//        /// </summary>
//        private SqlDataAdapter sqlDataAdapter_Spt = null;

//        /// <summary>
//        /// 设备位号关系表
//        /// </summary>
//        private List<M_EP_EquipTag> lstEquipTags = null;

//        /// <summary>
//        /// 缓存信息表
//        /// </summary>
//        private MTTable mtTemp = new MTTable();

//        #endregion

//        #region 启动方法

//        /// <summary>
//        /// 启动方法
//        /// </summary>
//        public void Start()
//        {
//            lstEquipTags = BLLFactory<B_EP_EquipTag>.Instance.GetAll();


//            sqlDataAdapter = new SqlDataAdapter("select * from T_BS_TagDefine", BasePublic.sqlConnection_Main);
//            SqlCommandBuilder sb = new SqlCommandBuilder(sqlDataAdapter);
//            dsTags = new DataSet();
//            sqlDataAdapter.Fill(dsTags, "T_BS_TagDefine");
//            sqlDataAdapter_Spt = new SqlDataAdapter();
//            SqlCommand selectCmd = new SqlCommand();
//            selectCmd.CommandText = "select * from T_BS_Realdata_Spt";
//            selectCmd.Connection = BasePublic.sqlConnection_Main;
//            sqlDataAdapter_Spt.SelectCommand = selectCmd;
//            sqlDataAdapter_Spt.Fill(dsTags, "T_BS_Realdata_Spt");
//            SqlCommand updateCmd = new SqlCommand();
//            updateCmd.CommandText = "update T_BS_Realdata_Spt set RealValue=@RealValue,RealTime=@RealTime where ID=@ID";
//            updateCmd.Parameters.Add("@RealValue", SqlDbType.Decimal, 18, "RealValue");
//            updateCmd.Parameters.Add("@RealTime", SqlDbType.DateTime, 18, "RealTime");
//            updateCmd.Parameters.Add("@ID", SqlDbType.Int, 18, "ID");
//            sqlDataAdapter_Spt.UpdateCommand = updateCmd;

//            dtS = DateTime.Now;
//            if (true)
//            {
//                lock (BasePublic.lockerConfig)
//                {
//                    foreach (var item in BasePublic.lstPLC)
//                    {
//                        Thread th = new Thread(new ParameterizedThreadStart(ThreadRun));
//                        th.Start(item);
//                    }
//                }
//            }
//        }

//        #endregion

//        #region 启动线程,获取数据

//        /// <summary>
//        /// 读取tagdefine表中的IP信息及指令并循环创建连接
//        /// </summary>
//        /// <param name="item"></param>
//        public void ThreadRun(object item)
//        {
//            while (true)
//            {
//                OneDeal(item);
//                Thread.Sleep(2000);
//            }
//        }

//        /// <summary>
//        /// 单个PLC取数据
//        /// </summary>
//        /// <param name="item"></param>
//        void OneDeal(object item)
//        {
//            lock (BasePublic.lockerConfig)
//            {
//                dtS = DateTime.Now;
//                Thread.Sleep(1);

//                var info = item as M_MT_TagDefine;
//                if (info == null)
//                {
//                    return;
//                }

//                #region 创建socket连接

//                string socketIP = info.TagAddr;
//                string[] arrSendOrder = info.SendOrder.Split(',');
//                var barrySendOrder = new byte[arrSendOrder.Length];
//                for (int i = 0; i < arrSendOrder.Length; i++)
//                {
//                    barrySendOrder[i] = Convert.ToByte(arrSendOrder[i], 16);
//                    string a = string.Format("0x{0:X}", barrySendOrder[i]);
//                }
//                int threadID = Thread.CurrentThread.ManagedThreadId; //获取当前线程的ID标识
//                DateTime dt = DateTime.Now;
//                int Port = 502; //访问的端口号
//                IPAddress IP = null;
//                try
//                {
//                    IP = IPAddress.Parse(socketIP);
//                }
//                catch (Exception e)
//                {
//                    string msg = string.Format("{0}  {1} {2}  {3}", dt, info.ID + "转换地址时出错", threadID,
//                         e.Message);
//                    Console.WriteLine(string.Format(msg));
//                    BasePublic.ExceptionDeal(BaseEnum.Collect, msg);
//                    return;
//                }
//                var ipe = new IPEndPoint(IP, Port); //把ip和端口转化为IPEndPoint的实例
//                var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); //创建socket实例
//                socket.SendTimeout = 1000;
//                socket.ReceiveTimeout = 1000;
//                socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.AcceptConnection, true);
//                #endregion

//                //socket连接建立及连接异常处理
//                try
//                {
//                    if (!socket.Connected)
//                    {
//                        socket.Connect(ipe);
//                    }
//                }
//                catch (Exception e)
//                {
//                    string msg = string.Format("{0}  模块<{1}>连接失败,{2},{3}", dt, info.ID, threadID,
//                        e.Message);
//                    Console.WriteLine(string.Format(msg));
//                    BasePublic.ExceptionDeal(BaseEnum.Collect, msg);
//                    socket.Close();
//                    socket.Dispose();
//                    return;
//                }
//                //Console.WriteLine(string.Format("{0}  模块<{1}({2})>连接成功,{3}", dt, info.TagName, info.ID, threadID));
//                try
//                {
//                    socket.Send(barrySendOrder, barrySendOrder.Length, 0); //modbusTCP套接字命令发送
//                }
//                catch (Exception e)
//                {
//                    string msg = string.Format("{0}  模块<{1}>通讯失败,{2},{3}", dt, info.ID, threadID,
//                        e.Message);
//                    Console.WriteLine(string.Format(msg));
//                    BasePublic.ExceptionDeal(BaseEnum.Collect, msg);
//                    socket.Close();
//                    socket.Dispose();
//                    return;
//                }
//                // Console.WriteLine(string.Format("{0}  模块<{1}({2})>通讯成功,{3}", dt, info.TagName, info.ID, threadID));
//                int n = barrySendOrder[11];
//                int m = 10 + 2 * n - 1;
//                var recData = new byte[m];
//                try
//                {
//                    socket.Receive(recData, recData.Length, 0); //套接字读取
//                    //socket.BeginReceive(recData, 0, recData.Length, 0, new AsyncCallback(ReadCallback), socket);
//                }
//                catch (Exception e)
//                {
//                    string msg = string.Format("{0}   模块<{1}>接收失败,{2},{3}", dt, info.ID, threadID,
//                        e.Message);
//                    Console.WriteLine(string.Format(msg));
//                    BasePublic.ExceptionDeal(BaseEnum.Collect, msg);
//                    socket.Close();
//                    socket.Dispose();
//                    return;
//                }
//                //Console.WriteLine(string.Format("{0}  模块<{1}({2})>接收成功,{3}", dt, info.TagName, info.ID, threadID));

//                string st = DataConvert.ByteArrayToHexStr(recData);

//                #region 数据转化到datatable
//                DataTable dtTags = dsTags.Tables["T_BS_TagDefine"];

//                DataTable dtStore = new DataTable();
//                CreateDataTable(ref dtStore, "T_BS_Realdata");
//                DataTable dtSend = dsTags.Tables["T_BS_Realdata_Spt"];
//                //CreateDataTable(ref dtSend);
//                DataTable dtAlarm = new DataTable();
//                CreateDataTable(ref dtAlarm, "T_EP_EquipWarning");
//                if (!string.IsNullOrEmpty(st))
//                {
//                    st = st.Substring(18, 2 * m - 18);
//                    var sd = new string[n];
//                    int t = 0;
//                    string msgValue = "";
//                    try
//                    {
//                        for (int i = 0; i < n; i++)
//                        {
//                            sd[i] = st.Substring(t, 4);
//                            sd[i] = DataConvert.realValue(sd[i]);
//                            t += 4;
//                        }
//                    }
//                    catch (Exception e)
//                    {
//                        //throw new Exception(e.Message);
//                        string msg = string.Format("{0}  {1}  {2},{3}", dt, info.ID + "转化数据失败", threadID,
//                       e.Message);
//                        Console.WriteLine(string.Format(msg));
//                        BasePublic.ExceptionDeal(BaseEnum.Collect, msg);
//                    }

//                    foreach (M_BS_TagDefine model in info.lstTags)
//                    {
//                        DataRow drTag = dtTags.Select(string.Format("ID='{0}'", model.ID)).FirstOrDefault();
//                        //MTKeyValue tempInfo= (from li in mtTemp where li.Key == model.ID select li).FirstOrDefault();
//                        DataRow drSend = dtSend.Select(string.Format("TagID='{0}'", model.ID)).FirstOrDefault();
//                        DataRow drStore = dtStore.NewRow();
//                        decimal dValue = DataConvert.GetOneData2(model, sd);
//                        drStore["TagID"] = model.ID;
//                        drStore["RealValue"] = dValue;
//                        drStore["RealTime"] = dtS.ToString(BasePublic.strTimeFormat);
//                        drStore["RecordTime"] = dtS.ToString(BasePublic.strTimeFormat);
//                        //drStore = drStore;

//                        if (drTag != null)
//                        {
//                            if (model.Formule != "10")
//                            {
//                                //dtSend.Rows.Add(drSend);
//                                int period = 0;
//                                if (drTag["StorePeriod"] != null)
//                                {
//                                    int.TryParse(drTag["StorePeriod"].ToString(), out period);
//                                }
//                                DateTime dt00;
//                                if (!DateTime.TryParse(drTag["LastTime"].ToString(), out dt00))
//                                {
//                                    dt00 = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 00:00"));
//                                }
//                                DateTime dt11 = dtS.AddSeconds(-period);
//                                if (dt00 <= dt11)
//                                {

//                                    dtStore.Rows.Add(drStore);
//                                    drTag["LastTime"] = dtS;
//                                    drTag.AcceptChanges();
//                                }
//                                drSend["RealValue"] = dValue;
//                                drSend["RealTime"] = dtS.ToString(BasePublic.strTimeFormat); ;
//                                //drSend.AcceptChanges();
//                                //dtSend.AcceptChanges();
//                                //dsTags.Tables["T_BS_Realdata_Spt"].AcceptChanges();
//                                //dsTags.AcceptChanges();
//                            }
//                            //object ob2 = drTag["LastTime"];
//                            //object ob3 = drTag[22];
//                            ////drTag["LastTime"] = dtS.ToString(strTimeFormat);
//                            //drTag[22] = dtS.ToString(strTimeFormat);

//                        }
//                        msgValue += string.Format("{0},", drStore["RealValue"]);
//                    }

//                    VerfyAlarm(BasePublic.lstTagsAll, dtStore, mtTemp, ref dtAlarm);
//                    //Console.WriteLine("{0}  模块<{1}({2})>数据：{3}", dt, info.TagName, info.ID, msgValue);
//                    //Console.WriteLine(msgValue);
//                }
//                #endregion
//                UpdateToServer();
//                InsertToServer(dtStore, "T_BS_Realdata");
//            }

//            //manualReset_Config.Set();
//        }
//        #endregion

//        #region 验证数据是否报警
//        /// <summary>
//        /// 验证数据是否报警，并操作
//        /// </summary>
//        /// <param name="lstAll"></param>
//        /// <param name="dtStore"></param>
//        /// <param name="mtTemp"></param>
//        /// <param name="dtAlarm"></param>
//        public void VerfyAlarm(List<M_BS_TagDefine> lstAll, DataTable dtStore, MTTable mtTemp, ref DataTable dtAlarm)
//        {
//            bool rs = false;
//            M_BS_TagDefine modelTag = null;
//            MTKeyValue tempInfo = null;
//            foreach (DataRow item in dtStore.Rows)
//            {
//                modelTag = (from li in lstAll where li.ID == item["TagID"].ToString() select li).FirstOrDefault();
//                tempInfo = (from li in mtTemp where li.Key == item["TagID"].ToString() select li).FirstOrDefault();
//                if (tempInfo == null || modelTag == null)
//                {
//                    continue;
//                }
//                if (tempInfo.Count == 0)
//                {
//                    tempInfo.StartTime = DateTime.Now;
//                    tempInfo.AlarmCount = 0;
//                }
//                if (tempInfo.Count >= BasePublic.OneDataCount)
//                {
//                    tempInfo.StartTime = DateTime.Now;
//                    tempInfo.Count = 0;
//                    tempInfo.AlarmCount = 0;
//                }
//                tempInfo.Count += 1;
//                bool IsAlarm = false;
//                int status = 0;
//                if (modelTag.IsAlarm)
//                {
//                    //该位号的前置位号
//                    M_BS_TagDefine modelPreTags =
//                        (from li in BasePublic.lstTagsAll where li.ID == modelTag.PreTags select li).FirstOrDefault();
//                    if (modelPreTags == null || string.IsNullOrEmpty(modelPreTags.PreTags))
//                    {
//                    }
//                    else
//                    {
//                        //查找前置位号的值
//                        DataRow modelPreValue = dtStore.Select(string.Format("TagID='{0}'", modelPreTags.ID)).FirstOrDefault();
//                        //(from li in lst where li.TagID == model_PreTags.ID select li).FirstOrDefault();
//                        if (modelPreValue != null && decimal.Parse(modelPreValue["RealValue"].ToString()) > 0)
//                        {
//                            IsAlarm = true;
//                        }
//                    }
//                    if (IsAlarm)
//                    {
//                        bool isDo = (decimal.Parse(item["RealValue"].ToString()) > modelTag.TagHH) || (decimal.Parse(item["RealValue"].ToString()) < modelTag.TagLL);
//                        if (isDo)
//                        {
//                            tempInfo.AlarmCount += 1;
//                            //tempInfo.Status = status;
//                        }
//                        else
//                        {
//                            tempInfo.AlarmCount = 0;
//                            //tempInfo.Status = 0;
//                        }
//                        if (tempInfo.AlarmCount >= BasePublic.LimitAlarmCount)
//                        {
//                            if (!(modelTag.AlarmTime <= DateTime.Parse(item["RealTime"].ToString()).AddSeconds(-BasePublic.AlarmPeriod)))
//                            {
//                                continue;
//                            }
//                            M_EP_EquipTag modelEquipTag =
//                                (from li in lstEquipTags where li.TagCode == modelTag.ID select li).FirstOrDefault();
//                            if (modelEquipTag == null)
//                            {
//                                continue;
//                            }
//                            DataRow drAlarm = dtAlarm.NewRow();
//                            drAlarm["ID"] = Guid.NewGuid();
//                            drAlarm["EquCode"] = modelEquipTag.EquCode;
//                            drAlarm["EquName"] = modelEquipTag.EquipName;
//                            drAlarm["TagCode"] = modelEquipTag.TagCode;
//                            drAlarm["TagName"] = modelEquipTag.TagName;
//                            drAlarm["TagEU"] = modelTag.TagEU;
//                            drAlarm["TagLL"] = modelTag.TagLL;
//                            drAlarm["TagL"] = modelTag.TagL;
//                            drAlarm["TagH"] = modelTag.TagH;
//                            drAlarm["TagHH"] = modelTag.TagHH;
//                            drAlarm["RealValue"] = item["RealValue"];
//                            drAlarm["RealState"] = 1;
//                            drAlarm["RealTime"] = item["RealTime"];
//                            drAlarm["IsDealed"] = 1;
//                            dtAlarm.Rows.Add(drAlarm);

//                            tempInfo.AlarmCount = 0;
//                            tempInfo.Status = 0;
//                        }
//                    }
//                }
//            }
//            InsertToServer(dtAlarm, "T_EP_EquipWarning");
//        }

//        #endregion

//        #region 创建dt格式
//        private void CreateDataTable(ref DataTable dt, string type = "")
//        {
//            dt = new DataTable();
//            if (type == "T_BS_Realdata")
//            {
//                #region MyRegion
//                dt.Columns.Add(new DataColumn("ID") { DataType = typeof(int) });
//                dt.Columns.Add(new DataColumn("TagID") { DataType = typeof(string) });
//                dt.Columns.Add(new DataColumn("RealValue") { DataType = typeof(decimal) });
//                dt.Columns.Add(new DataColumn("RealState") { DataType = typeof(int) });
//                dt.Columns.Add(new DataColumn("RealTime") { DataType = typeof(DateTime) });
//                dt.Columns.Add(new DataColumn("RealMinVal") { DataType = typeof(decimal) });
//                dt.Columns.Add(new DataColumn("RealMaxVal") { DataType = typeof(decimal) });
//                dt.Columns.Add(new DataColumn("RealVarVal") { DataType = typeof(decimal) });
//                dt.Columns.Add(new DataColumn("RecordTime") { DataType = typeof(DateTime) });
//                #endregion
//            }
//            if (type == "T_EP_EquipWarning")
//            {
//                #region MyRegion
//                dt.Columns.Add(new DataColumn("ID") { DataType = typeof(string) });
//                dt.Columns.Add(new DataColumn("EquCode") { DataType = typeof(string) });
//                dt.Columns.Add(new DataColumn("EquName") { DataType = typeof(string) });
//                dt.Columns.Add(new DataColumn("TagCode") { DataType = typeof(string) });
//                dt.Columns.Add(new DataColumn("TagName") { DataType = typeof(string) });
//                dt.Columns.Add(new DataColumn("TagEU") { DataType = typeof(string) });
//                dt.Columns.Add(new DataColumn("TagLL") { DataType = typeof(decimal) });
//                dt.Columns.Add(new DataColumn("TagL") { DataType = typeof(decimal) });
//                dt.Columns.Add(new DataColumn("TagH") { DataType = typeof(decimal) });
//                dt.Columns.Add(new DataColumn("TagHH") { DataType = typeof(decimal) });
//                dt.Columns.Add(new DataColumn("RealValue") { DataType = typeof(decimal) });
//                dt.Columns.Add(new DataColumn("RealState") { DataType = typeof(int) });
//                dt.Columns.Add(new DataColumn("RealTime") { DataType = typeof(DateTime) });
//                dt.Columns.Add(new DataColumn("IsDealed") { DataType = typeof(bool) });
//                #endregion
//            }
//        }

//        #endregion

//        #region 写入数据库

//        /// <summary>
//        ///     写入数据库
//        /// </summary>
//        public void InsertToServer(DataTable dt, string tableName)
//        {
//            if (dt.Rows.Count == 0)
//            {
//                return;
//            }
//            BulkWriteToServer(BasePublic.sqlConnection_Main, tableName, dt);
//        }

//        /// <summary>
//        ///     大数据写入数据库
//        /// </summary>
//        /// <param name="con"></param>
//        /// <param name="destinationtablename"></param>
//        private void BulkWriteToServer(SqlConnection con, string tableName, DataTable dt)
//        {
//            lock (BasePublic.lockerConfig)
//            {
//                try
//                {
//                    if (con.State != ConnectionState.Open)
//                    {
//                        con.Open();
//                    }

//                    var sqlBulkCopy = new SqlBulkCopy(con, SqlBulkCopyOptions.CheckConstraints | SqlBulkCopyOptions.FireTriggers, null);
//                    //一次批量的插入的数据量
//                    sqlBulkCopy.BatchSize = 1000;
//                    //超时之前操作完成所允许的秒数，如果超时则事务不会提交 ，数据将回滚，所有已复制的行都会从目标表中移除
//                    sqlBulkCopy.BulkCopyTimeout = 60;

//                    //設定 NotifyAfter 属性，以便在每插入10000 条数据时，呼叫相应事件。  
//                    sqlBulkCopy.NotifyAfter = 10000;
//                    sqlBulkCopy.SqlRowsCopied += new SqlRowsCopiedEventHandler(OnSqlRowsCopied);

//                    //映射关系
//                    //topbranddtcopy.ColumnMappings.Add("", "");
//                    CreateMapping(sqlBulkCopy, dt);
//                    sqlBulkCopy.DestinationTableName = tableName;
//                    sqlBulkCopy.WriteToServer(dt);
//                }
//                catch (Exception e)
//                {
//                    string msg = string.Format("{0}  {1}  {2}  {3}", dt, tableName, "批量插入数据库失败", e.Message);
//                    Console.WriteLine(string.Format(msg));
//                    BasePublic.ExceptionDeal(BaseEnum.Collect, msg);
//                }
//            }
//        }

//        #region 批量更新T_BS_Realdata_Spt
//        /// <summary>
//        /// 批量更新T_BS_Realdata_Spt
//        /// </summary>
//        void UpdateToServer()
//        {
//            if (sqlDataAdapter != null)
//            {

//            }
//            if (sqlDataAdapter_Spt != null)
//            {
//                SqlCommandBuilder sql = new SqlCommandBuilder(sqlDataAdapter_Spt);
//                sqlDataAdapter_Spt.Update(dsTags, "T_BS_Realdata_Spt");
//                dsTags.AcceptChanges();
//            }
//        }
//        #endregion


//        //响应时事件
//        void OnSqlRowsCopied(object sender, SqlRowsCopiedEventArgs e)
//        {
//            Console.WriteLine("入库行数：" + e.RowsCopied);
//        }

//        private void CreateMapping(SqlBulkCopy slqBulkCopy, DataTable dtTable)
//        {
//            foreach (DataColumn item in dtTable.Columns)
//            {
//                slqBulkCopy.ColumnMappings.Add(item.ColumnName, item.ColumnName);
//            }
//        }

//        #endregion
//    }
//}


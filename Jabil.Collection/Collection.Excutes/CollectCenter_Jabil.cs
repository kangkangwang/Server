using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Collection.Base;
using Collection.Common;
using Collection.Core.BLL;
using Collection.Core.Helper;
using Collection.Core.Model;
using Collection.Helper;
using DeerMonitor.Base.Core.Entity;
using DeerMonitor.EquipmentManage.Core.BLL;
using DeerMonitor.EquipmentManage.Core.Entity;
using DeerMonitor.Framework.ControlUtil;

namespace Collection.Excutes
{
    /// <summary>
    ///     数据采集中心
    /// </summary>
    public class CollectCenter_Jabil
    {
        #region 变量申明

        private string systemID = "0";
        /// <summary>
        ///     开始时间
        /// </summary>
        private DateTime dtS = DateTime.Now;

        /// <summary>
        ///     Tags的数据集
        /// </summary>
        private DataSet dsTags;

        /// <summary>
        ///     实时表spt的数据集
        /// </summary>
        private DataSet dsRealdata_Spt;

        /// <summary>
        ///     实时表spt的数据集
        /// </summary>
        private DataSet dsRealdata_Spt_NQ;

        /// <summary>
        ///     T_BS_TagDefine适配器
        /// </summary>
        private SqlDataAdapter sqlDataAdapter_Tags;

        /// <summary>
        ///     T_BS_Raldata_Spt适配器
        /// </summary>
        private SqlDataAdapter sqlDataAdapter_Spt;

        /// <summary>
        ///     T_BS_Raldata_Spt适配器
        /// </summary>
        private SqlDataAdapter sqlDataAdapter_Spt_NQ;

        /// <summary>
        ///     设备位号关系表
        /// </summary>
        private List<M_EP_EquipTag> lstEquipTags;

        /// <summary>
        ///     缓存信息表
        /// </summary>
        private readonly MTTable mtTemp = new MTTable();

        #endregion

        public CollectCenter_Jabil(string a = "0")
        {
            systemID = a;
        }

        #region 启动方法

        /// <summary>
        ///     启动方法
        /// </summary>
        public void Start()
        {
            dsTags = new DataSet();
            dsRealdata_Spt = new DataSet();
            //lstEquipTags = BLLFactory<B_EP_EquipTag>.Instance.GetAll();
            if (systemID == "0")
            {
                sqlDataAdapter_Tags = new SqlDataAdapter(string.Format(@"select b.* from T_BS_TagDefine a 
                                                                         inner join T_BS_TagDefine b on a.TagGroupID='-1' and a.TagType='{0}' 
                                                                         and b.TagGroupID=a.ID AND b.IsUse=1 ", systemID), BasePublic.sqlConnection_Main);
            }
            if (systemID == "NQ")
            {
                sqlDataAdapter_Tags = new SqlDataAdapter(string.Format(@"select b.* from T_BS_TagDefine a 
                                                                         inner join T_BS_TagDefine b on a.TagGroupID='-1' and a.TagType='{0}' 
                                                                         and b.TagGroupID=a.ID AND b.IsUse=1 ", systemID), BasePublic.sqlConnection_Main2);
            }
            var sb = new SqlCommandBuilder(sqlDataAdapter_Tags);
            sqlDataAdapter_Tags.Fill(dsTags, "T_BS_TagDefine");
            sqlDataAdapter_Spt = new SqlDataAdapter();
            var selectCmd = new SqlCommand();
            selectCmd.CommandText = string.Format(@"select c.* from T_BS_TagDefine a 
                                      inner join T_BS_TagDefine b on a.TagGroupID='-1' and a.TagType='{0}' and b.TagGroupID=a.ID AND b.IsUse=1
                                      inner join T_BS_Realdata_Spt c on c.TagID=b.ID  order by c.TagID", systemID);
            selectCmd.Connection = BasePublic.sqlConnection_Main;
            sqlDataAdapter_Spt.SelectCommand = selectCmd;
            sqlDataAdapter_Spt.Fill(dsRealdata_Spt, "T_BS_Realdata_Spt");
            var updateCmd = new SqlCommand();
            updateCmd.CommandText = "update T_BS_Realdata_Spt set RealValue=@RealValue,RealTime=@RealTime where ID=@ID ";
            updateCmd.Parameters.Add("@RealValue", SqlDbType.Decimal, 18, "RealValue");
            updateCmd.Parameters.Add("@RealTime", SqlDbType.DateTime, 18, "RealTime");
            updateCmd.Parameters.Add("@ID", SqlDbType.Int, 32, "ID");
            sqlDataAdapter_Spt.UpdateCommand = updateCmd;


            dtS = DateTime.Now;
            if (true)
            {
                lock (BasePublic.lockerConfig)
                {
                    if (systemID == "NQ")
                    {
                        foreach (var item in BasePublic.lstNQ2)
                        {
                            Thread th = new Thread(new ParameterizedThreadStart(ThreadRun));
                            th.Start(item);
                            //ThreadRun(BasePublic.lstNQ[0]);
                        }
                    }
                    else
                    {
                        foreach (var item in BasePublic.lstPLC)
                        {
                            Thread th = new Thread(new ParameterizedThreadStart(ThreadRun));
                            th.Start(item);
                            //ThreadRun(item);
                        }
                    }

                }
            }
        }

        #endregion

        #region 验证数据是否报警

        /// <summary>
        ///     验证数据是否报警，并操作
        /// </summary>
        /// <param name="lstAll"></param>
        /// <param name="dtStore"></param>
        /// <param name="mtTemp"></param>
        /// <param name="dtAlarm"></param>
        public void VerfyAlarm(List<M_BS_TagDefine> lstAll, DataTable dtStore, MTTable mtTemp, ref DataTable dtAlarm)
        {
            var rs = false;
            M_BS_TagDefine modelTag = null;
            MTKeyValue tempInfo = null;
            foreach (DataRow item in dtStore.Rows)
            {
                modelTag = (from li in lstAll where li.ID == item["TagID"].ToString() select li).FirstOrDefault();
                tempInfo = (from li in mtTemp where li.Key == item["TagID"].ToString() select li).FirstOrDefault();
                if (tempInfo == null || modelTag == null)
                {
                    continue;
                }
                if (tempInfo.Count == 0)
                {
                    tempInfo.StartTime = DateTime.Now;
                    tempInfo.AlarmCount = 0;
                }
                if (tempInfo.Count >= BasePublic.OneDataCount)
                {
                    tempInfo.StartTime = DateTime.Now;
                    tempInfo.Count = 0;
                    tempInfo.AlarmCount = 0;
                }
                tempInfo.Count += 1;
                var IsAlarm = false;
                var status = 0;
                if (modelTag.IsAlarm)
                {
                    //该位号的前置位号
                    var modelPreTags =
                        (from li in BasePublic.lstTagsAll where li.ID == modelTag.PreTags select li).FirstOrDefault();
                    if (modelPreTags == null || string.IsNullOrEmpty(modelPreTags.PreTags))
                    {
                    }
                    else
                    {
                        //查找前置位号的值
                        var modelPreValue =
                            dtStore.Select(string.Format("TagID='{0}'", modelPreTags.ID)).FirstOrDefault();
                        //(from li in lst where li.TagID == model_PreTags.ID select li).FirstOrDefault();
                        if (modelPreValue != null && decimal.Parse(modelPreValue["RealValue"].ToString()) > 0)
                        {
                            IsAlarm = true;
                        }
                    }
                    if (IsAlarm)
                    {
                        var isDo = (decimal.Parse(item["RealValue"].ToString()) > modelTag.TagHH) ||
                                   (decimal.Parse(item["RealValue"].ToString()) < modelTag.TagLL);
                        if (isDo)
                        {
                            tempInfo.AlarmCount += 1;
                            //tempInfo.Status = status;
                        }
                        else
                        {
                            tempInfo.AlarmCount = 0;
                            //tempInfo.Status = 0;
                        }
                        if (tempInfo.AlarmCount >= BasePublic.LimitAlarmCount)
                        {
                            if (
                                !(modelTag.AlarmTime <=
                                  DateTime.Parse(item["RealTime"].ToString()).AddSeconds(-BasePublic.AlarmPeriod)))
                            {
                                continue;
                            }
                            var modelEquipTag =
                                (from li in lstEquipTags where li.TagCode == modelTag.ID select li).FirstOrDefault();
                            if (modelEquipTag == null)
                            {
                                continue;
                            }
                            var drAlarm = dtAlarm.NewRow();
                            drAlarm["ID"] = Guid.NewGuid();
                            drAlarm["EquCode"] = modelEquipTag.EquCode;
                            drAlarm["EquName"] = modelEquipTag.EquipName;
                            drAlarm["TagCode"] = modelEquipTag.TagCode;
                            drAlarm["TagName"] = modelEquipTag.TagName;
                            drAlarm["TagEU"] = modelTag.TagEU;
                            drAlarm["TagLL"] = modelTag.TagLL;
                            drAlarm["TagL"] = modelTag.TagL;
                            drAlarm["TagH"] = modelTag.TagH;
                            drAlarm["TagHH"] = modelTag.TagHH;
                            drAlarm["RealValue"] = item["RealValue"];
                            drAlarm["RealState"] = 1;
                            drAlarm["RealTime"] = item["RealTime"];
                            drAlarm["IsDealed"] = 1;
                            dtAlarm.Rows.Add(drAlarm);

                            tempInfo.AlarmCount = 0;
                            tempInfo.Status = 0;
                        }
                    }
                }
            }
            InsertToServer(dtAlarm, "T_EP_EquipWarning");
        }

        #endregion

        #region 启动线程,获取数据

        /// <summary>
        ///     读取tagdefine表中的IP信息及指令并循环创建连接
        /// </summary>
        /// <param name="item"></param>
        public void ThreadRun(object item)
        {
            while (true)
            {
                var info = item as M_MT_TagDefine;
                if (info == null)
                {
                    return;
                }
                if (info.TagType.Contains("NQ"))
                {
                    OneDeal_NQ(info);
                }
                else
                {
                    OneDeal(info);
                }

                Thread.Sleep(info.CollectPeriod);
            }
        }

        /// <summary>
        ///     单个PLC取数据
        /// </summary>
        /// <param name="item"></param>
        private void OneDeal(M_MT_TagDefine info)
        {

            dtS = DateTime.Now;
            Thread.Sleep(1);
            if (info == null)
            {
                return;
            }

            #region 创建socket连接

            var socketIP = info.TagAddr;
            if (string.IsNullOrEmpty(socketIP))
            {
                return;
            }
            var arrSendOrder = info.SendOrder.Split(',');
            var barrySendOrder = new byte[arrSendOrder.Length];
            for (var i = 0; i < arrSendOrder.Length; i++)
            {
                barrySendOrder[i] = Convert.ToByte(arrSendOrder[i], 16);
                var a = string.Format("0x{0:X}", barrySendOrder[i]);
            }
            var threadID = Thread.CurrentThread.ManagedThreadId; //获取当前线程的ID标识
            var dt = DateTime.Now;
            var Port = 502; //访问的端口号
            IPAddress IP = null;
            try
            {
                IP = IPAddress.Parse(socketIP);
            }
            catch (Exception e)
            {
                var msg = string.Format("{0}  {1} {2}  {3}", dt, info.ID + "转换地址时出错", threadID,
                    e.Message);
                Console.WriteLine(string.Format(msg));
                BasePublic.ExceptionDeal(BaseEnum.Collect, msg);
                return;
            }
            var ipe = new IPEndPoint(IP, Port); //把ip和端口转化为IPEndPoint的实例
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); //创建socket实例
            socket.SendTimeout = 1000;
            socket.ReceiveTimeout = 1000;
            socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.AcceptConnection, true);

            #endregion

            //socket连接建立及连接异常处理
            try
            {
                if (!socket.Connected)
                {
                    socket.Connect(ipe);
                }
            }
            catch (Exception e)
            {
                var msg = string.Format("{0}  模块<{1}>连接失败,{2},{3}", dt, info.ID, threadID,
                    e.Message);
                Console.WriteLine(string.Format(msg));
                BasePublic.ExceptionDeal(BaseEnum.Collect, msg);
                socket.Close();
                socket.Dispose();
                return;
            }
            //Console.WriteLine(string.Format("{0}  模块<{1}({2})>连接成功,{3}", dt, info.TagName, info.ID, threadID));
            try
            {
                socket.Send(barrySendOrder, barrySendOrder.Length, 0); //modbusTCP套接字命令发送
            }
            catch (Exception e)
            {
                var msg = string.Format("{0}  模块<{1}>通讯失败,{2},{3}", dt, info.ID, threadID,
                    e.Message);
                Console.WriteLine(string.Format(msg));
                BasePublic.ExceptionDeal(BaseEnum.Collect, msg);
                socket.Close();
                socket.Dispose();
                return;
            }
            // Console.WriteLine(string.Format("{0}  模块<{1}({2})>通讯成功,{3}", dt, info.TagName, info.ID, threadID));
            int n = barrySendOrder[11];
            var m = 10 + 2 * n - 1;
            var recData = new byte[m];
            try
            {
                socket.Receive(recData, recData.Length, 0); //套接字读取
                //校验前缀 / 地址和数据长度
                var result_tag = true;
                for (var i = 6; i < 8; i++)
                {
                    result_tag = result_tag && (recData[i] == Convert.ToByte(arrSendOrder[i], 16));
                }
                if (result_tag && n == recData[8] / 2)
                {
                    recData = recData.Skip(9).ToArray();
                }
                else
                {
                    var msg = string.Format("{0}   模块<{1}>校验失败,{2}", dt, info.ID, threadID);
                    Console.WriteLine(string.Format(msg));
                    BasePublic.ExceptionDeal(BaseEnum.Collect, msg);
                    return;
                }

            }
            catch (Exception e)
            {
                var msg = string.Format("{0}   模块<{1}>接收失败,{2},{3}", dt, info.ID, threadID,
                    e.Message);
                Console.WriteLine(string.Format(msg));
                BasePublic.ExceptionDeal(BaseEnum.Collect, msg);
                socket.Close();
                socket.Dispose();
                return;
            }
            //Console.WriteLine(string.Format("{0}  模块<{1}({2})>接收成功,{3}", dt, info.TagName, info.ID, threadID));
            //string tttt = String.Join(",", recData);
            //string st = DataConvert.ByteArrayToHexStr(recData);
            var dtStore = new DataTable();
            lock (BasePublic.lockerConfig)
            {
                #region 数据转化到datatable

                var dtTags = dsTags.Tables["T_BS_TagDefine"];


                DataTableHelper.CreateDataTable(ref dtStore, "T_BS_Realdata");
                var dtSpt = dsRealdata_Spt.Tables["T_BS_Realdata_Spt"];
                //CreateDataTable(ref dtSpt);
                var dtAlarm = new DataTable();
                DataTableHelper.CreateDataTable(ref dtAlarm, "T_EP_EquipWarning");
                var fs = new object[n / 2];
                var analysis_index = 0; //recData第一个数据的0索引位置
                for (var i = 0; i < info.lstTags.Count; i++)
                {
                    var current_analysis = info.lstTags.Find(delegate(M_BS_TagDefine temp) { return temp.DataIndex == i; });

                    #region 数据解析

                    /*0orNull default(float)
                     * 1 char
                     * 2 int16
                     * 3 int32
                     * 4 int64
                     * 5 uint16
                     * 6 uint32
                     * 7 uint64
                     * 8 float
                     * 9 double
                     * */

                    #region 数据解析(过期)

                    //switch (current_analysis.DataType)
                    //{
                    //    case 1://char
                    //        {
                    //            byte[] aa = new byte[sizeof(char)];
                    //            Array.Copy(recData, analysis_index, aa, 0, sizeof(char));
                    //            aa=aa.Reverse().ToArray();
                    //            fs[i] = BitConverter.ToChar(aa, 0);
                    //            analysis_index += sizeof(char);
                    //        }
                    //        break;
                    //    case 2://int16
                    //        {
                    //            byte[] aa = new byte[sizeof(Int16)];
                    //            Array.Copy(recData, analysis_index, aa, 0, sizeof(Int16));
                    //            aa = aa.Reverse().ToArray();
                    //            fs[i] = BitConverter.ToInt16(aa, 0);
                    //            analysis_index += sizeof(Int16);
                    //        }
                    //        break;
                    //    case 3://int32
                    //        {
                    //            byte[] aa = new byte[sizeof(Int32)];
                    //            Array.Copy(recData, analysis_index, aa, 0, sizeof(Int32));
                    //            aa = aa.Reverse().ToArray();
                    //            fs[i] = BitConverter.ToInt32(aa, 0);
                    //            analysis_index += sizeof(Int32);
                    //        }
                    //        break;
                    //    case 4://int64
                    //        {
                    //            byte[] aa = new byte[sizeof(Int64)];
                    //            Array.Copy(recData, analysis_index, aa, 0, sizeof(Int64));
                    //            aa = aa.Reverse().ToArray();
                    //            fs[i] = BitConverter.ToInt64(aa, 0);
                    //            analysis_index += sizeof(Int64);
                    //        }
                    //        break;
                    //    case 5://uint16
                    //        {
                    //            byte[] aa = new byte[sizeof( UInt16)];
                    //            Array.Copy(recData, analysis_index, aa, 0, sizeof(UInt16));
                    //            aa = aa.Reverse().ToArray();
                    //            fs[i] = BitConverter.ToUInt16(aa, 0);
                    //            analysis_index += sizeof(UInt16);
                    //        }
                    //        break;
                    //    case 6://uint32
                    //        {
                    //            byte[] aa = new byte[sizeof(UInt32)];
                    //            Array.Copy(recData, analysis_index, aa, 0, sizeof(UInt32));
                    //            aa = aa.Reverse().ToArray();
                    //            fs[i] = BitConverter.ToUInt32(aa, 0);
                    //            analysis_index += sizeof(UInt32);
                    //        }
                    //        break;
                    //    case 7://uint64
                    //        {
                    //            byte[] aa = new byte[sizeof(Int64)];
                    //            Array.Copy(recData, analysis_index, aa, 0, sizeof(Int64));
                    //            aa = aa.Reverse().ToArray();
                    //            fs[i] = BitConverter.ToUInt64(aa, 0);
                    //            analysis_index += sizeof(Int64);
                    //        }
                    //        break;
                    //    case 8://float
                    //        {
                    //            byte[] aa = new byte[sizeof(float)];
                    //            Array.Copy(recData, analysis_index, aa, 0, sizeof(float));
                    //            aa = aa.Reverse().ToArray();
                    //            fs[i] = BitConverter.ToSingle(aa, 0);
                    //            analysis_index += sizeof(float);
                    //        }
                    //        break;
                    //    case 9:
                    //        {
                    //            byte[] aa = new byte[sizeof(double)];
                    //            Array.Copy(recData, analysis_index, aa, 0, sizeof(double));
                    //            aa = aa.Reverse().ToArray();
                    //            fs[i] = BitConverter.ToDouble(aa, 0);
                    //            analysis_index += sizeof(double);
                    //        }
                    //        break;
                    //    default://float
                    //        {
                    //            byte[] aa = new byte[sizeof(float)];
                    //            Array.Copy(recData, analysis_index, aa, 0, sizeof(float));
                    //            aa = aa.Reverse().ToArray();
                    //            fs[i] = BitConverter.ToSingle(aa, 0);
                    //            analysis_index += sizeof(float);
                    //        }
                    //        break;
                    //}

                    #endregion

                    //正确的转换顺序

                    #region 数据解析

                    switch (current_analysis.DataType)
                    {
                        case 1: //char
                            {
                                var aa = new byte[sizeof(char)];
                                Array.Copy(recData, analysis_index, aa, 0, sizeof(char));
                                aa = aa.Reverse().ToArray();
                                fs[i] = BitConverter.ToChar(aa, 0);
                                analysis_index += sizeof(char);
                            }
                            break;
                        case 2: //int16
                            {
                                var aa = new byte[sizeof(Int16)];
                                Array.Copy(recData, analysis_index, aa, 0, sizeof(Int16));
                                aa = aa.Reverse().ToArray();
                                fs[i] = BitConverter.ToInt16(aa, 0);
                                analysis_index += sizeof(Int16);
                            }
                            break;
                        case 3: //int32
                            {
                                var aa = new byte[sizeof(Int32)];
                                Array.Copy(recData, analysis_index, aa, 0, sizeof(Int32));
                                aa = aa.Reverse().ToArray();
                                fs[i] = BitConverter.ToInt32(new[] { aa[2], aa[3], aa[0], aa[1] }, 0);
                                analysis_index += sizeof(Int32);
                            }
                            break;
                        case 4: //int64
                            {
                                var aa = new byte[sizeof(Int64)];
                                Array.Copy(recData, analysis_index, aa, 0, sizeof(Int64));
                                aa = aa.Reverse().ToArray();
                                fs[i] = BitConverter.ToInt64(new[] { aa[4], aa[5], aa[2], aa[3], aa[0], aa[1] }, 0);
                                analysis_index += sizeof(Int64);
                            }
                            break;
                        case 5: //uint16
                            {
                                var aa = new byte[sizeof(UInt16)];
                                Array.Copy(recData, analysis_index, aa, 0, sizeof(UInt16));
                                aa = aa.Reverse().ToArray();
                                fs[i] = BitConverter.ToUInt16(aa, 0);
                                analysis_index += sizeof(UInt16);
                            }
                            break;
                        case 6: //uint32
                            {
                                var aa = new byte[sizeof(UInt32)];
                                Array.Copy(recData, analysis_index, aa, 0, sizeof(UInt32));
                                aa = aa.Reverse().ToArray();
                                fs[i] = BitConverter.ToUInt32(new[] { aa[2], aa[3], aa[0], aa[1] }, 0);
                                analysis_index += sizeof(UInt32);
                            }
                            break;
                        case 7: //uint64
                            {
                                var aa = new byte[sizeof(Int64)];
                                Array.Copy(recData, analysis_index, aa, 0, sizeof(Int64));
                                aa = aa.Reverse().ToArray();
                                fs[i] = BitConverter.ToUInt64(new[] { aa[4], aa[5], aa[2], aa[3], aa[0], aa[1] }, 0);
                                analysis_index += sizeof(Int64);
                            }
                            break;
                        case 8: //float
                            {
                                var aa = new byte[sizeof(float)];
                                Array.Copy(recData, analysis_index, aa, 0, sizeof(float));
                                aa = aa.Reverse().ToArray();
                                fs[i] = BitConverter.ToSingle(new[] { aa[2], aa[3], aa[0], aa[1] }, 0);
                                analysis_index += sizeof(float);
                            }
                            break;
                        case 9:
                            {
                                //byte[] aa = new byte[sizeof(double)];
                                //Array.Copy(recData, analysis_index, aa, 0, sizeof(double));
                                //aa = aa.Reverse().ToArray();
                                //fs[i] = BitConverter.ToDouble(new byte[] { aa[6],aa[7],aa[4],aa[5],aa[2], aa[3], aa[0], aa[1] }, 0);
                                //analysis_index += sizeof(double);

                                var aa = new byte[sizeof(UInt32)];
                                Array.Copy(recData, analysis_index, aa, 0, sizeof(UInt32));
                                aa = aa.Reverse().ToArray();
                                double dd = BitConverter.ToUInt32(new[] { aa[2], aa[3], aa[0], aa[1] }, 0);
                                analysis_index += sizeof(UInt32);

                                var bb = new byte[sizeof(UInt32)];
                                Array.Copy(recData, analysis_index, bb, 0, sizeof(UInt32));
                                bb = bb.Reverse().ToArray();
                                double de = BitConverter.ToUInt32(new[] { bb[2], bb[3], bb[0], bb[1] }, 0);
                                analysis_index += sizeof(UInt32);
                                var df = (dd * Math.Pow(10, 6) + de / 100) / 100;
                                fs[i] = df;
                            }
                            break;
                        case 10:
                            {
                                //byte[] aa = new byte[sizeof(double)];
                                //Array.Copy(recData, analysis_index, aa, 0, sizeof(double));
                                //aa = aa.Reverse().ToArray();
                                //fs[i] = BitConverter.ToDouble(new byte[] { aa[6],aa[7],aa[4],aa[5],aa[2], aa[3], aa[0], aa[1] }, 0);
                                //analysis_index += sizeof(double);

                                var aa = new byte[sizeof(UInt32)];
                                Array.Copy(recData, analysis_index, aa, 0, sizeof(UInt32));
                                aa = aa.Reverse().ToArray();
                                double dd = BitConverter.ToSingle(new[] { aa[2], aa[3], aa[0], aa[1] }, 0);
                                analysis_index += sizeof(UInt32);

                                var bb = new byte[sizeof(UInt32)];
                                Array.Copy(recData, analysis_index, bb, 0, sizeof(UInt32));
                                bb = bb.Reverse().ToArray();
                                double de = BitConverter.ToSingle(new[] { bb[2], bb[3], bb[0], bb[1] }, 0);
                                analysis_index += sizeof(UInt32);
                                var df = (dd * Math.Pow(10, 2) + de);
                                fs[i] = df;
                            }
                            break;
                        default: //float
                            {
                                var aa = new byte[sizeof(float)];
                                Array.Copy(recData, analysis_index, aa, 0, sizeof(float));
                                aa = aa.Reverse().ToArray();
                                fs[i] = BitConverter.ToSingle(new[] { aa[2], aa[3], aa[0], aa[1] }, 0);
                                analysis_index += sizeof(float);
                            }
                            break;
                    }

                    #endregion
                }

                    #endregion

                #region 数据操作
                foreach (var model in info.lstTags)
                {
                    var drTag = dtTags.Select(string.Format("ID='{0}'", model.ID)).FirstOrDefault();
                    if (drTag != null)
                    {
                        var drSpt = dtSpt.Select(string.Format("TagID='{0}'", model.ID)).FirstOrDefault();
                        var drStore = dtStore.NewRow();
                        var dValue = Convert.ToDouble(fs[model.DataIndex]);
                        drStore["TagID"] = model.ID;
                        decimal dValue3 = 0;
                        decimal.TryParse(dValue.ToString("#.000"), out dValue3);
                        if (model.CompensateRatio > 0)
                        {
                            dValue3 = dValue3 * model.CompensateRatio;
                        }
                        dValue3 = (Math.Abs(dValue3) > (decimal)(Math.Pow(10, 15) - 1)) ? 0 : dValue3;
                        lock (BasePublic.lockerConfig)
                        {
                            drStore["RealValue"] = dValue3;
                            drStore["RealTime"] = dtS.ToString(BasePublic.strTimeFormat);
                            drStore["RecordTime"] = dtS.ToString(BasePublic.strTimeFormat);
                        }


                        //dtSpt.Rows.Add(drSpt);
                        var period = 0;
                        if (drTag["StorePeriod"] != null)
                        {
                            int.TryParse(drTag["StorePeriod"].ToString(), out period);
                        }
                        DateTime dt00;
                        if (!DateTime.TryParse(drTag["LastTime"].ToString(), out dt00))
                        {
                            dt00 = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 00:00"));
                        }
                        var dt11 = dtS.AddSeconds(-period);
                        if (dt00 <= dt11)
                        {
                            dtStore.Rows.Add(drStore);
                            lock (BasePublic.lockerConfig)
                            {
                                drTag["LastTime"] = dtS;
                                drTag.AcceptChanges();
                            }
                        }
                        try
                        {
                            drSpt["RealTime"] = dtS.ToString(BasePublic.strTimeFormat);
                            drSpt["RealValue"] = dValue3;
                        }
                        catch (Exception ex)
                        {
                        }
                    }


                #endregion
                }
                //VerfyAlarm(BasePublic.lstTagsAll, dtStore, mtTemp, ref dtAlarm);

                #endregion
            }

            UpdateToServer();
            InsertToServer(dtStore, "T_BS_Realdata");


            //manualReset_Config.Set();
        }


        /// <summary>
        ///     耐奇数据
        /// </summary>
        private void OneDeal_NQ(M_MT_TagDefine info)
        {

            DataTable dtStore = null;
            //var dtTags = dsTags.Tables["T_BS_TagDefine"];

            
            DataTableHelper.CreateDataTable(ref dtStore, "T_BS_Realdata");
            try
            {

                if (BasePublic.mySqlConnection == null)
                {

                }
                var lst = BLLFactory<B_NQ_Monitordata>.Instance.GetNewList(null, BasePublic.mySqlConnection);//string.Format(" DeviceID='{0}'", info.ID), BasePublic.mySqlConnection);
                dtStore = DataConvert.FormateNQ2BS(lst, info.lstTags);
                if (dtStore != null && dtStore.Rows != null)
                {
                    Console.WriteLine(string.Format("{0}  获取到耐奇数据，共{1}条", DateTime.Now.ToString(BasePublic.strTimeFormat), dtStore.Rows.Count));
                }
                InsertToServer(dtStore, "T_BS_Realdata");
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("{0}获取耐奇数据出错", DateTime.Now.ToString(BasePublic.strTimeFormat)));
                return;
            }
            lock (BasePublic.lockerConfig)
            {
                var dtSpt = dsRealdata_Spt.Tables["T_BS_Realdata_Spt"];
                try
                {
                    foreach (DataRow item in dtStore.Rows)
                    {
                        var drSpt = dtSpt.Select(string.Format("TagID='{0}'", item["TagID"])).FirstOrDefault();
                        if (drSpt != null)
                        {
                            drSpt["RealTime"] = item["RealTime"];
                            drSpt["RealValue"] = item["RealValue"];
                            //drSpt.AcceptChanges();
                        }
                    }
                    UpdateToServer();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(string.Format("{0}  更新耐奇数据出错 ", DateTime.Now.ToString(BasePublic.strTimeFormat)));
                    return;
                }
            }

        }
        #endregion

        #region 写入数据库

        /// <summary>
        ///     写入数据库
        /// </summary>
        public void InsertToServer(DataTable dt, string tableName)
        {
            lock (BasePublic.lockerConfig)
            {
                if (dt == null || dt.Rows.Count == 0)
                {
                    return;
                }
                BulkWriteToServer(BasePublic.sqlConnection_Main, tableName, dt);
            }
        }

        /// <summary>
        ///     大数据写入数据库
        /// </summary>
        /// <param name="con"></param>
        /// <param name="destinationtablename"></param>
        private void BulkWriteToServer(SqlConnection con, string tableName, DataTable dt)
        {
            //lock (BasePublic.lockerConfig)
            //{
            try
            {
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }

                var sqlBulkCopy = new SqlBulkCopy(con,
                    SqlBulkCopyOptions.CheckConstraints | SqlBulkCopyOptions.Default, null);
                //一次批量的插入的数据量
                sqlBulkCopy.BatchSize = 1000;
                //超时之前操作完成所允许的秒数，如果超时则事务不会提交 ，数据将回滚，所有已复制的行都会从目标表中移除
                sqlBulkCopy.BulkCopyTimeout = 60;

                //設定 NotifyAfter 属性，以便在每插入10000 条数据时，呼叫相应事件。  
                sqlBulkCopy.NotifyAfter = 10000;
                sqlBulkCopy.SqlRowsCopied += OnSqlRowsCopied;

                //映射关系
                //topbranddtcopy.ColumnMappings.Add("", "");
                CreateMapping(sqlBulkCopy, dt);
                sqlBulkCopy.DestinationTableName = tableName;
                sqlBulkCopy.WriteToServer(dt);
            }
            catch (Exception e)
            {
                var msg = string.Format("{0}  {1}  {2}  {3}", dt, tableName, "批量插入数据库失败", e.Message);
                Console.WriteLine(string.Format(msg));
                BasePublic.ExceptionDeal(BaseEnum.Collect, msg);
            }
            //}
        }

        #region 批量更新T_BS_Realdata_Spt

        /// <summary>
        ///     批量更新T_BS_Realdata_Spt
        /// </summary>
        private void UpdateToServer(DataTable dt = null)
        {
            lock (BasePublic.lockerConfig)
            {
                if (sqlDataAdapter_Spt != null || dsRealdata_Spt.Tables["T_BS_Realdata_Spt"] == null || dsRealdata_Spt.Tables["T_BS_Realdata_Spt"].Rows.Count == 0)
                {
                }
                if (sqlDataAdapter_Spt != null)
                {
                    if (dt == null)
                    {
                        var sql = new SqlCommandBuilder(sqlDataAdapter_Spt);
                        var dtt = dsRealdata_Spt.Tables["T_BS_Realdata_Spt"];
                        sqlDataAdapter_Spt.Update(dsRealdata_Spt, "T_BS_Realdata_Spt");
                        dsRealdata_Spt.AcceptChanges();
                    }
                    else
                    {
                        if (dt.Rows == null || dt.Rows.Count == 0)
                        {
                            return;
                        }

                        DataSet ds = new DataSet();
                        dt.TableName = "T_BS_Realdata_Spt";
                        ds.Tables.Add(dt.Copy());
                        var sql = new SqlCommandBuilder(sqlDataAdapter_Spt);
                        var dtt = ds.Tables["T_BS_Realdata_Spt"];
                        sqlDataAdapter_Spt.Update(ds, "T_BS_Realdata_Spt");
                        ds.AcceptChanges();
                    }
                }
            }
        }

        #endregion

        //响应时事件
        private void OnSqlRowsCopied(object sender, SqlRowsCopiedEventArgs e)
        {
            Console.WriteLine("入库行数：" + e.RowsCopied);
        }

        private void CreateMapping(SqlBulkCopy slqBulkCopy, DataTable dtTable)
        {
            foreach (DataColumn item in dtTable.Columns)
            {
                slqBulkCopy.ColumnMappings.Add(item.ColumnName, item.ColumnName);
            }
        }

        #endregion
    }
}
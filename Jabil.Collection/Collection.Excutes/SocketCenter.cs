using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using Collection.Base;
using Collection.Common;
using Collection.Core.BLL;
using Collection.Core.Entity;
using Collection.Core.Model;
using DeerMonitor.Base.Core.BLL;
using DeerMonitor.Base.Core.Entity;
using DeerMonitor.Framework.ControlUtil;
using SanNiuSignal;
using Timer = DeerMonitor.Framework.Commons.Threading.Timer;

namespace Collection.Excutes
{
    /// <summary>
    /// socket中心
    /// </summary>
    public class SocketCenter
    {
        #region 变量申明

        private MTTable mtTable = new MTTable();
        private MTTable mtTableNQ = new MTTable();

        public ITxServer server;

        public int SendPeriod
        {
            get
            {
                var a = 2000;
                if (ConfigurationManager.AppSettings.AllKeys.Contains("SendPeriod"))
                {
                    int.TryParse(ConfigurationManager.AppSettings["SendPeriod"], out a);
                }
                return a;
            }
        }

        public int SendPeriod_Ele
        {
            get
            {
                var a = 2000;
                if (ConfigurationManager.AppSettings.AllKeys.Contains("SendPeriod_Ele"))
                {
                    int.TryParse(ConfigurationManager.AppSettings["SendPeriod_Ele"], out a);
                }
                return a;
            }
        }

        #endregion

        #region 构造函数

        public SocketCenter()
        {
            UpdateConfig();
        }

        private void UpdateConfig()
        {
            lock (BasePublic.lockerConfig)
            {
                if (BasePublic.lstTagsAll == null || BasePublic.lstTagsAll.Count == 0)
                {
                    BasePublic.lstTagsAll =
                        BLLFactory<B_BS_TagDefine>.Instance.Find(" TagGroupID <> '-1' and IsUse='1' and IsVirtual<1 ");
                }
                mtTable = new MTTable();
                foreach (var item in BasePublic.lstTagsAll)
                {
                    var model = new MTKeyValue();
                    model.Key = item.ID;
                    model.Count = 0;
                    model.StartTime = DateTime.Now;
                    model.AlarmCount = 0;
                    model.NoDataCount = 0;
                    model.Tag = item;
                    mtTable.Add(model);
                }

                mtTableNQ = new MTTable();
                foreach (var item in BasePublic.lstTagsAllNQ)
                {
                    var model = new MTKeyValue();
                    model.Key = item.ID;
                    model.Count = 0;
                    model.StartTime = DateTime.Now;
                    model.AlarmCount = 0;
                    model.NoDataCount = 0;
                    model.Tag = item;
                    mtTableNQ.Add(model);
                }
                Console.WriteLine(string.Format("Socket位号配置更新成功..."));
            }
        }

        #endregion

        #region 启动服务

        /// <summary>
        ///     启动服务
        /// </summary>
        public void StartTCPServer()
        {
            try
            {
                Console.WriteLine("{0}", "读取socket配置信息.....");
                server = TxStart.startServer(BasePublic.ServerIP, BasePublic.ServerPort);
                Console.WriteLine("socket服务器{0}{1}", server.Ip, "启动中.....");
                server.AcceptString += acceptString;
                server.AcceptByte += acceptBytes;
                server.Connect += connect;
                server.dateSuccess += dateSuccess;
                server.Disconnection += disconnection;
                server.EngineClose += engineClose;
                server.EngineLost += engineLost;
                server.StartEngine();
                Console.WriteLine("socket服务器{0}{1}", server.Ip + ":" + server.Port, "启动成功.....");


                //非电量线程
                Thread thread = new Thread(SendNotEle);
                thread.Start();
                //用电量线程
                Thread threadEle = new Thread(SendEle);
                threadEle.Start();
                //能耗线程
                Thread threadEnergy = new Thread(SendEnergy);
                threadEnergy.Start();

                while (true)
                {
                    Console.WriteLine("....");
                    Thread.Sleep(1000);
                }
            }
            catch (Exception e)
            {
                var msg = string.Format("{0}  Socket<{1}:{2}>{3}  {4}",
                    DateTime.Now.ToString(BasePublic.strTimeFormat), BasePublic.ServerIP, BasePublic.ServerPort, "启动失败",
                    e.Message);
                Console.WriteLine(string.Format(msg));
                BasePublic.ExceptionDeal(BaseEnum.Socket, msg);
            }
        }

        #endregion

        #region 当接收到来之客户端的文本信息的时候

        /// <summary>
        ///     当接收到来之客户端的文本信息的时候
        /// </summary>
        /// <param name="state"></param>
        /// <param name="str"></param>
        private void acceptString(IPEndPoint ipEndPoint, string str)
        {
            Console.WriteLine("{0}接收到{1}客户端信息{2}", DateTime.Now.ToString(BasePublic.strTimeFormat), ipEndPoint, str);
        }

        #endregion

        #region 当接收到来之客户端的图片信息的时候

        /// <summary>
        ///     当接收到来之客户端的图片信息的时候
        /// </summary>
        /// <param name="ipEndPoint"></param>
        /// <param name="bytes"></param>
        private void acceptBytes(IPEndPoint ipEndPoint, byte[] bytes)
        {
            //    MessageBox.Show(bytes.Length.ToString());
            //    this.pictureBox1.Image = objectByte.ReadImage(bytes);

            //    Console.WriteLine(string.Format("{0}接收到{1}客户端信息{2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), ipEndPoint, str));
        }

        #endregion

        #region 当有客户端连接上来的时候

        /// <summary>
        ///     当有客户端连接上来的时候
        /// </summary>
        /// <param name="state"></param>
        private void connect(IPEndPoint ipEndPoint)
        {
            Console.WriteLine("{0}客户端{1}{2}", DateTime.Now.ToString(BasePublic.strTimeFormat), ipEndPoint, "请求连接");
        }

        #endregion

        #region 当对方已收到我方发送数据的时候

        /// <summary>
        ///     当对方已收到我方发送数据的时候
        /// </summary>
        /// <param name="state"></param>
        private void dateSuccess(IPEndPoint ipEndPoint)
        {
            Console.WriteLine("{0}客户端{1}{2}", DateTime.Now.ToString(BasePublic.strTimeFormat), ipEndPoint, "成功接收数据");
        }

        #endregion

        #region 当有客户端掉线的时候

        /// <summary>
        ///     当有客户端掉线的时候
        /// </summary>
        /// <param name="state"></param>
        /// <param name="str"></param>
        private void disconnection(IPEndPoint ipEndPoint, string str)
        {
            Console.WriteLine("{0}客户端{1}{2}", DateTime.Now.ToString(BasePublic.strTimeFormat), ipEndPoint, "掉线");
        }

        #endregion

        #region 当服务器完全关闭的时候

        /// <summary>
        ///     当服务器完全关闭的时候
        /// </summary>
        private void engineClose()
        {
            Console.WriteLine("{0}{1}", DateTime.Now.ToString(BasePublic.strTimeFormat), "服务器关闭");
        }

        #endregion

        #region 当服务器非正常原因断开的时候

        /// <summary>
        ///     当服务器非正常原因断开的时候
        /// </summary>
        /// <param name="str"></param>
        private void engineLost(string str)
        {
            Console.WriteLine("{0}{1}", DateTime.Now.ToString(BasePublic.strTimeFormat), str);
        }

        #endregion

        #region 更新配置文件

        /// <summary>
        ///     更新配置文件
        /// </summary>
        /// <param name="lst"></param>
        public void CheckAndUpdateConfig(List<M_BS_TagDefine> lst)
        {
            lock (BasePublic.lockerConfig)
            {
                UpdateConfig();
            }
        }

        #endregion

        #region 发送至客户端

        private void SendNotEle()
        {
            while (true)
            {
                SendToClient(new object[] { "0" });
                Thread.Sleep(SendPeriod);
            }

        }

        private void SendEnergy()
        {
            while (true)
            {
                SendToClient_Energy(null);
                Thread.Sleep(SendPeriod);
            }

        }

        private void SendEle()
        {
            while (true)
            {
                lock (BasePublic.lockerConfig)
                {
                    //foreach (var item in BasePublic.lstNQ)
                    //{
                    //    SendToClient(new object[] { "NQ", item });
                    //}
                    SendToClient(new object[] { "NQ", new M_BS_TagDefine(){ID="NQ001"} });
                }
                Thread.Sleep(SendPeriod_Ele);
            }

        }

      

       

        /// <summary>
        ///     发送至客户端
        /// </summary>
        private void SendToClient(object ob)
        {
            object[] obs = ob as object[];
            string type = "";
            M_MT_TagDefine info = null;
            string strIds = "";
            if (obs == null)
            {
                return;
            }

            if (obs.Length == 1)
            {
                type = obs[0].ToString();
            }
            if (obs.Length == 2)
            {
                type = obs[0].ToString();
                info = obs[1] as M_MT_TagDefine;
            }
            lock (BasePublic.lockerConfig)
            {
                if (server.ClientAll != null && server.ClientAll.Count == 0)
                {
                    var msg = "服务端无连接,不发送";
                    Console.WriteLine(msg);
                }
                var lst = new List<M_BS_Realdata_Spt>();
                try
                {

                    lst = BLLFactory<B_BS_Realdata_Spt>.Instance.GetNewList(type, info);
                    if (lst == null)
                    {
                        return;
                    }
                    if (type.ToString() != "0")
                    {
                        //lst=(from li in lst where li.TagCode=="Epi" select li).ToList();
                    }
                }
                catch (Exception e)
                {
                    var msg = string.Format("{0}  {1}  {2}", DateTime.Now.ToString(BasePublic.strTimeFormat),
                        "查询实时数据spt出错", e.Message);
                    Console.WriteLine(string.Format(msg));
                    BasePublic.ExceptionDeal(BaseEnum.Socket, msg);
                    return;
                }

                DataTable dt = GetSendTable(lst, type);

                #region 发送到客户端

                //string strJson = BaseFunction.DataTable2Json(dt);
                var strJson = BaseFunction.DataTableToJson(dt);
                var byteLen = Encoding.Default.GetByteCount(strJson);

                var strMsg = string.Format("\n\n{0}向端口发送【非电量】数据：" + "*****************************",
                    DateTime.Now.ToString(BasePublic.strTimeFormat));
                if (type != "0")
                {
                    strMsg = string.Format("\n\n{0}向端口发送【电量】数据：" + "*****************************",
                   DateTime.Now.ToString(BasePublic.strTimeFormat));
                }
                if (strJson == null || strJson.Trim().Length == 0)
                {
                    strMsg += string.Format("\n{0}字符串长度为0，数据不发送", DateTime.Now.ToString(BasePublic.strTimeFormat));
                    return;
                }

                if (server.ClientAll == null)
                {
                    strMsg += string.Format("\n{0}", "无客户端连接，数据未发送");
                    Console.WriteLine(strMsg);
                    return;
                }
                strMsg += string.Format("\n允许最大在线人数：{0}\n当前连接数为：{1}", server.ClientMax, server.ClientAll.Count);
                foreach (var item in server.ClientAll)
                {
                    if (!server.clientCheck(item))
                    {
                        strMsg += string.Format("\n客户端{0}{1}", item.Address + ":" + item.Port, "不在线");
                        break;
                    }


                    Console.WriteLine(strMsg);
                    var sendData = Encoding.UTF8.GetBytes(strJson);
                    var aaa = sendData[sendData.Length - 1];
                    if (strJson == null || strJson.Trim(']').Length < 1)
                    {
                        strMsg += string.Format("\n{0}字符串长度为0，客户端{1}{2}", DateTime.Now.ToString(BasePublic.strTimeFormat),
                            item.Address + ":" + item.Port, "发送信息不成功");
                        return;
                    }
                    strMsg += string.Format("\n{0}客户端{1}{2}", DateTime.Now.ToString(BasePublic.strTimeFormat),
                        item.Address + ":" + item.Port, "发送信息成功");
                    Console.WriteLine(strMsg);
                    server.sendMessage(item, strJson);
                }

                #endregion

            }
        }

        /// <summary>
        /// 将实时数据进行处理
        /// </summary>
        /// <param name="lst"></param>
        /// <returns></returns>
        private DataTable GetSendTable(List<M_BS_Realdata_Spt> lst, string type = "0")
        {
            var dt = BaseFunction.CreateTable("TagID,TagValue,TagName,TagStatus,RealValue,RealTime");
            DataRow dr = null;
            double value = 0;
            var status = 0;
            var str = "";
            foreach (var item in lst)
            {
                M_BS_TagDefine model_Tag = null;
                MTKeyValue mtRow = null;
                if (type == "0")
                {
                    model_Tag = (from li in BasePublic.lstTagsAll where li.ID == item.TagID select li).FirstOrDefault();
                    //查找该位号的缓存数据
                    mtRow = (from li in mtTable where li.Key == item.TagID select li).FirstOrDefault();
                }
                else
                {
                    model_Tag = (from li in BasePublic.lstTagsAllNQ where li.ID == item.TagID select li).FirstOrDefault();
                    //查找该位号的缓存数据
                    mtRow = (from li in mtTableNQ where li.Key == item.TagID select li).FirstOrDefault();
                }

                if (mtRow == null || model_Tag == null)
                {
                    continue;
                }

                if (mtRow.Count == 0)
                {
                    mtRow.StartTime = DateTime.Now;
                }
                mtRow.Count += 1;
                mtRow.NoDataCount = 0;

                if (mtRow.Count >= BasePublic.OneDataCount)
                {
                    mtRow.Value = BasePublic.OneDataCount;
                    mtRow.Count = 0;
                    //mtRow.AlarmCount = 0;
                }
                dr = dt.NewRow();
                dr["RealTime"] = DateTime.Now.ToString(BasePublic.strTimeFormat);
                dr["RealValue"] = item.RealValue;
                dr["TagID"] = item.TagID;
                dr["TagName"] = model_Tag.TagName;
                str = "";
                status = 0;
                var IsAlarm = false;
                if (model_Tag.IsAlarm)
                {
                    var model_PreTags =
                        (from li in BasePublic.lstTagsAll where li.ID == model_Tag.PreTags select li).FirstOrDefault();
                    if (model_PreTags == null || model_PreTags.PreTags.Trim().Length == 0)
                    {
                        IsAlarm = true;
                    }
                    else
                    {
                        var model_PreValue =
                            (from li in lst where li.TagID == model_PreTags.ID select li).FirstOrDefault();
                        if (model_PreValue.RealValue > 0)
                        {
                            IsAlarm = true;
                        }
                    }
                    if (IsAlarm)
                    {
                        if (item.RealValue > model_Tag.TagHH)
                        {
                            status = 1;
                            str = string.Format("参数值({1})过高,正常值{2}({3}至{4})", model_Tag.TagName, item.RealValue,
                                model_Tag.TagVal, model_Tag.TagLL, model_Tag.TagHH);
                        }
                        if (item.RealValue < model_Tag.TagLL)
                        {
                            status = 1;
                            str = string.Format("参数值({1})过低,正常值{2}({3}至{4})", model_Tag.TagName, item.RealValue,
                                model_Tag.TagVal, model_Tag.TagLL, model_Tag.TagHH);
                        }
                        mtRow.AlarmCount += 1;
                        mtRow.Status = status;
                        mtRow.Msg = str;

                        if (mtRow.AlarmCount >= 10)
                        {
                            try
                            {
                            }
                            catch (Exception e)
                            {
                                var msg = string.Format("{0}  {1}  {2}",
                                    DateTime.Now.ToString(BasePublic.strTimeFormat), "转化实时数据出错", e.Message);
                                Console.WriteLine(string.Format(msg));
                                BasePublic.ExceptionDeal(BaseEnum.Socket, msg);
                                continue;
                            }
                        }
                    }
                }

                dr["TagStatus"] = status;
                dr["TagValue"] = str;
                dt.Rows.Add(dr);
            }
            return dt;
        }

        /// <summary>
        ///     发送能源数据到客户端
        /// </summary>
        private void SendToClient_Energy(object type)
        {
            //Thread.Sleep(SendPeriod);
            if (server.ClientAll != null && server.ClientAll.Count == 0)
            {
                var msg = "服务端无连接,不发送";
                Console.WriteLine(msg);
            }
            var lst = new List<M_BS_Realdata_Spt>();
            try
            {
                lst = BLLFactory<B_BS_Realdata_Spt>.Instance.GetEnergyRecordList(DateTime.Now);
                if (lst == null)
                {
                    return;
                }
            }
            catch (Exception e)
            {
                var msg = string.Format("{0}  {1}  {2}", DateTime.Now.ToString(BasePublic.strTimeFormat),
                    "查询实时数据spt出错", e.Message);
                Console.WriteLine(string.Format(msg));
                BasePublic.ExceptionDeal(BaseEnum.Socket, msg);
                return;
            }

            var dt = BaseFunction.CreateTable("TagID,TagValue,TagName,TagStatus,RealValue,RealTime");
            DataRow dr = null;
            double value = 0;
            var status = 0;
            var str = "";
            foreach (var item in lst)
            {
                var model_Tag =
                    (from li in BasePublic.lstTagsEnergy where li.TagCode == item.TagID select li).FirstOrDefault();
                //查找该位号的缓存数据
                var mtRow = (from li in mtTable where li.Key == item.TagID select li).FirstOrDefault();
                if (model_Tag == null)
                {
                    continue;
                }

                //if (mtRow.Count == 0)
                //{
                //    mtRow.StartTime = DateTime.Now;
                //}
                //mtRow.Count += 1;
                //mtRow.NoDataCount = 0;

                //if (mtRow.Count >= BasePublic.OneDataCount)
                //{
                //    mtRow.Value = BasePublic.OneDataCount;
                //    mtRow.Count = 0;
                //    //mtRow.AlarmCount = 0;
                //}
                dr = dt.NewRow();
                dr["RealTime"] = DateTime.Now.ToString(BasePublic.strTimeFormat);
                dr["RealValue"] = item.RealValue;
                dr["TagID"] = item.TagID;
                dr["TagName"] = model_Tag.TagName;
                str = "";
                status = 0;
                var IsAlarm = false;
                if (model_Tag.IsAlarm)
                {
                    //    M_BS_TagDefine model_PreTags =
                    //        (from li in BasePublic.lstTagsAll where li.ID == model_Tag.PreTags select li).FirstOrDefault();
                    //    if (model_PreTags == null || model_PreTags.PreTags.Trim().Length == 0)
                    //    {
                    //        IsAlarm = true;
                    //    }
                    //    else
                    //    {
                    //        var model_PreValue =
                    //            (from li in lst where li.TagID == model_PreTags.ID select li).FirstOrDefault();
                    //        if (model_PreValue.RealValue > 0)
                    //        {
                    //            IsAlarm = true;
                    //        }
                    //    }
                    if (IsAlarm)
                    {
                        if (item.RealValue > model_Tag.TagHH)
                        {
                            status = 1;
                            str = string.Format("参数值({1})过高,正常值{2}({3}至{4})", model_Tag.TagName, item.RealValue,
                                model_Tag.TagVal, model_Tag.TagLL, model_Tag.TagHH);
                        }
                        if (item.RealValue < model_Tag.TagLL)
                        {
                            status = 1;
                            str = string.Format("参数值({1})过低,正常值{2}({3}至{4})", model_Tag.TagName, item.RealValue,
                                model_Tag.TagVal, model_Tag.TagLL, model_Tag.TagHH);
                        }
                        mtRow.AlarmCount += 1;
                        mtRow.Status = status;
                        mtRow.Msg = str;

                        if (mtRow.AlarmCount >= 10)
                        {
                            try
                            {
                            }
                            catch (Exception e)
                            {
                                var msg = string.Format("{0}  {1}  {2}",
                                    DateTime.Now.ToString(BasePublic.strTimeFormat), "转化实时数据出错", e.Message);
                                Console.WriteLine(string.Format(msg));
                                BasePublic.ExceptionDeal(BaseEnum.Socket, msg);
                            }
                        }
                    }
                }
                //dr["TagStatus"] = status;
                //dr["TagValue"] = str;
                //dt.Rows.Add(dr);
                dt.Rows.Add(dr);
            }

            #region 发送到客户端

            //string strJson = BaseFunction.DataTable2Json(dt);
            var strJson = BaseFunction.DataTableToJson(dt);
            var byteLen = Encoding.Default.GetByteCount(strJson);

            var strMsg = string.Format("\n\n{0}向端口发送【能源】数据：" + "*****************************",
                DateTime.Now.ToString(BasePublic.strTimeFormat));
            if (strJson == null || strJson.Trim().Length == 0)
            {
                strMsg += string.Format("\n{0}字符串长度为0，数据不发送", DateTime.Now.ToString(BasePublic.strTimeFormat));
                return;
            }

            if (server.ClientAll == null)
            {
                strMsg += string.Format("\n{0}", "无客户端连接，数据未发送");
                Console.WriteLine(strMsg);
                return;
            }
            strMsg += string.Format("\n允许最大在线人数：{0}\n当前连接数为：{1}", server.ClientMax, server.ClientAll.Count);
            foreach (var item in server.ClientAll)
            {
                if (!server.clientCheck(item))
                {
                    strMsg += string.Format("\n客户端{0}{1}", item.Address + ":" + item.Port, "不在线");
                    break;
                }


                Console.WriteLine(strMsg);
                var sendData = Encoding.UTF8.GetBytes(strJson);
                var aaa = sendData[sendData.Length - 1];
                if (strJson == null || strJson.Trim(']').Length < 1)
                {
                    strMsg += string.Format("\n{0}字符串长度为0，客户端{1}{2}", DateTime.Now.ToString(BasePublic.strTimeFormat),
                        item.Address + ":" + item.Port, "发送信息不成功");
                    return;
                }
                strMsg += string.Format("\n{0}客户端{1}{2}", DateTime.Now.ToString(BasePublic.strTimeFormat),
                    item.Address + ":" + item.Port, "发送信息成功");
                Console.WriteLine(strMsg);
                server.sendMessage(item, strJson);
            }

            #endregion
        }

        #endregion
    }
}
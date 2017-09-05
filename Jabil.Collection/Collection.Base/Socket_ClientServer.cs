using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using SanNiuSignal;

namespace Collection.Base
{
    public class Socket_ClientServer
    {
        /// <summary>
        ///     客户端链接IP地址
        /// </summary>
        private readonly string ClientIP;

        /// <summary>
        ///     客户端链接端口
        /// </summary>
        private readonly int ClientPort;

        /// <summary>
        ///     服务器IP地址
        /// </summary>
        private readonly string ClientServerIP;

        /// <summary>
        ///     服务器端口
        /// </summary>
        private readonly int ClientServerPort;

        /// <summary>
        ///     时间格式化字符串
        /// </summary>
        private readonly string strTimeFormat = "yyyy-MM-dd HH:mm:ss";

        /// <summary>
        ///     客户端V
        /// </summary>
        private ITxClient TxClient;

        /// <summary>
        ///     客户端VS服务端
        /// </summary>
        private ITxServer TxClientServer;

        public Socket_ClientServer()
        {
            if (ConfigurationManager.AppSettings.AllKeys.Contains("ClientServerIP"))
            {
                ClientServerIP = ConfigurationManager.AppSettings["ClientServerIP"];
            }
            if (ConfigurationManager.AppSettings.AllKeys.Contains("ClientServerPort"))
            {
                int.TryParse(ConfigurationManager.AppSettings["ClientServerPort"], out ClientServerPort);
            }

            if (ConfigurationManager.AppSettings.AllKeys.Contains("ServerIP"))
            {
                ClientIP = ConfigurationManager.AppSettings["ServerIP"];
            }
            if (ConfigurationManager.AppSettings.AllKeys.Contains("ServerPort"))
            {
                int.TryParse(ConfigurationManager.AppSettings["ServerPort"], out ClientPort);
            }
        }

        public void Start()
        {
            try
            {
                Console.WriteLine("{0}", "读取配置信息.....");
                TxClientServer = TxStart.startServer(ClientServerIP, ClientServerPort);
                Console.WriteLine("服务器{0}{1}", TxClientServer.Ip, "启动中.....");
                TxClientServer.AcceptString += acceptString_Server;
                TxClientServer.AcceptByte += acceptBytes_Server;
                TxClientServer.Connect += connect_Server;
                TxClientServer.dateSuccess += dateSuccess_Server;
                TxClientServer.Disconnection += disconnection_Server;
                TxClientServer.EngineClose += engineClose_Server;
                TxClientServer.EngineLost += engineLost_Server;
                //server.BufferSize=12048;
                //server.FileLog = "C:\\test.txt";
                TxClientServer.StartEngine();
                Console.WriteLine("服务器{0}{1}", TxClientServer.Ip + ":" + TxClientServer.Port, "启动成功.....");


                Console.WriteLine("创建客户端.....");
                TxClient = TxStart.startClient(ClientIP, ClientPort);
                Console.WriteLine("客户端创建成功.....");
                TxClient.AcceptString += accptString_Client; //当收到文本数据的时候
                TxClient.dateSuccess += sendSuccess_Client; //当对方已经成功收到我数据的时候
                TxClient.EngineClose += engineClose_Client; //当客户端引擎完全关闭释放资源的时候
                TxClient.EngineLost += engineLost_Client; //当客户端非正常原因断开的时候
                TxClient.ReconnectionStart += reconnectionStart_Client; //当自动重连开始的时候
                TxClient.StartResult += startResult_Client; //当登录完成的时候
                //TxClient.BufferSize = 12048;//这里大小自己设置，默认为1KB，也就是1024个字节
                Console.WriteLine("客户端启动.....");
                TxClient.StartEngine();
                Console.WriteLine("客户端{0}{1}", TxClient.Ip + ":" + TxClient.Port, "启动成功.....");

                Console.ReadLine();
            }
            catch (Exception Ex)
            {
                Console.WriteLine("客户端启动异常.....");
            }
        }

        #region Server

        /// <summary>
        ///     当接收到来之客户端的文本信息的时候
        /// </summary>
        /// <param name="state"></param>
        /// <param name="str"></param>
        private void acceptString_Server(IPEndPoint ipEndPoint, string str)
        {
            //ListViewItem item = new ListViewItem(new string[] { DateTime.Now.ToString(), ipEndPoint.ToString(), str });
            //this.listView1.Items.Insert(0, item);
            Console.WriteLine("{0}接收到{1}客户端信息{2}", DateTime.Now.ToString(strTimeFormat), ipEndPoint, str);
        }

        /// <summary>
        ///     当接收到来之客户端的图片信息的时候
        /// </summary>
        /// <param name="ipEndPoint"></param>
        /// <param name="bytes"></param>
        private void acceptBytes_Server(IPEndPoint ipEndPoint, byte[] bytes)
        {
            //    MessageBox.Show(bytes.Length.ToString());
            //    this.pictureBox1.Image = objectByte.ReadImage(bytes);

            //    Console.WriteLine(string.Format("{0}接收到{1}客户端信息{2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), ipEndPoint, str));
        }

        /// <summary>
        ///     当有客户端连接上来的时候
        /// </summary>
        /// <param name="state"></param>
        private void connect_Server(IPEndPoint ipEndPoint)
        {
            Console.WriteLine("{0}客户端{1}{2}", DateTime.Now.ToString(strTimeFormat), ipEndPoint, "请求连接");
        }

        /// <summary>
        ///     当对方已收到我方发送数据的时候
        /// </summary>
        /// <param name="state"></param>
        private void dateSuccess_Server(IPEndPoint ipEndPoint)
        {
            Console.WriteLine("{0}客户端{1}{2}", DateTime.Now.ToString(strTimeFormat), ipEndPoint, "成功接收数据");
        }

        /// <summary>
        ///     当有客户端掉线的时候
        /// </summary>
        /// <param name="state"></param>
        /// <param name="str"></param>
        private void disconnection_Server(IPEndPoint ipEndPoint, string str)
        {
            Console.WriteLine("{0}客户端{1}{2}", DateTime.Now.ToString(strTimeFormat), ipEndPoint, "掉线");
        }

        /// <summary>
        ///     当服务器完全关闭的时候
        /// </summary>
        private void engineClose_Server()
        {
            Console.WriteLine("{0}{1}", DateTime.Now.ToString(strTimeFormat), "服务器关闭");
        }

        /// <summary>
        ///     当服务器非正常原因断开的时候
        /// </summary>
        /// <param name="str"></param>
        private void engineLost_Server(string str)
        {
            Console.WriteLine("{0}{1}", DateTime.Now.ToString(strTimeFormat), str);
        }

        #endregion

        #region Client

        /// <summary>
        ///     接收到文本数据的时候
        /// </summary>
        /// <param name="str"></param>
        private void accptString_Client(IPEndPoint end, string strJson)
        {
            Console.WriteLine("接收到服务器{0}发送来的数据{1}", end, strJson);

            #region 发送到客户端

            var strMsg = string.Format("\n\n{0}向端口发送数据：" + "*****************************",
                DateTime.Now.ToString(strTimeFormat));
            if (strJson == null || strJson.Trim().Length == 0)
            {
                strMsg += string.Format("\n{0}字符串长度为0，数据不发送", DateTime.Now.ToString(strTimeFormat));
                return;
            }

            if (TxClientServer.ClientAll == null)
            {
                strMsg += string.Format("\n{0}", "无客户端连接，数据未发送");
                Console.WriteLine(strMsg);
                return;
            }
            strMsg += string.Format("\n允许最大在线人数：{0}\n当前连接数为：{1}", TxClientServer.ClientMax,
                TxClientServer.ClientAll.Count);
            foreach (var item in TxClientServer.ClientAll)
            {
                if (!TxClientServer.clientCheck(item))
                {
                    strMsg += string.Format("\n客户端{0}{1}", item.Address + ":" + item.Port, "不在线");
                    break;
                }


                Console.WriteLine(strMsg);
                var sendData = Encoding.UTF8.GetBytes(strJson);
                var aaa = sendData[sendData.Length - 1];
                if (strJson == null || strJson.Trim(']').Length < 1)
                {
                    strMsg += string.Format("\n{0}字符串长度为0，客户端{1}{2}", DateTime.Now.ToString(strTimeFormat),
                        item.Address + ":" + item.Port, "发送信息不成功");
                    return;
                }
                strMsg += string.Format("\n{0}客户端{1}{2}", DateTime.Now.ToString(strTimeFormat),
                    item.Address + ":" + item.Port, "发送信息成功");
                Console.WriteLine(strMsg);
                TxClientServer.sendMessage(item, strJson);
            }

            #endregion
        }

        /// <summary>
        ///     当数据发送成功的时候
        /// </summary>
        private void sendSuccess_Client(IPEndPoint end)
        {
            //Console.WriteLine(string.Format("接收到服务器{0}发送来的数据{1}", end str));
        }

        /// <summary>
        ///     当客户端引擎完全关闭的时候
        /// </summary>
        private void engineClose_Client()
        {
        }

        /// <summary>
        ///     当客户端突然断开的时候
        /// </summary>
        /// <param name="str">断开原因</param>
        private void engineLost_Client(string str)
        {
        }

        /// <summary>
        ///     当自动重连开始的时候
        /// </summary>
        private void reconnectionStart_Client()
        {
            Console.WriteLine("{0}", "10秒后自动重连开始");
        }

        /// <summary>
        ///     当登录有结果的时候
        /// </summary>
        /// <param name="b">是否成功</param>
        /// <param name="str">失败或成功原因</param>
        private void startResult_Client(bool b, string str)
        {
            var rs = "失败";
            if (b)
            {
                rs = "成功";
            }
            Console.WriteLine("客户端连接{0}", rs);
        }

        #endregion
    }
}
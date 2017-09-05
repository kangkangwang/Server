using System;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using Collection.Base;

namespace Collection.Excutes
{
    /// <summary>
    /// 监听adobe
    /// </summary>
    public class ListenAdobeCenter
    {
        #region 启动程序

        public void Start()
        {
            try
            {
                var ip = IPAddress.Parse(BasePublic.ServerIP);
                ;
                lis = new TcpListener(ip, 843);
                lis.Start();
            }
            catch (Exception e)
            {
                var msg = string.Format("{0}  {1}  {2}", DateTime.Now, "启动843端口监听失败！", e.Message);
                Console.WriteLine(string.Format(msg));
                BasePublic.ExceptionDeal(BaseEnum.Adobe, msg);
                return;
            }
            var msg2 = string.Format("{0}  {1} ", DateTime.Now.ToString(BasePublic.strTimeFormat), "启动843端口成功！");
            Console.WriteLine(string.Format(msg2));
            while (true)
            {
                //Console.Write("**********");
                lis.BeginAcceptTcpClient(asc, lis);
                Thread.Sleep(1);
            }
        }

        #endregion

        #region 监听客户端连接

        /// <summary>
        ///     监听客户端连接
        /// </summary>
        private static void asc(IAsyncResult ar)
        {
            try
            {
                var listener = ar.AsyncState as TcpListener;
                var client = listener.EndAcceptTcpClient(ar);
                Console.WriteLine("Adobe请求验证");
                //BasePublic.ExceptionDeal(BaseEnum.Adobe, string.Format("{0}  {1} ", DateTime.Now.ToString(BasePublic.strTimeFormat), "Adobe请求验证"));
                Console.WriteLine(string.Format("客户端Adobe请求验证(网站)"));
                var num = 0;
                var size = 400;

                var info = new byte[size];
                var stream = client.GetStream();
                lock (stream)
                {
                    num = stream.Read(info, 0, size);
                }
                var msg = Encoding.UTF8.GetString(info, 0, num);
                var strSecurityPolicy = "<?xml version=\"1.0\"?>" +
                                        "<cross-domain-policy>" +
                                        "<site-control permitted-cross-domain-policies=\"all\"/>" +
                                        "<allow-access-from domain=\"*\" to-ports=\"*\" />" +
                                        "</cross-domain-policy>";
                // char[] b = strSecurityPolicy.ToCharArray();
                var b = Encoding.UTF8.GetBytes(strSecurityPolicy);
                stream.Write(b, 0, b.Length);

                stream.Close();
                client.Close();
            }
            catch (Exception e)
            {
            }
        }

        #endregion

        #region 变量申明

        private static Socket serverSocket;
        private static byte[] result = new byte[1024];
        private static byte[] info1;
        private static TcpListener lis;

        #endregion

        #region 其余函数

        private static Socket GetSocket(TcpClient cln)
        {
            var pi = cln.GetType().GetProperty("Client", BindingFlags.NonPublic | BindingFlags.Instance);
            var sock = (Socket) pi.GetValue(cln, null);
            return sock;
        }

        private static string GetRemoteIP(TcpClient cln)
        {
            var ip = "";
            try
            {
                ip = GetSocket(cln).RemoteEndPoint.ToString().Split(':')[0];
            }
            catch (Exception e)
            {
            }
            return ip;
        }

        public static int GetRemotePort(TcpClient cln)
        {
            var temp = "";
            var port = 0;
            try
            {
                temp = GetSocket(cln).RemoteEndPoint.ToString().Split(':')[1];
                port = Convert.ToInt32(temp);
            }
            catch (Exception)
            {
            }
            return port;
        }

        #endregion
    }
}
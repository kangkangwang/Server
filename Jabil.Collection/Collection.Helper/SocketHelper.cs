using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Collection.Base.Helper
{
    public static class SocketHelper
    {
        private static bool IsConnectionSuccessful;
        private static Exception socketexception;
        private static ManualResetEvent TimeoutObject = new ManualResetEvent(false);
        private static Socket client;

        public static bool ConnectTimeOut(this Socket socket, IPEndPoint remoteEndPoint, int timeout)
        {
            bool rs = false;
            TimeoutObject = new ManualResetEvent(false);
            TimeoutObject.Reset();
            socketexception = null;

            socket.BeginConnect(remoteEndPoint, new AsyncCallback(CallBackMethod), socket);
            //client = socket;
            if (TimeoutObject.WaitOne(timeout, false))
            {
                if (IsConnectionSuccessful)
                {
                    rs = true;
                }
                else
                {
                    rs = false;
                    throw socketexception;
                }
            }
            else
            {
                rs = false;
                //socket.Close();
                throw new TimeoutException("TimeOut Exception");
            }
            return rs;
        }

        private static void CallBackMethod(IAsyncResult asyncresult)
        {
            try
            {
                IsConnectionSuccessful = false;
                TcpClient tcpclien0t = asyncresult.AsyncState as TcpClient;
                var tcpclient = (Socket) asyncresult.AsyncState;
                if (tcpclient != null)
                {
                    tcpclient.EndConnect(asyncresult);
                    IsConnectionSuccessful = true;
                }
                //client.EndConnect(asyncresult);
            }
            catch (Exception ex)
            {
                IsConnectionSuccessful = false;
                socketexception = ex;
            }
            finally
            {
                TimeoutObject.Set();
            }
        }
    }
}
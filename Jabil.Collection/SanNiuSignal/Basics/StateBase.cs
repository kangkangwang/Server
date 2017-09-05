using System.Net;
using System.Net.Sockets;

namespace SanNiuSignal.Basics
{
    /// <summary>
    ///     一个State父类
    /// </summary>
    public class StateBase
    {
        private readonly int _bufferSize = 1024; //缓冲区大小
        private byte[] _sendDate; //已发送的数据
        private int _sendDateLabel; //发送数据的标签

        /// <summary>
        ///     带参数的构造函数
        /// </summary>
        /// <param name="socket">Socket</param>
        /// <param name="bufferSize">缓冲区大小</param>
        internal StateBase(Socket socket, int bufferSize)
        {
            IpEndPoint = null;
            SendFile = null;
            ReceiveFile = null;
            BufferBackup = null;
            _bufferSize = bufferSize;
            Buffer = new byte[bufferSize];
            WorkSocket = socket;
            try
            {
                IpEndPoint = (IPEndPoint) socket.RemoteEndPoint;
            }
            catch
            {
            }
        }

        /// <summary>
        ///     备份缓冲区;主要是缓冲区有时候需要增大或缩小的时候用到；
        /// </summary>
        internal byte[] BufferBackup { get; set; }

        /// <summary>
        ///     接收文件类
        /// </summary>
        internal FileBase ReceiveFile { get; set; }

        /// <summary>
        ///     发送文件类
        /// </summary>
        internal FileBase SendFile { get; set; }

        /// <summary>
        ///     缓冲区大小
        /// </summary>
        internal int BufferSize
        {
            get { return _bufferSize; }
        }

        /// <summary>
        ///     工作的Socket
        /// </summary>
        internal Socket WorkSocket { get; set; }

        /// <summary>
        ///     缓冲区
        /// </summary>
        internal byte[] Buffer { get; set; }

        /// <summary>
        ///     已发送的数据,主要用于对方没有收到信息可以重发用
        /// </summary>
        internal byte[] SendDate
        {
            get { return _sendDate; }
            set { _sendDate = value; }
        }

        /// <summary>
        ///     已发数据的标签
        /// </summary>
        internal int SendDateLabel
        {
            get { return _sendDateLabel; }
            set { _sendDateLabel = value; }
        }

        /// <summary>
        ///     IPEndPoint得到客户端地址,端口号；
        /// </summary>
        internal IPEndPoint IpEndPoint { get; set; }

        /// <summary>
        ///     同时设置发送数据和它的标签的方法
        /// </summary>
        /// <param name="Lable">标签</param>
        /// <param name="sendDate">已发送数据</param>
        internal void SendDateInitialization(int Lable, byte[] sendDate)
        {
            _sendDateLabel = Lable;
            _sendDate = sendDate;
        }
    }
}
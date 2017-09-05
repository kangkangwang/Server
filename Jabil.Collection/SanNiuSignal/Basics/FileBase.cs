using SanNiuSignal.PublicTool;

namespace SanNiuSignal.Basics
{
    internal class FileBase
    {
        private readonly byte _fileClassification = PasswordCode._photographCode; //图片，文本或其他
        private readonly int _fileLenth; //文件总长度

        /// <summary>
        ///     带参数的构造函数,发送方用
        /// </summary>
        /// <param name="fileDateAll"></param>
        internal FileBase(byte[] fileDateAll)
        {
            FileLabel = 0;
            SendDate = null;
            FileDateAll = fileDateAll;
        }

        /// <summary>
        ///     带参数的构造函数，接收方用
        /// </summary>
        /// <param name="fileClassification">图片，文本或其他</param>
        /// <param name="fileLabel">文件的标签</param>
        /// <param name="fileLenth">文件总长度</param>
        internal FileBase(byte fileClassification, int fileLabel, int fileLenth)
        {
            FileDateAll = null;
            SendDate = null;
            _fileClassification = fileClassification;
            FileLabel = fileLabel;
            _fileLenth = fileLenth;
        }

        /// <summary>
        ///     图片，文本或其他
        /// </summary>
        internal byte FileClassification
        {
            get { return _fileClassification; }
        }

        /// <summary>
        ///     文件长度
        /// </summary>
        internal int FileLenth
        {
            get { return _fileLenth; }
        }

        /// <summary>
        ///     已发送的数据；主要用于是否重发之用
        /// </summary>
        internal byte[] SendDate { get; set; }

        /// <summary>
        ///     要发送的数据
        /// </summary>
        internal byte[] FileDateAll { get; set; }

        /// <summary>
        ///     文件的标签
        /// </summary>
        internal int FileLabel { get; set; }
    }
}
using System.IO;
using SanNiuSignal.Basics;

namespace SanNiuSignal.FileCenter
{
    internal class FileState
    {
        private readonly int _fileLabel; //文件的标签
        private readonly long _fileLenth; //文件总长度
        private string _fileName = ""; //文件地址

        /// <summary>
        ///     发送方的构造函数
        /// </summary>
        /// <param name="fileLabel">文件标签</param>
        /// <param name="fileLenth">文件长度</param>
        /// <param name="fileName">文件地址</param>
        /// <param name="fileStream">文件流</param>
        internal FileState(int fileLabel, long fileLenth, string fileName, FileStream fileStream)
        {
            StateOne = null;
            StateFile = 0;
            FileOkLenth = 0;
            _fileLabel = fileLabel;
            _fileLenth = fileLenth;
            _fileName = fileName;
            Filestream = fileStream;
        }

        /// <summary>
        ///     文件已处理量
        /// </summary>
        internal long FileOkLenth { get; set; }

        /// <summary>
        ///     文件状态；0是等待；1是正在传输；2是暂停
        /// </summary>
        internal int StateFile { get; set; }

        /// <summary>
        ///     文件地址
        /// </summary>
        internal string FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }

        /// <summary>
        ///     发送文件流
        /// </summary>
        internal FileStream Filestream { get; set; }

        /// <summary>
        ///     文件长度
        /// </summary>
        internal long FileLenth
        {
            get { return _fileLenth; }
        }

        /// <summary>
        ///     文件的标签
        /// </summary>
        internal int FileLabel
        {
            get { return _fileLabel; }
        }

        /// <summary>
        ///     StateBase；
        /// </summary>
        internal StateBase StateOne { get; set; }
    }
}
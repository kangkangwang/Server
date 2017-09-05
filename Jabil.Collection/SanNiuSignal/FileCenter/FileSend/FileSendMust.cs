using SanNiuSignal.FileCenter.FileBase;
using SanNiuSignal.PublicTool;

namespace SanNiuSignal.FileCenter.FileSend
{
    internal class FileSendMust : FileMustBase, IFileSendMust
    {
        private readonly IFileSendMust fileSendMust;

        /// <summary>
        ///     构造函数
        /// </summary>
        /// <param name="FileSendMust">IFileSendMust</param>
        public FileSendMust(IFileSendMust FileSendMust)
            : base(FileSendMust)
        {
            fileSendMust = FileSendMust;
        }

        #region IFileSendMust 成员

        public void SendSuccess(int FileLabel)
        {
            CommonMethod.eventInvoket(() => { fileSendMust.SendSuccess(FileLabel); });
        }

        public void FileRefuse(int FileLabel)
        {
            CommonMethod.eventInvoket(() => { fileSendMust.FileRefuse(FileLabel); });
        }

        public void FileStartOn(int FileLabel)
        {
            CommonMethod.eventInvoket(() => { fileSendMust.FileStartOn(FileLabel); });
        }

        #endregion
    }
}
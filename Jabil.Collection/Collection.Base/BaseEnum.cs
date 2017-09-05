namespace Collection.Base
{
    public enum BaseEnum
    {
        /// <summary>
        ///     主进程
        /// </summary>
        Main = 0,

        /// <summary>
        ///     数据采集
        /// </summary>
        Collect = 1,

        /// <summary>
        ///     Socket发送
        /// </summary>
        Socket = 2,

        /// <summary>
        ///     短信中心
        /// </summary>
        Msg = 3,

        /// <summary>
        ///     Adobe验证
        /// </summary>
        Adobe = 4,

        /// <summary>
        ///     系统错误
        /// </summary>
        Exception = 5,
        /// <summary>
        ///     耐奇系统数据采集错误
        /// </summary>
        NQ = 6
    }
}
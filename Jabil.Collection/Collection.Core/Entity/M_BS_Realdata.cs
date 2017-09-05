using System;
using System.Runtime.Serialization;
using DeerMonitor.Framework.ControlUtil;

namespace Collection.Core.Entity
{
    /// <summary>
    ///     实时数据
    /// </summary>
    [DataContract]
    public class M_BS_Realdata_Spt : BaseEntity
    {
        public M_BS_Realdata_Spt()
        {
            TagVal = 0;
            TagLL = 0;
            TagL = 0;
            TagH = 0;
            TagHH = 0;
            ID = 0;
        }

        /// <summary>
        ///     位号编码
        /// </summary>
        [DataMember]
        public virtual string TagCode { get; set; }

        /// <summary>
        ///     位号名称
        /// </summary>
        [DataMember]
        public virtual string TagName { get; set; }

        /// <summary>
        ///     单位
        /// </summary>
        [DataMember]
        public virtual string TagEU { get; set; }

        /// <summary>
        ///     上上限
        /// </summary>
        [DataMember]
        public virtual decimal TagHH { get; set; }

        /// <summary>
        ///     上限
        /// </summary>
        [DataMember]
        public virtual decimal TagH { get; set; }

        /// <summary>
        ///     下限
        /// </summary>
        [DataMember]
        public virtual decimal TagL { get; set; }

        /// <summary>
        ///     下下限
        /// </summary>
        [DataMember]
        public virtual decimal TagLL { get; set; }

        /// <summary>
        ///     正常值
        /// </summary>
        [DataMember]
        public virtual decimal TagVal { get; set; }

        #region Field Members

        #endregion

        #region Property Members

        /// <summary>
        ///     ID
        /// </summary>
        [DataMember]
        public virtual int ID { get; set; }

        /// <summary>
        ///     位号ID
        /// </summary>
        [DataMember]
        public virtual string TagID { get; set; }

        /// <summary>
        ///     实时值
        /// </summary>
        [DataMember]
        public virtual decimal RealValue { get; set; }

        /// <summary>
        ///     状态
        /// </summary>
        [DataMember]
        public virtual string RealState { get; set; }

        /// <summary>
        ///     时间
        /// </summary>
        [DataMember]
        public virtual DateTime RealTime { get; set; }

        /// <summary>
        ///     最小值
        /// </summary>
        [DataMember]
        public virtual string RealMinVal { get; set; }

        /// <summary>
        ///     最大值
        /// </summary>
        [DataMember]
        public virtual string RealMaxVal { get; set; }

        /// <summary>
        ///     平均值
        /// </summary>
        [DataMember]
        public virtual string RealVarVal { get; set; }

        #endregion
    }
}
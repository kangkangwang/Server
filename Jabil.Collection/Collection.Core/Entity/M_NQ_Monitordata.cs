using System;
using System.Runtime.Serialization;
using DeerMonitor.Framework.ControlUtil;

//using System.ComponentModel.DataAnnotations;

namespace Collection.Core.Entity
{
    /// <summary>
    ///     M_NQ_Monitordata
    /// </summary>
    [DataContract]
    public class M_NQ_Monitordata : BaseEntity
    {
        #region Field Members

        public M_NQ_Monitordata()
        {
            DeviceID = 0;
        }

        #endregion

        #region Property Members

        public virtual int DeviceID { get; set; }

        public virtual DateTime InsertTime { get; set; }

        public virtual float ?   Ua { get; set; }

        public virtual float ?   Ub { get; set; }

        public virtual float ?   Uc { get; set; }

        public virtual float ?   U0 { get; set; }

        public virtual float ?   Ua1 { get; set; }

        public virtual float ?   Ub1 { get; set; }

        public virtual float ?   Uc1 { get; set; }

        public virtual float ?   Ia { get; set; }

        public virtual float ?   Ib { get; set; }

        public virtual float ?   Ic { get; set; }

        public virtual float ?   I0 { get; set; }

        public virtual float ?   Ia1 { get; set; }

        public virtual float ?   Ib1 { get; set; }

        public virtual float ?   Ic1 { get; set; }

        public virtual float ?   I01 { get; set; }

        public virtual float ?   Pa { get; set; }

        public virtual float ?   Pb { get; set; }

        public virtual float ?   Pc { get; set; }

        public virtual float ?   P0 { get; set; }

        public virtual float ?   Pa1 { get; set; }

        public virtual float ?   Pb1 { get; set; }

        public virtual float ?   Pc1 { get; set; }

        public virtual float ?   P01 { get; set; }

        public virtual float ?   Poa { get; set; }

        public virtual float ?   Pob { get; set; }

        public virtual float ?   Poc { get; set; }

        public virtual float ?   Po0 { get; set; }

        public virtual float ?   Poa1 { get; set; }

        public virtual float ?   Pob1 { get; set; }

        public virtual float ?   Poc1 { get; set; }

        public virtual float ?   Po01 { get; set; }

        public virtual float ?   Qa { get; set; }

        public virtual float ?   Qb { get; set; }

        public virtual float ?   Qc { get; set; }

        public virtual float ?   Q0 { get; set; }

        public virtual float ?   Sa { get; set; }

        public virtual float ?   Sb { get; set; }

        public virtual float ?   Sc { get; set; }

        public virtual float ?   S0 { get; set; }

        public virtual float ?   Wpa { get; set; }

        public virtual float ?   Wpb { get; set; }

        public virtual float ?   Wpc { get; set; }

        public virtual float ?   Wp { get; set; }

        public virtual float ?   Wqa { get; set; }

        public virtual float ?   Wqb { get; set; }

        public virtual float ?   Wqc { get; set; }

        public virtual float ?   Wq { get; set; }

        public virtual float ?   Ca { get; set; }

        public virtual float ?   Cb { get; set; }

        public virtual float ?   Cc { get; set; }

        public virtual float ? Epi { get; set; }

        public virtual float ? Epia { get; set; }

        public virtual float ? Epib { get; set; }

        public virtual float ? Epic { get; set; }

        public virtual float ? Epia1 { get; set; }

        public virtual float ? Epib1 { get; set; }

        public virtual float ? Epic1 { get; set; }

        public virtual float ? Epi1 { get; set; }

        public virtual float ? Epoa { get; set; }

        public virtual float ? Epob { get; set; }

        public virtual float ? Epoc { get; set; }

        public virtual float ? Epo { get; set; }

        public virtual float ? Epoa1 { get; set; }

        public virtual float ? Epob1 { get; set; }

        public virtual float ? Epoc1 { get; set; }

        public virtual float ? Epo1 { get; set; }

        public virtual float ?   Qo0 { get; set; }

        public virtual float ? EQind { get; set; }

        public virtual float ? EQcap { get; set; }

        public virtual float ?   Q01 { get; set; }

        public virtual float ?   Qo01 { get; set; }

        public virtual float ? EQind1 { get; set; }

        public virtual float ? EQcap1 { get; set; }

        public virtual float ?   Prf { get; set; }

        public virtual float ?   Pft { get; set; }

        public virtual float ?   Fhl { get; set; }

        public virtual float ?   JBa { get; set; }

        public virtual float ?   JBb { get; set; }

        public virtual float ?   JBc { get; set; }

        public virtual float ?   FPmaxd { get; set; }

        public virtual DateTime? FPmaxdT { get; set; }

        public virtual float ?   BPmaxd { get; set; }

        public virtual DateTime? BPmaxdT { get; set; }

        public virtual float ?   FQmaxd { get; set; }

        public virtual DateTime? FQmaxdT { get; set; }

        public virtual float ?   BQmaxd { get; set; }

        public virtual DateTime? BQmaxdT { get; set; }

        public virtual float ?   XBIav { get; set; }

        public virtual float ?   XBIbv { get; set; }

        public virtual float ?   XBIcv { get; set; }

        public virtual float ?   XBUav { get; set; }

        public virtual float ?   XBUbv { get; set; }

        public virtual float ?   XBUcv { get; set; }

        public virtual float ?   BPHu { get; set; }

        public virtual float ?   BPHi { get; set; }

        public virtual float ?   JBoUp { get; set; }

        public virtual float ?   JBoUn { get; set; }

        public virtual float ?   JBoIp { get; set; }

        public virtual float ?   JBoIn { get; set; }

        public virtual float ?   F { get; set; }

        #endregion
    }
}
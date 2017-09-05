using System;

namespace Collection.Core.Model
{
    public class M_MT_RealData
    {
        public int ID { get; set; }
        public string TagID { get; set; }
        public decimal RealValue { get; set; }
        public string RealState { get; set; }
        public DateTime RealTime { get; set; }
        public decimal RealMinVal { get; set; }
        public decimal RealMaxVal { get; set; }
        public decimal RealVarVal { get; set; }
        public DateTime RecordTime { get; set; }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Collection.Core.Entity;
using Collection.Core.IDAL;
using DeerMonitor.Framework.Commons;
using DeerMonitor.Framework.ControlUtil;
using MySql.Data.MySqlClient;

namespace Collection.Core.DALSQL
{
    /// <summary>
    ///     D_NQ_Monitordata
    /// </summary>
    public class D_NQ_Monitordata : BaseDALSQL<M_NQ_Monitordata>, I_NQ_Monitordata
    {

        #region 对象实例及构造函数

        public static D_NQ_Monitordata Instance
        {
            get { return new D_NQ_Monitordata(); }
        }

        public D_NQ_Monitordata()
            : base("T_NQ_Monitordata", "DeviceID")
        {
        }

        #endregion

        /// <summary>
        ///     获取字段中文别名（用于界面显示）的字典集合
        /// </summary>
        /// <returns></returns>
        public override Dictionary<string, string> GetColumnNameAlias()
        {
            var dict = new Dictionary<string, string>();

            #region 添加别名解析

            //dict.Add("ID", "编号");
            dict.Add("DeviceID", "");
            dict.Add("InsertTime", "");
            dict.Add("Ua", "");
            dict.Add("Ub", "");
            dict.Add("Uc", "");
            dict.Add("U0", "");
            dict.Add("Ua1", "");
            dict.Add("Ub1", "");
            dict.Add("Uc1", "");
            dict.Add("Ia", "");
            dict.Add("Ib", "");
            dict.Add("Ic", "");
            dict.Add("I0", "");
            dict.Add("Ia1", "");
            dict.Add("Ib1", "");
            dict.Add("Ic1", "");
            dict.Add("I01", "");
            dict.Add("Pa", "");
            dict.Add("Pb", "");
            dict.Add("Pc", "");
            dict.Add("P0", "");
            dict.Add("Pa1", "");
            dict.Add("Pb1", "");
            dict.Add("Pc1", "");
            dict.Add("P01", "");
            dict.Add("Poa", "");
            dict.Add("Pob", "");
            dict.Add("Poc", "");
            dict.Add("Po0", "");
            dict.Add("Poa1", "");
            dict.Add("Pob1", "");
            dict.Add("Poc1", "");
            dict.Add("Po01", "");
            dict.Add("Qa", "");
            dict.Add("Qb", "");
            dict.Add("Qc", "");
            dict.Add("Q0", "");
            dict.Add("Sa", "");
            dict.Add("Sb", "");
            dict.Add("Sc", "");
            dict.Add("S0", "");
            dict.Add("Wpa", "");
            dict.Add("Wpb", "");
            dict.Add("Wpc", "");
            dict.Add("Wp", "");
            dict.Add("Wqa", "");
            dict.Add("Wqb", "");
            dict.Add("Wqc", "");
            dict.Add("Wq", "");
            dict.Add("Ca", "");
            dict.Add("Cb", "");
            dict.Add("Cc", "");
            dict.Add("Epi", "");
            dict.Add("Epia", "");
            dict.Add("Epib", "");
            dict.Add("Epic", "");
            dict.Add("Epia1", "");
            dict.Add("Epib1", "");
            dict.Add("Epic1", "");
            dict.Add("Epi1", "");
            dict.Add("Epoa", "");
            dict.Add("Epob", "");
            dict.Add("Epoc", "");
            dict.Add("Epo", "");
            dict.Add("Epoa1", "");
            dict.Add("Epob1", "");
            dict.Add("Epoc1", "");
            dict.Add("Epo1", "");
            dict.Add("Qo0", "");
            dict.Add("EQind", "");
            dict.Add("EQcap", "");
            dict.Add("Q01", "");
            dict.Add("Qo01", "");
            dict.Add("EQind1", "");
            dict.Add("EQcap1", "");
            dict.Add("Prf", "");
            dict.Add("Pft", "");
            dict.Add("Fhl", "");
            dict.Add("JBa", "");
            dict.Add("JBb", "");
            dict.Add("JBc", "");
            dict.Add("FPmaxd", "");
            dict.Add("FPmaxdT", "");
            dict.Add("BPmaxd", "");
            dict.Add("BPmaxdT", "");
            dict.Add("FQmaxd", "");
            dict.Add("FQmaxdT", "");
            dict.Add("BQmaxd", "");
            dict.Add("BQmaxdT", "");
            dict.Add("XBIav", "");
            dict.Add("XBIbv", "");
            dict.Add("XBIcv", "");
            dict.Add("XBUav", "");
            dict.Add("XBUbv", "");
            dict.Add("XBUcv", "");
            dict.Add("BPHu", "");
            dict.Add("BPHi", "");
            dict.Add("JBoUp", "");
            dict.Add("JBoUn", "");
            dict.Add("JBoIp", "");
            dict.Add("JBoIn", "");
            dict.Add("F", "");

            #endregion

            //关联查询需将字段信息加在此处
            //自定义查询的字段写在此处

            return dict;
        }

        /// <summary>
        ///     设备实时数据查询
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public List<M_NQ_Monitordata> GetNewList(string where, MySqlConnection con)
        {
            if (con==null)
            {
                return null;
            }
            var lst = new List<M_NQ_Monitordata>();
            var sql = string.Format(@"select * from monitordata where 1=1 ");
            if (!string.IsNullOrWhiteSpace(where))
            {
                sql += "  and" + where;
            }
            if (con.State != ConnectionState.Open)
            {
                try
                {
                    con.Open();
                }
                catch
                {
                    return null;
                }
            }
            var mcd = con.CreateCommand();
            mcd.CommandText = sql;
            var reader = mcd.ExecuteReader();
            M_NQ_Monitordata entity = null;
            if (reader != null)
            {
                while (reader.Read())
                {
                    entity = DataReaderToEntity2(reader);

                    lst.Add(entity);
                }
                reader.Close();
            }
           
            return lst;
        }

        /// <summary>
        ///     将DataReader的属性值转化为实体类的属性值，返回实体类
        /// </summary>
        /// <param name="dr">有效的DataReader对象</param>
        /// <returns>实体类对象</returns>
        protected override M_NQ_Monitordata DataReaderToEntity(IDataReader dataReader)
        {
            var info = new M_NQ_Monitordata();
            var reader = new SmartDataReader(dataReader);
            var row = reader.FieldCount();
            for (var i = 0; i < row; i++)
            {
                var fieldName = reader.GetName(i);
                switch (fieldName)
                {
                    //    #region 模型转化

                    //    //				   
                    //case "DeviceID":
                    //    info.DeviceID = reader.GetInt32(fieldName);
                    //    break;
                    ////				   
                    //case "InsertTime":
                    //    info.InsertTime = reader.GetDateTime(fieldName);
                    //    break;
                    ////				   
                    //case "Ua":
                    //    info.Ua = reader.GetDouble(fieldName);
                    //    break;
                    ////				   
                    //case "Ub":
                    //    info.Ub = reader.GetDouble(fieldName);
                    //    break;
                    ////				   
                    //case "Uc":
                    //    info.Uc = reader.GetDouble(fieldName);
                    //    break;
                    ////				   
                    //case "U0":
                    //    info.U0 = reader.GetDouble(fieldName);
                    //    break;
                    ////				   
                    //case "Ua1":
                    //    info.Ua1 = reader.GetDouble(fieldName);
                    //    break;
                    ////				   
                    //case "Ub1":
                    //    info.Ub1 = reader.GetDouble(fieldName);
                    //    break;
                    ////				   
                    //case "Uc1":
                    //    info.Uc1 = reader.GetDouble(fieldName);
                    //    break;
                    ////				   
                    //case "Ia":
                    //    info.Ia = reader.GetDouble(fieldName);
                    //    break;
                    ////				   
                    //case "Ib":
                    //    info.Ib = reader.GetDouble(fieldName);
                    //    break;
                    ////				   
                    //case "Ic":
                    //    info.Ic = reader.GetDouble(fieldName);
                    //    break;
                    ////				   
                    //case "I0":
                    //    info.I0 = reader.GetDouble(fieldName);
                    //    break;
                    ////				   
                    //case "Ia1":
                    //    info.Ia1 = reader.GetDouble(fieldName);
                    //    break;
                    ////				   
                    //case "Ib1":
                    //    info.Ib1 = reader.GetDouble(fieldName);
                    //    break;
                    ////				   
                    //case "Ic1":
                    //    info.Ic1 = reader.GetDouble(fieldName);
                    //    break;
                    ////				   
                    //case "I01":
                    //    info.I01 = reader.GetDouble(fieldName);
                    //    break;
                    ////				   
                    //case "Pa":
                    //    info.Pa = reader.GetDouble(fieldName);
                    //    break;
                    ////				   
                    //case "Pb":
                    //    info.Pb = reader.GetDouble(fieldName);
                    //    break;
                    ////				   
                    //case "Pc":
                    //    info.Pc = reader.GetDouble(fieldName);
                    //    break;
                    ////				   
                    //case "P0":
                    //    info.P0 = reader.GetDouble(fieldName);
                    //    break;
                    ////				   
                    //case "Pa1":
                    //    info.Pa1 = reader.GetDouble(fieldName);
                    //    break;
                    ////				   
                    //case "Pb1":
                    //    info.Pb1 = reader.GetDouble(fieldName);
                    //    break;
                    ////				   
                    //case "Pc1":
                    //    info.Pc1 = reader.GetDouble(fieldName);
                    //    break;
                    ////				   
                    //case "P01":
                    //    info.P01 = reader.GetDouble(fieldName);
                    //    break;
                    ////				   
                    //case "Poa":
                    //    info.Poa = reader.GetDouble(fieldName);
                    //    break;
                    ////				   
                    //case "Pob":
                    //    info.Pob = reader.GetDouble(fieldName);
                    //    break;
                    ////				   
                    //case "Poc":
                    //    info.Poc = reader.GetDouble(fieldName);
                    //    break;
                    ////				   
                    //case "Po0":
                    //    info.Po0 = reader.GetDouble(fieldName);
                    //    break;
                    ////				   
                    //case "Poa1":
                    //    info.Poa1 = reader.GetDouble(fieldName);
                    //    break;
                    ////				   
                    //case "Pob1":
                    //    info.Pob1 = reader.GetDouble(fieldName);
                    //    break;
                    ////				   
                    //case "Poc1":
                    //    info.Poc1 = reader.GetDouble(fieldName);
                    //    break;
                    ////				   
                    //case "Po01":
                    //    info.Po01 = reader.GetDouble(fieldName);
                    //    break;
                    ////				   
                    //case "Qa":
                    //    info.Qa = reader.GetDouble(fieldName);
                    //    break;
                    ////				   
                    //case "Qb":
                    //    info.Qb = reader.GetDouble(fieldName);
                    //    break;
                    ////				   
                    //case "Qc":
                    //    info.Qc = reader.GetDouble(fieldName);
                    //    break;
                    ////				   
                    //case "Q0":
                    //    info.Q0 = reader.GetDouble(fieldName);
                    //    break;
                    ////				   
                    //case "Sa":
                    //    info.Sa = reader.GetDouble(fieldName);
                    //    break;
                    ////				   
                    //case "Sb":
                    //    info.Sb = reader.GetDouble(fieldName);
                    //    break;
                    ////				   
                    //case "Sc":
                    //    info.Sc = reader.GetDouble(fieldName);
                    //    break;
                    ////				   
                    //case "S0":
                    //    info.S0 = reader.GetDouble(fieldName);
                    //    break;
                    ////				   
                    //case "Wpa":
                    //    info.Wpa = reader.GetDouble(fieldName);
                    //    break;
                    ////				   
                    //case "Wpb":
                    //    info.Wpb = reader.GetDouble(fieldName);
                    //    break;
                    ////				   
                    //case "Wpc":
                    //    info.Wpc = reader.GetDouble(fieldName);
                    //    break;
                    ////				   
                    //case "Wp":
                    //    info.Wp = reader.GetDouble(fieldName);
                    //    break;
                    ////				   
                    //case "Wqa":
                    //    info.Wqa = reader.GetDouble(fieldName);
                    //    break;
                    ////				   
                    //case "Wqb":
                    //    info.Wqb = reader.GetDouble(fieldName);
                    //    break;
                    ////				   
                    //case "Wqc":
                    //    info.Wqc = reader.GetDouble(fieldName);
                    //    break;
                    ////				   
                    //case "Wq":
                    //    info.Wq = reader.GetDouble(fieldName);
                    //    break;
                    ////				   
                    //case "Ca":
                    //    info.Ca = reader.GetDouble(fieldName);
                    //    break;
                    ////				   
                    //case "Cb":
                    //    info.Cb = reader.GetDouble(fieldName);
                    //    break;
                    ////				   
                    //case "Cc":
                    //    info.Cc = reader.GetDouble(fieldName);
                    //    break;
                    ////				   
                    //case "Epi":
                    //    info.Epi = reader.GetDecimal(fieldName);
                    //    break;
                    ////				   
                    //case "Epia":
                    //    info.Epia = reader.GetDecimal(fieldName);
                    //    break;
                    ////				   
                    //case "Epib":
                    //    info.Epib = reader.GetDecimal(fieldName);
                    //    break;
                    ////				   
                    //case "Epic":
                    //    info.Epic = reader.GetDecimal(fieldName);
                    //    break;
                    ////				   
                    //case "Epia1":
                    //    info.Epia1 = reader.GetDecimal(fieldName);
                    //    break;
                    ////				   
                    //case "Epib1":
                    //    info.Epib1 = reader.GetDecimal(fieldName);
                    //    break;
                    ////				   
                    //case "Epic1":
                    //    info.Epic1 = reader.GetDecimal(fieldName);
                    //    break;
                    ////				   
                    //case "Epi1":
                    //    info.Epi1 = reader.GetDecimal(fieldName);
                    //    break;
                    ////				   
                    //case "Epoa":
                    //    info.Epoa = reader.GetDecimal(fieldName);
                    //    break;
                    ////				   
                    //case "Epob":
                    //    info.Epob = reader.GetDecimal(fieldName);
                    //    break;
                    ////				   
                    //case "Epoc":
                    //    info.Epoc = reader.GetDecimal(fieldName);
                    //    break;
                    ////				   
                    //case "Epo":
                    //    info.Epo = reader.GetDecimal(fieldName);
                    //    break;
                    ////				   
                    //case "Epoa1":
                    //    info.Epoa1 = reader.GetDecimal(fieldName);
                    //    break;
                    ////				   
                    //case "Epob1":
                    //    info.Epob1 = reader.GetDecimal(fieldName);
                    //    break;
                    ////				   
                    //case "Epoc1":
                    //    info.Epoc1 = reader.GetDecimal(fieldName);
                    //    break;
                    ////				   
                    //case "Epo1":
                    //    info.Epo1 = reader.GetDecimal(fieldName);
                    //    break;
                    ////				   
                    //case "Qo0":
                    //    info.Qo0 = reader.GetDouble(fieldName);
                    //    break;
                    ////				   
                    //case "EQind":
                    //    info.EQind = reader.GetDecimal(fieldName);
                    //    break;
                    ////				   
                    //case "EQcap":
                    //    info.EQcap = reader.GetDecimal(fieldName);
                    //    break;
                    ////				   
                    //case "Q01":
                    //    info.Q01 = reader.GetDouble(fieldName);
                    //    break;
                    ////				   
                    //case "Qo01":
                    //    info.Qo01 = reader.GetDouble(fieldName);
                    //    break;
                    ////				   
                    //case "EQind1":
                    //    info.EQind1 = reader.GetDecimal(fieldName);
                    //    break;
                    ////				   
                    //case "EQcap1":
                    //    info.EQcap1 = reader.GetDecimal(fieldName);
                    //    break;
                    ////				   
                    //case "Prf":
                    //    info.Prf = reader.GetDouble(fieldName);
                    //    break;
                    ////				   
                    //case "Pft":
                    //    info.Pft = reader.GetDouble(fieldName);
                    //    break;
                    ////				   
                    //case "Fhl":
                    //    info.Fhl = reader.GetDouble(fieldName);
                    //    break;
                    ////				   
                    //case "JBa":
                    //    info.JBa = reader.GetDouble(fieldName);
                    //    break;
                    ////				   
                    //case "JBb":
                    //    info.JBb = reader.GetDouble(fieldName);
                    //    break;
                    ////				   
                    //case "JBc":
                    //    info.JBc = reader.GetDouble(fieldName);
                    //    break;
                    ////				   
                    //case "FPmaxd":
                    //    info.FPmaxd = reader.GetDouble(fieldName);
                    //    break;
                    ////				   
                    //case "FPmaxdT":
                    //    info.FPmaxdT = reader.GetDateTime(fieldName);
                    //    break;
                    ////				   
                    //case "BPmaxd":
                    //    info.BPmaxd = reader.GetDouble(fieldName);
                    //    break;
                    ////				   
                    //case "BPmaxdT":
                    //    info.BPmaxdT = reader.GetDateTime(fieldName);
                    //    break;
                    ////				   
                    //case "FQmaxd":
                    //    info.FQmaxd = reader.GetDouble(fieldName);
                    //    break;
                    ////				   
                    //case "FQmaxdT":
                    //    info.FQmaxdT = reader.GetDateTime(fieldName);
                    //    break;
                    ////				   
                    //case "BQmaxd":
                    //    info.BQmaxd = reader.GetDouble(fieldName);
                    //    break;
                    ////				   
                    //case "BQmaxdT":
                    //    info.BQmaxdT = reader.GetDateTime(fieldName);
                    //    break;
                    ////				   
                    //case "XBIav":
                    //    info.XBIav = reader.GetDouble(fieldName);
                    //    break;
                    ////				   
                    //case "XBIbv":
                    //    info.XBIbv = reader.GetDouble(fieldName);
                    //    break;
                    ////				   
                    //case "XBIcv":
                    //    info.XBIcv = reader.GetDouble(fieldName);
                    //    break;
                    ////				   
                    //case "XBUav":
                    //    info.XBUav = reader.GetDouble(fieldName);
                    //    break;
                    ////				   
                    //case "XBUbv":
                    //    info.XBUbv = reader.GetDouble(fieldName);
                    //    break;
                    ////				   
                    //case "XBUcv":
                    //    info.XBUcv = reader.GetDouble(fieldName);
                    //    break;
                    ////				   
                    //case "BPHu":
                    //    info.BPHu = reader.GetDouble(fieldName);
                    //    break;
                    ////				   
                    //case "BPHi":
                    //    info.BPHi = reader.GetDouble(fieldName);
                    //    break;
                    ////				   
                    //case "JBoUp":
                    //    info.JBoUp = reader.GetDouble(fieldName);
                    //    break;
                    ////				   
                    //case "JBoUn":
                    //    info.JBoUn = reader.GetDouble(fieldName);
                    //    break;
                    ////				   
                    //case "JBoIp":
                    //    info.JBoIp = reader.GetDouble(fieldName);
                    //    break;
                    ////				   
                    //case "JBoIn":
                    //    info.JBoIn = reader.GetDouble(fieldName);
                    //    break;
                    ////				   
                    //case "F":
                    //    info.F = reader.GetDouble(fieldName);
                    //    break;

                    //    #endregion
                }
            }

            return info;
        }

        /// <summary>
        ///     将实体对象的属性值转化为Hashtable对应的键值
        /// </summary>
        /// <param name="obj">有效的实体对象</param>
        /// <returns>包含键值映射的Hashtable</returns>
        protected override Hashtable GetHashByEntity(M_NQ_Monitordata obj)
        {
            var info = obj;
            var hash = new Hashtable();

            hash.Add("DeviceID", info.DeviceID);
            hash.Add("InsertTime", info.InsertTime);
            hash.Add("Ua", info.Ua);
            hash.Add("Ub", info.Ub);
            hash.Add("Uc", info.Uc);
            hash.Add("U0", info.U0);
            hash.Add("Ua1", info.Ua1);
            hash.Add("Ub1", info.Ub1);
            hash.Add("Uc1", info.Uc1);
            hash.Add("Ia", info.Ia);
            hash.Add("Ib", info.Ib);
            hash.Add("Ic", info.Ic);
            hash.Add("I0", info.I0);
            hash.Add("Ia1", info.Ia1);
            hash.Add("Ib1", info.Ib1);
            hash.Add("Ic1", info.Ic1);
            hash.Add("I01", info.I01);
            hash.Add("Pa", info.Pa);
            hash.Add("Pb", info.Pb);
            hash.Add("Pc", info.Pc);
            hash.Add("P0", info.P0);
            hash.Add("Pa1", info.Pa1);
            hash.Add("Pb1", info.Pb1);
            hash.Add("Pc1", info.Pc1);
            hash.Add("P01", info.P01);
            hash.Add("Poa", info.Poa);
            hash.Add("Pob", info.Pob);
            hash.Add("Poc", info.Poc);
            hash.Add("Po0", info.Po0);
            hash.Add("Poa1", info.Poa1);
            hash.Add("Pob1", info.Pob1);
            hash.Add("Poc1", info.Poc1);
            hash.Add("Po01", info.Po01);
            hash.Add("Qa", info.Qa);
            hash.Add("Qb", info.Qb);
            hash.Add("Qc", info.Qc);
            hash.Add("Q0", info.Q0);
            hash.Add("Sa", info.Sa);
            hash.Add("Sb", info.Sb);
            hash.Add("Sc", info.Sc);
            hash.Add("S0", info.S0);
            hash.Add("Wpa", info.Wpa);
            hash.Add("Wpb", info.Wpb);
            hash.Add("Wpc", info.Wpc);
            hash.Add("Wp", info.Wp);
            hash.Add("Wqa", info.Wqa);
            hash.Add("Wqb", info.Wqb);
            hash.Add("Wqc", info.Wqc);
            hash.Add("Wq", info.Wq);
            hash.Add("Ca", info.Ca);
            hash.Add("Cb", info.Cb);
            hash.Add("Cc", info.Cc);
            hash.Add("Epi", info.Epi);
            hash.Add("Epia", info.Epia);
            hash.Add("Epib", info.Epib);
            hash.Add("Epic", info.Epic);
            hash.Add("Epia1", info.Epia1);
            hash.Add("Epib1", info.Epib1);
            hash.Add("Epic1", info.Epic1);
            hash.Add("Epi1", info.Epi1);
            hash.Add("Epoa", info.Epoa);
            hash.Add("Epob", info.Epob);
            hash.Add("Epoc", info.Epoc);
            hash.Add("Epo", info.Epo);
            hash.Add("Epoa1", info.Epoa1);
            hash.Add("Epob1", info.Epob1);
            hash.Add("Epoc1", info.Epoc1);
            hash.Add("Epo1", info.Epo1);
            hash.Add("Qo0", info.Qo0);
            hash.Add("EQind", info.EQind);
            hash.Add("EQcap", info.EQcap);
            hash.Add("Q01", info.Q01);
            hash.Add("Qo01", info.Qo01);
            hash.Add("EQind1", info.EQind1);
            hash.Add("EQcap1", info.EQcap1);
            hash.Add("PRF", info.Prf);
            hash.Add("PFT", info.Pft);
            hash.Add("FHL", info.Fhl);
            hash.Add("JBa", info.JBa);
            hash.Add("JBb", info.JBb);
            hash.Add("JBc", info.JBc);
            hash.Add("FPmaxd", info.FPmaxd);
            hash.Add("FPmaxdT", info.FPmaxdT);
            hash.Add("BPmaxd", info.BPmaxd);
            hash.Add("BPmaxdT", info.BPmaxdT);
            hash.Add("FQmaxd", info.FQmaxd);
            hash.Add("FQmaxdT", info.FQmaxdT);
            hash.Add("BQmaxd", info.BQmaxd);
            hash.Add("BQmaxdT", info.BQmaxdT);
            hash.Add("XBIav", info.XBIav);
            hash.Add("XBIbv", info.XBIbv);
            hash.Add("XBIcv", info.XBIcv);
            hash.Add("XBUav", info.XBUav);
            hash.Add("XBUbv", info.XBUbv);
            hash.Add("XBUcv", info.XBUcv);
            hash.Add("BPHu", info.BPHu);
            hash.Add("BPHi", info.BPHi);
            hash.Add("JBoUp", info.JBoUp);
            hash.Add("JBoUn", info.JBoUn);
            hash.Add("JBoIp", info.JBoIp);
            hash.Add("JBoIn", info.JBoIn);
            hash.Add("F", info.F);
            //除了数据表中的字段，不要加其他

            return hash;
        }

        /// <summary>
        ///     将DataReader的属性值转化为实体类的属性值，返回实体类
        /// </summary>
        /// <param name="dr">有效的DataReader对象</param>
        /// <returns>实体类对象</returns>
        protected  M_NQ_Monitordata DataReaderToEntity2(IDataReader dr)
        {
            var info = new M_NQ_Monitordata();
            var pis = info.GetType().GetProperties();

            foreach (var pi in pis)
            {
                try
                {
                    if (dr[pi.Name].ToString() != "")
                    {
                        var ob = dr[pi.Name];
                        pi.SetValue(info, ob ?? "", null);
                    }
                }
                catch
                {
                }
            }

            return info;
        }

       
    }
}
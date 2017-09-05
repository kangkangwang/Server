using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Collection.Core.Entity;
using Collection.Core.IDAL;
using Collection.Core.Model;
using DeerMonitor.Framework.Commons;
using DeerMonitor.Framework.ControlUtil;

namespace Collection.Core.DALSQL
{
    /// <summary>
    ///     实时数据
    /// </summary>
    public class D_BS_Realdata_Spt : BaseDALSQL<M_BS_Realdata_Spt>, I_BS_Realdata_Spt
    {

        #region 对象实例及构造函数

        public static D_BS_Realdata_Spt Instance
        {
            get { return new D_BS_Realdata_Spt(); }
        }

        public D_BS_Realdata_Spt()
            : base("T_BS_Realdata_Spt", "ID")
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
            dict.Add("TagID", "位号ID");
            dict.Add("RealValue", "实时值");
            dict.Add("RealState", "状态");
            dict.Add("RealTime", "时间");
            dict.Add("RealMinVal", "最小值");
            dict.Add("RealMaxVal", "最大值");
            dict.Add("RealVarVal", "平均值");

            dict.Add("TagCode", "位号编码");
            dict.Add("TagName", "位号名称");

            #endregion

            return dict;
        }

        /// <summary>
        ///     设备实时数据查询
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public List<M_BS_Realdata_Spt> GetNewList(string type, M_MT_TagDefine model = null)
        {
            var sql = string.Format(@"select c.*,b.TagCode from {0}T_BS_TagDefine a 
                                      inner join {0}T_BS_TagDefine b on a.TagGroupID='-1' and a.TagType='{1}' and b.TagGroupID=a.ID AND b.IsUse=1
                                      inner join {0}T_BS_Realdata_Spt c on c.TagID=b.ID", prefixName,type);
            if (model!=null)
            {
                sql += string.Format(" where a.ID ='{0}'",model.ID);
            }
            return GetList(sql, null);
        }

        /// <summary>
        ///     设备实时数据查询
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public List<M_BS_Realdata_Spt> GetEnergyRecordList(DateTime time)
        {
            var sql = string.Format(@"
                                  --时耗
                                  SELECT a.TagCode AS TagID,c.EnergyID,c.StructureID,b.ValueID,d.ConsumeTime AS RealTime,d.ActualCapacity AS RealValue,0 AS TagStatus {0}FROM T_EM_EnergyTag a 
                                  INNER JOIN {0}T_EM_EnergyValueRelation b ON b.ID=a.TagCode AND b.ValueID='ENV004'
                                  INNER JOIN {0}T_EM_EnergyStructureRelation c ON c.StructureID=a.EquCode AND c.ID=b.EnergyStructID 
                                  INNER JOIN {0}T_EM_EnergyValueRecordHour d ON d.EnergyStructID=b.EnergyStructID AND d.CalType=c.EnergyID AND d.EnergyStructID=c.ID
                                  WHERE d.ConsumeTime>'{1}' 
                                  UNION ALL
                                  ---日耗
                                  SELECT a.TagCode AS TagID,c.EnergyID,c.StructureID,b.ValueID,d.ConsumeTime AS RealTime,d.ActualCapacity AS RealValue,0 AS TagStatus FROM {0}T_EM_EnergyTag a 
                                  INNER JOIN {0}T_EM_EnergyValueRelation b ON b.ID=a.TagCode AND b.ValueID='ENV001'
                                  INNER JOIN {0}T_EM_EnergyStructureRelation c ON c.StructureID=a.EquCode AND c.ID=b.EnergyStructID 
                                  INNER JOIN {0}T_EM_EnergyValueRecordDay d ON d.EnergyStructID=b.EnergyStructID AND d.CalType=c.EnergyID AND d.EnergyStructID=c.ID
                                  WHERE d.ConsumeTime>'{2}' 
                                  UNION ALL
                                  ---月耗
                                  SELECT a.TagCode AS TagID,c.EnergyID,c.StructureID,b.ValueID,d.ConsumeTime AS RealTime,d.ActualCapacity AS RealValue,0 AS TagStatus FROM {0}T_EM_EnergyTag a 
                                  INNER JOIN {0}T_EM_EnergyValueRelation b ON b.ID=a.TagCode AND b.ValueID='ENV002'
                                  INNER JOIN {0}T_EM_EnergyStructureRelation c ON c.StructureID=a.EquCode AND c.ID=b.EnergyStructID 
                                  INNER JOIN {0}T_EM_EnergyValueRecordMonth d ON d.EnergyStructID=b.EnergyStructID AND d.CalType=c.EnergyID AND d.EnergyStructID=c.ID
                                  WHERE d.ConsumeTime>'{3}' 
                                  UNION ALL
                                  ---年耗
                                  SELECT a.TagCode AS TagID,c.EnergyID,c.StructureID,b.ValueID,d.ConsumeTime AS RealTime,d.ActualCapacity AS RealValue,0 AS TagStatus FROM {0}T_EM_EnergyTag a 
                                  INNER JOIN {0}T_EM_EnergyValueRelation b ON b.ID=a.TagCode AND b.ValueID='ENV003'
                                  INNER JOIN {0}T_EM_EnergyStructureRelation c ON c.StructureID=a.EquCode AND c.ID=b.EnergyStructID 
                                  INNER JOIN {0}T_EM_EnergyValueRecordMonth d ON d.EnergyStructID=b.EnergyStructID AND d.CalType=c.EnergyID AND d.EnergyStructID=c.ID
                                  WHERE d.ConsumeTime>'{4}' 

                                   ORDER BY StructureID,TagID,RealTime ", prefixName, time.AddHours((-1)),
                time.AddDays(-1), time.AddMonths((-1)), time.AddYears(-1));
            return GetList(sql, null);
        }

        /// <summary>
        ///     将DataReader的属性值转化为实体类的属性值，返回实体类
        /// </summary>
        /// <param name="dr">有效的DataReader对象</param>
        /// <returns>实体类对象</returns>
        protected override M_BS_Realdata_Spt DataReaderToEntity(IDataReader dataReader)
        {
            var info = new M_BS_Realdata_Spt();
            var reader = new SmartDataReader(dataReader);

            var row = reader.FieldCount();
            for (var i = 0; i < row; i++)
            {
                var fieldName = reader.GetName(i);
                switch (fieldName)
                {
                    #region 模型转换

                    //ID
                    //case "ID":
                    //    info.ID = reader.GetInt16(fieldName);
                    //    break;
                    //位号ID
                    case "TagID":
                        info.TagID = reader.GetString(fieldName);
                        break;
                    //实时值
                    case "RealValue":
                        info.RealValue = reader.GetDecimal(fieldName);
                        break;
                    //状态
                    case "RealState":
                        info.RealState = reader.GetString(fieldName);
                        break;
                    //时间
                    case "RealTime":
                        info.RealTime = reader.GetDateTime(fieldName);
                        break;
                    //最小值
                    case "RealMinVal":
                        info.RealMinVal = reader.GetString(fieldName);
                        break;
                    //最大值
                    case "RealMaxVal":
                        info.RealMaxVal = reader.GetString(fieldName);
                        break;
                    //平均值
                    case "RealVarVal":
                        info.RealVarVal = reader.GetString(fieldName);
                        break;


                    //位号编码
                    case "TagCode":
                        info.TagCode = reader.GetString(fieldName);
                        break;
                    //位号名称
                    case "TagName":
                        info.TagName = reader.GetString(fieldName);
                        break;
                    //单位
                    case "TagEU":
                        info.TagEU = reader.GetString(fieldName);
                        break;
                    //正常值
                    case "TagVal":
                        info.TagVal = reader.GetDecimal(fieldName);
                        break;
                    //上上限
                    case "TagHH":
                        info.TagHH = reader.GetDecimal(fieldName);
                        break;
                    //上限
                    case "TagH":
                        info.TagH = reader.GetDecimal(fieldName);
                        break;
                    //下限
                    case "TagL":
                        info.TagL = reader.GetDecimal(fieldName);
                        break;
                    //下下限
                    case "TagLL":
                        info.TagLL = reader.GetDecimal(fieldName);
                        break;
                    default:
                        break;

                    #endregion
                }
            }


            return info;
        }

        /// <summary>
        ///     将实体对象的属性值转化为Hashtable对应的键值
        /// </summary>
        /// <param name="obj">有效的实体对象</param>
        /// <returns>包含键值映射的Hashtable</returns>
        protected override Hashtable GetHashByEntity(M_BS_Realdata_Spt obj)
        {
            var info = obj;
            var hash = new Hashtable();

            hash.Add("TagID", info.TagID);
            hash.Add("RealValue", info.RealValue);
            hash.Add("RealState", info.RealState);
            hash.Add("RealTime", info.RealTime);
            hash.Add("RealMinVal", info.RealMinVal);
            hash.Add("RealMaxVal", info.RealMaxVal);
            hash.Add("RealVarVal", info.RealVarVal);

            return hash;
        }


    }
}
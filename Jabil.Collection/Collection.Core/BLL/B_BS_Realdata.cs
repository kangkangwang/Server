using System;
using System.Collections.Generic;
using System.Reflection;
using Collection.Core.Entity;
using Collection.Core.IDAL;
using Collection.Core.Model;
using DeerMonitor.Framework.ControlUtil;

namespace Collection.Core.BLL
{
    /// <summary>
    ///     实时数据
    /// </summary>
    public class B_BS_Realdata_Spt : BaseBLL<M_BS_Realdata_Spt>
    {
        private readonly I_BS_Realdata_Spt dal;

        public B_BS_Realdata_Spt()
        {
            var a = GetType().FullName;
            var b = Assembly.GetExecutingAssembly().GetName().Name;
            Init(GetType().FullName, Assembly.GetExecutingAssembly().GetName().Name);
            dal = baseDal as I_BS_Realdata_Spt;
        }

        public List<M_BS_Realdata_Spt> GetNewList(string type,M_MT_TagDefine model=null)
        {
            return dal.GetNewList(type,model);
        }

        public List<M_BS_Realdata_Spt> GetEnergyRecordList(DateTime where)
        {
            return dal.GetEnergyRecordList(where);
        }
    }
}
using System;
using System.Collections.Generic;
using Collection.Core.Entity;
using Collection.Core.Model;
using DeerMonitor.Framework.ControlUtil;

namespace Collection.Core.IDAL
{
    /// <summary>
    ///     实时数据
    /// </summary>
    public interface I_BS_Realdata_Spt : IBaseDAL<M_BS_Realdata_Spt>
    {
        List<M_BS_Realdata_Spt> GetNewList(string type, M_MT_TagDefine model = null);
        List<M_BS_Realdata_Spt> GetEnergyRecordList(DateTime dt);
    }
}
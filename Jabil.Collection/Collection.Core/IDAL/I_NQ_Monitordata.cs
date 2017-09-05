using System.Collections.Generic;
using Collection.Core.Entity;
using DeerMonitor.Framework.ControlUtil;
using MySql.Data.MySqlClient;

namespace Collection.Core.IDAL
{
    /// <summary>
    ///     I_NQ_Monitordata
    /// </summary>
    public interface I_NQ_Monitordata : IBaseDAL<M_NQ_Monitordata>
    {
        List<M_NQ_Monitordata> GetNewList(string where, MySqlConnection con);
    }
}
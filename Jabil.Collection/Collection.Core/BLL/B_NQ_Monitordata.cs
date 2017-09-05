using System.Collections.Generic;
using System.Reflection;
using Collection.Core.Entity;
using Collection.Core.IDAL;
using DeerMonitor.Framework.ControlUtil;
using MySql.Data.MySqlClient;

namespace Collection.Core.BLL
{
    /// <summary>
    ///     B_NQ_Monitordata
    /// </summary>
    public class B_NQ_Monitordata : BaseBLL<M_NQ_Monitordata>
    {
        private readonly I_NQ_Monitordata dal;

        public B_NQ_Monitordata()
        {
            Init(GetType().FullName, Assembly.GetExecutingAssembly().GetName().Name);
            dal = baseDal as I_NQ_Monitordata;
        }

        public List<M_NQ_Monitordata> GetNewList(string where, MySqlConnection con)
        {
            return dal.GetNewList(where, con);
        }
    }
}
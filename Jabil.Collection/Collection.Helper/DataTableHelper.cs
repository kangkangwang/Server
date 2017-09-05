using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Collection.Helper
{
   public class DataTableHelper
    {

       #region 创建dt格式

       public static void CreateDataTable(ref DataTable dt, string type = "")
       {
           dt = new DataTable();
           if (type == "T_BS_Realdata")
           {
               #region MyRegion

               dt.Columns.Add(new DataColumn("ID") { DataType = typeof(int) });
               dt.Columns.Add(new DataColumn("TagID") { DataType = typeof(string) });
               dt.Columns.Add(new DataColumn("RealValue") { DataType = typeof(decimal) });
               dt.Columns.Add(new DataColumn("RealState") { DataType = typeof(int) });
               dt.Columns.Add(new DataColumn("RealTime") { DataType = typeof(DateTime) });
               dt.Columns.Add(new DataColumn("RealMinVal") { DataType = typeof(decimal) });
               dt.Columns.Add(new DataColumn("RealMaxVal") { DataType = typeof(decimal) });
               dt.Columns.Add(new DataColumn("RealVarVal") { DataType = typeof(decimal) });
               dt.Columns.Add(new DataColumn("RecordTime") { DataType = typeof(DateTime) });

               #endregion
           }
           if (type == "T_EP_EquipWarning")
           {
               #region MyRegion

               dt.Columns.Add(new DataColumn("ID") { DataType = typeof(string) });
               dt.Columns.Add(new DataColumn("EquCode") { DataType = typeof(string) });
               dt.Columns.Add(new DataColumn("EquName") { DataType = typeof(string) });
               dt.Columns.Add(new DataColumn("TagCode") { DataType = typeof(string) });
               dt.Columns.Add(new DataColumn("TagName") { DataType = typeof(string) });
               dt.Columns.Add(new DataColumn("TagEU") { DataType = typeof(string) });
               dt.Columns.Add(new DataColumn("TagLL") { DataType = typeof(decimal) });
               dt.Columns.Add(new DataColumn("TagL") { DataType = typeof(decimal) });
               dt.Columns.Add(new DataColumn("TagH") { DataType = typeof(decimal) });
               dt.Columns.Add(new DataColumn("TagHH") { DataType = typeof(decimal) });
               dt.Columns.Add(new DataColumn("RealValue") { DataType = typeof(decimal) });
               dt.Columns.Add(new DataColumn("RealState") { DataType = typeof(int) });
               dt.Columns.Add(new DataColumn("RealTime") { DataType = typeof(DateTime) });
               dt.Columns.Add(new DataColumn("IsDealed") { DataType = typeof(bool) });

               #endregion
           }
       }

       #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Collection.Common
{
    public class BaseFunction
    {
        #region 创建DataTable字段

        /// <summary>
        ///     通过字符列表创建表字段，字段格式可以是：
        ///     1) a,b,c,d,e
        ///     2) a|int,b|string,c|bool,d|decimal
        /// </summary>
        /// <param name="nameString"></param>
        /// <returns></returns>
        public static DataTable CreateTable(string nameString)
        {
            var nameArray = nameString.Split(',', ';');
            var nameList = new List<string>();
            var dt = new DataTable();
            foreach (var item in nameArray)
            {
                if (!String.IsNullOrEmpty(item))
                {
                    var subItems = item.Split('|');
                    if (subItems.Length == 2)
                    {
                        dt.Columns.Add(subItems[0], ConvertType(subItems[1]));
                    }
                    else
                    {
                        dt.Columns.Add(subItems[0]);
                    }
                }
            }
            return dt;
        }

        #endregion

        #region 数据类型转换

        /// <summary>
        ///     数据类型转换
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        private static Type ConvertType(string typeName)
        {
            typeName = typeName.ToLower().Replace("system.", "");
            var newType = typeof (string);
            switch (typeName)
            {
                case "boolean":
                case "bool":
                    newType = typeof (bool);
                    break;
                case "int16":
                case "short":
                    newType = typeof (short);
                    break;
                case "int32":
                case "int":
                    newType = typeof (int);
                    break;
                case "long":
                case "int64":
                    newType = typeof (long);
                    break;
                case "uint16":
                case "ushort":
                    newType = typeof (ushort);
                    break;
                case "uint32":
                case "uint":
                    newType = typeof (uint);
                    break;
                case "uint64":
                case "ulong":
                    newType = typeof (ulong);
                    break;
                case "single":
                case "float":
                    newType = typeof (float);
                    break;

                case "string":
                    newType = typeof (string);
                    break;
                case "guid":
                    newType = typeof (Guid);
                    break;
                case "decimal":
                    newType = typeof (decimal);
                    break;
                case "double":
                    newType = typeof (double);
                    break;
                case "datetime":
                    newType = typeof (DateTime);
                    break;
                case "byte":
                    newType = typeof (byte);
                    break;
                case "char":
                    newType = typeof (char);
                    break;
            }
            return newType;
        }

        #endregion

        #region dataTable转换成Json格式

        /// <summary>
        ///     dataTable转换成Json格式
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string DataTable2Json(DataTable dt)
        {
            var jsonBuilder = new StringBuilder();
            //jsonBuilder.Append("{\"");
            //jsonBuilder.Append(dt.TableName);
            //jsonBuilder.Append("\":[");
            jsonBuilder.Append("[");
            for (var i = 0; i < dt.Rows.Count; i++)
            {
                jsonBuilder.Append("{");
                for (var j = 0; j < dt.Columns.Count; j++)
                {
                    jsonBuilder.Append("\"");
                    jsonBuilder.Append(dt.Columns[j].ColumnName);
                    jsonBuilder.Append("\":\"");
                    jsonBuilder.Append(dt.Rows[i][j]);
                    jsonBuilder.Append("\",");
                }
                jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
                jsonBuilder.Append("},");
            }
            jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
            jsonBuilder.Append("]");
            //jsonBuilder.Append("}");
            return jsonBuilder.ToString();
        }

        public static string DataTableToJson(DataTable dt)
        {
            var rs = "";
            foreach (DataRow row in dt.Rows)
            {
                rs += row[0] + "," + row[3] + "," + row[4] + "," + row[5] + "|";
            }
            rs = rs.Trim(',');
            rs = rs.Trim('|');
            //rs = "[" + rs+"]";
            return rs;
        }

        #endregion
    }
}
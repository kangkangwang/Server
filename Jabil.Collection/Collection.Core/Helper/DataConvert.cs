using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using Collection.Core.Entity;
using DeerMonitor.Base.Core.Entity;
using DeerMonitor.Framework.Commons;

namespace Collection.Core.Helper
{
    /// <summary>
    ///     数据转化类
    /// </summary>
    public class DataConvert
    {
        #region 获取单个位号转化后的值2

        /// <summary>
        ///     获取单个位号转化后的值
        /// </summary>
        /// <param name="formule"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static decimal GetOneData2(M_BS_TagDefine model, string[] arr)
        {
            decimal dValue = 0;
            var index = 0;
            index = model.DataIndex - 1;
            //dataindex<=0会造成求用电量时错误
            if (model == null || string.IsNullOrEmpty(model.Formule) || index < 0)
            {
                return dValue;
            }
            if (string.IsNullOrEmpty(arr[index]))
            {
                return dValue;
            }
            switch (model.Formule)
            {
                #region 获取转化后的值

                case "0":
                    dValue = decimal.Parse(arr[index]);
                    if (dValue > 50000)
                    {
                        dValue = dValue - 65536;
                    }
                    break; //直接读取和存储原值，无需转换
                case "1":
                    dValue = rhValue(arr[index]);
                    break; //温湿度传感器湿度转换
                case "2":
                    dValue = tValue(arr[index]);
                    break; //温湿度传感器温度转换
                case "3":
                    dValue = ttValue(arr[index]);
                    break; //温度传感器温度转换
                case "4":
                    dValue = ldValue(arr[index]);
                    break; //露点仪数据转换
                case "5":
                    dValue = ylValue(arr[index]);
                    break; //压力传感器压力值转换
                case "6":
                    dValue = csjValue(arr[index]);
                    break; //除湿机数据转换
                case "7":
                    dValue = kyjDl(arr[index]);
                    break;
                case "8":
                    dValue = gzjDl(arr[index]);
                    break;
                case "9":
                    dValue = zdjDl(arr[index]);
                    break;
                case "10":
                    dValue = decimal.Parse(arr[index]);
                    break;
                case "13":
                    if (index < 1)
                    {
                        return dValue;
                    }
                    dValue = kyjNh2(arr[index]) + kyjNh1(arr[index - 1]);
                    break;
                case "14":
                    if (index < 1)
                    {
                        return dValue;
                    }
                    dValue = gzjNh2(arr[index]) + gzjNh1(arr[index - 1]);
                    break;
                case "15":
                    if (index < 1)
                    {
                        return dValue;
                    }
                    dValue = zdjNh2(arr[index]) + zdjNh1(arr[index - 1]);
                    break;
                case "16":
                    dValue = kyjGl(arr[index]);
                    break;
                case "17":
                    dValue = gzjGl(arr[index]);
                    break;
                case "18":
                    dValue = zdjGl(arr[index]);
                    break;
                default:
                    break;

                #endregion
            }
            if (model.CompensateRatio > 0)
            {
                dValue = dValue * model.CompensateRatio;
            }
            return dValue;
        }

        #endregion

        #region 将数据转换成16进制

        /// <summary>
        ///     将数据转换成16进制字符串
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ByteArrayToHexStr(byte[] data)
        {
            var capacity = data.Length * 2;
            var sb = new StringBuilder(capacity);
            if (data != null)
            {
                for (var i = 0; i < data.Length; i++)
                {
                    sb.Append(data[i].ToString("X2"));
                }
            }
            return sb.ToString();
        }

        #endregion

        #region  数据转换

        /// <summary>
        ///     将16进制转化为十进制
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string realValue(string s)
        {
            var z = Convert.ToInt32(s, 16);
            var value = z.ToString();
            return value;
        }

        #endregion

        #region 露点值转换

        /// <summary>
        ///     露点值转换
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static decimal ldValue(string s)
        {
            var z = Convert.ToInt32(s);
            //double t1 = (Double.Parse(z.ToString()) - 800) * 80 / 3200 - 60;
            var t1 = (Double.Parse(z.ToString()) - 800) * 100 / 3200 - 80;
            var t2 = Double.Parse(t1.ToString("f2"));
            ////if (t2 < 0) t2 = 0;
            //string value = t2.ToString();
            //return value;
            return Convert.ToDecimal(t2);
        }

        #endregion

        #region 温度转换

        /// <summary>
        ///     温度转换
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static decimal ttValue(string s)
        {
            //int x = Convert.ToInt32(ss);
            //double t1 = (double.Parse(x.ToString()) - 800) * 70 / 3200 -10;
            var x = Convert.ToDouble(s);
            var t1 = (x - 800.0) * 200 / 3200.0 - 50;
            var t2 = Double.Parse(t1.ToString("f2"));
            if (t2 < 0) t2 = 0;
            //string value = t2.ToString();
            //return value;
            return Convert.ToDecimal(t2);
        }

        #endregion

        #region 压力转换

        /// <summary>
        ///     压力转换
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static decimal ylValue(string s)
        {
            var z = Convert.ToInt32(s);
            var t1 = (Double.Parse(z.ToString()) - 800) * 10 / 3200;
            //if (t1 > 50000)
            //{
            //    t1 = t1 - 65536;
            //}
            var t2 = Double.Parse(t1.ToString("f2"));
            if (t2 < 0) t2 = 0;
            //string value = t2.ToString();
            //return value;
            return Convert.ToDecimal(t2);
        }

        #endregion

        #region 除湿机数据处理

        /// <summary>
        ///     除湿机数据处理
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static decimal csjValue(string s)
        {
            //int x = Convert.ToInt32(ss);
            //double t1 = (double.Parse(x.ToString()) - 800) * 70 / 3200 -10;
            var x = Convert.ToDouble(s);
            if (x > 50000)
            {
                x = x - 65536;
            }
            var t1 = x * 0.1;
            var t2 = Double.Parse(t1.ToString("f2"));
            //if (t2 < 0) t2 = 0;
            //string value = t2.ToString();
            //return value;
            return Convert.ToDecimal(t2);
        }

        //public static string csjtValue(string s)
        //{
        //    //int x = Convert.ToInt32(ss);
        //    //double t1 = (double.Parse(x.ToString()) - 800) * 70 / 3200 -10;
        //    double x = Convert.ToDouble(s);
        //    double t1 = (x - 6400.0) * 100.0 / 25600.0 ;
        //    double t2 = double.Parse(t1.ToString("f2"));
        //    if (t2 < 0) t2 = 0;
        //    string value = t2.ToString();
        //    return value;
        //}
        //public static string csjrhValue(string s)
        //{
        //    //int x = Convert.ToInt32(ss);
        //    //double t1 = (double.Parse(x.ToString()) - 800) * 70 / 3200 -10;
        //    double x = Convert.ToDouble(s);
        //    double t1 = (x - 6400.0) * 200.0 / 25600.0 ;
        //    double t2 = double.Parse(t1.ToString("f2"));
        //    if (t2 < 0) t2 = 0;
        //    string value = t2.ToString();
        //    return value;
        //}
        //public static string csjldValue(string s)
        //{
        //    //int x = Convert.ToInt32(ss);
        //    //double t1 = (double.Parse(x.ToString()) - 800) * 70 / 3200 -10;
        //    double x = Convert.ToDouble(s);
        //    double t1 = (x - 6400.0) * 200.0 / 25600.0;
        //    double t2 = double.Parse(t1.ToString("f2"));
        //    if (t2 < 0) t2 = 0;
        //    string value = t2.ToString();
        //    return value;
        //}
        //public static string csjylValue(string s)
        //{
        //    //int x = Convert.ToInt32(ss);
        //    //double t1 = (double.Parse(x.ToString()) - 800) * 70 / 3200 -10;
        //    double x = Convert.ToDouble(s);
        //    double t1 = (x - 6400.0) * 200.0 / 25600.0;
        //    double t2 = double.Parse(t1.ToString("f2"));
        //    if (t2 < 0) t2 = 0;
        //    string value = t2.ToString();
        //    return value;
        //}

        #endregion

        #region 弃用

        /// <summary>
        ///     获取单个位号转化后的值
        /// </summary>
        /// <param name="formule"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static decimal GetOneData(string formule, int index, string[] arr)
        {
            decimal dValue = 0;
            if (string.IsNullOrEmpty(formule))
            {
                return dValue;
            }

            switch (formule)
            {
                #region 获取转化后的值

                case "0":
                    dValue = decimal.Parse(arr[index]);
                    break; //直接读取和存储原值，无需转换
                case "1":
                    dValue = rhValue(arr[index]);
                    break; //温湿度传感器湿度转换
                case "2":
                    dValue = tValue(arr[index]);
                    break; //温湿度传感器温度转换
                case "3":
                    dValue = ttValue(arr[index]);
                    break; //温度传感器温度转换
                case "4":
                    dValue = ldValue(arr[index]);
                    break; //露点仪数据转换
                case "5":
                    dValue = ylValue(arr[index]);
                    break; //压力传感器压力值转换
                case "6":
                    dValue = csjValue(arr[index]);
                    break; //除湿机数据转换
                case "7":
                    dValue = kyjDl(arr[index]);
                    break;
                case "8":
                    dValue = gzjDl(arr[index]);
                    break;
                case "9":
                    dValue = zdjDl(arr[index]);
                    break;
                case "10":
                    dValue = decimal.Parse(arr[index]);
                    break;
                case "13":
                    dValue = kyjNh2(arr[index]) + kyjNh1(arr[index - 1]);
                    break;
                case "14":
                    dValue = gzjNh2(arr[index]) + gzjNh1(arr[index - 1]);
                    break;
                case "15":
                    dValue = zdjNh2(arr[index]) + zdjNh1(arr[index - 1]);
                    break;
                case "16":
                    dValue = kyjGl(arr[index]);
                    break;
                case "17":
                    dValue = gzjGl(arr[index]);
                    break;
                case "18":
                    dValue = zdjGl(arr[index]);
                    break;
                default:
                    break;

                #endregion
            }
            return dValue;
        }

        #endregion

        #region 获取单个位号转化后的值2

        /// <summary>
        ///     获取单个位号转化后的值
        /// </summary>
        /// <param name="formule"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double GetOneData_Jabil(M_BS_TagDefine model, string[] arr)
        {
            double dValue = 0;
            var index = 0;
            index = model.DataIndex + 1;
            //dataindex<=0会造成求用电量时错误
            if (model == null || index < 0)
            {
                return dValue;
            }
            if (string.IsNullOrEmpty(arr[index]))
            {
                return dValue;
            }
            switch (model.Formule)
            {
                #region 获取转化后的值

                case "1000":

                    break; //直接读取和存储原值，无需转换
                default:

                    dValue = Convert.ToDouble(arr[index]) + Convert.ToDouble(arr[index - 1]);
                    break;

                #endregion
            }
            if (model.CompensateRatio > 0)
            {
                dValue = dValue * Convert.ToDouble(model.CompensateRatio.ToString());
            }
            return dValue;
        }

        #endregion

        #region 校验

        public static byte[] CRC16_C(byte[] data)
        {
            byte CRC16Lo;
            byte CRC16Hi; //CRC寄存器 
            byte CL;
            byte CH; //多项式码&HA001 
            byte SaveHi;
            byte SaveLo;
            byte[] tmpData;
            int Flag;
            CRC16Lo = 0xFF;
            CRC16Hi = 0xFF;
            CL = 0x01;
            CH = 0xA0;
            tmpData = data;
            for (var i = 0; i < tmpData.Length; i++)
            {
                CRC16Lo = (byte)(CRC16Lo ^ tmpData[i]); //每一个数据与CRC寄存器进行异或 
                for (Flag = 0; Flag <= 7; Flag++)
                {
                    SaveHi = CRC16Hi;
                    SaveLo = CRC16Lo;
                    CRC16Hi = (byte)(CRC16Hi >> 1); //高位右移一位 
                    CRC16Lo = (byte)(CRC16Lo >> 1); //低位右移一位 
                    if ((SaveHi & 0x01) == 0x01) //如果高位字节最后一位为1 
                    {
                        CRC16Lo = (byte)(CRC16Lo | 0x80); //则低位字节右移后前面补1 
                    } //否则自动补0 
                    if ((SaveLo & 0x01) == 0x01) //如果LSB为1，则与多项式码进行异或 
                    {
                        CRC16Hi = (byte)(CRC16Hi ^ CH);
                        CRC16Lo = (byte)(CRC16Lo ^ CL);
                    }
                }
            }
            var ReturnData = new byte[2];
            ReturnData[1] = CRC16Hi; //CRC 高位
            ReturnData[0] = CRC16Lo; //CRC 低位
            return ReturnData;
        }

        #endregion

        #region 温湿度转换

        /// <summary>
        ///     温度
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static decimal tValue(string s)
        {
            //int x = Convert.ToInt32(ss);
            //double t1 = (double.Parse(x.ToString()) - 800) * 70 / 3200 -10;
            var x = Convert.ToDouble(s);
            //double t1 = (x - 800.0) * 70 / 3200.0 - 10;
            var t1 = (x - 800.0) * 70 / 3200.0 - 13;
            var t2 = Double.Parse(t1.ToString("f2"));
            if (t2 < 0) t2 = 0;
            //string value = t2.ToString();
            //return value;
            return Convert.ToDecimal(t2);

            //int z = Convert.ToInt32(s);
            //double t1 = (Double.Parse(z.ToString()) - 800) * 100 / 3200-10;
            ////double t1 = (Double.Parse(z.ToString())) * 100 / 3200 - 25;
            //double t2 = Double.Parse(t1.ToString("f2"));
            //if (t2 < 0) t2 = 0;
            ////string value = t2.ToString();
            ////return value;
            //return Convert.ToDecimal(t2);
        }


        /// <summary>
        ///     湿度
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static decimal rhValue(string s)
        {
            var z = Convert.ToInt32(s);
            //double t1 = (Double.Parse(z.ToString()) - 800) * 100 / 3200;
            //double t1 = (Double.Parse(z.ToString())) * 100 / 3200 - 25;
            var t1 = (Double.Parse(z.ToString())) * 100 / 3200 - 25 + 9;
            var t2 = Double.Parse(t1.ToString("f2"));
            if (t2 < 0) t2 = 0;
            //string value = t2.ToString();
            //return value;
            return Convert.ToDecimal(t2);

            ////int x = Convert.ToInt32(ss);
            ////double t1 = (double.Parse(x.ToString()) - 800) * 70 / 3200 -10;
            //double x = Convert.ToDouble(s);
            //double t1 = (x - 800.0) * 70 / 3200.0 - 10;
            //double t2 = Double.Parse(t1.ToString("f2"));
            //if (t2 < 0) t2 = 0;
            ////string value = t2.ToString();
            ////return value;
            //return Convert.ToDecimal(t2);
        }

        #endregion

        #region 空压机、热泵电表数据处理

        /// <summary>
        ///     空压机、热泵电表数据处理
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static decimal kyjDl(string s) //空压机电流公式
        {
            var x = Convert.ToDouble(s);
            var t1 = x * 0.1;
            var t2 = Double.Parse(t1.ToString("f2"));
            if (t2 < 0) t2 = 0;
            //string value = t2.ToString();
            //return value;
            return Convert.ToDecimal(t2);
        }

        public static decimal kyjGl(string s) //空压机总有功功率公式
        {
            var x = Convert.ToDouble(s);
            var t1 = x * 0.01;
            var t2 = Double.Parse(t1.ToString("f2"));
            if (t2 < 0) t2 = 0;
            //string value = t2.ToString();
            //return value;
            return Convert.ToDecimal(t2);
        }

        public static decimal kyjNh1(string s) //空压机能耗值
        {
            var x = Convert.ToDouble(s);
            var t1 = x * 65536 * 0.1;
            var t2 = Double.Parse(t1.ToString("f2"));
            //string value = t2.ToString();
            //return value;
            return Convert.ToDecimal(t2);
        }

        public static decimal kyjNh2(string s) //空压机能耗值
        {
            var x = Convert.ToDouble(s);
            if (x < 0)
            {
                x = 65536 + x;
            }
            //else ;
            var t1 = x * 0.1;
            var t2 = Double.Parse(t1.ToString("f2"));
            //string value = t2.ToString();
            //return value;
            return Convert.ToDecimal(t2);
        }

        #endregion

        #region 冷干机、吸干机电表数据处理

        /// <summary>
        ///     干燥机机电流公式
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static decimal gzjDl(string s) //干燥机机电流公式
        {
            var x = Convert.ToDouble(s);
            var t1 = x * 0.008;
            var t2 = Double.Parse(t1.ToString("f2"));
            if (t2 < 0) t2 = 0;
            //string value = t2.ToString();
            //return value;
            return Convert.ToDecimal(t2);
        }

        /// <summary>
        ///     干燥机总有功功率公式
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static decimal gzjGl(string s) //干燥机总有功功率公式
        {
            var x = Convert.ToDouble(s);
            var t1 = x * 0.008;
            var t2 = Double.Parse(t1.ToString("f2"));
            if (t2 < 0) t2 = 0;
            //string value = t2.ToString();
            //return value;
            return Convert.ToDecimal(t2);
        }

        /// <summary>
        ///     干燥机能耗值（高字）
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static decimal gzjNh1(string s) //干燥机能耗值（高字）
        {
            var x = Convert.ToDouble(s);
            if (x < 0) x = 65536 + x;
            //else ;
            var t1 = x * 65536 * 0.008;
            var t2 = Double.Parse(t1.ToString("f2"));
            //string value = t2.ToString();
            //return value;
            return Convert.ToDecimal(t2);
        }

        /// <summary>
        ///     干燥机能耗值（低字）
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static decimal gzjNh2(string s) //干燥机能耗值（低字）
        {
            var x = Convert.ToDouble(s);
            if (x < 0)
            {
                x = 65536 + x;
            }
            //else x = x;
            var t1 = x * 0.008;
            var t2 = Double.Parse(t1.ToString("f2"));
            //string value = t2.ToString();
            //return value;
            return Convert.ToDecimal(t2);
        }

        #endregion

        #region 制氮机电表数据处理

        /// <summary>
        ///     干燥机机电流公式
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static decimal zdjDl(string s) //干燥机机电流公式
        {
            var x = Convert.ToDouble(s);
            var t1 = x * 0.02;
            var t2 = Double.Parse(t1.ToString("f2"));
            if (t2 < 0) t2 = 0;
            //string value = t2.ToString();
            //return value;
            return Convert.ToDecimal(t2);
        }

        /// <summary>
        ///     干燥机总有功功率公式
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static decimal zdjGl(string s) //干燥机总有功功率公式
        {
            var x = Convert.ToDouble(s);
            var t1 = x * 0.02;
            var t2 = Double.Parse(t1.ToString("f2"));
            if (t2 < 0) t2 = 0;
            //string value = t2.ToString();
            //return value;
            return Convert.ToDecimal(t2);
        }

        /// <summary>
        ///     干燥机能耗值（高字）
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static decimal zdjNh1(string s) //干燥机能耗值（高字）
        {
            var x = Convert.ToDouble(s);
            if (x < 0) x = 65536 + x;
            //else x = x;
            var t1 = x * 65536 * 0.02;
            var t2 = Double.Parse(t1.ToString("f2"));
            //string value = t2.ToString();
            //return value;
            return Convert.ToDecimal(t2);
        }

        /// <summary>
        ///     干燥机能耗值（低字）
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static decimal zdjNh2(string s) //干燥机能耗值（低字）
        {
            var x = Convert.ToDouble(s);
            if (x < 0)
            {
                x = 65536 + x;
            }
            //else x = x;
            var t1 = x * 0.02;
            var t2 = Double.Parse(t1.ToString("f2"));
            //string value = t2.ToString();
            //return value;
            return Convert.ToDecimal(t2);
        }

        #endregion

        #region 将耐奇的数据转化为RealData格式
        /// <summary>
        /// 将耐奇的数据转化为RealData格式
        /// </summary>
        /// <param name="lstNQ"></param>
        /// <param name="lstTags"></param>
        /// <returns></returns>
        public static DataTable FormateNQ2BS(List<M_NQ_Monitordata> lstNQ, List<M_BS_TagDefine> lstTags)
        {
            DataTable dt = null;
            if (lstNQ==null)
            {
                return dt;
            }
            Collection.Helper.DataTableHelper.CreateDataTable(ref dt, "T_BS_Realdata");
            //List<M_BS_Realdata> lstRs = new List<M_BS_Realdata>();
            foreach (var item in lstNQ)
            {
                var lstEquip = (from li in lstTags where li.TagAddr == item.DeviceID.ToString() select li).ToList();
                PropertyInfo[] properties = item.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
                foreach (var tag in lstEquip)
                {
                    var p = (from li in properties where li.Name == tag.TagCode select li).FirstOrDefault();

                    if (p==null||p.GetType()==typeof(DateTime))
                    {
                        continue;
                    }
                    var obj = p.GetValue(item, null);
                    decimal d = 0;
                    DataRow dr = dt.NewRow();
                    dr["TagID"] = tag.ID;
                    dr["RealTime"] = item.InsertTime;
                    if (obj != null)
                    {
                        if (decimal.TryParse(obj.ToString(), out d))
                        {

                        }
                        dr["RealValue"] = d;
                        dt.Rows.Add(dr);
                    }
                }

            }
            return dt;
        }
        #endregion
    }
}
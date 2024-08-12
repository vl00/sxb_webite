using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace ProductManagement.Framework.Foundation
{
    public class iSchool
    {
        public string iSchoolServer { get; set; }
    }
    //public class SQLHelper
    //{
    //    private string ConnStr;
    //    private iSchool _iSchool;
    //    public SQLHelper(IOptions<iSchool> set)
    //    {
    //        _iSchool = set.Value;
    //        ConnStr = _iSchool.iSchoolServer;
    //    }

    //    public enum ExcuteType { DataTable, NonQuery, Scalar };

    //    #region 基本查询，返回数据表
    //    public DataTable ExcuteDataTable(string strSelectCmd, params SqlParameter[] paras)
    //    {
    //        using (SqlConnection conn = new SqlConnection(ConnStr))
    //        {
    //            SqlDataAdapter da = new SqlDataAdapter(strSelectCmd, conn);
    //            if (paras != null)
    //            {
    //                da.SelectCommand.Parameters.AddRange(paras);
    //            }
    //            DataTable dt = new DataTable();
    //            da.Fill(dt);
    //            da.SelectCommand.Parameters.Clear();
    //            return dt;
    //        }
    //    }
    //    #endregion

    //    #region 基本非查询 
    //    public int ExcuteNonQuery(string strCmd, params SqlParameter[] paras)
    //    {
    //        using (SqlConnection conn = new SqlConnection(ConnStr))
    //        {
    //            SqlCommand cmd = new SqlCommand(strCmd, conn);
    //            cmd.Parameters.AddRange(paras);
    //            conn.Open();
    //            int result = cmd.ExecuteNonQuery();
    //            cmd.Parameters.Clear();
    //            conn.Close();
    //            conn.Dispose();
    //            return result;
    //        }
    //    }
    //    #endregion

    //    #region 基本查询，返回查询结果集中的第一行第一列(object)
    //    public object ExcuteScalar(string strSelectCmd, params SqlParameter[] paras)
    //    {
    //        using (SqlConnection conn = new SqlConnection(ConnStr))
    //        {
    //            SqlCommand cmd = new SqlCommand(strSelectCmd, conn);
    //            if (paras != null)
    //            {
    //                cmd.Parameters.AddRange(paras);
    //            }
    //            conn.Open();
    //            object result = cmd.ExecuteScalar();
    //            cmd.Parameters.Clear();
    //            conn.Close();
    //            conn.Dispose();
    //            return result == DBNull.Value ? null : result;
    //        }
    //    }
    //    #endregion

    //    #region 基本查询，返回查询结果集中的第一行第一列<T>
    //    public T ExcuteScalar<T>(string strSelectCmd, params SqlParameter[] paras)
    //    {
    //        using (SqlConnection conn = new SqlConnection(ConnStr))
    //        {
    //            SqlCommand cmd = new SqlCommand(strSelectCmd, conn);
    //            cmd.Parameters.AddRange(paras);
    //            conn.Open();
    //            object o = cmd.ExecuteScalar();
    //            cmd.Parameters.Clear();
    //            conn.Close();
    //            conn.Dispose();
    //            return o == null ? default(T) : (T)Convert.ChangeType(o, typeof(T));
    //        }
    //    }

    //    #endregion

    //    #region 执行多条SQL语句
    //    public class MultipleSqlItem
    //    {
    //        public string CmdString { get; set; }
    //        public SqlParameter[] Paras { get; set; }
    //        public ExcuteType ExcuteType { get; set; }

    //        public MultipleSqlItem(string cmdString, SqlParameter[] paras, ExcuteType excuteType)
    //        {
    //            CmdString = cmdString;
    //            Paras = paras;
    //            ExcuteType = excuteType;
    //        }
    //    }
    //    public object[] ExecuteMultipleSql(MultipleSqlItem[] multipleSqlList)
    //    {
    //        object[] results = new object[multipleSqlList.Length];
    //        using (SqlConnection conn = new SqlConnection(ConnStr))
    //        {
    //            conn.Open();
    //            for (int i = 0; i < multipleSqlList.Length; i++)
    //            {
    //                //File.AppendAllText(@"D:\error\t.txt", multipleSqlList[i].CmdString+"\r\n\r\n",Encoding.UTF8);

    //                SqlCommand cmd = new SqlCommand(multipleSqlList[i].CmdString, conn);
    //                if (multipleSqlList[i].Paras != null)
    //                    cmd.Parameters.AddRange(multipleSqlList[i].Paras);
    //                switch (multipleSqlList[i].ExcuteType)
    //                {
    //                    case ExcuteType.DataTable:
    //                        SqlDataAdapter da = new SqlDataAdapter(cmd);
    //                        DataTable dt = new DataTable();
    //                        da.Fill(dt);
    //                        results[i] = dt;
    //                        break;
    //                    case ExcuteType.NonQuery:
    //                        results[i] = cmd.ExecuteNonQuery();
    //                        break;
    //                    case ExcuteType.Scalar:
    //                        results[i] = cmd.ExecuteScalar();
    //                        break;
    //                }
    //                cmd.Parameters.Clear();
    //            }
    //            conn.Close();
    //            conn.Dispose();
    //        }
    //        return results;
    //    }
    //    #endregion

    //    #region null类型转DbNull
    //    public static object NullToDbValue(object value)
    //    {
    //        return value ?? DBNull.Value;
    //    }
    //    #endregion

    //    #region DbNull转null
    //    public static T DBNullToNull<T>(object obj)
    //    {
    //        return obj == DBNull.Value ? default(T) : (T)obj;
    //    }
    //    #endregion

    //    /// <summary>
    //    /// 存储过程执行
    //    /// </summary>
    //    /// <param name="strCmd">存储过程名称</param>
    //    /// <param name="para">参数（如果有参数为出参，需指定参数类型）</param>
    //    /// <param name="OutPutValue">出参</param>
    //    public DataTable ExecProc<T>(string ProcName, SqlParameter[] para, out List<object> OutPutValue) where T : new()
    //    {

    //        OutPutValue = new List<object>();
    //        //拼接存储过程指令
    //        string strCmd = "exec " + ProcName + GetSqlParameterByStr(para);

    //        List<T> TModelList = new List<T>();
    //        DataTable dt = null;
    //        using (SqlConnection conn = new SqlConnection(ConnStr))
    //        {
    //            SqlCommand cmd = conn.CreateCommand();
    //            //cmd.CommandType = CommandType.StoredProcedure;
    //            cmd.CommandText = strCmd;
    //            cmd.Parameters.AddRange(para);

    //            SqlDataAdapter da = new SqlDataAdapter(cmd);
    //            dt = new DataTable();
    //            da.Fill(dt);

    //            //存储过程表结果
    //            //TModelList = DataTableToList<T>(dt);

    //            //取出存储过程返回值
    //            foreach (SqlParameter paramenter in cmd.Parameters)
    //            {
    //                if (paramenter.Direction == ParameterDirection.Output)
    //                {
    //                    OutPutValue.Add(paramenter.Value);
    //                }
    //            }
    //        }
    //        return dt;
    //    }

    //    /// <summary>
    //    /// 动态参数封装（包括存储过程output指令）
    //    /// </summary>
    //    /// <param name="sqlParameters"></param>
    //    /// <returns></returns>
    //    public string GetSqlParameterByStr(SqlParameter[] sqlParameters)
    //    {
    //        string cmd = " ";
    //        foreach (SqlParameter para in sqlParameters)
    //        {
    //            if (para.Direction == ParameterDirection.Output)
    //            {
    //                cmd += para.ParameterName + " output" + ",";
    //            }
    //            else
    //            {
    //                cmd += para.ParameterName + ",";
    //            }
    //        }
    //        return cmd.Remove(cmd.Length - 1, 1);
    //    }
    //}
}

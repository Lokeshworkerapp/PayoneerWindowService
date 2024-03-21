using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using System.Dynamic;

namespace PayoneerWindowsService.BAL
{
    public class _Bal
    {
        public static string openConn()
        {
            string conn = ConfigurationManager.AppSettings["ConStr"].ToString();

            return conn;

        }

        public static string get_bdTimestamp()
        {
            DateTime epochStart = new DateTime(1970, 01, 01, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan timeSpan = DateTime.UtcNow - epochStart;

            return Convert.ToUInt64(timeSpan.TotalSeconds).ToString();

        }
        
        public static int commonIUD(string sp_name_, object paramObj)
        {

            int isUpdated = 0;
            SqlConnection con = new SqlConnection(openConn());
            SqlCommand cmd = new SqlCommand(sp_name_, con);
            cmd.CommandType = CommandType.StoredProcedure;


            foreach (var param_ in paramObj.GetType().GetProperties())
            {


                if (param_.PropertyType.FullName.Contains("System."))
                {

                    cmd.Parameters.AddWithValue("@" + param_.Name, param_.GetValue(paramObj));

                }
                else
                {

                    foreach (var item_ in (param_.GetValue(paramObj)).GetType().GetProperties())
                    {

                        if (item_.PropertyType.FullName.Contains("System."))
                        {

                            cmd.Parameters.AddWithValue("@" + (param_.Name + item_.Name), item_.GetValue((param_.GetValue(paramObj))));

                        }
                        else
                        {

                            foreach (var item1_ in item_.GetValue((param_.GetValue(paramObj))).GetType().GetProperties())
                            {


                                if (item1_.PropertyType.FullName.Contains("System."))
                                {

                                    cmd.Parameters.AddWithValue("@" + (param_.Name + item_.Name + item1_.Name), item1_.GetValue(item_.GetValue((param_.GetValue(paramObj)))));
                                }
                                else
                                {

                                    foreach (var item2_ in item1_.GetValue(item_.GetValue((param_.GetValue(paramObj)))).GetType().GetProperties())
                                    {

                                        cmd.Parameters.AddWithValue("@" + (param_.Name + item_.Name + item1_.Name + item2_.Name),
                                            item2_.GetValue(item1_.GetValue(item_.GetValue((param_.GetValue(paramObj))))));

                                    }

                                }
                            }
                        }
                    }

                }

            }


            con.Open();

            isUpdated = cmd.ExecuteNonQuery();

            con.Close();

            return isUpdated;

        }

        public static DataSet getTblData(string usp)
        {
            DataSet sql_ds = null;

            using (SqlConnection sql_con = new SqlConnection(openConn()))
            {

                using (SqlCommand sql_cmd = new SqlCommand())
                {
                    sql_cmd.Connection = sql_con;

                    sql_cmd.CommandType = CommandType.StoredProcedure;

                    sql_cmd.CommandText = usp;

                    SqlDataAdapter sql_da = new SqlDataAdapter();

                    sql_da.SelectCommand = sql_cmd;

                    sql_ds = new DataSet();

                    sql_da.Fill(sql_ds);

                }

            }

            return sql_ds;
        }

        public static DataSet getStatus(string usp, int statusCode)
        {
            DataSet sql_ds = null;

            using (SqlConnection sql_con = new SqlConnection(openConn()))
            {

                using (SqlCommand sql_cmd = new SqlCommand())
                {
                    sql_cmd.Connection = sql_con;

                    sql_cmd.CommandType = CommandType.StoredProcedure;

                    sql_cmd.CommandText = usp;

                    sql_cmd.Parameters.AddWithValue("@statusCode", statusCode);

                    SqlDataAdapter sql_da = new SqlDataAdapter();

                    sql_da.SelectCommand = sql_cmd;

                    sql_ds = new DataSet();

                    sql_da.Fill(sql_ds);

                }

            }


            return sql_ds;
        }

        public static DataSet getFyorinBanID(string benref_ID_)
        {
            DataSet sql_ds = null;

            using (SqlConnection sql_con = new SqlConnection(openConn()))
            {

                using (SqlCommand sql_cmd = new SqlCommand())
                {
                    sql_cmd.Connection = sql_con;

                    sql_cmd.CommandType = CommandType.StoredProcedure;

                    sql_cmd.CommandText = "getFyorinBanID";

                    sql_cmd.Parameters.AddWithValue("@benerefid", benref_ID_);

                    SqlDataAdapter sql_da = new SqlDataAdapter();

                    sql_da.SelectCommand = sql_cmd;

                    sql_ds = new DataSet();

                    sql_da.Fill(sql_ds);

                }

            }


            return sql_ds;
        }


        //=================By Sanat===============//

        public DataTable getTbl_Records(string usp, dynamic paramObj)
        {
            DataSet sql_ds = null;

            using (SqlConnection sql_con = new SqlConnection(openConn()))
            {
                using (SqlCommand sql_cmd = new SqlCommand())
                {
                    sql_cmd.Connection = sql_con;
                    sql_cmd.CommandType = CommandType.StoredProcedure;
                    sql_cmd.CommandText = usp;

                    // Create parameters from the properties of the paramObj object
                    foreach (var property in paramObj.GetType().GetProperties())
                    {
                        SqlParameter parameter = new SqlParameter("@" + property.Name, property.GetValue(paramObj) ?? DBNull.Value);
                        sql_cmd.Parameters.Add(parameter);
                    }

                    SqlDataAdapter sql_da = new SqlDataAdapter();
                    sql_da.SelectCommand = sql_cmd;
                    sql_ds = new DataSet();
                    sql_da.Fill(sql_ds);
                }
            }

            return sql_ds.Tables[0];
        }



    }
}

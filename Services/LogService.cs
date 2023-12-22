using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace WebApplication.Services
{
    public class LogService
    {
        string connstr = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
        public string AddLog(string request, string response, string action, string level, string creator)
        {
            string result = string.Empty;
            try
            {
                using (SqlConnection conn = new SqlConnection(connstr))
                {
                    conn.Open();

                    // add
                    SqlCommand command = new SqlCommand();
                    command.CommandText = "dbo.sp_add_log";
                    command.Connection = conn;
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@request", SqlDbType.NVarChar).Value = request;
                    command.Parameters.Add("@response", SqlDbType.NVarChar).Value = response;
                    command.Parameters.Add("@action", SqlDbType.NVarChar).Value = action;
                    command.Parameters.Add("@eventLevel", SqlDbType.NVarChar).Value = level;
                    command.Parameters.Add("@creator", SqlDbType.NVarChar).Value = creator;

                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
                return result;
            }

            return result;
        }
    }
}
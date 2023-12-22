using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace WebApplication.Services
{
    public class BrandService
    {
        string connstr = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
        public string getBrand(string email)
        {
            string brand = string.Empty;

            string connstr = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            using (SqlConnection conn = new SqlConnection(connstr))
            {
                string inputString = "select b.brand from [CarShop].[dbo].[AspNetUsers] a inner join[CarShop].[dbo].[Profiles] b on a.Id = b.Id where a.Email = '" + email + "' ";

                conn.Open();

                SqlCommand command = new SqlCommand(inputString, conn);
                DataSet ds = new DataSet();
                command.CommandText = inputString;
                SqlDataAdapter da = new SqlDataAdapter(command);
                da.Fill(ds);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    brand = ds.Tables[0].Rows[0]["brand"].ToString();
                }
            }
            return brand;
        }
    }
}
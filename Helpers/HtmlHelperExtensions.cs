using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Controllers;
using WebApplication.Models;


namespace WebApplication
{
    public static class HtmlHelperExtensions
    {

        public static MenuViewModel getMenu(this HtmlHelper html, string userName)
        {
            MenuViewModel model = new MenuViewModel();
            model.Menu = new List<menuItem>();
            string result = string.Empty;
            string connstr = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            using (SqlConnection conn = new SqlConnection(connstr))
            {
                string inputString = "select distinct d.* from aspnetusers a inner join UserRoles b on a.NameIdentifier = b.userName inner join RoleMenus c on b.userRole = c.roleId inner join Menus d on c.menuId = d.seq where a.NameIdentifier = '" + userName + "' and (d.isRoot='True' or d.isLink='True') order by d.orderSeq ";

                conn.Open();

                SqlCommand command = new SqlCommand(inputString, conn);
                DataSet ds = new DataSet();
                command.CommandText = inputString;
                SqlDataAdapter da = new SqlDataAdapter(command);
                da.Fill(ds);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    menuItem tmp = new menuItem();

                    tmp.id = ds.Tables[0].Rows[i]["seq"].ToString();
                    tmp.menuName = ds.Tables[0].Rows[i]["menuName"].ToString();
                    tmp.isRoot = ds.Tables[0].Rows[i]["isRoot"].ToString();
                    tmp.isLink = ds.Tables[0].Rows[i]["isLink"].ToString();
                    tmp.parent = ds.Tables[0].Rows[i]["parent"].ToString();
                    tmp.icon = ds.Tables[0].Rows[i]["icon"].ToString();

                    if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["isLink"].ToString()))
                    {
                        tmp.controller = ds.Tables[0].Rows[i]["controller"].ToString();
                        tmp.action = ds.Tables[0].Rows[i]["action"].ToString();
                    }

                    if (tmp.isRoot == "True")
                    {
                        tmp.Menu = new List<menuItem>();
                        model.Menu.Add(tmp);
                    }
                    else
                    {
                        menuItem parent = model.Menu.Where(x => x.id == tmp.parent).FirstOrDefault();
                        if (parent != null)
                        {
                            parent.Menu.Add(tmp);
                        }
                    }
                }
            }

            string sql = "select * from [UserRoles] where userName=@userName ";
            var dicValue = new Dictionary<string, string>();
            dicValue.Add("@userName", userName);

            string roleId = string.Empty;
             DataTable dt = APCommonFun.GetDataTable_MSSQL(sql, dicValue);
            if (dt.Rows.Count > 0)
            {
                roleId = dt.Rows[0]["userRole"].ToString();
            }

            List<string> removeList = new List<string>();
            for (int i = 0; i < model.Menu.Count; i++)
            {
                if (model.Menu[i].isRoot == "True")
                {
                    string sql2 = "SELECT  *  FROM [CarShop].[dbo].[RoleMenus] a inner join[CarShop].[dbo].[Menus]  b on a.menuId = b.seq where roleId = @roleId and b.parent = @parent ";

                    dicValue.Clear();
                    dicValue.Add("@roleId", roleId);
                    dicValue.Add("@parent", model.Menu[i].id);

                    DataTable dt2 = APCommonFun.GetDataTable_MSSQL(sql2, dicValue);
                    if (dt2.Rows.Count == 0)
                    {
                        removeList.Add(model.Menu[i].id);
                    }
                }
            }

            foreach (var iten in removeList)
            {
                model.Menu.RemoveAll(x => x.id == iten);
            }

            return model;
        }

        public static string getBrand(this HtmlHelper html, string userName)
        {
            string brand = string.Empty;

            string connstr = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            using (SqlConnection conn = new SqlConnection(connstr))
            {
                string inputString = "SELECT a.brand FROM [Profiles] a inner join aspnetusers b on a.id = b.id where b.email= '" + userName + "' ";

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

        public static string getTitle(this HtmlHelper html, string email)
        {
            string brand = string.Empty;
            string title = string.Empty;
            string roleName = string.Empty;

            string connstr = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            using (SqlConnection conn = new SqlConnection(connstr))
            {
                string inputString = "select d.brand, c.[RoleName] from [AspNetUsers] a inner join[UserRoles] b on a.[NameIdentifier] = b.userName inner join[Roles] c on b.[userRole] = c.seq inner join Profiles d on a.id = d.id where a.email = '" + email + "' ";

                conn.Open();

                SqlCommand command = new SqlCommand(inputString, conn);
                DataSet ds = new DataSet();
                command.CommandText = inputString;
                SqlDataAdapter da = new SqlDataAdapter(command);
                da.Fill(ds);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    brand = ds.Tables[0].Rows[0]["brand"].ToString();
                    roleName = ds.Tables[0].Rows[0]["RoleName"].ToString();
                }
            }
            return roleName == "品牌管理人員" ? brand + roleName : roleName;
        }
    }
}
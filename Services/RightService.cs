using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using WebApplication.Controllers;
using WebApplication.Models;

namespace WebApplication.Services
{
    public class RightService
    {
        CarShopEntities carShopEntities = new CarShopEntities();
        public bool checkRight(string controller, string action, string email)
        {
            string menuId = string.Empty;
            string roleName = string.Empty;
            string rootId = string.Empty;
            bool result = false;

            string connstr = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            using (SqlConnection conn = new SqlConnection(connstr))
            {
                string inputString = "select * from [Menus] where controller = '" + controller + "' and action= '" + action + "' and isRoot=0 ";

                conn.Open();

                SqlCommand command = new SqlCommand(inputString, conn);
                DataSet ds = new DataSet();
                command.CommandText = inputString;
                SqlDataAdapter da = new SqlDataAdapter(command);
                da.Fill(ds);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    menuId = ds.Tables[0].Rows[0]["seq"].ToString();
                    rootId = ds.Tables[0].Rows[0]["parent"].ToString();
                }

                var user = carShopEntities.AspNetUsers.Where(x => x.Email == email).FirstOrDefault();              
                if (user != null)
                {
                    int role = carShopEntities.UserRoles.Where(x => x.userName == user.NameIdentifier).Select(x => x.userRole).FirstOrDefault() ?? 0;
                    List<string> roleMenu = carShopEntities.RoleMenus.Where(x => x.roleId == role).Select(x => x.menuId.ToString()).ToList();

                    if (roleMenu.Contains(menuId) && roleMenu.Contains(rootId))
                    {
                        result = true;
                    }
                }
            }
            return result;
        }
    }
}
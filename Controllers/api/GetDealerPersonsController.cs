using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using WebApplication.Extensions;
using WebApplication.ViewModels;

namespace WebApplication.Controllers.api
{
    [JwtAuthActionFilter]
    public class GetDealerPersonsController : BaseAPIController
    {
        /// <summary>
        /// 取得銷售顧問
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public object Post(Info_GetDealerPersonsModel Data)
        {
            string InputIsok = "Y";
            string ReturnErr = "";
            JArray newJa = new JArray();
            JArray totalJa = new JArray();
            string fetch_subStr = string.Empty;

            try
            {
                string brand = string.Empty;
                if (Data.brand != null)
                {
                    brand = APCommonFun.CDBNulltrim(Data.brand);
                }

                string dealerPerson_id = string.Empty;
                string dealerPerson_condition = " and 1=1 ";
                if (Data.dealerPerson_id != null)
                {
                    dealerPerson_id = APCommonFun.CDBNulltrim(Data.dealerPerson_id);
                    if (!string.IsNullOrEmpty(dealerPerson_id))
                    {
                        dealerPerson_condition = " and seq=@dealerPerson_id  ";
                    }
                }

                string stronghold = string.Empty;
                if (Data.stronghold != null)
                {
                    stronghold = APCommonFun.CDBNulltrim(Data.stronghold);
                }

                string dealer = string.Empty;
                if (Data.dealer != null)
                {
                    dealer = APCommonFun.CDBNulltrim(Data.dealer);
                }

                string page = string.Empty;
                int page_start = 0;
                int page_size = 0;
                if (Data.page != null && !string.IsNullOrEmpty(Data.page))
                {
                    page = APCommonFun.CDBNulltrim(Data.page);
                    if (!page.Contains(","))
                    {
                        ReturnErr = "執行動作錯誤-page 欄位格式錯誤";
                        APCommonFun.Error("[GetDealerPersonsController]90-" + ReturnErr);
                        return ReturnError(ReturnErr);
                    }
                    else
                    {
                        string[] page_data = page.Split(',');
                        fetch_subStr = "OFFSET " + ((Convert.ToInt32(page_data[0].ToString()) - 1) * Convert.ToInt32(page_data[1].ToString())).ToString() + " rows fetch first " + page_data[1].ToString() + " rows only ";
                        page_start = Convert.ToInt32(page_data[0].ToString());
                        page_size = Convert.ToInt32(page_data[1].ToString());
                    }
                }

                //第一步 : 先判斷有沒有必填未填寫，
                if (brand == "" || dealer == "" || stronghold == "") //必填                       
                {
                    if (brand == "")
                    {
                        InputIsok = "N";
                        ReturnErr = "執行動作錯誤-brand 為必填欄位";
                    }
                    else if (dealer == "")
                    {
                        InputIsok = "N";
                        ReturnErr = "執行動作錯誤-dealer 為必填欄位";
                    }
                    else if (stronghold == "")
                    {
                        InputIsok = "N";
                        ReturnErr = "執行動作錯誤-stronghold 為必填欄位";
                    }
                }

                //第二步 : 如果有必填未給值，回傳告知必填欄位要有資料
                if (InputIsok == "N")
                {
                    APCommonFun.Error("[GetDealerPersonsController]90-" + ReturnErr);
                    return ReturnError(ReturnErr);
                }

                string sql = "select * from DealerPersons where brand=@brand  and dealer=@dealer  and businessOffice=@stronghold  " + dealerPerson_condition + " ";
                string today = DateTime.Now.ToString("yyyy-MM-dd");
                sql += " order by createTime desc ";// + fetch_subStr;


                DataTable dt = new DataTable();
                if (!string.IsNullOrEmpty(dealerPerson_id))
                {
                    dt = APCommonFun.GetSafeDataTable_MSSQL(
                        sql,
                        new List<SqlParameter>
                        {
                            new SqlParameter("@brand", brand),
                            new SqlParameter("@dealer", dealer),
                            new SqlParameter("@stronghold", stronghold),
                            new SqlParameter("@dealerPerson_id", dealerPerson_id)
                        }
                    );
                }
                else
                {
                    dt = APCommonFun.GetSafeDataTable_MSSQL(
                            sql,
                            new List<SqlParameter>
                            {
                            new SqlParameter("@brand", brand),
                            new SqlParameter("@dealer", dealer),
                            new SqlParameter("@stronghold", stronghold)
                            }
                        );
                }
                //DataTable dt = APCommonFun.GetDataTable_MSSQL(sql);
                if (dt.Rows.Count > 0)
                {
                    if (string.IsNullOrEmpty(page))
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            string seq = APCommonFun.CDBNulltrim(dr["seq"].ToString());
                            string name = APCommonFun.CDBNulltrim(dr["name"].ToString());
                            string mobile = APCommonFun.CDBNulltrim(dr["mobile"].ToString());
                            string email = APCommonFun.CDBNulltrim(dr["email"].ToString());
                            string area = APCommonFun.CDBNulltrim(dr["area"].ToString());

                            JObject tmpJoLay01 = new JObject();
                            tmpJoLay01.Add(new JProperty("seq", seq));
                            tmpJoLay01.Add(new JProperty("name", name));
                            tmpJoLay01.Add(new JProperty("mobile", mobile));
                            tmpJoLay01.Add(new JProperty("area", area));
                            tmpJoLay01.Add(new JProperty("email", email));
                            tmpJoLay01.Add(new JProperty("dealer", dealer));
                            tmpJoLay01.Add(new JProperty("stronghole", stronghold));
                            newJa.Add(tmpJoLay01);
                            totalJa.Add(tmpJoLay01);
                        }
                    }
                    else
                    {
                        int i = 0;
                        foreach (DataRow dr in dt.Rows)
                        {
                            string seq = APCommonFun.CDBNulltrim(dr["seq"].ToString());
                            string name = APCommonFun.CDBNulltrim(dr["name"].ToString());
                            string mobile = APCommonFun.CDBNulltrim(dr["mobile"].ToString());
                            string email = APCommonFun.CDBNulltrim(dr["email"].ToString());
                            string area = APCommonFun.CDBNulltrim(dr["area"].ToString());

                            JObject tmpJoLay01 = new JObject();
                            tmpJoLay01.Add(new JProperty("seq", seq));
                            tmpJoLay01.Add(new JProperty("name", name));
                            tmpJoLay01.Add(new JProperty("mobile", mobile));
                            tmpJoLay01.Add(new JProperty("area", area));
                            tmpJoLay01.Add(new JProperty("email", email));
                            tmpJoLay01.Add(new JProperty("dealer", dealer));
                            tmpJoLay01.Add(new JProperty("stronghole", stronghold));
                            totalJa.Add(tmpJoLay01);
                            if ((i >= (page_start - 1) * page_size && i < (page_start - 1) * page_size + page_size))
                            {
                                newJa.Add(tmpJoLay01);
                            }
                            i++;
                        }
                    }
                }

                var data = new
                {
                    Count = dt.Rows.Count,
                    Data = newJa
                };

                return ReturnOK(data);
            }
            catch (Exception ex)
            {
                APCommonFun.Error("[GetDealerPersonsController]99：" + ex.ToString());
                return ReturnException();
            }
        }
    }
}

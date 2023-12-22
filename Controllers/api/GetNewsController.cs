using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
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
    public class GetNewsController : BaseAPIController
    {
        /// <summary>
        /// 取得最新消息列表
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public object Post(Info_NewsModel Data)
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

                string page = string.Empty;
                int page_start = 0;
                int page_size = 0;
                if (Data.page != null && !string.IsNullOrEmpty(Data.page))
                {
                    page = APCommonFun.CDBNulltrim(Data.page);
                    if (!page.Contains(","))
                    {
                        ReturnErr = "執行動作錯誤-page 欄位格式錯誤";
                        APCommonFun.Error("[GetNewsController]90-" + ReturnErr);
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
                if (brand == "") //必填                       
                {
                    InputIsok = "N";
                    ReturnErr = "執行動作錯誤-brand 為必填欄位";
                }

                //第二步 : 如果有必填未給值，回傳告知必填欄位要有資料
                if (InputIsok == "N")
                {
                    APCommonFun.Error("[GetNewsController]90-" + ReturnErr);
                    return ReturnError(ReturnErr);
                }

                string sql = "select * from News where brand=@brand and status=N'啟用' order by createTime desc ";
                string today = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                //sql += fetch_subStr;


                DataTable dt = APCommonFun.GetSafeDataTable_MSSQL(
                    sql,
                    new List<SqlParameter>
                    {
                        new SqlParameter("@brand", brand)
                    }
                );
                if (dt.Rows.Count > 0)
                {

                    if (string.IsNullOrEmpty(page))
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            string seq = APCommonFun.CDBNulltrim(dr["seq"].ToString());
                            string title = APCommonFun.CDBNulltrim(dr["title"].ToString());
                            //string bodyContent = APCommonFun.CDBNulltrim(dr["bodyContent"].ToString());
                            string showBody = APCommonFun.CDBNulltrim(dr["showBody"].ToString());
                            string publishRange = APCommonFun.CDBNulltrim(dr["publishRange"].ToString());
                            string createTime = APCommonFun.CDBNulltrim(dr["createTime"].ToString());
                            string picture = APCommonFun.CDBNulltrim(dr["picturePath"].ToString());

                            string[] publishRangeArray = publishRange.Split('~');
                            if (publishRangeArray.Length == 2)
                            {
                                string start = publishRangeArray[0];//.Split('/')[2].Trim() + "-" + publishRangeArray[0].Split('/')[0].Trim() + "-" + publishRangeArray[0].Split('/')[1].Trim();

                                string end = publishRangeArray[1];//.Split('/')[2].Trim() + "-" + publishRangeArray[1].Split('/')[0].Trim() + "-" + publishRangeArray[1].Split('/')[1].Trim();

                                if (string.Compare(today, start.TrimStart().TrimEnd()) >= 0 && string.Compare(today, end.TrimStart().TrimEnd()) <= 0)
                                {
                                    JObject tmpJoLay01 = new JObject();
                                    tmpJoLay01.Add(new JProperty("seq", seq));
                                    tmpJoLay01.Add(new JProperty("title", title));
                                    tmpJoLay01.Add(new JProperty("showBody", showBody));
                                    //tmpJoLay01.Add(new JProperty("bodyContent", bodyContent));
                                    tmpJoLay01.Add(new JProperty("createTime", start));
                                    if (!string.IsNullOrEmpty(picture))
                                    {
                                        tmpJoLay01.Add(new JProperty("picture", new Services.FileService().GetRealUrl(picture)));

                                    }
                                    else
                                    {
                                        tmpJoLay01.Add(new JProperty("picture", ""));
                                    }
                                    newJa.Add(tmpJoLay01);
                                    totalJa.Add(tmpJoLay01);
                                }
                            }
                        }
                    }
                    else
                    {
                        int i = 0;
                        //for (int i = (page_start - 1) * page_size; i < page_size; i++)
                        foreach (DataRow dr in dt.Rows)
                        {
                            string seq = APCommonFun.CDBNulltrim(dr["seq"].ToString());
                            string title = APCommonFun.CDBNulltrim(dr["title"].ToString());
                            //string bodyContent = APCommonFun.CDBNulltrim(dr["bodyContent"].ToString());
                            string showBody = APCommonFun.CDBNulltrim(dr["showBody"].ToString());
                            string publishRange = APCommonFun.CDBNulltrim(dr["publishRange"].ToString());
                            string createTime = APCommonFun.CDBNulltrim(dr["createTime"].ToString());
                            string picture = APCommonFun.CDBNulltrim(dr["picturePath"].ToString());

                            string[] publishRangeArray = publishRange.Split('~');
                            if (publishRangeArray.Length == 2)
                            {
                                string start = publishRangeArray[0];//.Split('/')[2].Trim() + "-" + publishRangeArray[0].Split('/')[0].Trim() + "-" + publishRangeArray[0].Split('/')[1].Trim();

                                string end = publishRangeArray[1];//.Split('/')[2].Trim() + "-" + publishRangeArray[1].Split('/')[0].Trim() + "-" + publishRangeArray[1].Split('/')[1].Trim();

                                if (string.Compare(today, start.TrimStart().TrimEnd()) >= 0 && string.Compare(today, end.TrimStart().TrimEnd()) <= 0)
                                {
                                    JObject tmpJoLay01 = new JObject();
                                    tmpJoLay01.Add(new JProperty("seq", seq));
                                    tmpJoLay01.Add(new JProperty("title", title));
                                    tmpJoLay01.Add(new JProperty("showBody", showBody));
                                    //tmpJoLay01.Add(new JProperty("bodyContent", bodyContent));
                                    tmpJoLay01.Add(new JProperty("createTime", start));
                                    if (!string.IsNullOrEmpty(picture))
                                    {
                                        tmpJoLay01.Add(new JProperty("picture", new Services.FileService().GetRealUrl(picture)));
                                    }
                                    else
                                    {
                                        tmpJoLay01.Add(new JProperty("picture", ""));
                                    }

                                    totalJa.Add(tmpJoLay01);
                                    if ((i >= (page_start - 1) * page_size && i < (page_start - 1) * page_size + page_size))
                                    {
                                        newJa.Add(tmpJoLay01);
                                    }
                                    i++;
                                }
                            }
                        }
                    }
                }

                var data = new
                {
                    Count = totalJa.Count,
                    Data = newJa
                };

                return ReturnOK(data);
            }
            catch (Exception ex)
            {
                APCommonFun.Error("[GetNewsController]99：" + ex.ToString());
                return ReturnException();
            }
        }
    }
}

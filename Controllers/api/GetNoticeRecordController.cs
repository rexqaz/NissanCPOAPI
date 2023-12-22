using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using WebApplication.Extensions;
using WebApplication.ViewModels;

namespace WebApplication.Controllers.api
{
    [JwtAuthActionFilter]
    public class GetNoticeRecordController : BaseAPIController
    {
        /// <summary>
        /// 取得關注車輛
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public object Post(Info_GetNoticeRecordModel Data)
        {
            try
            {
                string InputIsok = "Y";
                string ReturnErr = "";
                JArray newJa = new JArray();
                JArray totalJa = new JArray();
                string fetch_subStr = string.Empty;


                string user_id = string.Empty; //若無則全部回傳
                if (Data.user_id != null)
                {
                    user_id = APCommonFun.CDBNulltrim(Data.user_id);
                }

                string getCount = string.Empty; //若無則全部回傳
                if (Data.getCount != null)
                {
                    getCount = APCommonFun.CDBNulltrim(Data.getCount);
                }

                string brand = string.Empty; //若無則全部回傳
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
                        APCommonFun.Error("[GetNoticeRecordController]90-" + ReturnErr);
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
                if (user_id == "" || brand == "" || getCount == "") //必填                       
                {
                    if (user_id == "")
                    {
                        InputIsok = "N";
                        ReturnErr = "執行動作錯誤-user_id 為必填欄位";
                    }
                    else if (getCount == "")
                    {
                        InputIsok = "N";
                        ReturnErr = "執行動作錯誤-getCount 為必填欄位";
                    }
                    else
                    {
                        InputIsok = "N";
                        ReturnErr = "執行動作錯誤-brand 為必填欄位";
                    }
                }

                //第二步 : 如果有必填未給值，回傳告知必填欄位要有資料
                if (InputIsok == "N")
                {
                    APCommonFun.Error("[GetNoticeRecordController]90-" + ReturnErr);
                    return ReturnError(ReturnErr);
                }

                string sql = string.Empty;

                sql = "select nr.*, s.status from NoticeRecords nr left join Shops s on nr.shopNo = s.seq where nr.member=@user_id  and nr.brand=@brand and s.status = N'上架中' ";
                if (getCount == "N")
                {
                    sql += " order by nr.noticeTime  ";// + fetch_subStr;
                }


                DataTable dt = APCommonFun.GetSafeDataTable_MSSQL(
                    sql,
                    new List<SqlParameter>
                    {
                        new SqlParameter("@user_id", user_id),
                        new SqlParameter("@brand", brand),
                    }
                );
                if (getCount == "Y")
                {
                    var data = new
                    {
                        Count = dt.Rows.Count
                    };
                    return ReturnOK(data);
                }
                else
                {
                    if (dt.Rows.Count > 0)
                    {
                        if (string.IsNullOrEmpty(page))
                        {
                            foreach (DataRow dr in dt.Rows)
                            {
                                //string seqShop = APCommonFun.CDBNulltrim(dr["seq"].ToString());
                                string noticeTime = APCommonFun.CDBNulltrim(dr["noticeTime"].ToString());
                                string shopSeq = APCommonFun.CDBNulltrim(dr["shopNo"].ToString());

                                JObject tmpJoLay01 = new JObject();
                                //tmpJoLay01.Add(new JProperty("seqShop", seqShop));
                                tmpJoLay01.Add(new JProperty("noticeTime", noticeTime));
                                tmpJoLay01.Add(new JProperty("shopNo", shopSeq));

                                newJa.Add(tmpJoLay01);
                                totalJa.Add(tmpJoLay01);
                            }
                        }
                        else
                        {
                            int i = 0;
                            foreach (DataRow dr in dt.Rows)
                            {

                                string seqShop = APCommonFun.CDBNulltrim(dr["seq"].ToString());
                                string noticeTime = APCommonFun.CDBNulltrim(dr["noticeTime"].ToString());
                                string shopNo = APCommonFun.CDBNulltrim(dr["shopNo"].ToString());
                                string shopSeq = APCommonFun.CDBNulltrim(dr["shopSeq"].ToString());

                                JObject tmpJoLay01 = new JObject();
                                tmpJoLay01.Add(new JProperty("seqShop", seqShop));
                                tmpJoLay01.Add(new JProperty("noticeTime", noticeTime));
                                tmpJoLay01.Add(new JProperty("shopNo", shopNo));
                                tmpJoLay01.Add(new JProperty("shopSeq", shopSeq));

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
                        Count = totalJa.Count,
                        Data = newJa
                    };
                    return ReturnOK(data);
                }
            }
            catch (Exception ex)
            {
                APCommonFun.Error("[GetNoticeRecordController]99：" + ex.ToString());
                return ReturnException();
            }
        }
    }
}

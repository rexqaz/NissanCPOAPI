using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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
    public class GetNewsOneController : BaseAPIController
    {
        /// <summary>
        /// 取得一篇最新消息
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public object Post(Info_NewsOneModel Data)
        {
            try
            {
                string InputIsok = "Y";
                string ReturnErr = "";
                JObject tmpJoLay01 = new JObject();

                string brand = string.Empty;
                if (Data.brand != null)
                {
                    brand = APCommonFun.CDBNulltrim(Data.brand);
                }

                string seq = string.Empty;
                if (Data.seq != null)
                {
                    seq = APCommonFun.CDBNulltrim(Data.seq);
                }

                //第一步 : 先判斷有沒有必填未填寫，
                if (brand == "") //必填                       
                {
                    InputIsok = "N";
                    ReturnErr = "執行動作錯誤-brand 為必填欄位";
                }

                if (seq == "") //必填                       
                {
                    InputIsok = "N";
                    ReturnErr = "執行動作錯誤-seq 為必填欄位";
                }

                //第二步 : 如果有必填未給值，回傳告知必填欄位要有資料
                if (InputIsok == "N")
                {
                    APCommonFun.Error("[GetNewsOneController]90-" + ReturnErr);
                    return ReturnError(ReturnErr);
                }

                string sql = "select * from News where brand=@brand  and status=N'啟用' and seq=@seq   order by createTime desc ";


                DataTable dt = APCommonFun.GetSafeDataTable_MSSQL(
                    sql,
                    new List<SqlParameter>
                    {
                        new SqlParameter("@brand", brand),
                        new SqlParameter("@seq", seq)
                    }
                );
                if (dt.Rows.Count > 0)
                {
                    string title = APCommonFun.CDBNulltrim(dt.Rows[0]["title"].ToString());
                    string bodyContent = APCommonFun.CDBNulltrim(dt.Rows[0]["bodyContent"].ToString());
                    string showBody = APCommonFun.CDBNulltrim(dt.Rows[0]["showBody"].ToString());
                    string createTime = APCommonFun.CDBNulltrim(dt.Rows[0]["createTime"].ToString());

                    tmpJoLay01.Add(new JProperty("seq", seq));
                    tmpJoLay01.Add(new JProperty("title", title));
                    tmpJoLay01.Add(new JProperty("bodyContent", bodyContent));
                    tmpJoLay01.Add(new JProperty("showBody", showBody));
                    tmpJoLay01.Add(new JProperty("createTime", createTime));
                }

                var data = new
                {
                    Data = tmpJoLay01
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

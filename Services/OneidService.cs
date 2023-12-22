using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Configuration;
using static WebApplication.Controllers.MemberController;

namespace WebApplication.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class OneidService
    {
        private readonly string _MemberApi = WebConfigurationManager.AppSettings["memberApi"];

        /// <summary>
        /// 
        /// </summary>
        public OneidService()
        {

        }

        /// <summary>
        /// 取得使用者資料(token主人的資料)
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public DataModels.ApiResult<DataModels.MembersModel> GetMember(string token)
        {
            try
            {
                var apiResult = new DataModels.ApiResult<DataModels.MembersModel>();

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    HttpResponseMessage getResponse = client.GetAsync($"{_MemberApi}/GetMember").Result;
                    if (getResponse.StatusCode == HttpStatusCode.OK)
                    {
                        var getResponseBody = getResponse.Content.ReadAsStringAsync().Result;
                        var jObject = JObject.Parse(getResponseBody);
                        apiResult.StatusCode = (int)jObject["StatusCode"];
                        apiResult.Message = jObject["Message"].ToString();

                        var data = JObject.Parse(jObject["Data"].ToString());
                        apiResult.Data = JsonConvert.DeserializeObject<DataModels.MembersModel>(data["member"].ToString());                        
                    }
                    else if (getResponse.StatusCode == HttpStatusCode.Forbidden)
                    {
                        var getResponseBody = getResponse.Content.ReadAsStringAsync().Result;
                        var jObject = JObject.Parse(getResponseBody);
                        apiResult.StatusCode = (int)jObject["StatusCode"];
                        apiResult.Message = jObject["Message"].ToString();
                    }
                    else
                    {
                        throw new Exception($"OneidService.GetMember() HttpStatusCode 異常。ResponseStatusCode={(int)getResponse.StatusCode}");
                    }
                }

                return apiResult;
            }
            catch (Exception ex)
            {
                throw new Exception($"OneidService.GetMember()。{ex}");
            }
        }



        /// <summary>
        /// 取得指定使用者資料
        /// </summary>
        /// <param name="token"></param>
        /// <param name="member_seq"></param>
        /// <returns></returns>
        public DataModels.ApiResult<DataModels.MembersModel> GetMember(string token, string member_seq)
        {
            try
            {
                var apiResult = new DataModels.ApiResult<DataModels.MembersModel>();

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    HttpResponseMessage getResponse = client.GetAsync($"{_MemberApi}/GetMember?id={member_seq}").Result;
                    if (getResponse.StatusCode == HttpStatusCode.OK)
                    {
                        var getResponseBody = getResponse.Content.ReadAsStringAsync().Result;
                        var jObject = JObject.Parse(getResponseBody);
                        apiResult.StatusCode = (int)jObject["StatusCode"];
                        apiResult.Message = jObject["Message"].ToString();

                        var data = JObject.Parse(jObject["Data"].ToString());
                        apiResult.Data = JsonConvert.DeserializeObject<DataModels.MembersModel>(data["member"].ToString());
                    }
                    else if (getResponse.StatusCode == HttpStatusCode.Forbidden)
                    {
                        var getResponseBody = getResponse.Content.ReadAsStringAsync().Result;
                        var jObject = JObject.Parse(getResponseBody);
                        apiResult.StatusCode = (int)jObject["StatusCode"];
                        apiResult.Message = jObject["Message"].ToString();
                    }
                    else
                    {
                        throw new Exception($"OneidService.GetMember() HttpStatusCode 異常。ResponseStatusCode={(int)getResponse.StatusCode}");
                    }
                }

                return apiResult;
            }
            catch (Exception ex)
            {
                throw new Exception($"OneidService.GetMember()。{ex}");
            }
        }

        /// <summary>
        /// 取得指定使用者資料
        /// </summary>
        /// <param name="token"></param>
        /// <param name="member_mobile"></param>
        /// <returns></returns>
        public DataModels.ApiResult<DataModels.MembersModel> GetMemberByMobile(string token, string member_mobile)
        {
            try
            {
                var apiResult = new DataModels.ApiResult<DataModels.MembersModel>();

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    HttpResponseMessage getResponse = client.GetAsync($"{_MemberApi}/GetMemberByMobile?id={member_mobile}").Result;
                    if (getResponse.StatusCode == HttpStatusCode.OK)
                    {
                        var getResponseBody = getResponse.Content.ReadAsStringAsync().Result;
                        var jObject = JObject.Parse(getResponseBody);
                        apiResult.StatusCode = (int)jObject["StatusCode"];
                        apiResult.Message = jObject["Message"].ToString();

                        var data = JObject.Parse(jObject["Data"].ToString());
                        apiResult.Data = JsonConvert.DeserializeObject<DataModels.MembersModel>(data["member"].ToString());
                    }
                    else if (getResponse.StatusCode == HttpStatusCode.Forbidden)
                    {
                        var getResponseBody = getResponse.Content.ReadAsStringAsync().Result;
                        var jObject = JObject.Parse(getResponseBody);
                        apiResult.StatusCode = (int)jObject["StatusCode"];
                        apiResult.Message = jObject["Message"].ToString();
                    }
                    else
                    {
                        throw new Exception($"OneidService.GetMemberByMobile() HttpStatusCode 異常。ResponseStatusCode={(int)getResponse.StatusCode}");
                    }
                }

                return apiResult;
            }
            catch (Exception ex)
            {
                throw new Exception($"OneidService.GetMemberByMobile()。{ex}");
            }
        }

        /// <summary>
        /// 取得使用者資料
        /// </summary>
        /// <param name="token"></param>
        /// <param name="brand"></param>
        /// <returns></returns>
        public DataModels.ApiResult<List<DataModels.MembersModel>> GetMemberList(string token, string brand)
        {
            try
            {
                var apiResult = new DataModels.ApiResult<List<DataModels.MembersModel>>();

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    HttpResponseMessage getResponse = client.GetAsync($"{_MemberApi}/GetMembers?id={brand}").Result;
                    if (getResponse.StatusCode == HttpStatusCode.OK)
                    {
                        var getResponseBody = getResponse.Content.ReadAsStringAsync().Result;
                        var jObject = JObject.Parse(getResponseBody);
                        apiResult.StatusCode = (int)jObject["StatusCode"];
                        apiResult.Message = jObject["Message"].ToString();

                        var data = JObject.Parse(jObject["Data"].ToString());
                        apiResult.Data = JsonConvert.DeserializeObject<List<DataModels.MembersModel>>(data["members"].ToString());
                    }
                    else if (getResponse.StatusCode == HttpStatusCode.Forbidden)
                    {
                        var getResponseBody = getResponse.Content.ReadAsStringAsync().Result;
                        var jObject = JObject.Parse(getResponseBody);
                        apiResult.StatusCode = (int)jObject["StatusCode"];
                        apiResult.Message = jObject["Message"].ToString();
                    }
                    else
                    {
                        throw new Exception($"OneidService.GetMember() HttpStatusCode 異常。ResponseStatusCode={(int)getResponse.StatusCode}");
                    }
                }

                return apiResult;
            }
            catch (Exception ex)
            {
                throw new Exception($"OneidService.GetMember()。{ex}");
            }
        }

        /// <summary>
        /// 取得全部使用者資料
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public DataModels.ApiResult<List<DataModels.MembersModel>> GetMemberList(string token)
        {
            try
            {
                var apiResult = new DataModels.ApiResult<List<DataModels.MembersModel>>();

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    HttpResponseMessage getResponse = client.GetAsync($"{_MemberApi}/GetMembers").Result;
                    if (getResponse.StatusCode == HttpStatusCode.OK)
                    {
                        var getResponseBody = getResponse.Content.ReadAsStringAsync().Result;
                        var jObject = JObject.Parse(getResponseBody);
                        apiResult.StatusCode = (int)jObject["StatusCode"];
                        apiResult.Message = jObject["Message"].ToString();

                        var data = JObject.Parse(jObject["Data"].ToString());
                        apiResult.Data = JsonConvert.DeserializeObject<List<DataModels.MembersModel>>(data["members"].ToString());
                    }
                    else if (getResponse.StatusCode == HttpStatusCode.Forbidden)
                    {
                        var getResponseBody = getResponse.Content.ReadAsStringAsync().Result;
                        var jObject = JObject.Parse(getResponseBody);
                        apiResult.StatusCode = (int)jObject["StatusCode"];
                        apiResult.Message = jObject["Message"].ToString();
                    }
                    else
                    {
                        throw new Exception($"OneidService.GetMember() HttpStatusCode 異常。ResponseStatusCode={(int)getResponse.StatusCode}");
                    }
                }

                return apiResult;
            }
            catch (Exception ex)
            {
                throw new Exception($"OneidService.GetMember()。{ex}");
            }
        }

        /// <summary>
        /// GetMembersBehavior
        /// </summary>
        /// <param name="token"></param>
        /// <param name="member_seq"></param>
        /// <returns></returns>
        public DataModels.ApiResult<List<MemberBehavior>> GetMembersBehavior(string token, long member_seq)
        {
            try
            {
                var apiResult = new DataModels.ApiResult<List<MemberBehavior>>();

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    HttpResponseMessage getResponse = client.GetAsync($"{_MemberApi}/GetMembersBehavior?id={member_seq}").Result;
                    if (getResponse.StatusCode == HttpStatusCode.OK)
                    {
                        var getResponseBody = getResponse.Content.ReadAsStringAsync().Result;
                        var jObject = JObject.Parse(getResponseBody);
                        apiResult.StatusCode = (int)jObject["StatusCode"];
                        apiResult.Message = jObject["Message"].ToString();

                        var data = JObject.Parse(jObject["Data"].ToString());
                        apiResult.Data = JsonConvert.DeserializeObject<List<MemberBehavior>>(data["members"].ToString());
                    }
                    else if (getResponse.StatusCode == HttpStatusCode.Forbidden)
                    {
                        var getResponseBody = getResponse.Content.ReadAsStringAsync().Result;
                        var jObject = JObject.Parse(getResponseBody);
                        apiResult.StatusCode = (int)jObject["StatusCode"];
                        apiResult.Message = jObject["Message"].ToString();
                    }
                    else
                    {
                        throw new Exception($"OneidService.GetMember() HttpStatusCode 異常。ResponseStatusCode={(int)getResponse.StatusCode}");
                    }
                }

                return apiResult;
            }
            catch (Exception ex)
            {
                throw new Exception($"OneidService.GetMember()。{ex}");
            }
        }

        /// <summary>
        /// 登入
        /// </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <returns>只取回ActiveToken</returns>
        public DataModels.ApiResult<JObject> Login(string account, string password)
        {
            try
            {
                var apiResult = new DataModels.ApiResult<JObject>();

                using (HttpClient client = new HttpClient())
                {
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"{_MemberApi}/FormLogin");
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    

                    // 將JSON資料轉換為StringContent
                    var jsonPayload = JsonConvert.SerializeObject(new { mobile = account, password = password });
                    HttpContent content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                    request.Content = content;


                    HttpResponseMessage getResponse = client.SendAsync(request).Result;
                    if (getResponse.StatusCode == HttpStatusCode.OK)
                    {
                        var getResponseBody = getResponse.Content.ReadAsStringAsync().Result;
                        var jObject = JObject.Parse(getResponseBody);
                        apiResult.StatusCode = (int)jObject["StatusCode"];
                        apiResult.Message = jObject["Message"].ToString();

                        var data = JObject.Parse(jObject["Data"].ToString());

                        apiResult.Data = data;
                    }
                    else if (getResponse.StatusCode == HttpStatusCode.Forbidden)
                    {
                        var getResponseBody = getResponse.Content.ReadAsStringAsync().Result;
                        var jObject = JObject.Parse(getResponseBody);
                        apiResult.StatusCode = (int)jObject["StatusCode"];
                        apiResult.Message = jObject["Message"].ToString();
                    }
                    else
                    {
                        throw new Exception($"OneidService.Login() HttpStatusCode 異常。ResponseStatusCode={(int)getResponse.StatusCode}");
                    }
                }

                return apiResult;
            }
            catch (Exception ex)
            {
                throw new Exception($"OneidService.Login()。{ex}");
            }
        }

        /// <summary>
        /// 更新使用者操作紀錄
        /// </summary>
        /// <param name="token"></param>
        /// <param name="member_seq">使用者seq</param>
        /// <param name="behavior">使用者行為紀錄</param>
        /// <param name="brand">使用者操作的品牌</param>
        /// <returns></returns>
        public string UpdateMemberLog(string token, long member_seq, string behavior, string brand)
        {
            try
            {
                var apiResult = new DataModels.ApiResult<JObject>();

                using (HttpClient client = new HttpClient())
                {
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"{_MemberApi}/UpdateMemberLog");
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);


                    // 將JSON資料轉換為StringContent
                    var jsonPayload = JsonConvert.SerializeObject(new 
                    {
                        Member_seq = member_seq,
                        Behavior = behavior,
                        Brand = brand
                    });
                    HttpContent content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                    request.Content = content;


                    HttpResponseMessage getResponse = client.SendAsync(request).Result;
                    if (getResponse.StatusCode == HttpStatusCode.OK || getResponse.StatusCode == HttpStatusCode.Forbidden)
                    {
                        var getResponseBody = getResponse.Content.ReadAsStringAsync().Result;
                        var jObject = JObject.Parse(getResponseBody);
                        apiResult.StatusCode = (int)jObject["StatusCode"];
                        apiResult.Message = jObject["Message"].ToString();

                        var data = JObject.Parse(jObject["Data"].ToString());

                        return jObject["Message"].ToString();
                    }
                    else
                    {
                        throw new Exception($"OneidService.UpdateMemberLog() HttpStatusCode 異常。ResponseStatusCode={(int)getResponse.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"OneidService.UpdateMemberLog()。{ex}");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="para"></param>
        /// <returns></returns>
        public DataModels.ApiResult<JObject> UpdateMember(string token, UpdateMemberParameter para)
        {
            try
            {
                var apiResult = new DataModels.ApiResult<JObject>();

                using (HttpClient client = new HttpClient())
                {
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"{_MemberApi}/UpdateMember");
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    // 將JSON資料轉換為StringContent
                    var jsonPayload = JsonConvert.SerializeObject(para);
                    HttpContent content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                    request.Content = content;


                    HttpResponseMessage getResponse = client.SendAsync(request).Result;
                    if (getResponse.StatusCode == HttpStatusCode.OK)
                    {
                        var getResponseBody = getResponse.Content.ReadAsStringAsync().Result;
                        var jObject = JObject.Parse(getResponseBody);
                        apiResult.StatusCode = (int)jObject["StatusCode"];
                        apiResult.Message = jObject["Message"].ToString();
                    }
                    else if (getResponse.StatusCode == HttpStatusCode.Forbidden)
                    {
                        var getResponseBody = getResponse.Content.ReadAsStringAsync().Result;
                        var jObject = JObject.Parse(getResponseBody);
                        apiResult.StatusCode = (int)jObject["StatusCode"];
                        apiResult.Message = jObject["Message"].ToString();
                    }
                    else
                    {
                        throw new Exception($"OneidService.Login() HttpStatusCode 異常。ResponseStatusCode={(int)getResponse.StatusCode}");
                    }
                }

                return apiResult;
            }
            catch (Exception ex)
            {
                throw new Exception($"OneidService.Login()。{ex}");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public DataModels.ApiResult<JObject> DeleteMember(string token, string mobile)
        {
            try
            {
                var apiResult = new DataModels.ApiResult<JObject>();

                using (HttpClient client = new HttpClient())
                {
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, $"{_MemberApi}/DeleteMember?id={mobile}");
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    HttpContent content = new StringContent("", Encoding.UTF8, "application/json");
                    request.Content = content;


                    HttpResponseMessage getResponse = client.SendAsync(request).Result;
                    if (getResponse.StatusCode == HttpStatusCode.OK)
                    {
                        var getResponseBody = getResponse.Content.ReadAsStringAsync().Result;
                        var jObject = JObject.Parse(getResponseBody);
                        apiResult.StatusCode = (int)jObject["StatusCode"];
                        apiResult.Message = jObject["Message"].ToString();
                    }
                    else if (getResponse.StatusCode == HttpStatusCode.Forbidden)
                    {
                        var getResponseBody = getResponse.Content.ReadAsStringAsync().Result;
                        var jObject = JObject.Parse(getResponseBody);
                        apiResult.StatusCode = (int)jObject["StatusCode"];
                        apiResult.Message = jObject["Message"].ToString();
                    }
                    else
                    {
                        throw new Exception($"OneidService.Login() HttpStatusCode 異常。ResponseStatusCode={(int)getResponse.StatusCode}");
                    }
                }

                return apiResult;
            }
            catch (Exception ex)
            {
                throw new Exception($"OneidService.Login()。{ex}");
            }
        }

        public class UpdateMemberParameter
        {
            public string seq { get; set; }
            public string email { get; set; }
            public string mobile { get; set; }
            public string password { get; set; }
            public string status { get; set; }
        }
    }
}
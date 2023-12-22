using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using WebApplication.Models;

namespace WebApplication.Services
{
    public class MemberService
    {
        private static string _apiUrl = ConfigurationManager.AppSettings["memberApi"];

        public static async Task<Members> GetMemberAsync(string token, string id)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await client.GetAsync($"{_apiUrl}/GetMember/{id}");

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return JsonConvert.DeserializeObject<Members>(await response.Content.ReadAsStringAsync());
                }
            }
            catch
            {

            }

            return null;
        }

        internal static Task GetMemberAsync(string v, object userId)
        {
            throw new NotImplementedException();
        }
    }
}
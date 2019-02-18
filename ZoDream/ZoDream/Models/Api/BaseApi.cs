using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Core;
using ZoDream.Helpers;

namespace ZoDream.Models.Api
{
    public class BaseApi
    {
        public async Task<Page<T>> GetPageAsync<T>(string uri)
        {
            return await GetPageAsync<T>(uri, null);
        }

        public async Task<Page<T>> GetPageAsync<T>(string uri, string body)
        {
            var content = await CreateHttp().SetBody(body).AppendPath(uri).ExecuteAsync();
            if (string.IsNullOrEmpty(content))
            {
                return null;
            }
            var page = JObject.Parse(content);
            return page.ToObject<Page<T>>();
        }

        public async Task<T> GetAsync<T>(string uri, string body = null)
        {
           return await CreateHttp().SetBody(body).AppendPath(uri).ExecuteAsync<T>();
        }

        public async Task<T> GetAsync<T>(string uri, JContainer body)
        {
            return await CreateHttp().SetBody(body).AppendPath(uri).ExecuteAsync<T>(null, async message => {
                var content = await message.Content.ReadAsStringAsync();
                var error = JObject.Parse(content);
                Log.Error(error["message"].ToString());
            });
        }

        public async Task<T> GetAsync<T>(string uri, Dictionary<string, string> body)
        {
            return await CreateHttp().AddParameters(body).AppendPath(uri).ExecuteAsync<T>();
        }

        public RestClient CreateHttp()
        {
            var headers = new Dictionary<string, string>
            {
                { "Date", DateTime.Now.ToLongTimeString() },
                { "Content-Type", "application/vnd.api+json" },
                { "Accept", "application/json" }
            };
            if (!string.IsNullOrEmpty(Configs.NewInstance().Token))
            {
                headers.Add("Authorization", "Bearer " + Configs.NewInstance().Token);
            }
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            return new RestClient(Configs.HOST)
                .AddQuery("appid", Configs.APPID).AddQuery("timestamp", timestamp)
                .AddQuery("sign", EncryptWithMD5(Configs.APPID + timestamp + Configs.SECRET)).AddHeaders(headers);
        }

        public RestClient CreatePostHttp()
        {
            var client = CreateHttp();
            client.Method = HttpMethod.Post;
            return client;
        }

        public static string ToBase64String(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return "";
            }
            var bytes = Encoding.UTF8.GetBytes(value);
            return Convert.ToBase64String(bytes);
        }

        public static string UnBase64String(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return "";
            }
            var bytes = Convert.FromBase64String(value);
            return Encoding.UTF8.GetString(bytes);
        }

        public static string EncryptWithMD5(string source)
        {
            var sor = Encoding.UTF8.GetBytes(source);
            var md5 = MD5.Create();
            var result = md5.ComputeHash(sor);
            var strbul = new StringBuilder(40);
            for (int i = 0; i < result.Length; i++)
            {
                strbul.Append(result[i].ToString("x2"));//加密结果"x2"结果为32位,"x3"结果为48位,"x4"结果为64位

            }
            return strbul.ToString();
        }
    }
}

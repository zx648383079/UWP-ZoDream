using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Core;
using ZoDream.Helpers;

namespace ZoDream.Models.Api
{
    public class BaseApi
    {
        public async Task<IList<T>> GetPageAsync<T>(string uri)
        {
            return await GetPageAsync<T>(uri, null);
        }

        public async Task<IList<T>> GetPageAsync<T>(string uri, string body)
        {
            var content = await CreateHttp().AppendPath(uri).ExecuteAsync();
            if (string.IsNullOrEmpty(content))
            {
                return null;
            }
            var page = JObject.Parse(content);
            var results = page["data"].Children().ToList();
            var data = new List<T>();
            foreach (var item in results)
            {
                data.Add(item.ToObject<T>());
            }
            return data;
        }

        public RestClient CreateHttp()
        {
            var headers = new Dictionary<string, string>
            {
                { "Authorization", "ZoDream " + ToBase64String(Configs.APPID + ":") },
                { "Date", DateTime.Now.ToLongTimeString() },
            };
            return new RestClient(Configs.HOST).AddQuery("token", Configs.NewInstance().Token);
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
    }
}

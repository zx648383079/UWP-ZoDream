using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Blog.Core;
using ZoDream.Blog.Helpers;

namespace ZoDream.Blog.Models.Api
{
    public class BaseApi
    {
        public async Task<IList<T>> GetPageAsync<T>(string uri)
        {
            return await GetPageAsync<T>(uri, null);
        }

        public async Task<IList<T>> GetPageAsync<T>(string uri, string body)
        {
            var results = await SendHttpAsync(uri, body).Children().ToList();
            if (null == results) {
                return null;
            }
            var data = new List<T>();
            foreach (var item in results)
            {
                data.Add(item.ToObject<T>());
            }
            return data;
        }

        public Http CreateHttp(string uri, string body)
        {
            var http = new Http(Configs.HOST + uri, body);
            http.Headers.Add("TOKEN:" + Configs.NewInstance().Token);
            return http;
        }

        public Http CreateHttp(string uri)
        {
            return CreateHttp(uri, null);
        }

        public async Task<JToken> SendHttpAsync(string uri, string body) {
            var content = await CreateHttp(uri, body).GetAsync();
            if (string.IsNullOrEmpty(content))
            {
                return null;
            }
            JObject page = JObject.Parse(content);
            if (page["code"].ToObject<int>() != 200)
            {
                return null;
            }
            return page["data"];
        }
    }
}

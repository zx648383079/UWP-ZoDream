using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;

namespace ZoDream.Helpers
{
    public class RestClient
    {
        public string BaseUri { get; set; }

        public string Path { get; set; }

        public HttpMethod Method { get; set; } = HttpMethod.Get;

        public Dictionary<string, string> Queries { get; set; } = new Dictionary<string, string>();

        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();

        public string Content { get; set; }

        public Dictionary<string, string> Contents { get; set; } = new Dictionary<string, string>();


        public RestClient()
        {

        }

        public RestClient(string baseUri)
        {
            BaseUri = baseUri;
        }

        public RestClient(string baseUri, HttpMethod method)
        {
            BaseUri = baseUri;
            Method = method;
        }

        public RestClient(string baseUri, string path)
        {
            BaseUri = baseUri;
            Path = path;
        }

        public RestClient(string baseUri, string path, HttpMethod method)
        {
            BaseUri = baseUri;
            Path = path;
            Method = method;
        }


        public RestClient AppendPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return this;
            }
            if (string.IsNullOrEmpty(Path))
            {
                Path = path;
                return this;
            }
            Path = Path.TrimEnd('/') + "/" + path.TrimStart('/');
            return this;
        }

        public RestClient AddQueries(Dictionary<string, string> parameters)
        {
            if (parameters == null) return this;
            foreach (var parameter in parameters)
            {
                this.Queries[parameter.Key] = parameter.Value;
            }
            return this;
        }

        public RestClient AddQuery(string key, string value)
        {
            Queries[key] = value;
            return this;
        }

        public RestClient AddParameters(Dictionary<string, string> parameters)
        {
            if (parameters == null) return this;
            foreach (var parameter in parameters)
            {
                this.Contents[parameter.Key] = parameter.Value;
            }
            return this;
        }

        public RestClient AddParameter(string key, string value)
        {
            Contents[key] = value;
            return this;
        }

        public RestClient AddHeader(string key, string value)
        {
            Headers[key] = value;
            return this;
        }

        public RestClient SetContents(Dictionary<string, string> data)
        {
            if (data != null)
                this.Contents = data;
            return this;
        }

        public async Task<T> ExecuteAsync<T>(Action<HttpResponseMessage> succes = null, Action<HttpResponseMessage> failure = null)
        {
            var content = await ExecuteAsync(succes, failure);
            return JsonConvert.DeserializeObject<T>(content);
        }

        public async Task<string> ExecuteAsync(Action<HttpResponseMessage> succes = null, Action<HttpResponseMessage> failure = null)
        {
            if (string.IsNullOrEmpty(BaseUri)) return string.Empty;
            var httpClient = new HttpClient();
            var requestMessage = new HttpRequestMessage
            {
                Method = Method
            };
            extractHeaders(httpClient);
            var uri = new Uri(BaseUri, UriKind.Absolute);
            if (Method != HttpMethod.Get && Method != HttpMethod.Delete)
            {
                if (!string.IsNullOrEmpty(Content))
                {
                    requestMessage.Content = new StringContent(Content);
                }
                else if (Contents != null && Contents.Any())
                {
                    requestMessage.Content = new StringContent(buildJson());//new FormUrlEncodedContent(Contents);
                }
            }
            requestMessage.RequestUri = new Uri(uri, addQeuryString());
            try
            {
                var responseMessage = await httpClient.SendAsync(requestMessage).ConfigureAwait(false);

                if (responseMessage == null)
                {
                    failure?.Invoke(null);
                    return string.Empty;
                }

                if (responseMessage.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    failure?.Invoke(responseMessage);
                    return string.Empty;
                }
                succes?.Invoke(responseMessage);
                return await responseMessage.Content.ReadAsStringAsync();
            }
            catch
            {
                failure?.Invoke(null);
            }
            return string.Empty;
        }

        private string addQeuryString()
        {
            var query = buildQuery();
            if (string.IsNullOrEmpty(query))
            {
                return Path;
            }
            if (!Path.Contains("?"))
            {
                return Path + "&" + buildQuery();
            }
            return Path + "?" + buildQuery();
        }

        private void extractHeaders(HttpClient httpClient)
        {
            if (Headers != null && Headers.Any())
            {
                foreach (var header in Headers)
                {
                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
                }
            }
        }

        private string buildQuery()
        {
            if (Queries == null || !Queries.Any()) return string.Empty;
            var builder = new StringBuilder();
            foreach (var content in Queries)
            {
                builder.Append($"{ System.Net.WebUtility.HtmlEncode(content.Key)}={ System.Net.WebUtility.HtmlEncode(content.Value)}&");
            }
            var data = builder.ToString();
            return data.Substring(0, data.Length - 1);
        }

        private string buildJson()
        {
            var jsonObject = new JObject(); ;
            foreach (var item in Contents)
            {
                jsonObject.Add(new JProperty(item.Key, item.Value));
            }
            return jsonObject.ToString();
        }
    }
}

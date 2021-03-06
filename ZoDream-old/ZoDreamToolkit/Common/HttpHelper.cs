﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;

namespace ZoDreamToolkit.Common
{
    public class HttpHelper
    {
        private static CookieContainer cookies = new CookieContainer();

        private static Uri referer;

        public static async Task<string> GetAsync(Uri url)
        {
            return await GetAsync(url, true);
        }

        public static async Task<string> GetAsync(Uri url, bool hasReturn)
        {
            var request = WebRequest.CreateHttp(url); //创建WebRequest对象              
            request.Method = "GET";    //设置请求方式为GET
            request.Headers[HttpRequestHeader.Accept] = "text/html, application/xhtml+xml, image/jxr, */*";
            request.Headers[HttpRequestHeader.AcceptLanguage] = "zh-Hans-CN, zh-Hans; q=0.5";
            request.Headers[HttpRequestHeader.Connection] = "Keep-Alive";
            request.Headers[HttpRequestHeader.Host] = url.Host;
            request.Headers[HttpRequestHeader.UserAgent] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.79 Safari/537.36 Edge/14.14393";
            request.Headers[HttpRequestHeader.AcceptEncoding] = "gzip, deflate"; //设置接收的编码 可以接受 gzip
            if (referer != null)
            {
                request.Headers[HttpRequestHeader.Referer] = referer.AbsoluteUri;
            }
            if (cookies.Count > 0)
            {
                request.CookieContainer = cookies;
            }

            var response = await request.GetResponseAsync();
            referer = url;
            var args = ((HttpWebResponse)response).Cookies;

            foreach (Cookie item in args)
            {
                cookies.Add(url, item);
            }
            if (!hasReturn)
            {
                request.Abort();
                response.Dispose();
                return null;
            }
            var stream = response.Headers[HttpRequestHeader.ContentEncoding].Equals("gzip",
            StringComparison.CurrentCultureIgnoreCase) ? new GZipStream(response.GetResponseStream(), CompressionMode.Decompress) : response.GetResponseStream();
            var ms = new MemoryStream();
            var buffer = new byte[1024];
            while (true)
            {
                if (stream == null) continue;
                var sz = stream.Read(buffer, 0, 1024);
                if (sz == 0) break;
                ms.Write(buffer, 0, sz);
            }
            var bytes = ms.ToArray();
            var html = GetEncoding(bytes, response.Headers[HttpRequestHeader.ContentType]).GetString(bytes);
            await stream.FlushAsync();
            request.Abort();
            response.Dispose();
            return html;
        }

        public static async Task<string> GetAsync(string url, bool hasReturn)
        {
            return await GetAsync(new Uri(url), hasReturn);
        }

        public static async Task<string> GetAsync(string url)
        {
            return await GetAsync(url, true);
        }

        public static Encoding GetEncoding(byte[] bytes, string charSet)
        {
            var html = Encoding.UTF8.GetString(bytes);
            var regCharset = new Regex(@"charset\b\s*=\s*""*(?<charset>[^""]*)");
            if (regCharset.IsMatch(html))
            {
                return Encoding.GetEncoding(regCharset.Match(html).Groups["charset"].Value);
            }
            if (string.IsNullOrEmpty(charSet))
            {
                return Encoding.UTF8;
            }
            try
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                return Encoding.GetEncoding(charSet);
            }
            catch (Exception)
            {
                return Encoding.UTF8;
            }
        }

        public static string GetAbsolute(string url, string relative)
        {
            return GetAbsolute(new Uri(url), relative);
        }

        public static string GetAbsolute(Uri url, string relative)
        {
            return new Uri(url, relative).ToString();
        }

        public static string GetLocalIp()
        {
            var icp = NetworkInformation.GetInternetConnectionProfile();

            if (icp?.NetworkAdapter == null) return null;
            var hostname =
                NetworkInformation.GetHostNames()
                    .SingleOrDefault(
                        hn =>
                            hn.IPInformation?.NetworkAdapter != null && hn.IPInformation.NetworkAdapter.NetworkAdapterId
                            == icp.NetworkAdapter.NetworkAdapterId);

            // the ip address  
            return hostname?.CanonicalName;
        }
    }
}

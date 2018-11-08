using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.Web.Http;

namespace ZoDreamToolkit.Common
{
    public class HttpRequest
    {
        #region Instance Field

        private readonly string _url;　　//请求的url　　
        private readonly string _body;　　//Post/Get时的数据
        private HttpClient _httpClient;
        private CancellationTokenSource _cts;　　//用于取消请求
        private IProgress<HttpProgress> _httpProgressDownload;　　//用于下载进度
        private IProgress<HttpProgress> _httpProgressUpload;
        private double progressUpload = 0;
        private double progressDownload = 0;  //下载进度

        #endregion

        #region Delegates

        public delegate void OnFailedEventHandle(string error, WebExceptionStatus status);
        public delegate void OnSucceedEventHandle(InMemoryRandomAccessStream randomAccessStream);
        public delegate void OnCancelEventHandle(string message);
        public delegate void OnProgressChangedEventHandle(double progress);

        #endregion

        #region Events
        //事件  分别用来处理获取失败、成功、取消、进度信息
        public event OnFailedEventHandle FailedEvent;
        public event OnSucceedEventHandle SucceedEvent;
        public event OnCancelEventHandle CancelEvent;
        public event OnProgressChangedEventHandle ProgressChangedEvent;

        #endregion
        //构造函数
        public HttpRequest(string url, string body = null)
        {
            this._url = url;
            this._body = body;
            _httpClient = new HttpClient();
            _cts = new CancellationTokenSource();
        }
        //开始运行 
        public void Run()
        {
            DoHttpClientRequest();
        }

        public async void DoHttpClientRequest()
        {
            //根据是否存在body判断是Get请求还是Post请求
            RequestType method = string.IsNullOrEmpty(_body) ? RequestType.Get : RequestType.Post;
            var request = CreateHttp(_url, method);
            if (_httpClient != null)
            {
                try
                {
                    HttpResponseMessage response = null;
                    if (method == RequestType.Post)
                    {
                        //POST
                        //_httpProgressUpload = new Progress<HttpProcess>(ProgressUploadHandler);
                        //response = await _httpClient.SendRequestAsync(request).AsTask(_cts.Token, _progressUpload);
                        response = await _httpClient.SendRequestAsync(request).AsTask(_cts.Token);
                    }
                    else if (method == RequestType.Get)
                    {
                        //GET
                        //下载进度状态信息
                        _httpProgressDownload = new Progress<HttpProgress>(ProgressDownloadHandler);
                        try
                        {
                            response = await _httpClient.SendRequestAsync(request).AsTask(_cts.Token, _httpProgressDownload);
                            //HttpCompletionOption.ResponseHeadersRead多了这个参数    在接受到头之后完成。  于是就不继续进行了
                            //response = await _httpClient.SendRequestAsync(request, HttpCompletionOption.ResponseHeadersRead).AsTask(_cts.Token, _httpProgressDownload);

                            _cts.Token.ThrowIfCancellationRequested();

                            //处理流
                            using (Stream responseStream = (await response.Content.ReadAsInputStreamAsync()).AsStreamForRead())
                            {
                                //将Stream转换为IRandomAccessStream
                                var randomAccessStream = new InMemoryRandomAccessStream();
                                var outputStream = randomAccessStream.GetOutputStreamAt(0);
                                await RandomAccessStream.CopyAsync(responseStream.AsInputStream(), outputStream);

                                if (randomAccessStream != null)
                                {
                                    if (SucceedEvent != null)
                                        SucceedEvent(randomAccessStream);   //获取到源的回调方法，并返回获取的内容
                                }
                            }
                        }
                        //中断Task时候会抛出异常，所以要通过try catch这种方法来获取是否终止。
                        catch (TaskCanceledException)
                        {
                            //请求被取消
                            CancelEvent("下载已停止");
                        }
                    }
                }
                catch (WebException e)
                {
                    FailedEvent(e.Message, e.Status);
                }
            }
        }

        public HttpRequestMessage CreateHttp(string url, RequestType type = RequestType.Get)
        {
            HttpRequestMessage request = null;
            try
            {
                if (type == RequestType.Get)
                {
                    request = new HttpRequestMessage(HttpMethod.Get, new Uri(url, UriKind.Absolute));
                }
                else
                {
                    request = new HttpRequestMessage(HttpMethod.Post, new Uri(url, UriKind.Absolute));
                    request.Content = SetPostContent(this._body);
                }
                SetHeaders();
            }
            catch (WebException e)
            {
                FailedEvent(e.Message, e.Status);
            }
            return request;
        }

        //Post请求内容
        public HttpStreamContent SetPostContent(string body)
        {
            byte[] subData = new byte[body.Length];
            MemoryStream stream = new MemoryStream(subData);
            HttpStreamContent streamContent = new HttpStreamContent(stream.AsInputStream());
            return streamContent;
        }
        public void SetHeaders()
        {
            //略
        }

        public void ProgressDownloadHandler(HttpProgress progress)
        {
            //处理进度  包括了很多状态 如ConnectingToServer、WaitingForResponse等
            string infoState = progress.Stage.ToString();
            double totalByteToRecive = 0;
            if (progress.TotalBytesToSend.HasValue)
            {
                //要发送的数据                
            }
            if (progress.TotalBytesToReceive.HasValue)
            {
                //接收数据 获取总接收数据
                totalByteToRecive = progress.TotalBytesToReceive.Value;
            }

            if (progress.Stage == HttpProgressStage.ReceivingContent)
            {
                progressUpload = progress.BytesReceived / totalByteToRecive;
                if (ProgressChangedEvent != null)
                {
                    ProgressChangedEvent(progressUpload * 100);
                }
            }
        }

        public void Cancel()
        {
            if (_cts.Token.CanBeCanceled)
            {
                //取消请求并且释放资源
                _cts.Cancel();
                _cts.Dispose();
            }
        }
    }
    //枚举变量 来判断是Get请求还是Post请求
    public enum RequestType
    {
        Post,
        Get
    }
}

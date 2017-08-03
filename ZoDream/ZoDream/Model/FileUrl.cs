using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Web.Http;
using ZoDream.Services;
using ZoDreamToolkit.Common;

namespace ZoDream.Model
{
    public class FileUrl : ObservableObject
    {

        public int Id { get; set; }

        /// <summary>
        /// The <see cref="Name" /> property's name.
        /// </summary>
        public const string NamePropertyName = "Name";

        private string _name = string.Empty;

        /// <summary>
        /// Sets and gets the Name property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                Set(NamePropertyName, ref _name, value);
            }
        }

        public Uri Url { get; set; }

        /// <summary>
        /// The <see cref="Process" /> property's name.
        /// </summary>
        public const string ProcessPropertyName = "Process";

        private double _process = 0;

        /// <summary>
        /// Sets and gets the Process property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public double Process
        {
            get
            {
                return _process;
            }
            set
            {
                Set(ProcessPropertyName, ref _process, value);
            }
        }

        private ulong? _length;

        /// <summary>
        /// The <see cref="Status" /> property's name.
        /// </summary>
        public const string StatusPropertyName = "Status";

        private FileStatus _status = FileStatus.None;

        /// <summary>
        /// Sets and gets the Status property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public FileStatus Status
        {
            get
            {
                return _status;
            }
            set
            {
                Set(StatusPropertyName, ref _status, value);
            }
        }

        private CancellationTokenSource _cts = new CancellationTokenSource();

        public async Task DownFileAsync()
        {

            var folder = await StorageHelper.OpenFolderAsync();
            if (folder == null)
            {
                return;
            }
            //开启一个异步线程
            await Task.Run(async () =>
            {
                //异步操作UI元素
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    Status = FileStatus.Doning;
                });
                using (var http = new HttpClient())
                {
                    var _httpProgressDownload = new Progress<HttpProgress>(_progressDownloadHandler);
                    var response = await http.GetAsync(Url, HttpCompletionOption.ResponseHeadersRead).AsTask(_cts.Token, _httpProgressDownload);//发送请求
                    if (response.StatusCode != Windows.Web.Http.HttpStatusCode.Ok)
                    {
                        DispatcherHelper.CheckBeginInvokeOnUI(() =>
                        {
                            Status = FileStatus.Failure;
                        });
                        Toast.ShowInfo("网址有误...");
                        return;
                    }
                    var httpFileName = response.Content.Headers.ContentDisposition.FileName;
                    if (!string.IsNullOrWhiteSpace(httpFileName))
                    {
                        DispatcherHelper.CheckBeginInvokeOnUI(() =>
                        {
                            Name = httpFileName;
                        });
                        
                    }
                    else if (string.IsNullOrWhiteSpace(Name))
                    {
                        DispatcherHelper.CheckBeginInvokeOnUI(() =>
                        {
                            Name = "新下载文件.txt";
                        });
                    }

                    _length = response.Content.Headers.ContentLength;   //文件大小       
                    Toast.ShowInfo(Name + " 正在下载...");
                    var file = await folder.CreateFileAsync(Name, CreationCollisionOption.ReplaceExisting);
                    
                    var streamResponse = response.Content.ReadAsInputStreamAsync();
                    _cts.Token.ThrowIfCancellationRequested();
                    using (var stream = await streamResponse)
                    {
                        using (var inputStream = stream.AsStreamForRead())
                        using (var fileStream = await file.OpenStreamForWriteAsync())
                        {
                            await inputStream.CopyToAsync(fileStream);
                            Toast.ShowInfo(Name + " 下载完成！");
                            DispatcherHelper.CheckBeginInvokeOnUI(() =>
                            {
                                Status = FileStatus.Success;
                            });
                        }
                    }
                }
            });
        }

        private void _progressDownloadHandler(HttpProgress progress)
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
                Process = progress.BytesReceived * 100 / totalByteToRecive;
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

        public FileUrl()
        {

        }

        public FileUrl(string url)
        {
            Url = new Uri(url);
            Name = Regex.Match(url, @"[^/\?]+(\.[^\?]+)?", RegexOptions.RightToLeft).Value;
        }

        public FileUrl(Uri url)
        {
            Url = url;
            Name = Regex.Match(Url.LocalPath, @"[^/\?]+(\.[^\?]+)?", RegexOptions.RightToLeft).Value;
        }
    }
}

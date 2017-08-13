using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.ComponentModel;

namespace ABSystem
{
    [Serializable]
    public class ABRemoteSetting
    {
        public string RemoteVersionURI; // 获取远程版本号的uri
        public string RemoteAssetBundleListURI; // 获取远程版本的AB包的json信息列表的uri
        public string RemoteAssetBundleDownloadEntry; // 远程ab包的入口
    }

    public class ABRemoteManager
    {
        private ABRemoteSetting setting;
        private ABLocalManager localManager;
        private bool IsCheckSize;


        public Queue<ABDownloadItem> DownloadQueue { get; private set; }
        public ABDownloadItem CurrentDownloadItem { get; private set; }

        public long TotalBytes { get; private set; }
        public long BytesReceive { get; private set; }

        public ABRemoteManager(ABRemoteSetting setting, ABLocalManager localManager)
        {
            this.setting = setting;
            this.localManager = localManager;
        }

        /// <summary>
        /// 远程版本信息
        /// </summary>
        public string Version
        {
            get
            {
                using (var webClient = new WebClient())
                {
                    Stream stream = webClient.OpenRead(setting.RemoteVersionURI);
                    StreamReader sr = new StreamReader(stream);
                    return ABUtility.JsonToVersion(sr.ReadToEnd());
                }
            }
        }

        /// <summary>
        /// 远程ab包信息列表
        /// </summary>
        public List<ABInfo> GetAseetBundleList(string version)
        {
            using (var webClient = new WebClient())
            {
                Stream stream = webClient.OpenRead(string.Format("{0}?Version={1}", setting.RemoteAssetBundleListURI, version));
                StreamReader sr = new StreamReader(stream);
                return ABUtility.JsonToABList(sr.ReadToEnd());
            }
        }

        /// <summary>
        /// 设置下载队列
        /// </summary>
        /// <param name="downloadABList"></param>
        public void SetDownloadQueue(IEnumerable<ABInfo> downloadABList, string version)
        {
            DownloadQueue = new Queue<ABDownloadItem>();
            foreach (var abinfo in downloadABList)
            {
                var abItem = new ABDownloadItem()
                {
                    Name = abinfo.Name,
                    Hash = abinfo.Hash,
                    Version = version
                };
                DownloadQueue.Enqueue(abItem);
                var abmfItem = new ABDownloadItem()
                {
                    Name = abinfo.Name + ".manifest",
                    Hash = abinfo.Hash,
                    Version = version
                };

                DownloadQueue.Enqueue(abmfItem);
            }
            var abMainItem = new ABDownloadItem()
            {
                Name = "AssetBundles",
                Version = version
            };
            DownloadQueue.Enqueue(abMainItem);
            var abmfMainItem = new ABDownloadItem()
            {
                Name = "AssetBundles.manifest",
                Version = version
            };
            DownloadQueue.Enqueue(abmfMainItem);
        }

        /// <summary>
        /// 获取需要下载的字节数, 注意, 这里使用HEAD的方式, 且使用了http 1.0的版本
        /// </summary>
        /// <param name="downloadQueue"></param>
        /// <returns></returns>
        public void GetDownloadSize()
        {
            // 检查各ab包的大小
            long downloadSize = 0;
            foreach(var item in DownloadQueue)
            {
                var abRequest = new HttpWebRequest(GetABDownloadUri(item))
                {
                    Method = "HEAD",
                    AllowAutoRedirect = false
                };
                using (var response = abRequest.GetResponse() as HttpWebResponse)
                {
                    // 200时直接得到大小
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var size = response.ContentLength;
                        item.TotalBytesToReceive = size;
                        downloadSize += size;
                    }
                    // 支持301和302重定向
                    if(response.StatusCode == HttpStatusCode.Moved || response.StatusCode == HttpStatusCode.Redirect)
                    {
                        var size = GetDownloadSizeFollowRedirect(response);
                        item.TotalBytesToReceive = size;
                        downloadSize += size;
                    }
                }
            }
            TotalBytes = downloadSize;
            IsCheckSize = true;
        }

        /// <summary>
        /// 更跟随重定向, 并继续用HEAD方法请求
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        private long GetDownloadSizeFollowRedirect(WebResponse response)
        {
            var rdRequest = new HttpWebRequest(new Uri(response.Headers["Location"]))
            {
                Method = "HEAD",
                AllowAutoRedirect = false
            };
            using (var rdResponse = rdRequest.GetResponse())
            {
                var size = rdResponse.ContentLength;
                return size;
            }
        }

        /// <summary>
        /// 获取AssetBundle下载的uri
        /// </summary>
        /// <param name="abinfo"></param>
        /// <returns></returns>
        private Uri GetABDownloadUri(ABDownloadItem item)
        {
            return GetABDownloadUri(item.Name, item.Version);
        }

        /// <summary>
        /// 获取AssetBundle下载的uri
        /// </summary>
        /// <param name="abinfo"></param>
        /// <returns></returns>
        private Uri GetABDownloadUri(string name, string version)
        {
            var uri = new Uri(string.Format("{0}?Name={1}&Version={2}", setting.RemoteAssetBundleDownloadEntry, name, version));
            return uri;
        }

        /// <summary>
        /// 开始下载ab包
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="storagPath"></param>
        public void StartDownload()
        {
            if (!IsCheckSize) GetDownloadSize();
            if(DownloadQueue.Count > 0)
            {
                CurrentDownloadItem = DownloadQueue.Dequeue();
                Download();
            }
            
        }

        /// <summary>
        /// 异步下载ab包
        /// </summary>
        private void Download()
        {
            var abUri = GetABDownloadUri(CurrentDownloadItem);
            var abPath = localManager.TryCreateDirectory(CurrentDownloadItem);
            using (var webClient = new WebClient())
            {
                webClient.DownloadFileCompleted += ABDownloadCompleted;
                webClient.DownloadProgressChanged += ABDownlaodProgressChanged;
                webClient.DownloadFileAsync(abUri, abPath);
            }
        }

        /// <summary>
        /// 异步下载完成时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ABDownloadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null) throw e.Error;
            BytesReceive += CurrentDownloadItem.TotalBytesToReceive;
            try
            {
                CurrentDownloadItem = DownloadQueue.Dequeue();
                Download();
            }
            catch(InvalidOperationException)
            {
                CurrentDownloadItem = null;
            }
            
        }

        /// <summary>
        /// 异步下载进度改变时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ABDownlaodProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            CurrentDownloadItem.BytesReceived = e.BytesReceived;
            CurrentDownloadItem.ProgressPercentage = e.ProgressPercentage;
        }


        


    }
}


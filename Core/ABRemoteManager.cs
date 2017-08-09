using System;
using System.IO;
using System.Net;
using System.Collections.Generic;

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
        public List<AssetBundleInfo> AseetBundleList
        {
            get
            {
                using (var webClient = new WebClient())
                {
                    Stream stream = webClient.OpenRead(setting.RemoteAssetBundleListURI);
                    StreamReader sr = new StreamReader(stream);
                    return ABUtility.JsonToAseetBundleList(sr.ReadToEnd());
                }
            }
        }

        /// <summary>
        /// 下载ab包
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="storagPath"></param>
        public void DownloadAssetBundles(IEnumerable<AssetBundleInfo> assetBundleinfo, string version)
        {
            foreach (var abinfo in assetBundleinfo)
            {
                DownloadAssetBundle(abinfo);           
            }
            // 下载主AssetBundles包
            var mainFilePath = localManager.TryCreateDirectory("AssetBundles");
            using (var webClient = new WebClient())
            {
                var mainUri = new Uri(string.Format("{0}?type={1}&Name={2}&Version={3}", setting.RemoteAssetBundleDownloadEntry, "MainAssetBundles", "AssetBundles", version));
                webClient.DownloadFile(mainUri, mainFilePath);
                
            }
            using (var webClient = new WebClient())
            {
                var mfUri = new Uri(string.Format("{0}?type={1}&Name={2}", setting.RemoteAssetBundleDownloadEntry, "MainManifest", "AssetBundles"));
                webClient.DownloadFileAsync(mfUri, mainFilePath + ".manifest");
            }
        }

        private void DownloadAssetBundle(AssetBundleInfo abInfo)
        {
            var filePath = localManager.TryCreateDirectory(abInfo);
            using (var webClient = new WebClient())
            {
                var abUri = new Uri(string.Format("{0}?type={1}&Name={2}&Hash={3}", setting.RemoteAssetBundleDownloadEntry, "AssetBundle", abInfo.Name, abInfo.Hash));
                webClient.DownloadFileAsync(abUri, filePath);
            }
            using (var webClient = new WebClient())
            {
                var abManifestUri = new Uri(string.Format("{0}?type={1}&Name={2}", setting.RemoteAssetBundleDownloadEntry, "Manifest", abInfo.Name));
                webClient.DownloadFileAsync(abManifestUri, filePath + ".manifest");
            }
        }


    }
}


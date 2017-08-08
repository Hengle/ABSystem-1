using System.IO;
using System.Net;
using System.Collections.Generic;
using UnityEngine;

namespace ABSystem
{
    [System.Serializable]
    public class ABRemoteSetting
    {
        public string RemoteVersionURI; // 获取远程版本号的uri
        public string RemoteAssetBundleListURI; // 获取远程版本的AB包的json信息列表的uri
        public string RemoteAssetBundleDownloadEntry; // 远程ab包的入口
    }

    public class ABRemoteManager
    {
        private ABRemoteSetting Setting;

        public ABRemoteManager(ABRemoteSetting setting)
        {
            Setting = setting;
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
                    Stream stream = webClient.OpenRead(Setting.RemoteVersionURI);
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
                    Stream stream = webClient.OpenRead(Setting.RemoteAssetBundleListURI);
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
        public void DownloadAssetBundle(IEnumerable<AssetBundleInfo> assetBundleinfo, ABLocalManager localManager)
        {
            using (var webClient = new WebClient())
            {
                foreach(var abinfo in assetBundleinfo)
                {
                    var filePath = localManager.TryCreateDirectory(abinfo);
                    webClient.DownloadFile(string.Format("{0}/{1}", Setting.RemoteAssetBundleDownloadEntry, abinfo.Name), filePath);
                    webClient.DownloadFile(string.Format("{0}/{1}", Setting.RemoteAssetBundleDownloadEntry, abinfo.Name + ".manifest"), filePath + ".manifest");
                }
                // 下载主AssetBundles包
                var mainFilePath = localManager.TryCreateDirectory("AssetBundles");
                webClient.DownloadFile(string.Format("{0}/{1}", Setting.RemoteAssetBundleDownloadEntry, "AssetBundles"), mainFilePath);
                webClient.DownloadFile(string.Format("{0}/{1}", Setting.RemoteAssetBundleDownloadEntry, "AssetBundles.manifest"), mainFilePath + ".manifest");
            }
        }


    }
}


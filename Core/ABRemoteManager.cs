using System.IO;
using System.Net;
using System.Collections.Generic;

namespace ABSystem
{
    public class ABRemoteManager
    {
        /// <summary>
        /// 获取远程版本信息
        /// </summary>
        /// <param name="remoteVersionURI"></param>
        /// <returns></returns>
        public static string GetVersion(string remoteVersionURI)
        {
            using (var webClient = new WebClient())
            {
                Stream stream = webClient.OpenRead(remoteVersionURI);
                StreamReader sr = new StreamReader(stream);
                return ABUtility.JsonToVersion(sr.ReadToEnd());
            } 
        }

        /// <summary>
        /// 获取远程ab包的清单
        /// </summary>
        /// <param name="remoteAssetBundleListURI"></param>
        /// <returns></returns>
        public static List<AssetBundleInfo> GetAseetBundleList(string remoteAssetBundleListURI)
        {
            using (var webClient = new WebClient())
            {
                Stream stream = webClient.OpenRead(remoteAssetBundleListURI);
                StreamReader sr = new StreamReader(stream);
                return ABUtility.JsonToAseetBundleList(sr.ReadToEnd());
            }
        }

        /// <summary>
        /// 下载ab包
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="storagPath"></param>
        public static void DownloadAssetBundle(IEnumerable<string> assetBundleName, string downloadEntry, string storagPath)
        {
            using (var webClient = new WebClient())
            {
                foreach(var name in assetBundleName)
                {
                    string filePath = Path.Combine(storagPath, name);
                    if (!Directory.Exists(filePath))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                    }
                    webClient.DownloadFile(string.Format("{0}/{1}", downloadEntry, name), filePath);
                    webClient.DownloadFile(string.Format("{0}/{1}", downloadEntry, name +".manifest"), filePath + ".manifest");
                }
            }
        }

        /// <summary>
        /// 下载主AssetBundles包
        /// </summary>
        /// <param name="downloadEntry"></param>
        /// <param name="storagPath"></param>
        /// <param name="name"></param>
        public static void DownloadMainAssetBundle(string downloadEntry, string storagPath, string name = "AssetBundles")
        {
            using (var webClient = new WebClient())
            {
                webClient.DownloadFile(string.Format("{0}/{1}", downloadEntry, name), Path.Combine(storagPath, name));
                webClient.DownloadFile(string.Format("{0}/{1}", downloadEntry, name + ".manifest"), Path.Combine(storagPath, name) + ".manifest");
            }
        }

    }
}


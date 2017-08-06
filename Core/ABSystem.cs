using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace ABSystem
{
    public class ABSystem : MonoBehaviour
    {
        public string Host;
        public string Post;
        public string protocol; // 使用的是http协议, 还是https协议, 还是其他
        public string RemoteVersionURI; // 获取远程版本号的uri
        public string RemoteAssetBundleListURI; // 获取远程版本的AB包的json信息列表的uri
        public string RemoteAssetBundleDownloadEntry; // 远程ab包的入口

        private string remoteVersionURI;
        private string remoteAssetBundleListURI;
        private string remoteAssetBUndleDownloadEntry;
        private string localVersionFilePath;   // 放置本机版本号文件的路径
        private string localAssetBundleListFilePath;   // 本地ab包json信息列表的路径
        private string localAssetBundleStoragPath; // ab包的本地储存路径

        private string localVersion;
        private string remoteVersion;
        private List<AssetBundleInfo> localAssetBundleList;
        private List<AssetBundleInfo> remoteAssetBundleList;

        private void Awake()
        {
            remoteVersionURI = string.Format("{0}://{1}:{2}/{3}", protocol, Host, Post, RemoteVersionURI);
            remoteAssetBundleListURI = string.Format("{0}://{1}:{2}/{3}", protocol, Host, Post, RemoteAssetBundleListURI);
            remoteAssetBUndleDownloadEntry = string.Format("{0}://{1}:{2}/{3}", protocol, Host, Post, RemoteAssetBundleDownloadEntry);
            localVersionFilePath = Path.Combine(Application.persistentDataPath, "Version.json");
            localAssetBundleListFilePath = Path.Combine(Application.persistentDataPath, "ResourceList.json");
            localAssetBundleStoragPath = Path.Combine(Application.persistentDataPath, "AssetBundles");
        }

        private void Start()
        {
            UpdateToNew();
        }

        private void UpdateToNew()
        {
            if(HasNewVersion())
            {
                MakeAssetBundleList();
                DownloadAssetBundle();
                ClearAssetBundle();
                // 下载后进行数据校验, 合法则写入新的信息文件
                if(ABLocalManager.DataVerification())
                {
                    ABLocalManager.CreateVersionFile(remoteVersion, Application.persistentDataPath);
                    ABLocalManager.CreateAssetBundleListFile(remoteAssetBundleList, Application.persistentDataPath);
                }
            }
        }

        /// <summary>
        /// 检查是否有新版本
        /// </summary>
        /// <returns></returns>
        private bool HasNewVersion()
        {
            localVersion = ABLocalManager.GetVersion(localVersionFilePath);
            remoteVersion = ABRemoteManager.GetVersion(remoteVersionURI);
            return localVersion.Equals(remoteVersion) ? false : true;
        }

        /// <summary>
        /// 生成ab包列表数据
        /// </summary>
        private void MakeAssetBundleList()
        {
            // 读取本地ab包的清单
            localAssetBundleList = ABLocalManager.GetAseetBundleList(localAssetBundleListFilePath);
            // 读取远程ab包的清单
            remoteAssetBundleList = ABRemoteManager.GetAseetBundleList(remoteAssetBundleListURI);
        }

        /// <summary>
        /// 下载要更新的ab包
        /// </summary>
        private void DownloadAssetBundle()
        {
            // 获取更新列表
            var updateList = from remoteab in remoteAssetBundleList
                             from localab in localAssetBundleList
                             where localab.HasNewVersion(remoteab)
                             select remoteab.Name;
            ABRemoteManager.DownloadAssetBundle(updateList, remoteAssetBUndleDownloadEntry, localAssetBundleStoragPath);
            var newList = from remoteab in remoteAssetBundleList
                            where !localAssetBundleList.Contains(remoteab)
                            select remoteab.Name;
            ABRemoteManager.DownloadAssetBundle(newList, remoteAssetBUndleDownloadEntry, localAssetBundleStoragPath);
            // 下载main包
            ABRemoteManager.DownloadMainAssetBundle(remoteAssetBUndleDownloadEntry, localAssetBundleStoragPath);
        }
        
        /// <summary>
        /// 若本地ab包中有远程版本没有的ab包, 说明该包应该要被删除
        /// </summary>
        private void ClearAssetBundle()
        {
            var deleteList = from localab in localAssetBundleList
                             where !remoteAssetBundleList.Contains(localab)
                             select localab.Name;
            foreach (var name in deleteList)
            {
                File.Delete(Path.Combine(localAssetBundleStoragPath, name));
                File.Delete(Path.Combine(localAssetBundleStoragPath, name) + ".manifest");
            }
            ABLocalManager.ClearEmptyDitectory(Application.persistentDataPath);
        }
        
       

    }
}


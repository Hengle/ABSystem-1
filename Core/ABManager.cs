using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace ABSystem
{
    public class ABManager : MonoBehaviour
    {
        // 远程管理器和设置
        public ABRemoteSetting RemoteSetting;
        private ABRemoteManager remoteManager;
        // 本地管理器和设置
        public ABLocalSetting LocalSetting;
        private ABLocalManager localManager;

        private string localVersion;    // 本地版本号
        private string remoteVersion;   // 远程版本号
        private List<AssetBundleInfo> localAssetBundleList;
        private List<AssetBundleInfo> remoteAssetBundleList;

        public static ABManager Instance;

        private void Awake()
        {
            Instance = this;
            remoteManager = new ABRemoteManager(RemoteSetting);
            localManager = new ABLocalManager(LocalSetting);
        }

        private void Start()
        {
            UpdateToNew();
        }

        /// <summary>
        /// 检查是否有新版本
        /// </summary>
        /// <returns></returns>
        public bool HasNewVersion
        {
            get
            {
                localVersion = localManager.Version;
                remoteVersion = remoteManager.Version;
                return localVersion.Equals(remoteVersion) ? false : true;
            }
        }

        public void UpdateToNew()
        {
            if (HasNewVersion)
            {
                // 读取本地ab包的清单
                localAssetBundleList = localManager.AseetBundleList;
                // 读取远程ab包的清单
                remoteAssetBundleList = remoteManager.AseetBundleList;
                IEnumerable<AssetBundleInfo> updateList;
                if (localAssetBundleList.Count == 0)
                {
                    updateList = remoteAssetBundleList;
                }
                else
                {
                    // 获取更新列表
                    updateList = from remoteab in remoteAssetBundleList
                                 from localab in localAssetBundleList
                                 where localab.HasNewVersion(remoteab) || !localAssetBundleList.Contains(remoteab)
                                 select remoteab;
                }
                // 开始下载
                remoteManager.DownloadAssetBundle(updateList, localManager);
                // 清空本地不用的ab包
                localManager.Clear(ABUtility.GetDeleteABList(localAssetBundleList, remoteAssetBundleList));
                // 写入新的信息文件
                localManager.Version = remoteVersion;
                localManager.AseetBundleList = remoteAssetBundleList;
            }
        }



    }
}


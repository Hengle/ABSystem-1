using System;
using System.IO;
using System.Collections.Generic;
using LitJson;
using UnityEngine;

namespace ABSystem
{
    [Serializable]
    public class ABLocalSetting
    {
        public string AssetBundlePath = "AssetBundles"; // 本地储存根目录名称
        public string DefaultVersion = "0.0.0"; // 当无法读取到本地版本信息时, 使用的默认版本
    }

    /// <summary>
    /// 本地信息管理器
    /// </summary>
    public class ABLocalManager
    {
        private string localAssetBundlePath;
        private string versionPath;
        private string assetBundleListPath;
        private ABLocalSetting setting;

        public ABLocalManager(ABLocalSetting setting)
        {
            this.setting = setting;
            localAssetBundlePath = Path.Combine(Application.persistentDataPath, setting.AssetBundlePath);
            DirectoryInfo dir = new DirectoryInfo(localAssetBundlePath);
            if(!dir.Exists)
            {
                dir.Create();
            }
            versionPath = Path.Combine(localAssetBundlePath, "Version.json");
            assetBundleListPath = Path.Combine(localAssetBundlePath, "ResourceList.json");
        }

        /// <summary>
        /// 版本信息相关, 底层使用的是特定的文件来储存json信息
        /// </summary>
        public string Version
        {
            get
            {
                if(!File.Exists(versionPath))
                {
                    Version = setting.DefaultVersion;
                }
                using (StreamReader st = new StreamReader(versionPath))
                {
                    string jsonStr = st.ReadLine();
                    return ABUtility.JsonToVersion(jsonStr);
                }
            }
            set
            {
                using (var sw = new StreamWriter(versionPath))
                {
                    VersionInfo v = new VersionInfo()
                    {
                        Version = value
                    };
                    sw.Write(JsonMapper.ToJson(v));
                }
            }
        }

        /// <summary>
        /// 本地ab包的信息列表相关
        /// </summary>
        public List<AssetBundleInfo> AseetBundleList
        {
            get
            {
                if (!File.Exists(assetBundleListPath))
                {
                    AseetBundleList = new List<AssetBundleInfo>();
                }
                using (StreamReader st = new StreamReader(assetBundleListPath))
                {
                    string localABInfo = st.ReadToEnd();
                    var localAseetBundleList = ABUtility.JsonToAseetBundleList(localABInfo);
                    return localAseetBundleList;
                }
            }
            set
            {
                using (var sw = new StreamWriter(assetBundleListPath))
                {
                    sw.Write(JsonMapper.ToJson(value));
                }
            }
        }

        /// <summary>
        /// 给定一个文件名, 尝试创建该文件所需的目录, 并返回包含文件名的完整路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string TryCreateDirectory(AssetBundleInfo abinfo)
        {
            string filePath = Path.Combine(localAssetBundlePath, abinfo.Name);
            DirectoryInfo dir = new DirectoryInfo(Path.GetDirectoryName(filePath));
            if(!dir.Exists)
            {
                dir.Create();
            }
            return filePath;
        }

        /// <summary>
        /// 给定一个文件名, 尝试创建该文件所需的目录, 并返回包含文件名的完整路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string TryCreateDirectory(string path)
        {
            string filePath = Path.Combine(localAssetBundlePath, path);
            DirectoryInfo dir = new DirectoryInfo(Path.GetDirectoryName(filePath));
            if (!dir.Exists)
            {
                dir.Create();
            }
            return filePath;
        }

        /// <summary>
        /// 清除不再使用的ab包
        /// </summary>
        /// <param name="deleteList"></param>
        public void Clear(IEnumerable<AssetBundleInfo> deleteList)
        {
            foreach (var abinfo in deleteList)
            {
                File.Delete(Path.Combine(localAssetBundlePath, abinfo.Name));
                File.Delete(Path.Combine(localAssetBundlePath, abinfo.Name + ".manifest"));
            }
            // 清除空目录
            DirectoryInfo dir = new DirectoryInfo(localAssetBundlePath);
            DirectoryInfo[] dirs = dir.GetDirectories("*", SearchOption.AllDirectories);
            foreach(DirectoryInfo subDir in dirs)
            {
                FileSystemInfo[] subFiles = subDir.GetFileSystemInfos();
                if (subFiles.Length == 0)
                {
                    subDir.Delete();
                }
            }
        }
    }
}


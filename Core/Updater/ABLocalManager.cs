using System;
using System.IO;
using System.Collections.Generic;
using LitJson;
using UnityEngine;
using ABSystem.Inner.Date;

namespace ABSystem
{
    public partial class ABUpdater
    {
        /// <summary>
        /// 本地管理器设置相关
        /// </summary>
        [Serializable]
        public class ABLocalSetting
        {
            public string AssetBundlePath = "AssetBundles"; // 本地储存根目录名称
            public string DefaultVersion = "0.0.0"; // 当无法读取到本地版本信息时, 使用的默认版本
        }

        /// <summary>
        /// 本地信息管理器
        /// </summary>
        private class ABLocalManager
        {
            public string LocalAssetBundlePath { get; private set; }
            private string versionPath;
            private string assetBundleListPath;

            public ABLocalManager(ABLocalSetting setting)
            {
                // 初始化储存目录
                LocalAssetBundlePath = Path.Combine(Application.persistentDataPath, setting.AssetBundlePath);
                DirectoryInfo dir = new DirectoryInfo(LocalAssetBundlePath);
                if (!dir.Exists) dir.Create();
                // 初始化Version.json文件
                versionPath = Path.Combine(LocalAssetBundlePath, "Version.json");
                if (!File.Exists(versionPath)) Version = setting.DefaultVersion;
                // 初始化ResourceList.json文件
                assetBundleListPath = Path.Combine(LocalAssetBundlePath, "ResourceList.json");
                if (!File.Exists(assetBundleListPath)) AseetBundleList = new List<ABInfo>();
            }

            /// <summary>
            /// 版本信息相关, 底层使用的是特定的文件来储存json信息
            /// </summary>
            public string Version
            {
                get
                {
                    using (StreamReader sr = new StreamReader(versionPath))
                    {
                        string jsonStr = sr.ReadLine();
                        return ABUtility.JsonToVersion(jsonStr);
                    }
                }
                set
                {
                    using (var sw = new StreamWriter(versionPath))
                    {
                        ABVersion v = new ABVersion()
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
            public List<ABInfo> AseetBundleList
            {
                get
                {
                    using (StreamReader st = new StreamReader(assetBundleListPath))
                    {
                        string localABInfo = st.ReadToEnd();
                        var localAseetBundleList = ABUtility.JsonToABList(localABInfo);
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
            /// 清除不再使用的ab包
            /// </summary>
            /// <param name="deleteList"></param>
            public void Clear(IEnumerable<ABInfo> oldList, IEnumerable<ABInfo> newList)
            {
                var deleteList = ABUtility.GetDeleteABList(oldList, newList);
                foreach (var name in deleteList)
                {
                    File.Delete(Path.Combine(LocalAssetBundlePath, name));
                    File.Delete(Path.Combine(LocalAssetBundlePath, name + ".manifest"));
                }
                // 清除空目录
                ABUtility.ClearEmtry(LocalAssetBundlePath);
            }

        }   // end class



    }

   
}


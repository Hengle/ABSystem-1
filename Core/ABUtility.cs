using System.Collections.Generic;
using System.Linq;
using LitJson;
using UnityEngine;

namespace ABSystem
{
    public class ABUtility
    {
        /// <summary>
        /// 将版本的json信息转为字符串
        /// </summary>
        /// <param name="jsonStrint"></param>
        /// <returns></returns>
        public static string JsonToVersion(string jsonString)
        {
            var versionInfo = JsonMapper.ToObject<VersionInfo>(jsonString);
            return versionInfo.Version;
        }

        /// <summary>
        /// 将ab包信息的json数组转为List;
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static List<AssetBundleInfo> JsonToAseetBundleList(string jsonString)
        {
            var aseetBundleList = new List<AssetBundleInfo>();
            JsonData JsonList = JsonMapper.ToObject(jsonString);
            foreach (JsonData item in JsonList)
            {
                aseetBundleList.Add(JsonMapper.ToObject<AssetBundleInfo>(item.ToJson()));
            }
            return aseetBundleList;
        }

        /// <summary>
        /// 从AssetBundles.manifest中, 生成ab包信息
        /// </summary>
        /// <param name="assetBundlePath"></param>
        /// <returns></returns>
        public static List<AssetBundleInfo> CreateABListFromManifest(AssetBundleManifest manifest)
        {
            string[] assetName = manifest.GetAllAssetBundles();
            var assetBundleList = new List<AssetBundleInfo>();
            foreach (var assetBundleName in assetName)
            {
                var abinfo = new AssetBundleInfo()
                {
                    Name = assetBundleName,
                    Hash = manifest.GetAssetBundleHash(assetBundleName).ToString()
                };
                assetBundleList.Add(abinfo);
            }
            return assetBundleList;
        }

        /// <summary>
        /// 获取删除列表
        /// </summary>
        /// <param name="oldList"></param>
        /// <param name="newList"></param>
        /// <returns></returns>
        public static IEnumerable<AssetBundleInfo> GetDeleteABList(List<AssetBundleInfo> oldList, List<AssetBundleInfo> newList)
        {
            var deleteList = oldList.Except(newList);
            return deleteList;
        }

    }
}



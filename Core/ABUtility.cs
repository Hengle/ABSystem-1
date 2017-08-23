using System.IO;
using System.Collections.Generic;
using System.Linq;
using LitJson;
using UnityEngine;
using ABSystem.Inner.Date;

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
            var versionInfo = JsonMapper.ToObject<ABVersion>(jsonString);
            if (versionInfo == null) throw new ABVersionJsonException("The json data about version can not parse.");
            return versionInfo.Version;
        }

        /// <summary>
        /// 将ab包信息的json数组转为List;
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static List<ABInfo> JsonToABList(string jsonString)
        {
            var aseetBundleList = new List<ABInfo>();
            JsonData JsonList = JsonMapper.ToObject(jsonString);
            if (JsonList == null) throw new ABInfoListJsonException("The json date about resource can not parse.");
            foreach (JsonData item in JsonList)
            {
                aseetBundleList.Add(JsonMapper.ToObject<ABInfo>(item.ToJson()));
            }
            return aseetBundleList;
        }

        /// <summary>
        /// 从AssetBundles.manifest中, 生成ab包信息
        /// </summary>
        /// <param name="assetBundlePath"></param>
        /// <returns></returns>
        public static List<ABInfo> CreateABListFromManifest(AssetBundleManifest manifest)
        {
            string[] assetBundleNames = manifest.GetAllAssetBundles();
            var assetBundleList = new List<ABInfo>();
            foreach (var name in assetBundleNames)
            {
                var abinfo = new ABInfo()
                {
                    Name = name,
                    Hash = manifest.GetAssetBundleHash(name).ToString()
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
        public static IEnumerable<string> GetDeleteABList(IEnumerable<ABInfo> oldList, IEnumerable<ABInfo> newList)
        {
            var ol = from o in oldList
                     select o.Name;
            var nl = from n in newList
                     select n.Name;
            var deleteList = from o in ol
                             from n in nl
                             where !nl.Contains(o)
                             select o;
            return deleteList;
        }

        /// <summary>
        /// 清除指定目录下的所有空目录
        /// </summary>
        /// <param name="rootPath"></param>
        public static void ClearEmtry(string rootPath)
        {
            DirectoryInfo dir = new DirectoryInfo(rootPath);
            DirectoryInfo[] dirs = dir.GetDirectories("*", SearchOption.AllDirectories);
            foreach (DirectoryInfo subDir in dirs)
            {
                FileSystemInfo[] subFiles = subDir.GetFileSystemInfos();
                if (subFiles.Length == 0) subDir.Delete();
            }
        }

        /// <summary>
        /// 给定一个文件名, 尝试创建该文件所需的目录, 并返回包含文件名的完整路径
        /// </summary>
        /// <param name="abinfo"></param>
        /// <param name="localAssetBundlePath"></param>
        /// <returns></returns>
        public static string TryCreateDirectory(ABInfo abinfo, string localAssetBundlePath)
        {
            return TryCreateDirectory(abinfo.Name, localAssetBundlePath);
        }

        /// <summary>
        /// 给定一个文件名, 尝试创建该文件所需的目录, 并返回包含文件名的完整路径
        /// </summary>
        /// <param name="path"></param>
        /// <param name="localAssetBundlePath"></param>
        /// <returns></returns>
        public static string TryCreateDirectory(string filename, string localAssetBundlePath)
        {
            var filePath = Path.Combine(localAssetBundlePath, filename);
            DirectoryInfo dir = new DirectoryInfo(Path.GetDirectoryName(filePath));
            if (!dir.Exists) dir.Create();
            return filePath;
        }

    }
}



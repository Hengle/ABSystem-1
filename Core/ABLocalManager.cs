using System.IO;
using System.Collections.Generic;
using LitJson;

namespace ABSystem
{
    /// <summary>
    /// 本地信息管理器
    /// </summary>
    public class ABLocalManager
    {
        /// <summary>
        /// 获取本地版本信息
        /// </summary>
        /// <param name="localVersionFilePath"></param>
        /// <returns></returns>
        public static string GetVersion(string localVersionFilePath)
        {
            using (StreamReader st = new StreamReader(localVersionFilePath))
            {
                string jsonStr = st.ReadLine();
                return ABUtility.JsonToVersion(jsonStr);
            }
        }

        /// <summary>
        /// 获取本地ab包的信息列表
        /// </summary>
        /// <param name="localAssetBundleListFilePath"></param>
        /// <returns></returns>
        public static List<AssetBundleInfo> GetAseetBundleList(string localAssetBundleListFilePath)
        {
            using (StreamReader st = new StreamReader(localAssetBundleListFilePath))
            {
                string localABInfo = st.ReadToEnd();
                var localAseetBundleList = ABUtility.JsonToAseetBundleList(localABInfo);
                return localAseetBundleList;
            }  
        }

        /// <summary>
        /// 创建版本信息文件
        /// </summary>
        /// <param name="versioninfo"></param>
        /// <param name="storagPath"></param>
        public static void CreateVersionFile(string versioninfo, string storagPath)
        {
            VersionInfo v = new VersionInfo();
            v.Version = versioninfo;
            var jsonStr = JsonMapper.ToJson(v);
            using (var sw = new StreamWriter(Path.Combine(storagPath, "Version.json")))
            {
                sw.Write(jsonStr);
            }
        }

        /// <summary>
        /// 创建ab包列表信息
        /// </summary>
        /// <param name="list"></param>
        /// <param name="storagPath"></param>
        public static void CreateAssetBundleListFile(List<AssetBundleInfo> list, string storagPath)
        {
            using (var sw = new StreamWriter(Path.Combine(storagPath, "ResourceList.json")))
            {
                sw.Write(JsonMapper.ToJson(list));
            }
        }

        /// <summary>
        /// 数据校验
        /// </summary>
        public static bool DataVerification()
        {
            // TODO: 进行数据校验
            return true;
        }

        /// <summary>
        /// 清除根目录
        /// </summary>
        /// <param name="rootPath"></param>
        public static void ClearEmptyDitectory(string rootPath)
        {

        }


    }
}


using System.Collections.Generic;
using LitJson;

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
    }
}



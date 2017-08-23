using UnityEditor;
using UnityEngine;
using System.IO;
using LitJson;
using ABSystem.Inner.Date;

namespace ABSystem
{
    public class CreateAssetBundles : EditorWindow
    {
        private static bool ISCreateVersionInfo = true;
        private static string Version;
        private static bool IsCreateResourceList = true;
        private static string OutputPath = "AssetBundles";

        [MenuItem("ABSystem/Create AssetBundles")]
        static void ShowWindow()
        {
            GetWindow(typeof(CreateAssetBundles), true, "Create AssetBundles");
        }

        private void OnGUI()
        {
            GUILayout.Label("Settings", EditorStyles.boldLabel);
            GUILayout.Label(string.Format("Current Version: {0}", GetCurrentVersion()), EditorStyles.helpBox);
            ISCreateVersionInfo = EditorGUILayout.Toggle("Create Version Info", ISCreateVersionInfo);
            if (ISCreateVersionInfo)
            {
                Version = EditorGUILayout.TextField("Version", Version);
            }
            IsCreateResourceList = EditorGUILayout.Toggle("Create Resource List", IsCreateResourceList);
            if (GUILayout.Button("Create"))
            {
                Create();
            }
            if (GUILayout.Button("Clear"))
            {
                Clear();
            }

        }

        private static string GetCurrentVersion()
        {
            string filePath = Path.Combine(OutputPath, "Version.json");
            if (!File.Exists(filePath))
            {
                return "UnKnow";
            }
            else
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    return JsonMapper.ToObject<ABVersion>(sr.ReadToEnd()).Version;
                }
            }
        }

        private void Create()
        {
            if (string.IsNullOrEmpty(Version))
            {
                throw new InvalidDataException("必须填写版本号");
            }
            if(Version == GetCurrentVersion())
            {
                Debug.LogWarning("不使用新的版本号将导致新的版本无法被察觉, 从而导致无法升级");
            }
            if (!Directory.Exists(OutputPath))
            {
                Directory.CreateDirectory(OutputPath);
                var manifest = BuildPipeline.BuildAssetBundles(OutputPath, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
                if(manifest)
                {
                    CreateResourceListJsonFile(manifest);
                    CreateVersionJsonFile();
                    ABUtility.ClearEmtry(OutputPath);
                }
                else
                {
                    Clear();
                }
                
            }
            else
            {
                var ab = AssetBundle.LoadFromFile(Path.Combine(OutputPath, "AssetBundles"));
                var oldManifest = ab.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                var oldABList = ABUtility.CreateABListFromManifest(oldManifest);
                ab.Unload(true);
                var newManifest = BuildPipeline.BuildAssetBundles(OutputPath, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
                if(newManifest)
                {
                    CreateResourceListJsonFile(newManifest);
                    CreateVersionJsonFile();
                    var newABList = ABUtility.CreateABListFromManifest(newManifest);
                    var deleteList = ABUtility.GetDeleteABList(oldABList, newABList);
                    foreach (var name in deleteList)
                    {
                        File.Delete(Path.Combine(OutputPath, name));
                        File.Delete(Path.Combine(OutputPath, name + ".manifest"));
                    }
                    ABUtility.ClearEmtry(OutputPath);
                }
                else
                {
                    Clear();
                }
            }
            
        }

        /// <summary>
        /// 生成Version.json信息
        /// </summary>
        private void CreateVersionJsonFile()
        {
            if (ISCreateVersionInfo)
            {
                JsonData versionJson = new JsonData();
                versionJson["Version"] = Version;
                string versionJsonStr = JsonMapper.ToJson(versionJson);
                using (StreamWriter sw = new StreamWriter(Path.Combine(OutputPath, "Version.json")))
                {
                    sw.Write(versionJsonStr);
                }
            }
        }

        /// <summary>
        /// 生成ResourceList.json信息
        /// </summary>
        private void CreateResourceListJsonFile(AssetBundleManifest manifest)
        {
            if (IsCreateResourceList)
            {
                var abList = ABUtility.CreateABListFromManifest(manifest);
                string jsonStr = JsonMapper.ToJson(abList);
                using (StreamWriter sw = new StreamWriter(Path.Combine(OutputPath, "ResourceList.json")))
                {
                    sw.Write(jsonStr);
                }
            }
        }

        /// <summary>
        /// 清空输出目录
        /// </summary>
        private void Clear()
        {
            Directory.Delete(OutputPath, true);
        }
    }

}

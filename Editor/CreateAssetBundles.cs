using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using LitJson;

namespace ABSystem
{
    public class CreateAssetBundles : EditorWindow
    {
        private static bool ISCreateVersionInfo;
        private static string Version;
        private static bool IsCreateResourceList;

        [MenuItem("ABSystem/Create AssetBundles")]
        static void ShowWindow()
        {
            GetWindow(typeof(CreateAssetBundles), true, "Create AssetBundles");
        }

        private void OnGUI()
        {
            GUILayout.Label("Settings", EditorStyles.boldLabel);
            ISCreateVersionInfo = EditorGUILayout.Toggle("Create Version Info", ISCreateVersionInfo);
            if(ISCreateVersionInfo)
            {
                Version = EditorGUILayout.TextField("Version", Version);
            }
            IsCreateResourceList = EditorGUILayout.Toggle("Create Resource List", IsCreateResourceList);
            if(GUILayout.Button("Create"))
            {
                Create();
            }

        }

        void Create()
        {
            string path = "AssetBundles";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);
            if(IsCreateResourceList)
            {
                // 生成ResourceList.json信息
                var assetBundle = AssetBundle.LoadFromFile(Path.Combine(path, "AssetBundles"));
                var manifest = assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                string[] assetName = manifest.GetAllAssetBundles();
                JsonData jsonList = new JsonData();
                foreach (var assetBundleName in assetName)
                {
                    var tempJson = new JsonData();
                    tempJson["Name"] = assetBundleName;
                    tempJson["Hash"] = manifest.GetAssetBundleHash(assetBundleName).ToString();
                    jsonList.Add(tempJson);
                }
                string jsonStr = jsonList.ToJson();
                using (StreamWriter sw = new StreamWriter(Path.Combine(path, "ResourceList.json")))
                {
                    sw.Write(jsonStr);
                }
            }
            if(ISCreateVersionInfo)
            {
                JsonData versionJson = new JsonData();
                versionJson["Version"] = Version;
                string versionJsonStr = JsonMapper.ToJson(versionJson);
                using (StreamWriter sw = new StreamWriter(Path.Combine(path, "Version.json")))
                {
                    sw.Write(versionJsonStr);
                }
            }
            // 生成Version.json信息
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class BuildAssetBundle : Editor
{
    [MenuItem("Bubble/打包")]
    static void BuildAB()
    {
        string assetBundleDirectory = "Assets/AssetBundle";
        if (!Directory.Exists(assetBundleDirectory))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }

        BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);
    }
    
    [MenuItem("Assets/Bubble/创建ABSetting",false,2)]
    static void CreateAssetAbSetting()
    {
        for (int i = 0; i < Selection.assetGUIDs.Length; i++)
        {
            var abAsset = ScriptableObject.CreateInstance<ABAssetSetting>();
            var path =  AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[i]);
            var directoryPath = IOTools.GetDirectoryPath(path);
            var assetSettingPath = directoryPath + "/ABAssetSetting.asset";
            if (File.Exists(assetSettingPath))
            {
                continue;
            }
            AssetDatabase.CreateAsset(abAsset, assetSettingPath);
            AssetDatabase.SaveAssets(); //存储资源
            AssetDatabase.Refresh(); //刷新
        }
    }


}

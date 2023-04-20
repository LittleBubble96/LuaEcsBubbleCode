using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bubble.Tools;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public static class AssetUtility
{
   
    public static List<AssetProcessor> LoadAssetProcessors(EAssetProcessStep processStep)
    {
        //6=>00000110
        //2=>00000010
        //2&6 => 00000010
        var list = ReflectionTools.GetInstancesOfSubClass<AssetProcessor>()
            .Where((t) => (t.AssetProcessStep & processStep) != 0).ToList();
        foreach (var lProcessor in list)
        {
            lProcessor.LoadAllAssetSettings();
        }
        return list;
    }

    public static void ImportAssets(string[] importedAssets,List<AssetProcessor> processors)
    {
        if (importedAssets.Length <= 0)
        {
            return;
        }
        using (BubbleEditorProgress.CreateProgress("全局资源加载", importedAssets.Length))
        {
            List<UnityEngine.Object> dirtyObjs = new List<Object>();
            for (int i = 0; i < importedAssets.Length; i++)
            {
                if (string.IsNullOrEmpty(importedAssets[i]))
                {
                    continue;
                }

                AssetImporter importer = AssetImporter.GetAtPath(importedAssets[i]);
                if (importer == null)
                {
                    continue;
                }
                if (!BubbleEditorProgress.Display("全局资源加载:",i,importedAssets.Length))
                {
                    Debug.Log("资源加载取消于:" + importedAssets[i]);
                    break;
                }

                foreach (var processor in processors)
                {
                    UnityEngine.Object dirtyObj = processor.Progress(importer, importedAssets[i]);
                    if (dirtyObj != null)
                    {
                        dirtyObjs.Add(dirtyObj);
                    }
                }
            }

            foreach (var dirty in dirtyObjs)
            {
                EditorUtility.SetDirty(dirty);
            }
        }
      
    }
}

//
[Flags]
public enum EAssetProcessStep
{
    None = 0 ,  //无
    Manual = 1<<0,  //手动的 
    Import= 1<<1,//自动的
    All  = -1//全部
}

public abstract class AssetProcessor
{
    private List<AssetSettingProxy> listSetting = new List<AssetSettingProxy>();
    //获取资源设置的Filter 
    public abstract string GetFilter();

    public abstract EAssetProcessStep AssetProcessStep { get; }

    public UnityEngine.Object Progress(AssetImporter importer , string assetPath)
    {
        if (importer == null)
        {
            return null;
        }

        if (string.IsNullOrEmpty(assetPath))
        {
            return null;
        }
        AssetSettingProxy proxy = GetMatchSetting(assetPath);
        return Progress(importer,assetPath,proxy);
    }

    public abstract UnityEngine.Object Progress(AssetImporter importer, string assetPath, AssetSettingProxy settingProxy);

    public void LoadAllAssetSettings()
    {
        var guids  = AssetDatabase.FindAssets(GetFilter());
        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var assetSetting = AssetDatabase.LoadAssetAtPath<AssetSetting>(path);
            if (assetSetting!=null)
            {
                AssetSettingProxy proxy = new AssetSettingProxy(assetSetting, path);
                listSetting.Add(proxy);
                // Debug.Log("获取到全局资源设置路径为:" + path);
            }
        }
    }

    private AssetSettingProxy GetMatchSetting(string assetPath)
    {
        return listSetting.Find((s) => assetPath.StartsWith(s.path));
    }
}

/// <summary>
/// 文件设置信息代理
/// </summary>
public class  AssetSettingProxy
{
    private AssetSetting setting;
    /// <summary>
    /// 用于匹配对应的文件夹的资源设置
    /// </summary>
    public string path { get; set; }

    public AssetSettingProxy(AssetSetting assetSetting , string path)
    {
        setting = assetSetting;
        this.path = IOTools.GetDirectoryPath(path);
        // Debug.Log("资源设置文件的文件夹位置为： " + this.path);
    }

    public T GetSetting<T>() where  T : AssetSetting
    {
        return setting as T;
    }
    
}


/// <summary>
/// 文件夹打包设置
/// </summary>
public class AssetSetting : ScriptableObject
{
    public bool exclude;

    public string[] ignores;

}



/// <summary>
/// 导入资源设置
/// </summary>
internal class  AssetPostProcessor : AssetPostprocessor
{
    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
        string[] movedFromAssetPaths)
    {
        var list = AssetUtility.LoadAssetProcessors(EAssetProcessStep.Import);
        AssetUtility.ImportAssets(importedAssets,list);
        AssetUtility.ImportAssets(movedAssets,list);
    }
}



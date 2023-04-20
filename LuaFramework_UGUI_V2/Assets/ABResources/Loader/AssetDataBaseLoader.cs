using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class AssetDataBaseLoader : ILoader
{

    /// <summary>
    /// 加载图片
    /// </summary>
    /// <param name="assetBundleName"></param>
    /// <param name="spriteName"></param>
    /// <returns></returns>
    public Sprite LoadSprite(string assetBundleName , string spriteName)
    {
        string assetPath = GetPathByAbName(assetBundleName);
        Object[] os =  AssetDatabase.LoadAllAssetsAtPath(assetPath);
        foreach (var o in os)
        {
            if (o is Sprite s && spriteName == s.name)
            {
                return s;
            }
        }
        return null;
    }

    public T Load<T>(string assetPath , bool package) where T : UnityEngine.Object
    {

         string assetName = Path.GetFileName(assetPath);
         string assetBundleName = SpecificationAbName(assetPath , package);

         string path = GetPathByAbName(assetBundleName,assetName);

        return LoadAssetAtPath<T>(path);
    }

    T LoadAssetAtPath<T>(string path) where T : UnityEngine.Object
    {
        return AssetDatabase.LoadAssetAtPath<T>(path);
    }

    string GetPathByAbName(string abName , string assetName = "")
    {
        string[] assetPaths = AssetDatabase.GetAssetPathsFromAssetBundle(abName);
        if (assetPaths == null || assetPaths.Length <= 0)
        {
            Debug.LogError("加载得文件路径不对 abName ： "+ abName + "  assetName:" + assetName);
            return "";
        }
        if (string.IsNullOrEmpty(assetName))
        {
            return assetPaths[0];
        }
        else
        {
            return System.Array.Find( assetPaths ,(a)=> a.Contains(assetName));
        }
    }

    private string SpecificationAbName(string fileName , bool package = false)
    {
        if (package)
        {
            return IOTools.GetDirectoryPath(fileName).ToLower();
        }
        int lastIndex = fileName.LastIndexOf('.');
        lastIndex = lastIndex <= 0 ? fileName.Length : lastIndex;
        return fileName.Substring(0, lastIndex).ToLower();
    }
}


using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ABAssetProcesser : AssetProcessor
{

    private string[] _ignoreExtensions = {".controller", ".anim", ".fbx"};

    public override string GetFilter()
    {
        return $"t:ABAssetSetting";
    }
    
    public override EAssetProcessStep AssetProcessStep => EAssetProcessStep.All;
    
    public override Object Progress(AssetImporter importer, string assetPath, AssetSettingProxy settingProxy)
    {
        //到此步 importer 和 assetPath 都不为空

        if (settingProxy == null)
        {
            if (!string.IsNullOrEmpty(importer.assetBundleName))
                importer.SetAssetBundleNameAndVariant("", "");
            return null;
        }

        if (Directory.Exists(assetPath))
        {
            //该路径为文件夹
            if(!string.IsNullOrEmpty(importer.assetBundleName))
                importer.SetAssetBundleNameAndVariant("","");
            return null;
        }
        Debug.Log("ABAssetProcesser设置的资源路径为："+assetPath);
        var setting = settingProxy.GetSetting<ABAssetSetting>();
        //文件设置忽略信息 内包含忽略字段就设置为空
        if (setting == null || setting.exclude || setting.ignores.Count(assetPath.Contains) > 0)
        {
            if(!string.IsNullOrEmpty(importer.assetBundleName))
                importer.SetAssetBundleNameAndVariant("","");
            return null;
        }
        
        //需要忽略的文件信息不设置abname
        var extension = IOTools.GetExtensionByBath(assetPath);
        if (_ignoreExtensions.Contains(extension))
        {
            if(!string.IsNullOrEmpty(importer.assetBundleName))
                importer.SetAssetBundleNameAndVariant("","");
            return null;
        }

        if (assetPath.Contains("ABAssetSetting"))
        {
            if(!string.IsNullOrEmpty(importer.assetBundleName))
                importer.SetAssetBundleNameAndVariant("","");
            return null;
        }

        string lowName = SpecificationAbName(assetPath , setting.package);
        //正常需要设置ab Name
        importer.SetAssetBundleNameAndVariant(lowName,"");
        return null;
    }

    private string SpecificationAbName(string fileName , bool package = false )
    {
        int firstIndex = fileName.IndexOf('/') + 1;
        if (package)
        {
            string directory = IOTools.GetDirectoryPath(fileName);
            return directory.Substring(firstIndex, directory.Length - firstIndex).ToLower();
        }
        int lastIndex = fileName.LastIndexOf('.');
        return fileName.Substring(firstIndex, lastIndex - firstIndex).ToLower();
    }
}

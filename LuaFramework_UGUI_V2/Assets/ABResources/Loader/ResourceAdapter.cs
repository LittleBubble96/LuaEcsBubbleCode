using System.IO;
using System.Runtime.Versioning;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using XLua;

// [LuaCallCSharp]
public class ResourceAdapter 
{

    private ILoader _loader;

    static ResourceAdapter instance;


    ResourceAdapter()
    {
        #if UNITY_EDITOR
            _loader = new AssetDataBaseLoader();
        #else
            _loader = new AssetBundleLoader();
        #endif
    }

    
    #region Lua接口
    // public void LuaCall_LoadAsset(string assetPath ,bool package = false, bool resoucrce = false , LuaFunction func = null)
    // {
    //     Object o = LoadAsset<UnityEngine.Object>(assetPath,package,resoucrce);
    //     if (o)
    //     {
    //         func?.Call(o);
    //     }
    // }
    //
    // public void LuaCall_LoadSprite(string assetBundleName, string spriteName, LuaFunction func = null)
    // {
    //     Sprite s = LoadSprite(assetBundleName, spriteName);
    //     if (s)
    //     {
    //         func?.Call(s);
    //     }
    // }

    #endregion

    public Sprite LoadSprite(string assetBundleName , string spriteName)
    {
        return _loader.LoadSprite(assetBundleName, spriteName);
    }

    public T LoadAsset<T>(string assetPath ,bool package = false, bool resource = false)  where T : UnityEngine.Object
    {
        T asset = LoadPrefabAsset<T>(assetPath, package, resource);
        T view = GameObject.Instantiate(asset);
        string assetName = Path.GetFileNameWithoutExtension(assetPath);
        view.name = assetName;
        return view;
    }
    
    public T LoadPrefabAsset<T>(string assetPath ,bool package = false, bool resource = false)  where T : UnityEngine.Object
    {
        if(string.IsNullOrEmpty(assetPath))
        {
            return null;
        }
        T view = null;
        if(resource)
        {
            view = Resources.Load<T>(assetPath);
        }
        else
        {
            view = _loader.Load<T>(assetPath , package);
        }
        return view;
    }

    
   

    public static  ResourceAdapter GetInstance()
    {
        if (instance == null)
        {
            instance = new ResourceAdapter();
        }
        return instance;
    }


}
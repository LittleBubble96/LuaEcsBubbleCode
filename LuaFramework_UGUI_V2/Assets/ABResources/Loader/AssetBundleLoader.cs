using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetBundleLoader : ILoader
{
    public T Load<T>(string assetBundleName, bool package) where T : UnityEngine.Object
    {
        return default(T);
    }

    public Sprite LoadSprite(string assetBundleName, string spriteName)
    {
        return null;
    }
}


using UnityEngine;

public static class ResourceAdapterWarpper
{
    public static GameObject LoadAsset(string assetPath ,bool package = false, bool resource = false)
    {
        var result = ResourceAdapter.GetInstance().LoadAsset<GameObject>(assetPath, package, resource);
        return result;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILoader
{
   T Load<T>(string assetBundleName, bool package)  where T : UnityEngine.Object;
   Sprite LoadSprite(string assetBundleName, string spriteName);

}

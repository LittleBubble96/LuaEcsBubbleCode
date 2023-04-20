using System.IO;
using UnityEngine;

public static class LuaConst
{
  
    public static string luaDir = @$"{System.Environment.CurrentDirectory}" + @"\PublishResources\lua\LuaFramework\Lua";                //lua逻辑代码目录
    public static string luaProductDir = @$"{System.Environment.CurrentDirectory}" + @"\PublishResources\lua\product";                //lua项目代码目录
    public static string toluaDir = @$"{System.Environment.CurrentDirectory}" + @"\PublishResources\lua\LuaFramework\ToLua\Lua";        //tolua lua文件目录
    public static string toluaAPIDir = @$"{System.Environment.CurrentDirectory}" + @"\PublishResources\lua\lua_api";        //lua_api文件目录

    public static string toAssetluaDir = Application.dataPath + "/LuaFramework/ToLua/Lua";        //tolua Asset文件目录
    public static string luaAPIDir = "PublishResources/lua/lua_api";                    //emmy lua api文件目录

#if UNITY_STANDALONE
    public static string osDir = "Win";
#elif UNITY_ANDROID
    public static string osDir = "Android";            
#elif UNITY_IPHONE
    public static string osDir = "iOS";        
#else
    public static string osDir = "";        
#endif

    public static string luaResDir = string.Format("{0}/{1}/Lua", Application.persistentDataPath, osDir);      //手机运行时lua文件下载目录    

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN    
    public static string zbsDir = "D:/ZeroBraneStudio/lualibs/mobdebug";        //ZeroBraneStudio目录       
#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
	public static string zbsDir = "/Applications/ZeroBraneStudio.app/Contents/ZeroBraneStudio/lualibs/mobdebug";
#else
    public static string zbsDir = luaResDir + "/mobdebug/";
#endif    

    public static bool openLuaSocket = true;            //是否打开Lua Socket库
    public static bool openLuaDebugger = false;         //是否连接lua调试器
}

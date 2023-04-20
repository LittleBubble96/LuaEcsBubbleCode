using LuaInterface;

public class AppLuaProxy
{
    public static void OnUpdate(LuaFunction onUpdate)
    {
        ScriptsManager.GetInstance.RegisterUpdate(onUpdate);
    }
    
    public static void OnFixedUpdate(LuaFunction onFixedUpdate)
    {
        ScriptsManager.GetInstance.RegisterFixedUpdate(onFixedUpdate);
    }
    
    public static void OnLateUpdate(LuaFunction onLateUpdate)
    {
        ScriptsManager.GetInstance.RegisterLateUpdate(onLateUpdate);
    }
}
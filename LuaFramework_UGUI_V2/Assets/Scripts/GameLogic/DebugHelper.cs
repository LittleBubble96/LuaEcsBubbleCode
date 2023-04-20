using UnityEngine;
using BaseUtil;
using LuaFramework;
using LuaInterface;

class DebugHelper
{
	static bool m_Inited = false;
	public static void InitLogWrapper(string logLevel)
	{
		if(m_Inited)
		{
			return;
		}
		if (System.Enum.TryParse(logLevel, true, out LogHelper.Level ElogLevel))
		{
			m_Inited = true;
			LogWrapper.SetLevel(logLevel.ToLower());
#if UNITY_EDITOR
            LogWrapper.SetConsole(true);
#elif UNITY_ANDROID || UNITY_IOS
			LogWrapper.SetConsole(false);
#else
			LogWrapper.SetConsole(false);
#endif
			LogWrapper.SetKey("assert", true);
			LogWrapper.SetKey("exception", true);
			LogWrapper.SetKey("profile", true);
			Application.logMessageReceived += OnLogCallback;
		}
	}

	private static void OnLogCallback(string message, string stacktrace, LogType type)
	{
		if (type == LogType.Exception)
		{
			LogWrapper.LogError("DebugHelper.OnLogCallback ,", message, "\n", stacktrace);
			ShowExceptionMsgBoxImp(message, stacktrace);
		}
	}
	private static LuaFunction m_ShowException;
	private static void ShowExceptionMsgBoxImp(string message, string stacktrace)
	{
		if(m_ShowException == null)
		{
			LuaManager mgr = AppFacade.Instance.GetManager<LuaManager>(ManagerName.Lua);
			if (mgr != null) m_ShowException = mgr.GetLuaState()?.GetFunction("Log.showexception");
			if(m_ShowException == null)
			{
				LogWrapper.LogError(() => $"DebugHelper.ShowExceptionMsgBoxImp not find func, msg: {message},stacktrace: {stacktrace}");
				return;
			}
		}
		m_ShowException?.Call(message, System.Environment.CurrentDirectory);
	}
	private static void OnExceptionMsgBoxOk(object obj)
	{
#if !UNITY_IOS
		Application.Quit();
#endif
	}
	public static void EnableUWA(bool flag)
	{      
			//GameObject UWA_Launcher = GameObject.Find("UWA_Launcher");
			//UIHelper.SetActive(UWA_Launcher,flag);
			//if(UWA_Launcher.activeSelf){
			 //	UWA_Launcher.SetActive(flag);
			// }
			// else{
			//	 UWA_Launcher.SetActive(false);
			// }
	}
}

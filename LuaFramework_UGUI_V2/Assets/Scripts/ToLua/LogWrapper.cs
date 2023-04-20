#if (UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
#define USE_UNITY_LOG 
#endif

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
public static class LogWrapper
{
	public static string Args2Str(params object[] args)
	{
		string str = "";
		foreach(var arg in args) str += arg == null ? $" <null>" : $" {arg}";
		return str;
	}
	private static readonly bool s_DefaultConsole = BaseUtil.LogHelper.ToConsole.Enable = true;
	private static readonly HashSet<string> s_EnableKeys = new HashSet<string>();
	private static bool s_EnableAssert = false;
	private static bool s_EnableException = false;
	private static bool s_EnableProfile = false;
	public static bool SetLevel(string level)
	{
		if(level == "all")
		{
			BaseUtil.LogHelper.s_LogLevel = BaseUtil.LogHelper.Level.All;
			return true;
		}
		else if(level == "trace")
		{
			BaseUtil.LogHelper.s_LogLevel = BaseUtil.LogHelper.Level.Trace;
			return true;
		}
		else if(level == "debug")
		{
			BaseUtil.LogHelper.s_LogLevel = BaseUtil.LogHelper.Level.Debug;
			return true;
		}
		else if(level == "info")
		{
			BaseUtil.LogHelper.s_LogLevel = BaseUtil.LogHelper.Level.Info;
			return true;
		}
		else if(level == "warning")
		{
			BaseUtil.LogHelper.s_LogLevel = BaseUtil.LogHelper.Level.Warning;
			return true;
		}
		else if(level == "error")
		{
			BaseUtil.LogHelper.s_LogLevel = BaseUtil.LogHelper.Level.Error;
			return true;
		}
		else if(level == "fatal")
		{
			BaseUtil.LogHelper.s_LogLevel = BaseUtil.LogHelper.Level.Fatal;
			return true;
		}
		else if(level == "none")
		{
			BaseUtil.LogHelper.s_LogLevel = BaseUtil.LogHelper.Level.None;
			return true;
		}
		return false;
	}
	public static string GetLevel()
	{
		switch(BaseUtil.LogHelper.s_LogLevel)
		{
		case BaseUtil.LogHelper.Level.All: return "all";
		case BaseUtil.LogHelper.Level.Trace: return "trace";
		case BaseUtil.LogHelper.Level.Debug: return "debug";
		case BaseUtil.LogHelper.Level.Info: return "info";
		case BaseUtil.LogHelper.Level.Warning: return "warning";
		case BaseUtil.LogHelper.Level.Error: return "error";
		case BaseUtil.LogHelper.Level.Fatal: return "fatal";
		case BaseUtil.LogHelper.Level.None: return "none";
		}
		return "";
	}
	public static bool SetPath(string path)
	{
		BaseUtil.LogHelper.ToFile.Path = path;
		return BaseUtil.LogHelper.ToFile.Path == path;
	}
	public static string GetPath()
	{
		return BaseUtil.LogHelper.ToFile.Path;
	}
	public static void SetConsole(bool enable)
	{
		BaseUtil.LogHelper.ToConsole.Enable = enable;
	}
	public static bool GetConsole()
	{
		return BaseUtil.LogHelper.ToConsole.Enable;
	}
	public static bool SetKey(string key, bool enable)
	{
		if(enable)
		{
			if(s_EnableKeys.Contains(key)) return false;
			s_EnableKeys.Add(key);
			if(key == "assert") s_EnableAssert = true;
			else if(key == "exception") s_EnableException = true;
			else if(key == "profile") s_EnableProfile = true;
		}
		else
		{
			if(!s_EnableKeys.Contains(key)) return false;
			s_EnableKeys.Remove(key);
			if(key == "assert") s_EnableAssert = false;
			else if(key == "exception") s_EnableException = false;
			else if(key == "profile") s_EnableProfile = false;
		}
		return true;
	}
	public static bool GetKey(string key)
	{
		return s_EnableKeys.Contains(key);
	}
	public static void ResetKeys()
	{
		s_EnableKeys.Clear();
	}
	[Conditional("USE_LOGWRAPPER")]
	public static void LogTrace(string msg)
	{
		BaseUtil.LogHelper.Trace(msg);
	}
	[Conditional("USE_LOGWRAPPER")]
	public static void LogDebug(string msg)
	{
		BaseUtil.LogHelper.Debug(msg);
	}
	[Conditional("USE_LOGWRAPPER")]
	public static void LogInfo(string msg)
	{
		BaseUtil.LogHelper.Info(msg);
	}
	[Conditional("USE_LOGWRAPPER")]
	public static void LogWarning(string msg)
	{
		BaseUtil.LogHelper.Warning(msg);
	}
	[Conditional("USE_LOGWRAPPER")]
	public static void LogError(string msg)
	{
		BaseUtil.LogHelper.Error(msg);
	}
	[Conditional("USE_LOGWRAPPER")]
	public static void LogFatal(string msg)
	{
		BaseUtil.LogHelper.Fatal(msg);
	}
	[Conditional("USE_LOGWRAPPER")]
	public static void LogTrace(Func<string> func)
	{
		BaseUtil.LogHelper.Trace(func);
	}
	[Conditional("USE_LOGWRAPPER")]
	public static void LogDebug(Func<string> func)
	{
		BaseUtil.LogHelper.Debug(func);
	}
	[Conditional("USE_LOGWRAPPER")]
	public static void LogInfo(Func<string> func)
	{
		BaseUtil.LogHelper.Info(func);
	}
	[Conditional("USE_LOGWRAPPER")]
	public static void LogWarning(Func<string> func)
	{
		BaseUtil.LogHelper.Warning(func);
	}
	[Conditional("USE_LOGWRAPPER")]
	public static void LogError(Func<string> func)
	{
		BaseUtil.LogHelper.Error(func);
	}
	[Conditional("USE_LOGWRAPPER")]
	public static void LogFatal(Func<string> func)
	{
		BaseUtil.LogHelper.Fatal(func);
	}
	[Conditional("USE_LOGWRAPPER")]
	public static void LogTrace(params object[] args)
	{
		BaseUtil.LogHelper.Trace(() => Args2Str(args));
	}
	[Conditional("USE_LOGWRAPPER")]
	public static void LogDebug(params object[] args)
	{
		BaseUtil.LogHelper.Debug(() => Args2Str(args));
	}
	[Conditional("USE_LOGWRAPPER")]
	public static void LogInfo(params object[] args)
	{
		BaseUtil.LogHelper.Info(() => Args2Str(args));
	}
	[Conditional("USE_LOGWRAPPER")]
	public static void LogWarning(params object[] args)
	{
		BaseUtil.LogHelper.Warning(() => Args2Str(args));
	}
	[Conditional("USE_LOGWRAPPER")]
	public static void LogError(params object[] args)
	{
		BaseUtil.LogHelper.Error(() => Args2Str(args));
	}
	[Conditional("USE_LOGWRAPPER")]
	public static void LogFatal(params object[] args)
	{
		BaseUtil.LogHelper.Fatal(() => Args2Str(args));
	}
	[Conditional("USE_LOGWRAPPER")]
	public static void LogAssert(bool cond, string msg = "")
	{
		if(cond || !s_EnableAssert || null == BaseUtil.LogHelper.LogFatal) return;
		BaseUtil.LogHelper.LogFatal.Invoke($"[Assert] {msg}{Environment.NewLine}{BaseUtil.Invoked.stack}");
		Trace.Assert(cond, msg);
	}
	[Conditional("USE_LOGWRAPPER")]
	public static void LogAssert(bool cond, Func<string> func)
	{
		if(cond || !s_EnableAssert || null == BaseUtil.LogHelper.LogFatal) return;
		BaseUtil.LogHelper.LogFatal.Invoke($"[Assert] {func()}{Environment.NewLine}{BaseUtil.Invoked.stack}");
		Trace.Assert(cond, func());
	}
	[Conditional("USE_LOGWRAPPER")]
	public static void LogException(Exception e)
	{
		if(!s_EnableException) return;
#if USE_UNITY_LOG
		BaseUtil.LogHelper.ToFile.Error($"[Exception] {e}");
		UnityEngine.Debug.LogException(e);
#else
		BaseUtil.LogHelper.LogError.Invoke($"[Exception] {e}");
#endif
	}
	[Conditional("USE_LOGWRAPPER")]
	public static void LogKey(string key, string msg)
	{
		if(!GetKey(key) || null == BaseUtil.LogHelper.LogInfo) return;
		BaseUtil.LogHelper.LogInfo.Invoke($"[{key}] {msg}");
	}
	[Conditional("USE_LOGWRAPPER")]
	public static void LogKey(string key, Func<string> func)
	{
		if(!GetKey(key) || null == BaseUtil.LogHelper.LogInfo) return;
		BaseUtil.LogHelper.LogInfo.Invoke($"[{key}] {func()}");
	}
	[Conditional("USE_LOGWRAPPER")]
	public static void LogProf(string msg)
	{
		if(!s_EnableProfile || null == BaseUtil.LogHelper.LogInfo) return;
		BaseUtil.LogHelper.LogInfo.Invoke($"[Profile] {msg}");
	}
	[Conditional("USE_LOGWRAPPER")]
	public static void LogProf(Func<string> func)
	{
		if(!s_EnableProfile || null == BaseUtil.LogHelper.LogInfo) return;
		BaseUtil.LogHelper.LogInfo.Invoke($"[Profile] {func()}");
	}
}

namespace BaseUtil
{
	public static class Invoked
	{
		// 获取当前类名
		public static string cls => new StackTrace(new StackFrame(1, false)).GetFrame(0).GetMethod().DeclaringType?.FullName;
		// 获取当前方法名
		public static string mod => new StackTrace(new StackFrame(1, false)).GetFrame(0).GetMethod().Name;
		// 获取当前调用堆栈
		public static string stack => $"{new StackTrace(true)}";
	}
    /// <summary>
    /// first invoked must belong the main-thread of unity
    /// </summary>
	public static class LogHelper
	{
		#region input control
		public enum Level
		{
			All, // 不过滤
			Trace, // 函数出入口
			Debug, // 调试
			Info, // 重要信息
			Warning, // 警告
			Error, // 错误
			Fatal, // 致命错误
			None, // 全过滤
		}

        private static readonly object s_OperateLock = new object();
		public static volatile Level s_LogLevel = Level.All;
		// first init must belong the main-thread of unity
		public static readonly int s_MainThreadId = Thread.CurrentThread.ManagedThreadId;
		public static bool EnableLevel(Level level) => s_LogLevel <= level;

		public static void Trace(string msg)
		{
			if(!EnableLevel(Level.Trace) || null == LogTrace) return;
			LogTrace.Invoke(msg);
        }
		public static void Debug(string msg)
		{
			if(!EnableLevel(Level.Debug) || null == LogDebug) return;
			LogDebug.Invoke(msg);
		}
		public static void Info(string msg)
		{
			if(!EnableLevel(Level.Info) || null == LogInfo) return;
			LogInfo.Invoke(msg);
		}
		public static void Warning(string msg)
		{
			if(!EnableLevel(Level.Warning) || null == LogWarning) return;
			LogWarning.Invoke(msg);
		}
		public static void Error(string msg)
		{
			if(!EnableLevel(Level.Error) || null == LogError) return;
			LogError.Invoke(msg);
		}
		public static void Fatal(string msg)
		{
			if(!EnableLevel(Level.Fatal) || null == LogFatal) return;
			LogFatal.Invoke(msg);
		}
		public static void Trace(Func<string> func)
		{
			if(!EnableLevel(Level.Trace) || null == LogTrace) return;
			LogTrace.Invoke(func());
		}
		public static void Debug(Func<string> func)
		{
			if(!EnableLevel(Level.Debug) || null == LogDebug) return;
			LogDebug.Invoke(func());
		}
		public static void Info(Func<string> func)
		{
			if(!EnableLevel(Level.Info) || null == LogInfo) return;
			LogInfo.Invoke(func());
		}
		public static void Warning(Func<string> func)
		{
			if(!EnableLevel(Level.Warning) || null == LogWarning) return;
			LogWarning.Invoke(func());
		}
		public static void Error(Func<string> func)
		{
			if(!EnableLevel(Level.Error) || null == LogError) return;
			LogError.Invoke(func());
		}
		public static void Fatal(Func<string> func)
		{
			if(!EnableLevel(Level.Fatal) || null == LogFatal) return;
			LogFatal.Invoke(func());
		}
		#endregion

		#region output control
		public static Action<string> LogTrace = null;
		public static Action<string> LogDebug = null;
		public static Action<string> LogInfo = null;
		public static Action<string> LogWarning = null;
		public static Action<string> LogError = null;
		public static Action<string> LogFatal = null;
		public static class ToFile
		{
			public static Func<Level, string> Head { get; set; } = (level) =>
			{
				string head;
#if USE_UNITY_LOG
				var curThread = Thread.CurrentThread.ManagedThreadId;
				if(curThread == s_MainThreadId)
				{
                    // the DateTime, only read is thread safe
					head = $"{DateTime.Now:HH:mm:ss.fff} {UnityEngine.Time.frameCount} {level}\t| ";
				}
				else
				{
					head = $"{DateTime.Now:HH:mm:ss.fff} <{curThread}> {level}\t| ";
				}
#else
				var curThread = Thread.CurrentThread.ManagedThreadId;
				head = $"{DateTime.Now:HH:mm:ss.fff} <{curThread}> {level}\t| ";
#endif
				return head;
			};
			// 路径非空且可用时绑定输出，否则释放
			public static string Path
			{
				get => s_Path;
				set
				{
					if(s_Path == value) return;
                    lock (s_OperateLock)
                    {
                        if (s_Writer != null)
                        {
                            LogTrace -= Trace;
                            LogDebug -= Debug;
                            LogInfo -= Info;
                            LogWarning -= Warning;
                            LogError -= Error;
                            LogFatal -= Fatal;
                            s_Writer.Dispose();
                            s_Writer = null;
                            s_Path = null;
                        }

                        if (!string.IsNullOrEmpty(value))
                        {
                            if (!Directory.Exists(value)) Directory.CreateDirectory(value);
                            var file = $"{value}/{DateTime.Now:yyyyMMdd_HHmmss}.log";
                            s_Writer = new StreamWriter(new FileStream(file, FileMode.OpenOrCreate));
                            s_Writer.AutoFlush = true;
                            s_Path = value;
                            LogTrace += Trace;
                            LogDebug += Debug;
                            LogInfo += Info;
                            LogWarning += Warning;
                            LogError += Error;
                            LogFatal += Fatal;
                        }
                    }
                }
			}
			private static string s_Path = null;
			private static StreamWriter s_Writer = null;
			public static void Trace(string msg)
			{
				Write($"{Head(Level.Trace)}{msg}");
			}
			public static void Debug(string msg)
			{
				Write($"{Head(Level.Debug)}{msg}");
			}
			public static void Info(string msg)
			{
				Write($"{Head(Level.Info)}{msg}");
			}
			public static void Warning(string msg)
			{
				Write($"{Head(Level.Warning)}{msg}");
			}
			public static void Error(string msg)
			{
				Write($"{Head(Level.Error)}{msg}");
			}
			public static void Fatal(string msg)
			{
				Write($"{Head(Level.Fatal)}{msg}");
			}
			public static void Write(string msg)
			{
				if(s_Writer == null) return;
				lock(s_Writer)
				{
					s_Writer.WriteLine(msg);
				}
			}
		}
		public static class ToConsole
		{
			public static Func<Level, string> Head { get; set; } = (level) =>
			{
				string head;
#if USE_UNITY_LOG
				var curThread = Thread.CurrentThread.ManagedThreadId;
				if(curThread == s_MainThreadId)
				{
					head = $"<color=#FFFF00>[{DateTime.Now:HH:mm:ss.fff}]</color><color=#00FFFF>[{UnityEngine.Time.frameCount}]</color> {level}\t| ";
				}
				else
				{
					head = $"<color=#FFFF00>[{DateTime.Now:HH:mm:ss.fff}]</color><color=#FF00FF><{curThread}></color> {level}\t| ";
				}
#else
				var curThread = Thread.CurrentThread.ManagedThreadId;
				head = $"{DateTime.Now:HH:mm:ss.fff} <{curThread}> {level}\t| ";
#endif
				return head;
			};
			// 允许时绑定输出，否则释放
			public static bool Enable
			{
				get => s_Enable;
				set
				{
					if(s_Enable == value) return;
                    lock (s_OperateLock)
                    {
                        if (s_Enable)
                        {
                            LogTrace -= Trace;
                            LogDebug -= Debug;
                            LogInfo -= Info;
                            LogWarning -= Warning;
                            LogError -= Error;
                            LogFatal -= Fatal;
                            s_Enable = false;
                        }

                        if (value)
                        {
                            s_Enable = true;
                            LogTrace += Trace;
                            LogDebug += Debug;
                            LogInfo += Info;
                            LogWarning += Warning;
                            LogError += Error;
                            LogFatal += Fatal;
                        }
                    }
                }
			}
			private static bool s_Enable = false;
			public static void Trace(string msg)
			{
#if USE_UNITY_LOG
				UnityEngine.Debug.Log($"{Head(Level.Trace)}{msg}");
#else
				ToConsole.Write($"{ToConsole.Head(LogHelper.Level.Trace)}{msg}", ConsoleColor.DarkGray);
#endif
			}
			public static void Debug(string msg)
			{
#if USE_UNITY_LOG
				UnityEngine.Debug.Log($"{Head(Level.Debug)}{msg}");
#else
				ToConsole.Write($"{ToConsole.Head(LogHelper.Level.Debug)}{msg}", ConsoleColor.White);
#endif
			}
			public static void Info(string msg)
			{
#if USE_UNITY_LOG
				UnityEngine.Debug.Log($"{Head(Level.Info)}{msg}");
#else
				ToConsole.Write($"{ToConsole.Head(LogHelper.Level.Info)}{msg}", ConsoleColor.Green);
#endif
			}
			public static void Warning(string msg)
			{
#if USE_UNITY_LOG
				UnityEngine.Debug.LogWarning($"{Head(Level.Warning)}{msg}");
#else
				ToConsole.Write($"{ToConsole.Head(LogHelper.Level.Warning)}{msg}", ConsoleColor.Yellow);
#endif
			}
			public static void Error(string msg)
			{
#if USE_UNITY_LOG
				UnityEngine.Debug.LogError($"{Head(Level.Error)}{msg}");
#else
				ToConsole.Write($"{ToConsole.Head(LogHelper.Level.Error)}{msg}", ConsoleColor.Red);
#endif
			}
			public static void Fatal(string msg)
			{
#if USE_UNITY_LOG
				UnityEngine.Debug.LogError($"{Head(Level.Fatal)}{msg}");
#else
				ToConsole.Write($"{ToConsole.Head(LogHelper.Level.Fatal)}{msg}", ConsoleColor.Cyan);
#endif
			}
            // this function is thread safe
			public static void Write(string msg, ConsoleColor fColor = default, ConsoleColor bColor = default)
			{
				if(fColor == default) fColor = Console.ForegroundColor;
				if(bColor == default) bColor = Console.BackgroundColor;
				var rfColor = Console.ForegroundColor;
				var rbColor = Console.BackgroundColor;
				Console.ForegroundColor = fColor;
				Console.BackgroundColor = bColor;
				Console.WriteLine(msg);
				Console.ForegroundColor = rfColor;
				Console.BackgroundColor = rbColor;
			}
		}
		#endregion
	}
}
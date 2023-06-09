﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class AppLuaProxyWrap
{
	public static void Register(LuaState L)
	{
		L.BeginClass(typeof(AppLuaProxy), typeof(System.Object));
		L.RegFunction("OnUpdate", OnUpdate);
		L.RegFunction("OnFixedUpdate", OnFixedUpdate);
		L.RegFunction("OnLateUpdate", OnLateUpdate);
		L.RegFunction("New", _CreateAppLuaProxy);
		L.RegFunction("__tostring", ToLua.op_ToString);
		L.EndClass();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateAppLuaProxy(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 0)
			{
				AppLuaProxy obj = new AppLuaProxy();
				ToLua.PushObject(L, obj);
				return 1;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to ctor method: AppLuaProxy.New");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int OnUpdate(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			LuaFunction arg0 = ToLua.CheckLuaFunction(L, 1);
			AppLuaProxy.OnUpdate(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int OnFixedUpdate(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			LuaFunction arg0 = ToLua.CheckLuaFunction(L, 1);
			AppLuaProxy.OnFixedUpdate(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int OnLateUpdate(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			LuaFunction arg0 = ToLua.CheckLuaFunction(L, 1);
			AppLuaProxy.OnLateUpdate(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}
}


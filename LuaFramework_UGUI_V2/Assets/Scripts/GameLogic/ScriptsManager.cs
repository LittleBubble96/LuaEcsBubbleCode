using System;
using LuaInterface;
using UnityEngine;

public class ScriptsManager : MonoSingleton<ScriptsManager>
{
    private LuaState m_luaState;

    private bool m_isInited = false;

    #region lua回调函数

    private LuaFunction m_Update;
    private LuaFunction m_FixedUpdate;
    private LuaFunction m_LateUpdate;
    
    #endregion
    public void Init(LuaState luaState)
    {
        m_luaState = luaState;
        m_isInited = true;
    }

    private void Update()
    {
        if (!m_isInited)
        {
            return;
        }

        if (m_Update == null)
        {
            return;
        }

        float deltaTimeMs = Time.deltaTime * 1000;
        float unscaledDeltaTimeMs = Time.unscaledDeltaTime * 1000;
        float timeMs = Time.time * 1000;
        float unscaledTimeMS = Time.unscaledTime * 1000;
        m_Update.Call(deltaTimeMs,unscaledDeltaTimeMs,timeMs,unscaledTimeMS);
    }

    private void FixedUpdate()
    {
        if (!m_isInited)
        {
            return;
        }

        if (m_FixedUpdate == null)
        {
            return;
        }
        var e = Time.fixedDeltaTime * 1000;
        m_FixedUpdate.Call(e);
    }

    private void LateUpdate()
    {
        if (!m_isInited)
        {
            return;
        }

        if (m_LateUpdate == null)
        {
            return;
        }
        m_LateUpdate.Call();
    }


    public void RegisterUpdate(LuaFunction updateCall)
    {
        if (updateCall == null)
        {
            return;
        }

        if (m_Update != null)
        {
            m_Update.Dispose();
            m_Update = null;
        }
        m_Update = updateCall;
    }
    
    public void RegisterFixedUpdate(LuaFunction fixedUpdateCall)
    {
        if (fixedUpdateCall == null)
        {
            return;
        }

        if (m_FixedUpdate != null)
        {
            m_FixedUpdate.Dispose();
            m_FixedUpdate = null;
        }
        m_FixedUpdate = fixedUpdateCall;
    }
    
    public void RegisterLateUpdate(LuaFunction lateUpdateCall)
    {
        if (lateUpdateCall == null)
        {
            return;
        }

        if (m_LateUpdate != null)
        {
            m_LateUpdate.Dispose();
            m_LateUpdate = null;
        }
        m_LateUpdate = lateUpdateCall;
    }
}
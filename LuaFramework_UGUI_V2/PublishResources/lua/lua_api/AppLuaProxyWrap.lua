---@class AppLuaProxy : object
local m = {}
---@param onUpdate LuaInterface.LuaFunction
function m.OnUpdate(onUpdate) end
---@param onFixedUpdate LuaInterface.LuaFunction
function m.OnFixedUpdate(onFixedUpdate) end
---@param onLateUpdate LuaInterface.LuaFunction
function m.OnLateUpdate(onLateUpdate) end
AppLuaProxy = m
return m
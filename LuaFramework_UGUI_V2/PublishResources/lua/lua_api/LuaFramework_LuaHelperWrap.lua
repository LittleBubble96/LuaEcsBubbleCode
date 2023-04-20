---@class LuaFramework.LuaHelper
local m = {}
---@param classname string
---@return System.Type
function m.GetType(classname) end
---@return LuaFramework.PanelManager
function m.GetPanelManager() end
---@return LuaFramework.ResourceManager
function m.GetResManager() end
---@return LuaFramework.NetworkManager
function m.GetNetManager() end
---@return LuaFramework.SoundManager
function m.GetSoundManager() end
---@param data LuaInterface.LuaByteBuffer
---@param func LuaInterface.LuaFunction
function m.OnCallLuaFunc(data, func) end
---@param data string
---@param func LuaInterface.LuaFunction
function m.OnJsonCallFunc(data, func) end
LuaFramework = {}
LuaFramework.LuaHelper = m
return m
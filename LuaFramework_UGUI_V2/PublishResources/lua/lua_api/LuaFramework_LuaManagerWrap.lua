---@class LuaFramework.LuaManager : Manager
local m = {}
function m:InitStart() end
---@return LuaInterface.LuaState
function m:GetLuaState() end
---@param filename string
function m:DoFile(filename) end
---@param funcName string
---@param args table
---@return table
function m:CallFunction(funcName, args) end
function m:LuaGC() end
function m:Close() end
LuaFramework = {}
LuaFramework.LuaManager = m
return m
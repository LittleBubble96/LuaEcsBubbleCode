---@class LuaFramework.ThreadManager : Manager
local m = {}
---@param ev ThreadEvent
---@param func System.Action
function m:AddEvent(ev, func) end
LuaFramework = {}
LuaFramework.ThreadManager = m
return m
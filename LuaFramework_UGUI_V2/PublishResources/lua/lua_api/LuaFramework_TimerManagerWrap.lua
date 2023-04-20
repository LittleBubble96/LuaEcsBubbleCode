---@class LuaFramework.TimerManager : Manager
---@field Interval float
local m = {}
---@param value float
function m:StartTimer(value) end
function m:StopTimer() end
---@param info LuaFramework.TimerInfo
function m:AddTimerEvent(info) end
---@param info LuaFramework.TimerInfo
function m:RemoveTimerEvent(info) end
---@param info LuaFramework.TimerInfo
function m:StopTimerEvent(info) end
---@param info LuaFramework.TimerInfo
function m:ResumeTimerEvent(info) end
LuaFramework = {}
LuaFramework.TimerManager = m
return m
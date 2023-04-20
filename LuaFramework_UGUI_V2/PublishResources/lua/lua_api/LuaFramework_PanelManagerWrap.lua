---@class LuaFramework.PanelManager : Manager
local m = {}
---@param name string
---@param func LuaInterface.LuaFunction
function m:CreatePanel(name, func) end
---@param name string
function m:ClosePanel(name) end
LuaFramework = {}
LuaFramework.PanelManager = m
return m
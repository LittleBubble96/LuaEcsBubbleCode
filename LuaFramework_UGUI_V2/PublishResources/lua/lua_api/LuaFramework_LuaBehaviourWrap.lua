---@class LuaFramework.LuaBehaviour : View
local m = {}
---@param go UnityEngine.GameObject
---@param luafunc LuaInterface.LuaFunction
function m:AddClick(go, luafunc) end
---@param go UnityEngine.GameObject
function m:RemoveClick(go) end
function m:ClearClick() end
LuaFramework = {}
LuaFramework.LuaBehaviour = m
return m
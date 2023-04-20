---@class LuaFramework.ResourceManager : Manager
local m = {}
---@param manifestName string
---@param initOK System.Action
function m:Initialize(manifestName, initOK) end
---@overload fun(abName:string, assetNames:table, func:System.Action):void
---@overload fun(abName:string, assetNames:table, func:LuaInterface.LuaFunction):void
---@param abName string
---@param assetName string
---@param func System.Action
function m:LoadPrefab(abName, assetName, func) end
---@param abName string
---@param isThorough bool
function m:UnloadAssetBundle(abName, isThorough) end
LuaFramework = {}
LuaFramework.ResourceManager = m
return m
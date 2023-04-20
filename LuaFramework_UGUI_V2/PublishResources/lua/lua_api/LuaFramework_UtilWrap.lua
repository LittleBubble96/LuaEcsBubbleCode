---@class LuaFramework.Util : object
---@field DataPath string
---@field NetAvailable bool
---@field IsWifi bool
local m = {}
---@param o object
---@return int
function m.Int(o) end
---@param o object
---@return float
function m.Float(o) end
---@param o object
---@return long
function m.Long(o) end
---@overload fun(min:float, max:float):float
---@param min int
---@param max int
---@return int
function m.Random(min, max) end
---@param uid string
---@return string
function m.Uid(uid) end
---@return long
function m.GetTime() end
---@overload fun(go:UnityEngine.Transform, subnode:string):UnityEngine.GameObject
---@param go UnityEngine.GameObject
---@param subnode string
---@return UnityEngine.GameObject
function m.Child(go, subnode) end
---@overload fun(go:UnityEngine.Transform, subnode:string):UnityEngine.GameObject
---@param go UnityEngine.GameObject
---@param subnode string
---@return UnityEngine.GameObject
function m.Peer(go, subnode) end
---@param source string
---@return string
function m.md5(source) end
---@param file string
---@return string
function m.md5file(file) end
---@param go UnityEngine.Transform
function m.ClearChild(go) end
function m.ClearMemory() end
---@return string
function m.GetRelativePath() end
---@param path string
---@return string
function m.GetFileText(path) end
---@return string
function m.AppContentPath() end
---@param str string
function m.Log(str) end
---@param str string
function m.LogWarning(str) end
---@param str string
function m.LogError(str) end
---@return int
function m.CheckRuntimeFile() end
---@param module string
---@param func string
---@param args table
---@return table
function m.CallMethod(module, func, args) end
---@return bool
function m.CheckEnvironment() end
LuaFramework = {}
LuaFramework.Util = m
return m
---@class LuaFramework.NetworkManager : Manager
local m = {}
function m:OnInit() end
function m:Unload() end
---@param func string
---@param args table
---@return table
function m:CallMethod(func, args) end
---@param _event int
---@param data LuaFramework.ByteBuffer
function m.AddEvent(_event, data) end
function m:SendConnect() end
---@param buffer LuaFramework.ByteBuffer
function m:SendMessage(buffer) end
LuaFramework = {}
LuaFramework.NetworkManager = m
return m
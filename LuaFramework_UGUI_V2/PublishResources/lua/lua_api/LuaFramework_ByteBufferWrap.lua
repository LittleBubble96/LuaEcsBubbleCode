---@class LuaFramework.ByteBuffer : object
local m = {}
function m:Close() end
---@param v byte
function m:WriteByte(v) end
---@param v int
function m:WriteInt(v) end
---@param v ushort
function m:WriteShort(v) end
---@param v long
function m:WriteLong(v) end
---@param v float
function m:WriteFloat(v) end
---@param v double
function m:WriteDouble(v) end
---@param v string
function m:WriteString(v) end
---@param v table
function m:WriteBytes(v) end
---@param strBuffer LuaInterface.LuaByteBuffer
function m:WriteBuffer(strBuffer) end
---@return byte
function m:ReadByte() end
---@return int
function m:ReadInt() end
---@return ushort
function m:ReadShort() end
---@return long
function m:ReadLong() end
---@return float
function m:ReadFloat() end
---@return double
function m:ReadDouble() end
---@return string
function m:ReadString() end
---@return table
function m:ReadBytes() end
---@return LuaInterface.LuaByteBuffer
function m:ReadBuffer() end
---@return table
function m:ToBytes() end
function m:Flush() end
LuaFramework = {}
LuaFramework.ByteBuffer = m
return m
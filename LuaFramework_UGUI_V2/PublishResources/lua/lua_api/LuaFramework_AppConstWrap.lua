---@class LuaFramework.AppConst : object
---@field FrameworkRoot string
---@field DebugMode bool
---@field ExampleMode bool
---@field UpdateMode bool
---@field LuaByteMode bool
---@field LuaBundleMode bool
---@field TimerInterval int
---@field GameFrameRate int
---@field AppName string
---@field LuaTempDir string
---@field AppPrefix string
---@field ExtName string
---@field AssetDir string
---@field WebUrl string
---@field UserId string
---@field SocketPort int
---@field SocketAddress string
local m = {}
LuaFramework = {}
LuaFramework.AppConst = m
return m
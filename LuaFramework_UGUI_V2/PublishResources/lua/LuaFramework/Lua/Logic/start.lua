require "Logic/log_helper"
require "Logic/object"
require "logic/u_object"
Singleton = require "logic/singleton"
GameGlobal = require "logic/game_global"

GameGlobal:GetInstance():init()
--DoUpdate
AppLuaProxy.OnUpdate(function(e,unscaled,timeMs,unscaledTimeMs)
    GameGlobal:GetInstance():doUpdate(e,unscaled,timeMs,unscaledTimeMs)
end)
--FixedUpdate
AppLuaProxy.OnFixedUpdate(function(e)
    GameGlobal:GetInstance():doFixedUpdate(e)
end)
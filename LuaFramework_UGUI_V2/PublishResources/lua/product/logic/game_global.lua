GameGlobal = class("GameGlobal",Singleton)
function GameGlobal:init()
    --流程开始
    Log.init()
    Log.debug("GameGlobal:init() 流程开始~~~")
end

function GameGlobal:doUpdate(e,unscaled,timeMs,unscaledTimeMs)
    if UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.W) then
        Log.debug("GameGlobal:doUpdate() ~~~"..e)
    end
end

function GameGlobal:doFixedUpdate(e)
    --Log.debug("GameGlobal:doUpdate() ~~~"..e)
end

function GameGlobal:doLateUpdate(e)
    --Log.debug("GameGlobal:doUpdate() ~~~"..e)
end

return GameGlobal
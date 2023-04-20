---@class LuaFramework.SoundManager : Manager
local m = {}
---@param path string
---@return UnityEngine.AudioClip
function m:LoadAudioClip(path) end
---@return bool
function m:CanPlayBackSound() end
---@param name string
---@param canPlay bool
function m:PlayBacksound(name, canPlay) end
---@return bool
function m:CanPlaySoundEffect() end
---@param clip UnityEngine.AudioClip
---@param position UnityEngine.Vector3
function m:Play(clip, position) end
LuaFramework = {}
LuaFramework.SoundManager = m
return m
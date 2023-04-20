---@class UnityEngine.Animation : UnityEngine.Behaviour
---@field clip UnityEngine.AnimationClip
---@field playAutomatically bool
---@field wrapMode UnityEngine.WrapMode
---@field isPlaying bool
---@field Item UnityEngine.AnimationState
---@field animatePhysics bool
---@field cullingType UnityEngine.AnimationCullingType
---@field localBounds UnityEngine.Bounds
local m = {}
---@overload fun(name:string):void
function m:Stop() end
---@overload fun(name:string):void
function m:Rewind() end
function m:Sample() end
---@param name string
---@return bool
function m:IsPlaying(name) end
---@overload fun(mode:UnityEngine.PlayMode):bool
---@overload fun(animation:string):bool
---@overload fun(animation:string, mode:UnityEngine.PlayMode):bool
---@return bool
function m:Play() end
---@overload fun(animation:string, fadeLength:float):void
---@overload fun(animation:string, fadeLength:float, mode:UnityEngine.PlayMode):void
---@param animation string
function m:CrossFade(animation) end
---@overload fun(animation:string, targetWeight:float):void
---@overload fun(animation:string, targetWeight:float, fadeLength:float):void
---@param animation string
function m:Blend(animation) end
---@overload fun(animation:string, fadeLength:float):UnityEngine.AnimationState
---@overload fun(animation:string, fadeLength:float, queue:UnityEngine.QueueMode):UnityEngine.AnimationState
---@overload fun(animation:string, fadeLength:float, queue:UnityEngine.QueueMode, mode:UnityEngine.PlayMode):UnityEngine.AnimationState
---@param animation string
---@return UnityEngine.AnimationState
function m:CrossFadeQueued(animation) end
---@overload fun(animation:string, queue:UnityEngine.QueueMode):UnityEngine.AnimationState
---@overload fun(animation:string, queue:UnityEngine.QueueMode, mode:UnityEngine.PlayMode):UnityEngine.AnimationState
---@param animation string
---@return UnityEngine.AnimationState
function m:PlayQueued(animation) end
---@overload fun(clip:UnityEngine.AnimationClip, newName:string, firstFrame:int, lastFrame:int):void
---@overload fun(clip:UnityEngine.AnimationClip, newName:string, firstFrame:int, lastFrame:int, addLoopFrame:bool):void
---@param clip UnityEngine.AnimationClip
---@param newName string
function m:AddClip(clip, newName) end
---@overload fun(clipName:string):void
---@param clip UnityEngine.AnimationClip
function m:RemoveClip(clip) end
---@return int
function m:GetClipCount() end
---@param layer int
function m:SyncLayer(layer) end
---@return System.Collections.IEnumerator
function m:GetEnumerator() end
---@param name string
---@return UnityEngine.AnimationClip
function m:GetClip(name) end
UnityEngine = {}
UnityEngine.Animation = m
return m
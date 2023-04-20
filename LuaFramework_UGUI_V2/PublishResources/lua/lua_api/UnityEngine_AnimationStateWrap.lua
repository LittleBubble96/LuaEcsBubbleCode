---@class UnityEngine.AnimationState : UnityEngine.TrackedReference
---@field enabled bool
---@field weight float
---@field wrapMode UnityEngine.WrapMode
---@field time float
---@field normalizedTime float
---@field speed float
---@field normalizedSpeed float
---@field length float
---@field layer int
---@field clip UnityEngine.AnimationClip
---@field name string
---@field blendMode UnityEngine.AnimationBlendMode
local m = {}
---@overload fun(mix:UnityEngine.Transform, recursive:bool):void
---@param mix UnityEngine.Transform
function m:AddMixingTransform(mix) end
---@param mix UnityEngine.Transform
function m:RemoveMixingTransform(mix) end
UnityEngine = {}
UnityEngine.AnimationState = m
return m
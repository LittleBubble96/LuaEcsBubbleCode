---@class UnityEngine.SkinnedMeshRenderer : UnityEngine.Renderer
---@field quality UnityEngine.SkinQuality
---@field updateWhenOffscreen bool
---@field forceMatrixRecalculationPerRender bool
---@field rootBone UnityEngine.Transform
---@field bones table
---@field sharedMesh UnityEngine.Mesh
---@field skinnedMotionVectors bool
---@field vertexBufferTarget UnityEngine.GraphicsBuffer.Target
local m = {}
---@param index int
---@return float
function m:GetBlendShapeWeight(index) end
---@param index int
---@param value float
function m:SetBlendShapeWeight(index, value) end
---@overload fun(mesh:UnityEngine.Mesh, useScale:bool):void
---@param mesh UnityEngine.Mesh
function m:BakeMesh(mesh) end
---@return UnityEngine.GraphicsBuffer
function m:GetVertexBuffer() end
---@return UnityEngine.GraphicsBuffer
function m:GetPreviousVertexBuffer() end
UnityEngine = {}
UnityEngine.SkinnedMeshRenderer = m
return m
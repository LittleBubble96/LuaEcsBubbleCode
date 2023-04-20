---@class UnityEngine.RectTransform : UnityEngine.Transform
---@field rect UnityEngine.Rect
---@field anchorMin UnityEngine.Vector2
---@field anchorMax UnityEngine.Vector2
---@field anchoredPosition UnityEngine.Vector2
---@field sizeDelta UnityEngine.Vector2
---@field pivot UnityEngine.Vector2
---@field anchoredPosition3D UnityEngine.Vector3
---@field offsetMin UnityEngine.Vector2
---@field offsetMax UnityEngine.Vector2
---@field drivenByObject UnityEngine.Object
local m = {}
function m:ForceUpdateRectTransforms() end
---@param fourCornersArray table
function m:GetLocalCorners(fourCornersArray) end
---@param fourCornersArray table
function m:GetWorldCorners(fourCornersArray) end
---@param edge UnityEngine.RectTransform.Edge
---@param inset float
---@param size float
function m:SetInsetAndSizeFromParentEdge(edge, inset, size) end
---@param axis UnityEngine.RectTransform.Axis
---@param size float
function m:SetSizeWithCurrentAnchors(axis, size) end
UnityEngine = {}
UnityEngine.RectTransform = m
return m
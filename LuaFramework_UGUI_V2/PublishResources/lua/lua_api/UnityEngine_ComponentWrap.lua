---@class UnityEngine.Component : UnityEngine.Object
---@field transform UnityEngine.Transform
---@field gameObject UnityEngine.GameObject
---@field tag string
local m = {}
---@overload fun(type:string):UnityEngine.Component
---@param type System.Type
---@return UnityEngine.Component
function m:GetComponent(type) end
---@param type System.Type
---@param component UnityEngine.Component
---@return bool
function m:TryGetComponent(type, component) end
---@overload fun(t:System.Type):UnityEngine.Component
---@param t System.Type
---@param includeInactive bool
---@return UnityEngine.Component
function m:GetComponentInChildren(t, includeInactive) end
---@overload fun(t:System.Type):table
---@param t System.Type
---@param includeInactive bool
---@return table
function m:GetComponentsInChildren(t, includeInactive) end
---@overload fun(t:System.Type):UnityEngine.Component
---@param t System.Type
---@param includeInactive bool
---@return UnityEngine.Component
function m:GetComponentInParent(t, includeInactive) end
---@overload fun(t:System.Type):table
---@param t System.Type
---@param includeInactive bool
---@return table
function m:GetComponentsInParent(t, includeInactive) end
---@overload fun(type:System.Type, results:table):void
---@param type System.Type
---@return table
function m:GetComponents(type) end
---@param tag string
---@return bool
function m:CompareTag(tag) end
---@overload fun(methodName:string, value:object):void
---@overload fun(methodName:string):void
---@overload fun(methodName:string, options:UnityEngine.SendMessageOptions):void
---@param methodName string
---@param value object
---@param options UnityEngine.SendMessageOptions
function m:SendMessageUpwards(methodName, value, options) end
---@overload fun(methodName:string):void
---@overload fun(methodName:string, value:object, options:UnityEngine.SendMessageOptions):void
---@overload fun(methodName:string, options:UnityEngine.SendMessageOptions):void
---@param methodName string
---@param value object
function m:SendMessage(methodName, value) end
---@overload fun(methodName:string, parameter:object):void
---@overload fun(methodName:string):void
---@overload fun(methodName:string, options:UnityEngine.SendMessageOptions):void
---@param methodName string
---@param parameter object
---@param options UnityEngine.SendMessageOptions
function m:BroadcastMessage(methodName, parameter, options) end
UnityEngine = {}
UnityEngine.Component = m
return m
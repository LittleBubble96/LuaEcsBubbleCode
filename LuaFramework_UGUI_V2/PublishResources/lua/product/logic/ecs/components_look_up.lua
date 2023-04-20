---@class ComponentsLookUp
class("ComponentsLookUp")
ComponentsLookUp = ComponentsLookUp
function ComponentsLookUp:ctor(rawStrArray)
    --传入枚举
    self.EL_RawStrArray = rawStrArray
    InternalRefresh()
end

---@param other_Lookup ComponentsLookUp
function ComponentsLookUp:MergeLookup(other_Lookup)
    local rawArray = self.EL_RawStrArray
    local add_rawArray = self.EL_RawStrArray
    local total = #rawArray
    for k, v in ipairs(add_rawArray) do
        if v~= nil then
            rawArray[total + 1] = v
        end
    end
    InternalRefresh()
end

--刷新枚举键值
function ComponentsLookUp:InternalRefresh()
    local rawArray = self.EL_RawStrArray
    for k, v in ipairs(rawArray) do
        self[v] = k
    end
end

return ComponentsLookUp
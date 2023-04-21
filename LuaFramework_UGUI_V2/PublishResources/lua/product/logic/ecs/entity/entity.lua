class("Entity")
Entity = Entity

function Entity:ctor()
    self.components = {}
    --添加组件事件
    self.EV_OnAddComponrnt = DelegateEvent.new()
    --移除组件事件
    self.EV_OnRomoveComponrnt = DelegateEvent.new()    
end

--添加组件
function Entity:AddComponent(comptIndex ,compt)
    if self.HasComponent(comptIndex) then
        return
    end
    self.components[comptIndex] = compt
    --TODO 广播添加组件事件
end

--是否存在该组件
function Entity:HasComponent(comptIndex)
    for key, value in pairs(self.components) do
        if key == comptIndex then
            return true
        end
    end
    return false
end

--移除组件
function Entity:RemoveComponent(comptIndex)
    if ~self.HasComponent(comptIndex) then
        return
    end
    table.remove(self.components,comptIndex)
    --TODO 广播移除组件事件
end

--替换组件
function Entity:ReplaceComponent(comptIndex ,compt)
    self.RemoveComponent(comptIndex)
    self.AddComponent(comptIndex,compt)
end


function Entity:Dispose()
    self.EV_OnAddComponrnt = nil
    self.EV_OnRomoveComponrnt = nil
    self.components = nil
end


return Entity
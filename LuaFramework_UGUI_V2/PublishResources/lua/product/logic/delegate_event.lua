DelegateEvent = class("DelegateEvent")

function DelegateEvent:ctor()
    self._callArray = {}
end

--添加事件
function DelegateEvent:AddEvent(source ,func)
    table.insert(self._callArray,{source,func})
end

--移除事件
function DelegateEvent:RemoveEvent(source,func)
    for key, value in pairs(self._callArray) do
        if value[1] == source and value[2] == func then
            table.remove(self._callArray,key)
        end
    end
end

--清除事件
function DelegateEvent:Clear()
    table.clear(self._callArray)
end 


--广播事件
function DelegateEvent:Call(...)
    for key, value in pairs(self._callArray) do
        value[2](value[1],...)
    end
end

return DelegateEvent
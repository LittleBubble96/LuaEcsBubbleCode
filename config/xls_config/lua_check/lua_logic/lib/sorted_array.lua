--[[------------------------------------------------------------------------------------------
    容器：排序的数组
]]--------------------------------------------------------------------------------------------
---@class SortedArray:Object
_class( "SortedArray", Object )
SortedArray = SortedArray
local floor = math.floor

--[[-------------------------------------------
    compare_method和comparer的具体含义，见Algorithm.lua
]]---------------------------------------------
function SortedArray:Constructor(compare_method, comparer)
    self.elements = {}
    if compare_method == Algorithm.COMPARE_LESS then
        self.comparer = Algorithm.LessComparer
    elseif compare_method == Algorithm.COMPARE_GREATER then
        self.comparer = Algorithm.GreaterComparer
    elseif  compare_method == Algorithm.COMPARE_CUSTOM then
        self.comparer = comparer
    else
        self.comparer = Algorithm.LessComparer  --默认从小到大排列
    end
    self.allow_duplicate = false  --默认不允许重复元素
end

function SortedArray:Empty()
    return #self.elements == 0
end

function SortedArray:Size()
    return #self.elements
end

function SortedArray:Clear()
    self.elements = {}
end

--[[-------------------------------------------
    允许重复元素
]]---------------------------------------------
function SortedArray:AllowDuplicate()
    self.allow_duplicate = true
end

--[[-------------------------------------------
    插入时需要排序
    检查nil是为了维护好索引的连续性
]]---------------------------------------------
function SortedArray:Insert(value)
    if value == nil then
        return
    end
    local index, exist = self:FindInsertIndexInternal(value)
    if exist and not self.allow_duplicate then
        if _DEBUG then
            LogError("SortedArray:Insert, duplicate element")
        end
        self.elements[index] = value
    else
        local elements = self.elements
        for i = #elements, index, -1 do
            elements[i+1] = elements[i]
        end
        elements[index] = value
    end
end

--[[-------------------------------------------
    删除一个值为value的元素
    如果允许重复元素，只删除一个
]]---------------------------------------------
function SortedArray:Remove(value)
    if value == nil then
        return false
    end
    local index = self:BinarySearchInternal(value)
    if index == -1 then
        return false
    end
    local elements = self.elements
    local size = #elements
    for i = index, size - 1 do
        elements[i] = elements[i+1]
    end
    elements[size] = nil
    return true
end

--[[-------------------------------------------
    删除并返回索引index处的元素
]]---------------------------------------------
function SortedArray:RemoveByIndex(index)
    local elements = self.elements
    local size = #elements
    if index < 1 or index > size then
        return nil
    end
	local temp = elements[index]
    for i = index, size - 1 do
        elements[i] = elements[i + 1]
    end
    elements[size] = nil
    return temp
end

--[[-------------------------------------------
    RemoveAt等同于RemoveByIndex
]]---------------------------------------------
SortedArray.RemoveAt = SortedArray.RemoveByIndex

--[[-------------------------------------------
    搜索值为value的元素，如果找到则返回索引
]]---------------------------------------------
function SortedArray:Find(value)
    if value == nil then
        return -1
    end
    return self:BinarySearchInternal(value)
end

--[[-------------------------------------------
    是否包含元素value
]]---------------------------------------------
function SortedArray:Contains(value)
    return self:Find(value) > 0
end

--[[-------------------------------------------
    返回索引index处的元素
]]---------------------------------------------
function SortedArray:GetAt(index)
    return self.elements[index]
end

--[[-------------------------------------------
    对所有元素依次调用函数func
]]---------------------------------------------
function SortedArray:ForEach(func)
    local elements = self.elements
    for i = 1, #elements do
        func(elements[i])
    end
end

--[[-------------------------------------------
    内部函数：二叉搜索
]]---------------------------------------------
function SortedArray:BinarySearchInternal(value)
    local elements = self.elements
    local low = 1
    local high = #elements
    local comparer = self.comparer
    local mid, result
    while low <= high do
        mid = floor((low + high) * 0.5)
        result = comparer(value, elements[mid])
        if result > 0 then
            high = mid - 1
        elseif result < 0 then
            low = mid + 1
        else
            return mid
        end
    end
    return -1
end

--[[-------------------------------------------
    内部函数：搜索value的插入位置
]]---------------------------------------------
function SortedArray:FindInsertIndexInternal(value)
    local elements = self.elements
    local low = 1
    local high = #elements
    local comparer = self.comparer
    local mid, result
    while low <= high do
        mid = floor((low + high) * 0.5)
        result = comparer(value, elements[mid])
        if result > 0 then
            high = mid - 1
        elseif result < 0 then
            low = mid + 1
        else
            return mid, true
        end
    end
    return low, false
end

--[[-------------------------------------------
    内部函数：插入排序方式插入value
]]---------------------------------------------
function SortedArray:IncertionSort(value)
    local elements = self.elements
    local comparer = self.comparer
    local i = #elements
    while i > 0 and comparer(value, elements[i]) > 0 do
        elements[i+1] = elements[i]
        i = i - 1
    end
    elements[i + 1] = value
end
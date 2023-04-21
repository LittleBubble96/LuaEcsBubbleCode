BaseWorld = class("BaseWorld")

function BaseWorld:ctor(worldInfo)
    --信息
    this.contextInfo = worldInfo
    --系统集合
    this._systems = {}
    --匹配组集合
    this._groups = {}
    --实体集合
    this._entities = {}
end

--创建渲染实体
---@public
function BaseWorld:CreateRenderEntity()
    
end

--创建逻辑实体
function BaseWorld:CreateLogicEntity()

end

--移除实体
function BaseWorld:DestoryEntity(e)

end



-- 获取匹配数据 {}
---@public
function BaseWorld: GetGroup(...)
    if #... <= 0  then
        return nil;
    end
    --按照 枚举最小至大 顺序输出
    local sort_enum = {}
    for index, value in ipairs(...) do
        local isInsert = false
        for sort_index, sort_value in ipairs(...) do
            if value < sort_value then
                table.insert(sort_enum,sort_index,value)
                isInsert = true
                break
            end
        end
        if ~isInsert then
            table.insert(sort_enum,value)
        end
    end

    --查找Group  如果不存在就新建
    local group 
    for i=1, #sort_enum do 
        if i == 1 then
            group = self._groups[sort_enum[i]]
        else
            group = group.groups[sort_enum[i]]
        end
        if not group then
            return CreateGroup(sort_enum)
        end
        
    end 
end

-- 创建匹配组
---@private
function BaseWorld:CreateGroup(...)
    local local_groups =  self._groups
    local match_entities = self._entities
   
    for index, value in ipairs(...) do      
        local get_group
        get_group = local_groups[value]
        if not get_group then
            get_group = MathchGroup.new(value)
        
            --遍历组件 匹配插入实体
            for key, e in pairs(match_entities) do
                if e.HasComponent(value) then
                    table.insert(get_group.entities,e)
                end
            end
            local_groups[value] = get_group
        end
        local_groups = get_group.groups
        match_entities = get_group.entities
    end 
    return self._groups[(...)[1]]
end
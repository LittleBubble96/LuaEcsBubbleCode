BaseWorld = class("BaseWorld")

function BaseWorld:ctor(worldInfo)
    --信息
    this.contextInfo = worldInfo
    --系统集合
    this.systems = {}
    --匹配组集合
    this.groups = {}
    --实体集合
    this.entities = {}
end
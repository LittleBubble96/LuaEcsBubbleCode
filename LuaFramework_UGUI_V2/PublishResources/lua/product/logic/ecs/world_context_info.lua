WorldContextInfo = class("WorldContextInfo")

function WorldContextInfo:ctor()
    --创建渲染实体id
    this.creationRenderEntityId = 1 
    --创建开始渲染实体id
    this.creationStratRenderEntityId = 1
    --创建逻辑实体id
    this.creationLogicEntityId = 1
    --创建开始逻辑实体id
    this.creationStratLogicEntityId = 1 
end

return WorldContextInfo

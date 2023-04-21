MathchGroup = class("MathchGroup")

function MathchGroup:ctor()
    --多匹配数据结构
    self.groups = {}
    --匹配的实体信息
    self.entities = {}
end


function MathchGroup:Dispose()
    self.groups = nil
    self.entities = nil
end
return MathchGroup
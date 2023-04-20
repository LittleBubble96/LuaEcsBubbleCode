--传入类名字 生成实例对象
function class(classname,...)
    --记录类名
    local cls = {__cname = classname}
    --设置父类
    local supers = {...}
    for _ , super in ipairs(supers) do
        local superType = type(super)
        assert(superType == "nil" or superType == "table"  or superType == "function" , 
                string.format("class() - create class \"%s\"with invalid super class type \"%s", classname,superType))
        if superType == "function" then
            --如果是方法 将传入 方法设置创建 类 方法
            assert(cls.__create == nil,
                string.format("class() - create class \"%s\" with more then one createing function",classname))
            cls.__create = super
        elseif superType == "table" then
            if super[".isclass"] then
                -- 如果是类 ，将传入 table 设置为 table（基类） 的创建方法
                assert(cls.__create == nil,
                string.format("class() - create class \"%s\" this more than one creating function or native calss"))
                cls.__create = function() return super:create() end
            else
                -- super is pure lua class
                cls.__supers = cls.__supers or {}
                cls.__supers[#cls.__supers + 1] = super
                if not cls.super then
                    cls.super = super
                end 
            end
        else
            error(string.format("class() - create class \"%s\" with invalid super type",classname),0)
        end
    end
    
    cls.__index = cls
    if not cls.__supers or #cls.__supers == 1 then
        setmetatable(cls, {__index = cls.super})
    else
        setmetatable(cls, {__index = function(_, key)
            local supers = cls.__supers
            for i = 1, #supers do
                local super = supers[i]
                if super[key] then return super[key] end
            end
        end})
    end

    if not cls.ctor then
        -- add default consttructor
        cls.ctor = function() end
    end
    cls.new = function(...)
        local instance
        if cls.__create then
            instance = cls.__create(...)
        else
            instance = {}
        end
        setmetatable(instance,cls)
        instance.class = cls
        instance:ctor(...)
        return instance
    end
    cls.create = function(_,...)
        return cls.new(...)
    end
    return cls
end
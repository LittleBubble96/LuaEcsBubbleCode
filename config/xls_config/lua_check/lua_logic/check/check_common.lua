CheckCommon = {}

--例子
--[[不建议使用这种，1.太废效率 2.条件不够复杂
    CheckCommon.CheckExist("cfg_pet_skill", "NormalSkill", "cfg_pet_battle_skill", "ID", true)
    CheckCommon.CheckExist("cfg_pet_skill", "ActiveSkill", "cfg_pet_battle_skill", "ID", true)
    CheckCommon.CheckExist("cfg_pet_skill", "PassiveSkill", "cfg_passive_skill", "ID", true)
--]]
--某个talbe的行值，是另外一个table的id或者主键
---@param srcname 原表的表名
---@param srcfield 原表的字段
---@param desname 目标表的表名
---@param desfield 目标表的id或者主键
---@param desfieldiskey 是否是主键加速查询
---@param zerovaild 零是否是有效值
function CheckCommon.CheckExist(srcname, srcfield, desname, desfield, desfieldiskey, zerovaild)
    local srctable = Cfg[srcname]()
    local destable = Cfg[desname]()

    for ks, vs in pairs(srctable) do
        local srcvalue = vs[srcfield]

        if (srcvalue == 0 and zerovaild == true) or (srcvalue ~= 0) then
            if (desfieldiskey) then
                if destable[srcvalue] == nil then
                    Log.error(
                        srcname .. " id:" .. ks .. " " .. srcfield .. ":<" .. srcvalue .. "> no match " .. desname
                    )
                end
            else
                local bfind = false
                for kd, vd in pairs(destable) do
                    local dstvalue = vd[desfield]
                    if dstvalue == srcvalue then
                        bfind = true
                        break
                    end
                end
                if bfind == false then
                    Log.error(
                        srcname .. " id:" .. ks .. " " .. srcfield .. ":<" .. srcvalue .. "> no match " .. desname
                    )
                end
            end
        end
    end
end
---@param srcvalue 值
---@param desname 目标表的表名
---@param desfield 目标表的id或者主键
---@param desfieldiskey 是否是主键加速查询
---@param zerovaild 零是否为有效值
function CheckCommon.CheckValueExist(srcvalue, desname, desfield, desfieldiskey, zerovaild)
    local destable = Cfg[desname]()
    if (srcvalue == 0 and zerovaild == true) or (srcvalue ~= 0) then
        if (desfieldiskey) then
            if destable[srcvalue] == nil then
                return false
            end
        else
            local bfind = false
            for kd, vd in pairs(destable) do
                local dstvalue = vd[desfield]
                if dstvalue == srcvalue then
                    bfind = true
                    break
                end
            end
            return bfind
        end
    end
end
--某个talbe的行值，是另外一个table的id或者主键
---@param srcname 表名
---@param srcfield 字段
---@param minvalue 最小值
---@param maxvalue 最大值
function CheckCommon.CheckNumValue(srcname, srcfield, minvalue, maxvalue)
    local srctable = Cfg[srcname]()

    for ks, vs in pairs(srctable) do
        local srcvalue = vs[srcfield]
        local srcnum = tonumber(srcvalue)
        if (srcnum < minvalue or srcnum > maxvalue) then
            Log.error(
                srcname ..
                    " id:" ..
                        ks .. " " .. srcfield .. ":<" .. srcvalue .. "> no begin " .. minvalue .. " and " .. maxvalue
            )
        end
    end
end
function CheckCommon.CheckInRange(srctable, srcfield, valuetable)
end
function CheckCommon.CheckNotInRange(srctable, srcfield, valuetable)
end

--返回一个连续的 table
function CheckCommon.GenNumberRangeTable(minvalue, maxvalue)
    local TriggerTypeRange = {}
    for i = minvalue, maxvalue do
        TriggerTypeRange[i] = i
    end
    return TriggerTypeRange
end

--table 添加连续的key，value
function CheckCommon.AddNumberRangeTable(tabs, minvalue, maxvalue)
    for i = minvalue, maxvalue do
        tabs[i] = i
    end
end
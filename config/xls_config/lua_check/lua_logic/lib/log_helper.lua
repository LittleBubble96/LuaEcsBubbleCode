--local file = io.open("logtext.txt","a")--the lua print is closed
local oldPrint = print
local print0 = function(str, ...)
    oldPrint(str, ...)
    --file:write(tostring(str).."\n")
    --file:flush()
end

--local _iowrite = _iowrite
local nowtime = {}
local dumpvisited
local dumpfrom = ""

local function indented(level, ...)
    -- if PUBLIC then return end
    --print0(table.concat({ ('  '):rep(level), ...}))
    local s = table.concat({("  "):rep(level), ...})
    table.insert(tsss, s)
end
local function dumpval(level, name, value, limit)
    local index
    if type(name) == "number" then
        index = string.format("[%d] = ", name)
    elseif
        type(name) == "string" and
            (name == "__VARSLEVEL__" or name == "__ENVIRONMENT__" or name == "__GLOBALS__" or name == "__UPVALUES__" or
                name == "__LOCALS__")
     then
        --ignore these, they are debugger generated
        return
    elseif type(name) == "string" and string.find(name, "^[_%a][_.%w]*$") then
        index = name .. " = "
    else
        index = string.format("[%q] = ", tostring(name))
    end
    if type(value) == "table" then
        if dumpvisited[value] then
            indented(level, index, string.format("ref%q,", dumpvisited[value]))
        else
            dumpvisited[value] = tostring(value)
            if (limit or 0) > 0 and level + 1 >= limit then
                indented(level, index, dumpvisited[value])
            else
                indented(level, index, "{  -- ", dumpvisited[value])
                for n, v in pairs(value) do
                    dumpval(level + 1, n, v, limit)
                end
                dumpval(level + 1, ".meta", getmetatable(value), limit)
                indented(level, "},")
            end
        end
    else
        if type(value) == "string" then
            if string.len(value) > 40 then
                indented(level, index, "[[", value, "]];")
            else
                indented(level, index, string.format("%q", value), ",")
            end
        else
            indented(level, index, tostring(value), ",")
        end
    end
end

local function dumpvar(value, limit, name)
    dumpvisited = {}
    dumpval(0, name or tostring(value), value, limit)
    dumpvisited = nil
end

debug.dumpdepth = 5
function dump(v, depth)
    -- if PUBLIC then return end
    local info = debug.getinfo(2)
    dumpfrom = info.source .. "|" .. info.currentline --info.currentline
    _G.tsss = {}
    dumpvar(v, (depth or debug.dumpdepth) + 1, tostring(v))
    local s = debug.getinfo(2)
    local _, _, src = string.find(s.short_src, "([%w%_]+%.lua)")
    s = table.concat {"dump ", src, ":", s.currentline}
    s = s .. table.concat(tsss, "\n")
    print0(s)
end

local doprint = function(head, t)
    local s = {head, " "}
    s[#s + 1] = os.date("%Y-%m-%d %H:%M:%S", os.time()) .. "  "
    local len = table.maxn(t)
    for i = 1, len do
        local v = t[i]
        table.insert(s, tostring(v))
        table.insert(s, ",")
    end
    if len >= 1 then
        table.remove(s)
    end
    print0(table.concat(s))
end

local logprint = function(head, t)
    local s = {head, " "}
    --s[#s+1] = os.date("%Y-%m-%d %H:%M:%S", os.time()) .. '  '
    local len = table.maxn(t)
    for i = 1, len do
        local v = t[i]
        table.insert(s, tostring(v))
        table.insert(s, " ")
    end
    print0(table.concat(s))
end

local debugprint = function(head, t)
    local s = {head, " "}
    s[#s + 1] = os.date("%Y-%m-%d %H:%M:%S", os.time()) .. "  "
    local len = table.maxn(t)
    for i = 1, len do
        local v = t[i]
        table.insert(s, tostring(v))
        table.insert(s, " ")
    end
    if len >= 1 then
        table.remove(s)
    end
    print0(table.concat(s))
end

Log = {}
Log.pure = function(...)
    local t = {...}
    local s = {}
    local len = table.maxn(t)
    for i = 1, len do
        local v = t[i]
        table.insert(s, tostring(v))
    end
    print0(table.concat(s))
end

Log.sys = function(...)
    local s = debug.getinfo(2)
    logprint(table.concat {"sys ", s.short_src, ":", s.currentline}, {...}, true, true)
end

Log.debug = function(...)
    local s = debug.getinfo(2)
    --if os.info.system ~= 'windows' then return end
    logprint(table.concat {"debug ", s.short_src, ":", s.currentline}, {...}, true, true)
end

Log.notice = function(...)
    local s = debug.getinfo(2)
    --if os.info.system ~= 'windows' then return end
    logprint(table.concat {"notice ", s.short_src, ":", s.currentline}, {...}, true, true)
end

Log.warn = function(...)
    local s = debug.getinfo(2)
    logprint(table.concat {"warn ", s.short_src, ":", s.currentline}, {...}, true, true)
end

Log.fatal = function(...)
    local s = debug.getinfo(2)
    logprint(table.concat {"fatal ", s.short_src, ":", s.currentline}, {...}, true, true)
end

Log.error = function(...)
    local s = debug.getinfo(2)

    local ll = string.match(s.short_src, ".*/(.*)$");
    if ll == nil then
        ll = s.short_src;
    end

    logprint( table.concat{ 'error ', ll, ':', s.currentline, "  "}, { ... }, true, true )
end

Log.info = function(...)
    local s = debug.getinfo(2)
    logprint(table.concat {"info ", s.short_src, ":", s.currentline}, {...}, true, true)
end

Log.print = function(...)
    local s = debug.getinfo(2)
    debugprint(table.concat {"print ", s.short_src, ":", s.currentline}, {...}, true, true)
end

Log.tick = function(...)
    if os.info.system == "windows" then
        return
    end
    local s = debug.getinfo(2)
    logprint(table.concat {"tick ", s.short_src, ":", s.currentline}, {...}, true, true)
end

Log.print0 = print0

local programmers = {"ylw", "yqq", "zn", "cj"}
for i, v in pairs(programmers) do
    _G["_" .. v] = function(...)
        -- if PUBLIC then return end
        local s = debug.getinfo(2)
        debugprint(table.concat {"debug ", s.short_src, ":", s.currentline}, {...}, true, true)
    end
end

---禁掉print
---[[
print = function()
    error("print is forbidden")
end
--]]

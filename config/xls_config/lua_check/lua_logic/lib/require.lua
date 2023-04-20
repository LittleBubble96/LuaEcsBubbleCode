local str_find = string.find
local str_lower = string.lower


--使用link，这里就可以跟随exe了
local currPath = lfs.currentdir()
local codepaht = "lua_logic"
local originalPath = currPath .. "/" .. codepaht .. "/";

local libpath = "./lib/"
local checkfullpath = originalPath .. "/check"
local clientfullpath = originalPath .. "/resconfig/clientconfig"

-- 修改 clientfullpath 需要修改这里
local excofig = {"/clientconfig/"}

--print("currPath = " .. currPath)
--print("originalPath = " .. originalPath)
--print("codepaht = " .. codepaht)
--print("libpath = " .. libpath)
--print("checkfullpath = " .. checkfullpath)
--print("clientfullpath = " .. clientfullpath)
--print("serverfullpath = " .. serverfullpath)

--print("path = " .. package.path)
package.path = originalPath .. "/?.lua"
--print("package.path = " .. package.path)

local ignoreDir = {"unit_test", "lua_api"}--文件映射忽视的目录

require (libpath .. "object")
require (libpath .. "conf")
require (libpath .. "table_helper")
require (libpath .. "algorithm")
require (libpath .. "sorted_array")
require (libpath .. "sorted_dictionary")
require (libpath .. "string_helper")
require (libpath .. "log_helper")

local filePaths = SortedDictionary:New()
--保证requireall顺序一致
function SearchforStringInWhichFile(path)
    for file in lfs.dir(path) do
        if file ~= "." and file ~= ".." then
            local f = path .. "/" .. file
            local attr = lfs.attributes(f)
            assert(type(attr) == "table")
            if attr.mode == "directory" then
                SearchforStringInWhichFile(f)
            elseif attr.mode == "file" then
                local ignored = false
                for _, ignore in pairs(ignoreDir) do
                    if str_find(f, ignore) then
                        ignored = true
                        break
                    end
                end
                if not ignored then
                    local _, _, head = str_find(file, "(.*)%.lua$")                    
                    if head then
                        local _, _, p = str_find(f, codepaht .."/(.*)%.lua$")
                        --Log.pure("   " ..  f)
                        --Log.pure("adding " .. p .. " <<<<as>>>> " .. head)
                        filePaths:Insert(str_lower(head), p)
                    end
                end
            end
        end
    end
end

local oldRequire = require

function require(name)
    local path = filePaths:Find(name)
    if not path then
        ---[[
        local ok, err = pcall(oldRequire, name)
        if not ok then
            Log.fatal(err)
            return nil
        end
        return err;
        --]]            
        --return oldRequire(name)
    else
        ---[[
        local ok, err = pcall(oldRequire, path)
        if not ok then
            Log.fatal(err)
            return nil
        end
        return err;
        --]]
        --return oldRequire(path)
    end
end

local excludeFiles = {"require", "start", "launch"}
function requireall(excludeDirs)
    for i = 1, filePaths:Size() do
        local file, path = filePaths:GetPairAt(i)
        local bfind = false

        if excludeDirs then
            for _, exclude in pairs(excludeDirs) do
                if str_find(path, exclude) then
                    bfind = true
                    break
                end
            end
        end

        if not bfind then
            local b = false
            for _, v in pairs(excludeFiles) do
                if str_find(file, v) then
                    b = true
                    break
                end
            end
            if not b then
                --Log.pure("require===", path)
                local ok, err = pcall(oldRequire, path)
                if not ok then
                    Log.fatal(err)
                end
            end
        end
    end
end

local oldDofile = dofile
function dofile(name)
    local path = filePaths:Find(name)
    if not path then
        --Log.pure("fullPath name===".. name)
        ---[[
        local ok, err = pcall(oldDofile, name)
        if not ok then
            Log.fatal(err)
            return nil
        end
        return err;
        --]]
        --return oldDofile(name)
    else
        local fullPath = originalPath .. path .. ".lua"
        --Log.pure("fullPath===".. originalPath .. "   " .. path)
        ---[[
        local ok, err = pcall(oldDofile, fullPath)
        if not ok then
            Log.fatal(err)
            return nil
        end
        return err
        --]]
        --return oldDofile(fullPath)
    end
end

--加载config
SearchforStringInWhichFile(clientfullpath)
require("common_data")
requireall()
-- SearchforStringInWhichFile(serverfullpath)
-- requireall();

--加载check文件
SearchforStringInWhichFile(checkfullpath)
---加载一个check的头文件
require ("const_def")
requireall(excofig)

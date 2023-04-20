local cfg_item,config_name = Cfg.cfg_item()

Log.pure("____________________________>>>>>>> start " .. config_name .. " ____________________________")

local function check_config()
    for k, v in pairs(cfg_item) do
        if v.ItemType == 6 then
            if not(v.Overlay == 9999) then
                Log.error(config_name, " id =", k,"wrong overlay999")
            end
        end 

    end
end

--立即执行
check_config()

Log.pure("____________________________" .. config_name .. " end<<<<<<<<< ____________________________")
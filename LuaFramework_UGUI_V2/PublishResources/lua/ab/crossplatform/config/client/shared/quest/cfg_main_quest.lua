---this config file was auto-generated by Excel2lua tool, do not modify it!

local key = {ID=1, Name=2, Cond=3, CondDesc=4, AutoAccept=5, AcceptCond=6, AcceptCondDesc=7, UnlockCond=8, UnlockCondDesc=9, PreID=10, Version=11, }
local config = {
[101]={101, "任务1", "(type:2, target:10 & type:3, target:1000 )|(type:6, target:100)", "str_common_always_true", true, nil, nil, "type :2,target:10", nil, nil, 0, },
[102]={102, "任务2", "type :3,target:1000", "str_common_quest_get_coin", nil, nil, nil, nil, nil, "id:1", },
[103]={103, "任务3", "type :2,target:10", "str_common_always_true", true, nil, nil, nil, nil, "id:1&id:2", 1, },
}
return config, 'ID', key
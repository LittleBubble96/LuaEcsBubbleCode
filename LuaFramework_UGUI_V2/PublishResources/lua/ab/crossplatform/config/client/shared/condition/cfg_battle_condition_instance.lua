---this config file was auto-generated by Excel2lua tool, do not modify it!

local key = {ID=1, ConditionNode=2, Desc=3, Param1=4, Param2=5, Param3=6, Param4=7, Param5=8, }
local config = {
[10000]={10000, 1, "无条件", },
[10100]={10100, 2, "100% 概率", "100", },
[10200]={10200, 3, "起手技能", "1", },
[10201]={10201, 4, "连招数大于等于3之后", "3", },
[10202]={10202, 4, "连招里面的第5次连招", "5", },
[10203]={10203, 4, "连招数大于等于2之后", "2", },
[10204]={10204, 4, "连招数大于等于4之后", "4", },
[10205]={10205, 4, "连招数大于等于10之后", "10", },
[10300]={10300, 5, "当前技能对焦点目标的克制关系是否与参数相同", "1", },
[10301]={10301, 5, "当前技能对焦点目标的克制关系是否与参数相同", "-1", },
[10302]={10302, 5, "当前技能对焦点目标的克制关系是否与参数相同", "0", },
[10400]={10400, 6, "当前技能是否已成功打断目标蓄力", },
[10401]={10401, 0, "当前技能是否暴击过（举例暂未开发）", },
[10501]={10501, 7, "当前血量是否够", "<", "0.7", },
[10601]={10601, 8, "对位或对位两侧是否为空或存活", "0", },
[10602]={10602, 8, "对位或对位两侧是否为空或存活", "1", },
[10603]={10603, 8, "对位或对位两侧是否为空或存活", "1", "0", },
[10701]={10701, 9, "判断属性是否衰减", "RDMPercentage", },
[10801]={10801, 10, "判断角色是否有指定buff", "1", "10003", },
[10802]={10802, 10, "判断角色是否有某类buff", "2", "2", },
[10803]={10803, 10, "怪物B是否有护盾", "1", "202003", },
[10804]={10804, 11, "太阳印记是否6层", "1", "102007", "6", "=", },
[10805]={10805, 10, "怪物A是否有负面buff", "2", "2", },
[10806]={10806, 10, "angry是否有标记", "1", "203001", },
[10807]={10807, 10, "怪物B是否有护盾", "1", "306001", },
[10808]={10808, 10, "怪物B是否有护盾", "1", "304001", },
[10809]={10809, 201, "腐化没有冷却中技能的判断", "1", },
[10810]={10810, 11, "怪物buff超过5层", "1", "215002", "10", ">=", },
[10811]={10811, 11, "区域1（受击切技能）-超过10层", "1", "281505", "10", ">=", },
[10812]={10812, 10, "区域3（单体&群体&次数盾）-greed-技能3-次数盾", "1", "281401", },
[10813]={10813, 20, "玩家主动入战", },
[10814]={10814, 10, "是否有虚弱buff", "1", "282313", },
[11201]={11201, 12, "施放技能类型：普通", "2", "1", },
[11202]={11202, 12, "施放技能类型：必杀", "2", "2", },
[11203]={11203, 12, "施放技能类型：派生", "2", "3", },
[11211]={11211, 12, "耐鲁施放技能2", "1", "10600102", },
[20111]={20111, 103, "连招2次后结束", "2", },
[20112]={20112, 101, "技能2次后结束", "2", },
[20113]={20113, 104, "队伍累计消耗12点行动值", "12", },
[21020]={21020, 102, "连招状态：结束", },
[21021]={21021, 102, "连招状态：连击1次", "1", },
[21022]={21022, 101, "释放三次普通技能", "3", "TRUE", },
[21023]={21023, 12, "普通技能开始", "3", },
[21024]={21024, 13, "连招内最后一个技能是否重复", },
[21025]={21025, 108, "连招离开普通阶段 ", },
[21026]={21026, 102, "连招结束返回True", },
[22001]={22001, 13, "连招内技能重复", },
[22002]={22002, 14, "耗尽被动条件1 -无行动值", "=", "0", },
[22003]={22003, 15, "耗尽被动条件2-行动值满过", },
[32601]={32601, 11, "宇宙技能2派生条件：【强化】达到3层", "1", "103013", "3", ">=", },
[32602]={32602, 11, "宇宙必杀buff条件：【强化】达到5层", "1", "103013", "5", ">=", },
[32801]={32801, 12, "耐鲁-普攻-技能1释放", "1", "12800101", },
[32802]={32802, 102, "连招状态：连击4次", "3", },
[1350301]={1350301, 17, "输出9.5（附加生命）-世界-技能3-自身当前生命比例高于技能目标", ">", },
[1360301]={1360301, 10, "输出9.3（周期减冷却）-柯锡斯-技能3-判断是否有强化效果", "1", "136004", },
[1330307]={1330307, 16, "输出9.2（必杀反消耗）-太阳-技能3-派生点（Hit数大于等于10）", ">=", "10", },
[1340301]={1340301, 16, "共鸣9.2（Hit叠暗伤）-世界-技能3-Buff点（Hit数大于等于2）", ">=", "2", },
[1421302]={1421302, 16, "体验关-世界-技能3-派生点", ">=", "11", },
[1401303]={1401303, 16, "体验关-节制-技能3-派生点", ">=", "6", },
[1401304]={1401304, 18, "体验关-节制-必杀暴击率增加", "1", },
[1401305]={1401305, 11, "挑战关-受击回血", "1", "281507", "11", ">=", },
[1401306]={1401306, 11, "挑战关-受击加防御", "1", "282302", "16", ">=", },
[1471302]={1471302, 19, "V期-输出3-世界(助战)-技能3-全部友方参与连招", "TRUE", "3", },
[148004]={148004, 10, "V期-共鸣1-耐鲁(传递)-技能2-身上是否有【力量传递】buff", "1", "148001", },
[148003]={148003, 12, "V期-共鸣1-耐鲁(传递)-技能3-是否释放了技能3", "1", "14810103", },
[1501205]={1501205, 2, "V期-共鸣3-太阳(终结)-技能2-概率加不进入冷却的效果", "50", },
}
return config, 'ID', key
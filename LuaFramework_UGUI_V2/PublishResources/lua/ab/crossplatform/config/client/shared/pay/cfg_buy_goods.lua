---this config file was auto-generated by Excel2lua tool, do not modify it!

local key = {ID=1, Name=2, Des=3, Price=4, ItemList=5, }
local commonData = {{{21020401,1,}, {21020501,2,}, }, {{21020401,3,}, {21020501,6,}, }, {{3,60,}, }, {{3,600,}, }, }
local config = {
["com.tencent.baiye.item1"]={"com.tencent.baiye.item1", "直购道具1", "内含1张小经验卡,2张大经验卡", 60, commonData[1], },
["com.tencent.baiye.item2"]={"com.tencent.baiye.item2", "直购道具2", "内含3张小经验卡,6张大经验卡", 160, commonData[2], },
["com.tencent.baiye60"]={"com.tencent.baiye60", "60光尘", "光尘60个", 6, commonData[3], },
["com.tencent.baiye600"]={"com.tencent.baiye600", "600光尘", "光尘600个", 60, commonData[4], },
["com.tencent.baiyeint.testitem1"]={"com.tencent.baiyeint.testitem1", "直购道具1", "内含1张小经验卡,2张大经验卡", 60, commonData[1], },
["com.tencent.baiyeint.testitem2"]={"com.tencent.baiyeint.testitem2", "直购道具2", "内含3张小经验卡,6张大经验卡", 160, commonData[2], },
["com.tencent.baiyeint.testdust60"]={"com.tencent.baiyeint.testdust60", "60光尘", "光尘60个", 6, commonData[3], },
["com.tencent.baiyeint.testdust600"]={"com.tencent.baiyeint.testdust600", "600光尘", "光尘600个", 60, commonData[4], },
["com.tencent.baiyeint.testdust1000"]={"com.tencent.baiyeint.testdust1000", "1000光尘", "光尘1000个", 100, {{3,1000,}, }, },
["com.tencent.baiyeint.testitem3"]={"com.tencent.baiyeint.testitem3", "直购道具3", "内含10张小经验卡,15张大经验卡", 200, {{21020401,10,}, {21020501,15,}, }, },
}
return config, 'ID', key
local _true = { }
local _false = { }
local _nil = { }
local function wrapv( v )
	if type( v ) == "boolean" then
		return v and _true or _false
	end
	if v == nil then return _nil end
	return v
end

local function parsewhere(ws)
	local ks = table.keys(ws)
	table.sort(ks)
	local vs={ }
	for i, k in next, ks do
		vs[i] = wrapv( ws[k] )
	end
	return ks, vs
end

local tbs = {}
local tbpkeys = {}
local tbindexs = {}

local function index(tbname, ks, vs)
	local indexk = table.concat(ks,"|")
	local tbpkey = tbpkeys[tbname]
	local tbindex = tbindexs[tbname]
	if not tbindex then
		tbindex = { }
		tbindexs[tbname] = tbindex
	end
	local idx = tbindex[indexk]
	if not idx then
		local tb = tbs[tbname]
		idx={}
		for _, line in next, tb do
			local pkeys = idx
			for ii = 1, #ks do
				local k = ks[ii]
				local v = wrapv( line[k] )
				pkeys[v] = pkeys[v] or { }
				pkeys = pkeys[v]
			end
			table.insert(pkeys, line[tbpkey])
		end
		tbindex[indexk] = idx
	end
	local pv = idx
	for ii = 1, #vs do
		local v = vs[ii]
		pv = pv[v]
		if not pv then return end
	end
	return pv
end

local function pvalue(tbname, cond)
	local ks, vs = parsewhere(cond)
	local len = #ks
	if len <= 0 then
		return
	elseif len==1 and ks[1] == tbpkeys[tbname] then
		return vs[1]
	end
	return index(tbname, ks, vs)
end

local function conf(tbname, cond)
	local tb = tbs[tbname]
	if not tb then
		local cfg, pkey = dofile(tbname)
		--Log.debug( "loadConfig >> tbname:",tbname,">>>cfg:",cfg,">>>cfgType:", type(cfg),">>>pkey:",pkey,">>>pkeyType:", type(pkey), "|########" )
		--cfg._tabname = tbname;
		tbs[tbname] = cfg
		tbpkeys[tbname] = pkey
		tb = tbs[tbname]
	end
	if not cond or not next(cond) then
		return tb, tbname
	end
	local pv = pvalue(tbname, cond)
	if type(pv)=='table' then
		local length = #pv
		if length <= 0 then return end
		local vs={}
		for ii = 1, length do
			vs[ii] = tb[pv[ii]]
		end
		return vs
	end
	local idx = pv and tb[pv]
	return idx and {idx}
end

local oq = { }
local cache = {}
local sets = {}
local names = {}
local function callout(tb, func)
	setmetatable(tb, {__index = function(tb, fn)
		local set = sets[fn]
		if set then return set end
		local cachetb = cache[tb]
		if not cachetb then
			cachetb = { }
			cache[tb] = cachetb
		end
		local cfg = cachetb[fn]
		if cfg then return cfg,fn end
		local v = rawget(tb, fn)
		if v~=nil then
			return v,fn
		end
		cfg = setmetatable({}, {__call = function(_, args)
			return func(tb, fn, args)
		end, __index = function( _, key )
			local v1 = func( tb, fn )
			--_lj( "v1##########", type(v1), key )
			return v1[key]
		end})
		cachetb[fn] = cfg
		return cfg, fn
	end})
end

_G.Cfg={}
callout(Cfg, function(cfg, tbname, cond)
	return conf(tbname, cond)
end)

function _G.CfgClear( tbname )
	if tbname then
		tbs[tbname] = nil
		tbpkeys[tbname] = nil
		tbindexs[tbname] = nil
	else
		tbs={}
		tbpkeys={}
		tbindexs={}
	end
end

_T = function( s ) return s end

function _G.BattleSkillCfg(skillId)
	local skillCfg =  Cfg.cfg_battle_skill[skillId] or Cfg.cfg_pet_battle_skill[skillId]
	return skillCfg
end
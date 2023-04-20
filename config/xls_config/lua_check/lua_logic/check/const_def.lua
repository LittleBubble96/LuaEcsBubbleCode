--这里统一加载 通用的require 和 全局枚举等
require "check_common"


--!!!!!!!!!!!!!!!!!尽量不要写全局变量，除非几个cfg公用

--如果有跳续，这么写也可以应对
skill_scope_type = {};
skill_scope_type[1] = 1;
skill_scope_type[2] = 2;
skill_scope_type[3] = 3;
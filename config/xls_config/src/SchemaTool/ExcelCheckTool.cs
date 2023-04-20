using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchemaTool
{
    ////检查类型
    //enum RuleType
    //{
    //    EXIST,       //数据是否存在于
    //    RANGE,       //数据范围
    //    INVALID,     //无效类型
    //}

    //条件组成关系
    internal enum CompositeType
    {
        CT_AND,     //逻辑运算且
        CT_OR,      //逻辑运算或
    }

    internal enum CompareRelation
    {
        CR_BT,      //大于
        CR_LT,      //小与
        CR_EQ,      //等于
        CR_NEQ,     //不等于
    }

    internal class CheckRule
    {
        public CompositeType m_com_type = CompositeType.CT_AND;
        public List<string> m_desc_params;
        public FlagType m_flag_type = new FlagType();
        public string m_excel_name = "";
        public int m_index = -1;

        /// <summary>
        /// 参数条件初始化,初始化规则时调用
        /// </summary>
        /// <param name="input_params">处理后的条件参数</param>
        /// <param name="excel_name">该规则所在表的名称</param>
        /// <param name="excel_name">该规则为第几项参数</param>
        /// <param name="type">该列的数据类型</param>
        /// <returns>若配置的规则参数错误,则返回false,否则返回true</returns>
        public virtual bool InitParams(List<string> input_params, string excel_name, int index, FlagType type)
        {
            m_flag_type.Copy(type);
            m_desc_params = input_params;
            m_excel_name = excel_name;
            m_index = index;
            return true;
        }

        /// <summary>
        /// 部分条件需要加载完成配置后才能初始化,例如检查是否包含,所以添加了在检查前的规则检查,规则只检查一次用m_load_finish确定
        /// </summary>
        /// <returns>若配置的规则参数错误,则返回false,否则返回true</returns>
        public virtual bool RuleCheckBeforeRun()
        {
            return true;
        }

        /// <summary>
        /// 检测数值是否符合参数的接口
        /// </summary>
        /// <param name="input_datas">待检测的数据所在的一整行数据</param>
        /// <returns>若检查的数值不符合该规则,则返回false,否则返回true</returns>
        public virtual bool Check(List<string> input_datas) { return true; }
    }

    internal class ExistRule : CheckRule
    {
        public List<string> m_id_list = new List<string>();

        public string m_target_cfg_name = "";
        public string m_field_name = "";

        public override bool InitParams(List<string> input_params, string excel_name, int index, FlagType type)
        {
            if (input_params.Count == 0)
            {
                return false;
            }
            m_target_cfg_name = excel_name;
            m_field_name = input_params[0];
            int dot_pos = input_params[0].IndexOf('.');
            if (dot_pos != -1)
            {
                m_target_cfg_name = input_params[0].Substring(0, dot_pos);
                m_field_name = input_params[0].Substring(dot_pos + 1);
            }
            return base.InitParams(input_params, excel_name, index, type);
        }

        public override bool RuleCheckBeforeRun()
        {
            ExcelInfo target_excel_info = new ExcelInfo();
            //如果已经加载过,则读取数据
            if (!ExcelLoader.tables.TryGetValue(m_target_cfg_name, out target_excel_info))
            {
                bool find_flag = false;
                foreach (var cfg in ConfigLoader.appConfig.export)
                {
                    foreach (string file in Directory.EnumerateFiles(cfg.excel_path, m_target_cfg_name + ".xlsx", SearchOption.AllDirectories))
                    {
                        if (Path.GetExtension(file) == ".xlsx" && Path.GetFileNameWithoutExtension(file) == m_target_cfg_name)
                        {
                            target_excel_info = ExcelLoader.CreateExcelInfo(file);
                            find_flag = target_excel_info != null;
                            break;
                        }
                    }
                    if (find_flag)
                    {
                        break;
                    }
                }
                if (!find_flag)
                {
                    return false;
                }
            }
            int field_index = 0;
            if (target_excel_info.FieldIndex.TryGetValue(m_field_name, out field_index))
            {
                FlagInfo flag_info = target_excel_info.FieldFlags[field_index];
                if (m_flag_type.m_type == eFlagType.ARRAY)
                {
                    if (m_flag_type.m_inner_type.m_type != flag_info.m_field_type.m_type)
                        return false;
                }
                else
                {
                    if (m_flag_type.m_type != flag_info.m_field_type.m_type)
                        return false;
                }
                foreach (var row_data in target_excel_info.ExcelRows)
                {
                    m_id_list.Add(row_data.Value[field_index]);
                }
            }
            else
            {
                return false;
            }
            return true;
        }

        public override bool Check(List<string> input_datas)
        {
            //初始化加载检查范围
            string input_data = input_datas[m_index];
            if (string.IsNullOrWhiteSpace(input_data))
            {
                return false;
            }
            return CheckOneData(input_data, m_flag_type, 0);
        }

        private bool CheckOneData(string input_data, FlagType type, int times)
        {
            if (times > 1)
            {
                return false;
            }
            times++;
            if (type.m_type == eFlagType.INT || type.m_type == eFlagType.STRING)
            {
                return m_id_list.Contains(input_data);
            }
            else if (type.m_type == eFlagType.ARRAY)
            {
                List<string> values = input_data.Split(type.m_delimiter).ToList();
                foreach (string it_str in values)
                {
                    if (!CheckOneData(it_str, type.m_inner_type, times))
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }
    }

    internal class RangeRule : CheckRule
    {
        public override bool InitParams(List<string> input_params, string excel_name, int index, FlagType type)
        {
            if (input_params.Count != 2)
            {
                return false;
            }
            bool ret = CheckRange(input_params[0], input_params[1], type, 0);
            if (!ret) return false;
            return base.InitParams(input_params, excel_name, index, type);
        }

        public override bool Check(List<string> input_datas)
        {
            string input_data = input_datas[m_index];
            if (string.IsNullOrWhiteSpace(input_data))
            {
                //内容若不可为空但为空则在加载时已经报错,所以这里允许可空
                return true;
            }
            return CheckOneData(m_desc_params[0], m_desc_params[1], input_data, m_flag_type, 0);
        }

        private bool CheckRange(string minstr, string maxstr, FlagType type, int times)
        {
            //限制嵌套层数
            if (times > 1)
            {
                return false;
            }
            times++;
            bool check_min = minstr.ToLower() != "min";
            bool check_max = maxstr.ToLower() != "max";
            if (type.m_type == eFlagType.INT)
            {
                if (check_max)
                {
                    int max = 0;
                    if (!int.TryParse(maxstr, out max))
                    {
                        return false;
                    }
                }
                if (check_min)
                {
                    int min = 0;
                    if (!int.TryParse(minstr, out min))
                    {
                        return false;
                    }
                }
                return true;
            }
            else if (type.m_type == eFlagType.FLOAT)
            {
                if (check_max)
                {
                    float max = 0;
                    if (!float.TryParse(maxstr, out max))
                    {
                        return false;
                    }
                }
                if (check_min)
                {
                    float min = 0;
                    if (!float.TryParse(minstr, out min))
                    {
                        return false;
                    }
                }
                return true;
            }
            else if (type.m_type == eFlagType.ARRAY)
            {
                return CheckRange(minstr, maxstr, type.m_inner_type, times);
            }
            return false;
        }

        private bool CheckOneData(string min_str, string max_str, string input_data, FlagType type, int times)
        {
            if (times > 1)
            {
                return false;
            }
            times++;
            bool parse_min = min_str.ToLower() != "min";
            bool parse_max = max_str.ToLower() != "max";
            if (type.m_type == eFlagType.INT)
            {
                int data = int.Parse(input_data);
                int min = int.MinValue;
                int max = int.MaxValue;
                if (parse_min)
                {
                    min = int.Parse(min_str);
                }
                if (parse_max)
                {
                    max = int.Parse(max_str);
                }
                return data <= max && data >= min;
            }
            else if (type.m_type == eFlagType.FLOAT)
            {
                float data = float.Parse(input_data);
                float min = float.MinValue;
                float max = float.MaxValue;
                if (parse_min)
                {
                    min = float.Parse(min_str);
                }
                if (parse_max)
                {
                    max = float.Parse(max_str);
                }
                return data <= max && data >= min;
            }
            else if (type.m_type == eFlagType.ARRAY)
            {
                List<string> values = input_data.Split(type.m_delimiter).ToList();
                foreach (string it_str in values)
                {
                    if (!CheckOneData(min_str, max_str, it_str, type.m_inner_type, times))
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }
    }

    internal class NullAbleRule : CheckRule
    {
        public CompareRelation m_cond_compare = CompareRelation.CR_EQ;
        public int m_target_index = -1;
        public string m_target_field;
        public string m_target_value;

        public override bool InitParams(List<string> input_params, string excel_name, int index, FlagType type)
        {
            if (input_params.Count == 0)
            {
                return false;
            }

            if (input_params[0].IndexOf("==") != -1)
            {
                m_cond_compare = CompareRelation.CR_EQ;
                int sym_index = input_params[0].IndexOf("==");
                m_target_field = input_params[0].Substring(0, sym_index);
                m_target_value = input_params[0].Substring(sym_index + 2);
            }
            else if (input_params[0].IndexOf("!=") != -1)
            {
                m_cond_compare = CompareRelation.CR_NEQ;
                int sym_index = input_params[0].IndexOf("!=");
                m_target_field = input_params[0].Substring(0, sym_index);
                m_target_value = input_params[0].Substring(sym_index + 2);
            }
            else
            {
                return false;
            }

            return base.InitParams(input_params, excel_name, index, type);
        }

        public override bool RuleCheckBeforeRun()
        {
            ExcelInfo target_excel_info = new ExcelInfo();

            if (!ExcelLoader.tables.TryGetValue(m_excel_name, out target_excel_info))
            {
                return false;
            }
            if (!target_excel_info.FieldIndex.TryGetValue(m_target_field, out m_target_index))
            {
                return false;
            }
            return true;
        }

        public override bool Check(List<string> input_datas)
        {
            //若待检查数值不为空,返回真,为空则检查是否满足条件
            string input_data = input_datas[m_index];
            if (!string.IsNullOrWhiteSpace(input_data))
            {
                return true;
            }
            string rule_data = input_datas[m_target_index];

            if (m_cond_compare == CompareRelation.CR_EQ)
            {
                return rule_data == m_target_value;
            }
            else if (m_cond_compare == CompareRelation.CR_NEQ)
            {
                return rule_data != m_target_value;
            }
            else
            {
                return false;
            }
        }
    }

    internal class CheckFactory
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 根据规则描述字符串生成单个检查规则
        /// </summary>
        /// <param name="desc">规则字符串</param>
        /// <param name="rule">单个规则</param>
        /// <return> 正确生成规则时返回值为true,否则为false</return>>
        public static bool TryCreateCheckRule(string desc, string excel_name, int field_index, FlagType flag_type, out CheckRule rule)
        {
            int left_bracket_pos = desc.IndexOf('(');
            int right_bracket_pos = desc.IndexOf(')');
            rule = null;
            if (left_bracket_pos * right_bracket_pos < 0)
            {
                return false;
            }
            //单个检查规则只能有一组参数
            if (left_bracket_pos != desc.LastIndexOf('(') || right_bracket_pos != desc.LastIndexOf(')'))
            {
                return false;
            }

            string rule_type_str = (left_bracket_pos == -1) ? desc.Substring(0) : desc.Substring(0, left_bracket_pos);
            rule_type_str = rule_type_str.ToLower();
            if (rule_type_str == "range")
            {
                rule = new RangeRule();
            }
            else if (rule_type_str == "exist")
            {
                rule = new ExistRule();
            }
            else if (rule_type_str == "nullable")
            {
                rule = new NullAbleRule();
            }
            if (rule == null)
            {
                return false;
            }

            if (left_bracket_pos == -1)
            {
                return true;
            }
            string rule_cond_str = desc.Substring(left_bracket_pos + 1, right_bracket_pos - left_bracket_pos - 1);
            if (!rule.InitParams(GetRuleParams(rule_cond_str), excel_name, field_index, flag_type))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 根据规则描述字符串生成规则集合
        /// </summary>
        /// <param name="desc">规则</param>
        /// <param name="list_desc">规则集合</param>
        /// <return> 正确生成集合时返回值为true,否则为false</return>>
        public static bool TryCreateRuleList(string desc, string excel_name, int field_index, FlagType flag_type, out List<CheckRule> list_desc)
        {
            list_desc = new List<CheckRule>();
            List<string> rule_strs = new List<string>();
            char[] link_symbols = { '|', '&' };
            int symbol_pos = 0;
            while (symbol_pos != -1)
            {
                int last_pos = symbol_pos;
                last_pos = (last_pos == 0) ? 0 : last_pos + 1;
                symbol_pos = desc.IndexOfAny(link_symbols, last_pos);
                string rule_str = (symbol_pos == -1) ? desc.Substring(last_pos) : desc.Substring(last_pos, symbol_pos - last_pos);
                CheckRule rule = null;
                if (!TryCreateCheckRule(rule_str, excel_name, field_index, flag_type, out rule))
                {
                    logger.Error($"{rule_str}配置有误,请检查规则名及参数");
                    return false;
                }
                if (last_pos != 0)
                {
                    switch (desc[last_pos - 1])
                    {
                        case '|': rule.m_com_type = CompositeType.CT_OR; break;
                        case '&': rule.m_com_type = CompositeType.CT_AND; break;
                    }
                }
                list_desc.Add(rule);
            }
            return true;
        }

        private static List<string> GetRuleParams(string rule_cond_str)
        {
            rule_cond_str = rule_cond_str + ',';
            List<string> rule_params = new List<string>();
            int cur_pos = rule_cond_str.IndexOf(',');
            int last_pos = 0;
            while (cur_pos != -1)
            {
                string param_str = rule_cond_str.Substring(last_pos, cur_pos - last_pos);
                last_pos = cur_pos + 1;
                cur_pos = rule_cond_str.IndexOf(',', last_pos);
                if (!string.IsNullOrWhiteSpace(param_str))
                {
                    rule_params.Add(param_str);
                }
            }
            return rule_params;
        }
    }

    //使用项目本身的检测条件
    //class ExcelCheckTool
    //{
    //    static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
    //    public static bool CheckExcel(string excel_path)
    //    {
    //        bool all_check = true;
    //        foreach (var excel_info in ExcelLoader.tables.Values)
    //        {
    //            //跳过非本目录下的excel
    //            if (!excel_info.ExcelPath.StartsWith(excel_path))
    //            {
    //                continue;
    //            }
    //            //根据检查规则遍历需要检查的每一列
    //            foreach (var rules_pair in excel_info.FieldCheckRules)
    //            {
    //                int field_index = rules_pair.Key;
    //                List<CheckRule> rule_list = rules_pair.Value;

    //                if (!CheckRuleBeforeCheckCfg(rule_list))
    //                {
    //                    string col_no = ExcelLoader.GetColumnChar(field_index);
    //                    logger.Error($"表 {excel_info.ExcelName} 检查规则行 第 {col_no} 列,检查规则有误(可能是引用其他配置文件时表名或列名错误),请检查.");
    //                    ExcelLoader.RemoveErrorFilesMd5(excel_info.ExcelPath);
    //                    all_check = false;
    //                    continue;
    //                }

    //                var it = excel_info.ExcelRows.GetEnumerator();
    //                while(it.MoveNext())
    //                {
    //                    var row = it.Current;
    //                    List<string> data = row.Value;
    //                    string row_key = row.Key;

    //                    if(!CheckSingleCellData(rule_list,data))
    //                    {
    //                        string col_no = ExcelLoader.GetColumnChar(field_index);
    //                        logger.Error($"表 {excel_info.ExcelName} 行主键: {row_key}, 第 {col_no} 列,数据内容不满足检查规则,请检查.");
    //                        ExcelLoader.RemoveErrorFilesMd5(excel_info.ExcelPath);
    //                        all_check = false;
    //                        continue;
    //                    }
    //                }
    //            }
    //        }
    //        logger.Trace("----------------------检查完毕！---------------------------");
    //        return all_check;
    //    }

    //    public static bool CheckRuleBeforeCheckCfg(List<CheckRule> rule_list)
    //    {
    //        bool all_true = true;
    //        foreach (CheckRule rule in rule_list)
    //        {
    //            if(!rule.RuleCheckBeforeRun())
    //            {
    //                all_true = false;
    //            }
    //        }
    //        return all_true;
    //    }

    //    public static bool CheckSingleCellData(List<CheckRule> rule_list, List<string> data)
    //    {
    //        bool ret = true;
    //        foreach (CheckRule rule in rule_list)
    //        {
    //            bool one_ret = rule.Check(data);
    //            if (rule.m_com_type == CompositeType.CT_AND)
    //            {
    //                ret = ret && one_ret;
    //            }
    //            else if (rule.m_com_type == CompositeType.CT_OR)
    //            {
    //                ret = ret || one_ret;
    //            }
    //        }
    //        return ret;
    //    }
    //}
}
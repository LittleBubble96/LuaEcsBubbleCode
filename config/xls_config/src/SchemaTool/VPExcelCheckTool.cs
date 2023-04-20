using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchemaTool
{
    /// <summary>
    /// 描述数据类
    /// </summary>
    internal class DescData
    {
        public DescData()
        {
        }

        public DescData(string data, FlagType flagType)
        {
            Init(data, flagType);
        }

        public void Init(string data, FlagType flagType)
        {
            FlagType = flagType;

            if (string.IsNullOrWhiteSpace(data))
            {
                Data = null;
                IsNull = true;
                Length = 0;
            }
            else
            {
                Data = data;
                IsNull = false;
                if (FlagType.m_type == eFlagType.ARRAY)
                {
                    string[] ss = Data.Split(FlagType.m_delimiter);
                    Length = ss.Length;
                }
                else
                {
                    Length = 1;
                }
            }
        }

        public bool IsNull { get; private set; }
        public int Length { get; private set; }
        public FlagType FlagType { get; private set; }
        public string Data { get; private set; }
    }

    internal class VPExcelCheckTool
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private static string s_ExcelPath = "";
        public static Dictionary<string, ExcelInfo> checktables = new Dictionary<string, ExcelInfo>();

        /// <summary>
        /// 检测每个表中的约束条件
        /// 约束条件不能循环引用
        /// </summary>
        public static bool CheckExcelFieldCondition(string excel_path)
        {
            s_ExcelPath = excel_path;
            foreach (var excelInfo in ExcelLoader.tables.Values)
            {
                foreach (var fieldCondition in excelInfo.FieldIndexCondition)
                {
                    //字段条件列Index下标
                    var conditionIndex = fieldCondition.Key;
                    //字段条件内容
                    var conditionValue = fieldCondition.Value;

                    //容器用来检测是否存在循环引用 每个新的条件检测列 重新生成一个
                    Dictionary<string, List<string>> container = new Dictionary<string, List<string>>();

                    //遍历每一行数据
                    var itor = excelInfo.ExcelRows.GetEnumerator();
                    while (itor.MoveNext())
                    {
                        //key list<string>
                        var row = itor.Current;
                        //找到对应行中对应表格数据
                        string data = row.Value[conditionIndex];
                        string rowKey = row.Key;

                        //遍历所有有条件约束的表的数据
                        if (!CheckSingleCellData(excelInfo, rowKey, conditionIndex, conditionValue, data, container))
                        {
                            int errorColumn = conditionIndex + 1;
                            logger.Error(excelInfo.ExcelName+"表"+" 行主键:"+ rowKey + " 第"+errorColumn+ "列"+ "数据内容: "+ data+" 不满足约束条件---导表失败");
                            ExcelLoader.RemoveErrorFilesMd5(excelInfo.ExcelPath);
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 检测单元格数据是否满足条件
        /// </summary>
        /// <param name="conditionValue">条件列表 </param>
        /// <param name="data">单元格数据</param>
        private static bool CheckSingleCellData(ExcelInfo excelInfo, string rowKey, int conditionIndex, List<string> conditionValue, string data, Dictionary<string, List<string>> container)
        {
            var columnNum = conditionIndex + 1;
            List<bool> conditionGroupsResult = new List<bool>();
            List<string> conditionGroupLogicStr = new List<string>();

            if (conditionValue.Count>2)
            {
                conditionGroupLogicStr = conditionValue.Last().Split(':').ToList();
                if (conditionGroupLogicStr.Count!= conditionValue.Count-2)
                {
                    logger.Error(excelInfo.ExcelName + "表 " + "第" + columnNum + "列" +"  逻辑判断组符号应该比条件组数目少一个 配置错误");
                    return true;
                }
            }
            else if(conditionValue.Count==2)
            {
                logger.Error(excelInfo.ExcelName + "表 " + "第" + columnNum + "列" + "约束条件组有两个的时候 至少要有一个逻辑判断组 配置错误");
                return true;
            }
            //条件组数目==1 或者大于等于3
            for (int i = 0; i < conditionValue.Count; i++)
            {
                //条件项 可能是条件组 cnd,cnd,& 也可能是 条件组的关系 &
                var conditionGroup = conditionValue[i];
                //条件组中单个条件结果
                List<bool> conditionGroupItemResult = new List<bool>();

                //条件组拆分为单个条件与逻辑关系
                string[] conditionStrs = conditionGroup.Split('-');
                try
                {
                    string[] conditionGroupItemCndStrs = conditionStrs[0].Split(':');
                    if (conditionGroupItemCndStrs[0]=="&"|| conditionGroupItemCndStrs[0] == "|")
                    {
                        //达到条件组中的末尾
                        break;
                    }

                    List<string> conditionGroupItemLogicStr=new List<string>();
                    if (conditionGroupItemCndStrs.Length>1)
                    {
                        conditionGroupItemLogicStr = conditionStrs[1].Split(':').ToList();

                        if (conditionGroupItemLogicStr.Count!= conditionGroupItemCndStrs.Length-1)
                        {
                            logger.Error(excelInfo.ExcelName + "表 " + columnNum + "列" + conditionStrs[0] + "条件组内 逻辑判断符号应该比条件数目少一个 配置错误");
                            return true;
                        }
                    }

                    for (int j = 0; j < conditionGroupItemCndStrs.Length; j++)
                    {
                        string[] fieldstr = conditionGroupItemCndStrs[j].Split('@');
                        if (fieldstr.Length != 2)
                        {
                            logger.Error(excelInfo.ExcelName + "表 " + columnNum + "列" + conditionValue[i] + " 检测条件配置错误");
                            return true;
                        }
                        //this.ID.Value or data
                        string[] target = fieldstr[0].Split('.');
                        //Range(0,10)
                        string function = fieldstr[1];

                        bool singleConditionItemResult = GetSingleConditionItemResult(excelInfo, rowKey, conditionIndex, target, function, data, container);
                        conditionGroupItemResult.Add(singleConditionItemResult);
                    }

                    if (conditionGroupItemResult.Count > 0)
                    {
                        bool conditionGroupResult = GetConditionGroupResult(conditionGroupItemResult, conditionGroupItemLogicStr);
                        conditionGroupsResult.Add(conditionGroupResult);
                    }
                }
                catch
                {
                    return true;
                }
            }

            bool finalResult = GetConditionGroupResult(conditionGroupsResult, conditionGroupLogicStr);
            return finalResult;
        }

        /// <summary>
        /// 得到条件组结果
        /// </summary>
        /// <param name="conditionGroupItemResult"></param>
        /// <param name="logicalOperators"></param>
        /// <returns></returns>
        private static bool GetConditionGroupResult(List<bool> conditionGroupItemResult, List<string> logicalOperatorsList)
        {
            bool result = conditionGroupItemResult[0];

            if (conditionGroupItemResult.Count ==1)
            {
                return result;
            }

            List<bool> cndItemResultList = new List<bool>();
            cndItemResultList.Add(result);
            for (int i = 1; i < conditionGroupItemResult.Count; i++)
            {
                cndItemResultList.Add(conditionGroupItemResult[i]);
                bool cndItemResult = GetConditionItemPairResult(cndItemResultList, logicalOperatorsList[i-1]);

                cndItemResultList.Clear();
                cndItemResultList.Add(cndItemResult);
            }

            return cndItemResultList.First();
        }

        /// <summary>
        /// 得到条件之间的结果
        /// </summary>
        /// <param name="conditionGroupItemResult"></param>
        /// <param name="logicalOperators"></param>
        /// <returns></returns>
        private static bool GetConditionItemPairResult(List<bool> conditionGroupItemResult, string logicalOperators)
        {
            bool result = conditionGroupItemResult[0];
            for (int i = 0; i < conditionGroupItemResult.Count; i++)
            {
                if (logicalOperators == "&")
                {
                    result = result && conditionGroupItemResult[i];
                }
                else if (logicalOperators == "|")
                {
                    result = result || conditionGroupItemResult[i];
                }
                else
                {
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// 得到单个条件项的结果
        /// </summary>
        /// <param name="excelInfo">当前检测的Excel表</param>
        /// <param name="rowKey">行的键值 通过这个确定数据在哪一行</param>
        /// <param name="conditionIndex">列序号</param>
        /// <param name="target">配置的条件字符串 表示 目标数据 </param>
        /// <param name="function">配置的条件字符串 表示 条件方法</param>
        /// <param name="data">当前检测的单元格内的字符串</param>
        /// <param name="container">容器用来检测是否存在循环引用</param>
        /// <returns></returns>
        private static bool GetSingleConditionItemResult(ExcelInfo excelInfo, string rowKey, int conditionIndex, string[] target, string function, string data, Dictionary<string, List<string>> container)
        {
            var columnNum = conditionIndex + 1;
            DescData targetData = new DescData();

            if (target.Length == 1)
            {
                //当前单元格数据
                if (target[0].ToLower() == "data")
                {
                    var flagInfo= excelInfo.FieldFlags[conditionIndex];
                    targetData = new DescData(data, flagInfo.m_field_type);
                }
            }else if (target.Length==2)
            {
                if (target[0].ToLower() == "this")
                {
                    string targetField = target[1];
                    if (excelInfo.FieldIndex.ContainsKey(targetField))
                    {
                        int targetFieldColumnIndex = excelInfo.FieldIndex[targetField];
                        var flagInfo = excelInfo.FieldFlags[targetFieldColumnIndex];
                        string selectdata = excelInfo.ExcelRows[rowKey][targetFieldColumnIndex];

                        targetData = new DescData(selectdata, flagInfo.m_field_type);
                    }
                    else
                    {
                        logger.Error(excelInfo.ExcelName + "表 " + columnNum + "列 配置错误 没有在" + target + "中找到目标字段");
                        return true;
                    }
                }
            }

            string[] functionStrs = function.Split('(');
            string functionParamStr = functionStrs[1].Replace(")", "");

            string functionName = functionStrs[0];
            string[] functionParams = functionParamStr.Split(',');

            if (functionName.ToLower()=="range")
            {
                return Range(targetData,functionParams);
            }
            else if (functionName.ToLower() == "length")
            {
                return Length(targetData, functionParams);
            }
            else if (functionName.ToLower() == "exist")
            {
                return Exist(excelInfo, targetData, functionParams);
            }
            else if (functionName.ToLower() == "notnull")
            {
                return NotNull(targetData);
            }
            else if (functionName.ToLower() == "isnull")
            {
                return IsNull(targetData);
            }
            else if (functionName.ToLower() == "notcycleref")
            {
                return NotCycleRef(targetData, rowKey, container);
            }

            return true;
        }

        /// <summary>
        /// 检测值不能循环引用
        /// </summary>
        /// <param name="targetData"></param>
        /// <param name="rowKey"></param>
        /// <param name="container">循环检测容器</param>
        /// <returns></returns>
        private static bool NotCycleRef(DescData targetData, string rowKey, Dictionary<string, List<string>> container)
        {
            if (targetData.FlagType.m_type != eFlagType.INT && targetData.FlagType.m_type != eFlagType.ARRAY && targetData.FlagType.m_type != eFlagType.STRING)
            {
                logger.Error("不存在循环引用的条件 检测的数据需要是整型 字符串 或者数组格式");
                return true;
            }
            if (targetData.Data == null)
                return true;

            List<string> dataValue = new List<string>();

            if (targetData.FlagType.m_type == eFlagType.ARRAY)
            {
                string[] dataItemArray = targetData.Data.Split(targetData.FlagType.m_delimiter);
                for (int i = 0; i < dataItemArray.Length; i++)
                {
                    string item = dataItemArray[i];
                    dataValue.Add(item);
                }
            }
            else
            {
                dataValue.Add(targetData.Data);
            }

            foreach (var item in container)
            {
                for (int i = 0; i < item.Value.Count; i++)
                {
                    if (item.Value[i]== rowKey)
                    {
                        for (int j = 0; j < dataValue.Count; j++)
                        {
                            if (container.ContainsKey(dataValue[j]))
                            {
                                logger.Error("存在循环引用条件");
                                return false;
                            }
                        }
                    }
                }
            }
            container.Add(rowKey, dataValue);

            return true;
        }

        private static bool IsNull(DescData targetData)
        {
            if (targetData.Data == null)
                return true;

            return false;
        }

        private static bool NotNull(DescData targetData)
        {
            if (targetData.Data == null)
                return false;

            return true;
        }

        /// <summary>
        /// 表示目标值存在目标列中
        /// </summary>
        /// <param name="targetData"></param>
        /// <param name="functionParams">目标表.目标字段</param>
        /// <returns></returns>
        private static bool Exist(ExcelInfo excelInfo, DescData targetData, string[] functionParams)
        {
            if (targetData.FlagType.m_type!= eFlagType.INT&& targetData.FlagType.m_type != eFlagType.ARRAY && targetData.FlagType.m_type != eFlagType.STRING)
            {
                logger.Error("检测的数据需要是整型 字符串 或者数组格式");
                return true;
            }

            //如果为空则返回包含
            if (targetData.IsNull)
            {
                return true;
            }

            if (functionParams.Length != 1)
            {
                logger.Error("Exsit 方法的参数应该是1个 请检查");
                return true;
            }
            string[] paramStrs = functionParams[0].Split('.');
            if (paramStrs.Length!=2)
            {
                logger.Error("Exsit 方法的参数格式错误 应该是xxx.字段 如 this.ID 请检查");
                return true;
            }
            string targetExcelName = paramStrs[0].Replace(" ","");
            string targetExcelField = paramStrs[1].Replace(" ", ""); ;

            ExcelInfo targetExcelInfo = new ExcelInfo();
            if (targetExcelName.ToLower()=="this")
            {
                targetExcelInfo = excelInfo;
            }
            else
            {
                if (ExcelLoader.tables.ContainsKey(targetExcelName))
                {
                    targetExcelInfo = ExcelLoader.tables[targetExcelName];
                }
                else
                {
                    targetExcelInfo = LoadExcel(targetExcelName);
                    if (targetExcelInfo.ExcelName != targetExcelName)
                    {
                        logger.Error("Exsit 方法的参数错误 没有找到" + targetExcelName + "表请检查");
                        return true;
                    }
                }
            }

            int targetColumn = 0;
            if (targetExcelInfo.FieldIndex.ContainsKey(targetExcelField))
            {
                targetColumn = targetExcelInfo.FieldIndex[targetExcelField];
            }
            else
            {
                logger.Error("Exsit 方法的参数错误 没有找到" + targetExcelName + "表中 "+ targetExcelField+"字段请检查");
                return true;
            }

            if (targetExcelInfo.FieldFlags[targetColumn].m_field_type.m_type != eFlagType.INT)
            {
                logger.Error("Exsit 方法的参数错误" + targetExcelName + "表中 " + targetExcelField + "应该是整型字段");
                return true;
            }
            bool isContain = false;

            if (targetData.FlagType.m_type== eFlagType.ARRAY)
            {
                string[] dataItemArray = targetData.Data.Split(targetData.FlagType.m_delimiter);
                for (int i = 0; i < dataItemArray.Length; i++)
                {
                    isContain = false;
                    string item = dataItemArray[i];
                    foreach (var excelRow in targetExcelInfo.ExcelRows.Values)
                    {
                        if (excelRow[targetColumn].Contains(item))
                        {
                            isContain = true;
                            break;
                        }
                    }

                    if (isContain==false)
                    {
                        break;
                    }
                }
            }
            else
            {
                foreach (var excelRow in targetExcelInfo.ExcelRows.Values)
                {
                    if (excelRow[targetColumn].Contains(targetData.Data))
                    {
                        isContain = true;
                        break;
                    }
                }
            }
            return isContain;
        }

        private static ExcelInfo LoadExcel(string excelName)
        {
            //节约时间
            if(checktables.ContainsKey(excelName))
            {
                return checktables[excelName];
            }
            else
            {
                ExcelInfo excelInfo = new ExcelInfo();

                foreach (string file in Directory.EnumerateFiles(s_ExcelPath, excelName + ".xlsx", SearchOption.AllDirectories))
                {
                    if (Path.GetExtension(file) == ".xlsx" && Path.GetFileNameWithoutExtension(file) == excelName)
                    {
                        excelInfo = ExcelLoader.CreateExcelInfo(file);
                        break;
                    }
                }

                checktables.Add(excelInfo.ExcelName, excelInfo);
                return excelInfo;
            }
        }

        /// <summary>
        /// 表示 目标值在范围内
        /// </summary>
        /// <param name="targetData">整型或者浮点型数据</param>
        /// <param name="functionParams">最小值 最大值</param>
        /// <returns></returns>
        private static bool Range(DescData targetData,string[] functionParams)
        {
            if (targetData.FlagType.m_type != eFlagType.INT && targetData.FlagType.m_type != eFlagType.FLOAT)
            {
                logger.Error("数据格式不是 整数或者浮点数 不能适用Range方法判断 ");
                return true;
            }

            if (functionParams.Length !=2)
            {
                logger.Error("Range 方法的参数应该是两个 请检查");
                return true;
            }

            //数据为空始终满足
            if (targetData.Data==null)
            {
                return true;
            }

            string minStr = functionParams[0];
            string maxStr = functionParams[1];

            if (targetData.FlagType.m_type== eFlagType.INT)
            {
                int data = int.Parse(targetData.Data);
                int min = int.MinValue;
                int max = int.MaxValue;

                if (minStr.ToLower() != "min")
                {
                    if (!int.TryParse(minStr, out min))
                    {
                        logger.Error("检测数据是整型 Range 方法的参数应该是两个整数 请检查");
                        return true;
                    }
                }
                if (maxStr.ToLower() != "max")
                {
                    if (!int.TryParse(maxStr, out max))
                    {
                        logger.Error("检测数据是整型 Range 方法的参数应该是两个整数 请检查");
                        return true;
                    }
                }

                if (data<= max&& data>=min)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            if (targetData.FlagType.m_type == eFlagType.FLOAT)
            {
                float data = float.Parse(targetData.Data);
                float min = float.MinValue;
                float max = float.MaxValue;

                if (minStr.ToLower() != "min")
                {
                    if (!float.TryParse(minStr, out min))
                    {
                        logger.Error("检测数据是浮点型 Range 方法的参数应该是两个浮点型 请检查");
                        return true;
                    }
                }
                if (maxStr.ToLower() != "max")
                {
                    if (!float.TryParse(maxStr, out max))
                    {
                        logger.Error("检测数据是浮点型 Range 方法的参数应该是两个浮点型 请检查");
                        return true;
                    }
                }

                if (data <= max && data >= min)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 表示 目标值长度在范围内
        /// </summary>
        /// <param name="targetData">整型或者浮点型数据</param>
        /// <param name="functionParams">最小长度 最大长度 如果只有一个参数 就是设定长度要是多少</param>
        /// <returns></returns>
        private static bool Length(DescData targetData, string[] functionParams)
        {
            if (targetData.FlagType.m_type == eFlagType.TIME)
            {
                logger.Error("时间类型不适合 length 方法判断长度 ");
                return true;
            }

            if (functionParams.Length != 2&& functionParams.Length!=1)
            {
                logger.Error("Length 方法的参数应该是 一个 或者 两个 请检查");
                return true;
            }

            if (functionParams.Length == 1)
            {
                string lengthStr = functionParams[0];
                int length = 0;
                if (!int.TryParse(lengthStr, out length))
                {
                    logger.Error("Length 方法的参数为一个时 参数应该是 整数  请检查");
                    return true;
                }
                return targetData.Length == length;
            }
            else if (functionParams.Length == 2)
            {
                string minLengthStr = functionParams[0];
                string maxLengthStr = functionParams[1];
                int minLength = int.MinValue;
                int maxLength = int.MaxValue;

                if (minLengthStr.ToLower()!="min")
                {
                    if (!int.TryParse(minLengthStr, out minLength))
                    {
                        logger.Error("Length 方法的参数为两个时 参数应该是  两个 整数 或者 (min,max) 请检查");
                        return true;
                    }
                }

                if (maxLengthStr.ToLower() != "max")
                {
                    if (!int.TryParse(maxLengthStr, out maxLength))
                    {
                        logger.Error("Length 方法的参数为两个时 参数应该是  两个 整数 或者 (min,max) 请检查");
                        return true;
                    }
                }

                if (targetData.Length<= maxLength&& targetData.Length>= minLength)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }
    }
}
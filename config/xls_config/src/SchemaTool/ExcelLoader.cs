using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using Monitor.Core.Utilities;

namespace SchemaTool
{
    using CommonDataTable = Dictionary<FlagType, Dictionary<string, CommonData>>;
    class ExcelInfo
    {
        //路径
        public string ExcelPath;
        //名字
        public string ExcelName;
        //字段名-下标
        public Dictionary<string, int> FieldIndex = new Dictionary<string, int>();
        //下标-字段
        public List<string> FieldNames = new List<string>();
        //标记
        public List<FlagInfo> FieldFlags = new List<FlagInfo>();
        //所有行，必有key
        public Dictionary<string, List<string>> ExcelRows = new Dictionary<string, List<string>>();

        public Dictionary<string, Dictionary<string, List<string>>> DevideExcelRows = null;
        //索引键集合
        public List<string> IndexList = null;

        public Dictionary<string, List<string>> LinkRelations = new Dictionary<string, List<string>>();

        public List<int> ArrayParts = new List<int>();

        public CommonDataTable one_sheet_data_table = null;

        //列下标 - 字段条件列表
        public Dictionary<int, List<CheckRule>> FieldCheckRules = new Dictionary<int, List<CheckRule>>();
		//项目检查规则
        public Dictionary<int, List<string>> FieldIndexCondition = new Dictionary<int, List<string>>();


        //key字段
        public string KeyFieldName;
        //index字段,用于生成索引键表
        public FlagInfo IndexFieldType = new FlagInfo();

        public FlagInfo LinkFieldType = new FlagInfo();
        public string LinkFieldName;
        //是否输出csv
        public bool csvflag = false;
        //是否需要拆分
        public bool devideflag = false;
        //拆分间隔
        public int devide_num = -1;
        //是否需要索引键
        public bool indexflag = false;
    }

    class CommonData
    {
        public FlagInfo flag_info = new FlagInfo();
        public bool duplicate = false;
        public int index = 0;
        public CommonData(FlagInfo info)
        {
            flag_info.Copy(info);
        }
    }

    class MD5Info
    {
        //配置文件,工具可执行文件等MD5
        public Dictionary<string, string> other_MD5 = new Dictionary<string, string>();
        //输入文件MD5
        public Dictionary<string, string> input_MD5 = new Dictionary<string, string>();
        //输出文件MD5
        public Dictionary<string, string> output_MD5 = new Dictionary<string, string>();
    }


    static class ExcelLoader
    {
        static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static Dictionary<string, ExcelInfo> tables = new Dictionary<string, ExcelInfo>();
        public static Dictionary<string, ExcelInfo> revert_tables = new Dictionary<string, ExcelInfo>();
        public static MD5Info changes = new MD5Info();

        public static Dictionary<string, string> repeat_values = new Dictionary<string, string>();
        public static List<string> exist_files = new List<string>();
        public static List<string> lost_files = new List<string>();
        public const string config_name = "appconfig";
        private const string exe_name = "app";
        private const string md5_name = "md5.json";

        public static List<string> filePaths = new List<string>();
        public static List<string> OutputChangedXlsx = new List<string>();

        public static bool m_force = false;
        public static bool m_config_change = false;
        public static bool m_exe_change = false;

        public static string config_md5 = "";
        static MD5 md5 = new MD5CryptoServiceProvider();
        static HashSet<string> output_type_list = new HashSet<string>() { "lua", "json", "csv"};

        public static void Init(string md5_path, string config_path, string global_config_path)
        {
            LoadMd5(md5_path, global_config_path);
            m_config_change = CheckFileChange(config_path, config_name);
            string exe_path = System.Reflection.Assembly.GetExecutingAssembly().Location;
            m_exe_change = CheckFileChange(exe_path, exe_name);
        }

        public static void LoadFile(string dir)
        {
            string md5file = ConfigLoader.appConfig.md5_path;
            Stopwatch watcher = new Stopwatch();
            watcher.Start();
            int file_count = LoadInputDir(dir);
            watcher.Stop();
            logger.Info($"加载{file_count}个表格，用时{watcher.ElapsedMilliseconds}毫秒");
        }

        public static bool CheckFileChange(string file_path, string md5_name)
        {
            string md5_str;
            using (FileStream fs = new FileStream(file_path, FileMode.Open, FileAccess.Read))
            {
                byte[] retval = md5.ComputeHash(fs);
                StringBuilder sb = new StringBuilder();
                foreach (byte b in retval)
                {
                    sb.Append(b.ToString("x2"));
                }
                md5_str = sb.ToString();
            }
            if (!changes.other_MD5.ContainsKey(md5_name))
            {
                changes.other_MD5.Add(md5_name, md5_str);
            }
            else
            {
                if (changes.other_MD5[md5_name] == md5_str)
                {
                    return false;
                }
                changes.other_MD5[md5_name] = md5_str;
            }
            logger.Trace($"{file_path} changed");
            return true;
        }
        public static int LoadInputDir(string dir)
        {
            int file_count = 0;
            if (!Directory.Exists(dir))
            {
                logger.Trace($"路径{dir}不存在");
                return 0;
            }
            foreach (string file in Directory.EnumerateFiles(dir))
            {
                string file_path = file.Replace('\\', '/');
                bool legal = CheckPreFixFile(file_path);
                if (legal == true)
                {
                    if (Path.GetExtension(file_path) == ".xlsx")
                    {
                        if (ImportExcelFileNoSchema(file_path))
                            file_count++;
                        FilePathsFolderAdd(file_path);
                        filePaths.Add(file_path);
                    }
                    if (Path.GetExtension(file_path) == ".csv")
                    {
                        ImportCsvFileNoSchema(file_path);
                        file_count++;
                        FilePathsFolderAdd(file_path);
                        filePaths.Add(file_path);
                    }
                }
                else
                {
                    logger.Fatal("不合法文件: " + file);
                }
            }
            foreach (string subdir in Directory.EnumerateDirectories(dir))
            {
                file_count += LoadInputDir(subdir);
            }
            return file_count;
        }

        public static void EndExporting(string md5path, string config_path)
        {
            RevertChangedOutput();
            SaveMD5(md5path);
			//项目不使用p4
            //ReconcileOutputFiles();
            //ReconcileOtherFiles(config_path);
        }

        public static bool CheckPreFixFile(string filePath)
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            if (fileName.Contains("$") || fileName.Contains("~") || fileName.Contains("~$"))
            {
                return false;
            }
            return true;
        }

        public static void FilePathsFolderAdd(string filePath)
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            foreach (var file in filePaths)
            {
                string name = Path.GetFileNameWithoutExtension(file);
                if (name == fileName && filePath != file)
                {
                    logger.Fatal("同名文件路径: " + file);
                    logger.Fatal("同名文件路径: " + filePath);
                    Console.ReadKey();
                }
            }
        }

        public static void LoadMd5(string md5Path, string global_config_path)
        {
            if (!ConfigLoader.appConfig.use_md5)
            {
                return;
            }
            string index_file = Path.Combine(global_config_path, Excel2LuaTool.INDEX_FILE_NAME);
            string md5file = md5Path;
            if (JunctionPoint.Exists(md5Path))
            {
                string replace = JunctionPoint.GetTarget(md5Path);
                DirectoryInfo parent = Directory.GetParent(md5Path);
                replace = Path.Combine(parent.Parent.FullName, replace);
                if (!Directory.Exists(replace))
                {
                    Directory.CreateDirectory(replace);
                }
                else
                {
                    md5file = Path.Combine(replace, md5_name);
                }
            }
            else if (!Directory.Exists(md5Path))
            {
                Directory.CreateDirectory(md5Path);
            }
            else
            {
                md5file = Path.Combine(md5Path, md5_name);
            }
            if (!File.Exists(md5file) || !File.Exists(index_file))
            {
                return;
            }
            string json = File.ReadAllText(md5file);
            try
            {
                changes = JsonConvert.DeserializeObject<MD5Info>(json);
            }
            catch
            {
                changes = new MD5Info();
                logger.Trace("new md5 file");
            }
        }

        public static void FindLostFile()
        {
            foreach (var file_Path in changes.input_MD5.Keys)
            {
                if (!exist_files.Contains(file_Path))
                {
                    lost_files.Add(file_Path);
                }
            }
            if (lost_files.Count > 0)
            {
                foreach (var file_path in lost_files)
                {
                    string tmp_file = file_path.Replace('\\', '/');
                    changes.input_MD5.Remove(tmp_file);
                }
            }
        }

        public static void RemoveOutputFileMD5(string filepath)
        {
            string tmp_file = filepath.Replace('\\', '/');
            if (ExcelLoader.changes.output_MD5.ContainsKey(tmp_file))
            {
                ExcelLoader.changes.output_MD5.Remove(tmp_file);
            }
        }

        public static void FindChangedOutput()
        {
            List<string> changed_output = new List<string>();
            foreach (var kv in changes.output_MD5)
            {
                if (!File.Exists(kv.Key))
                {
                    changed_output.Add(kv.Key);
                    continue;
                }
                using (FileStream fs = new FileStream(kv.Key, FileMode.Open, FileAccess.Read))
                {
                    string md5_str;
                    byte[] retval = md5.ComputeHash(fs);
                    StringBuilder md5_sb = new StringBuilder();
                    foreach (byte b in retval)
                    {
                        md5_sb.Append(b.ToString("x2"));
                    }
                    md5_str = md5_sb.ToString();
                    if (string.Compare(md5_str, kv.Value) != 0)
                    {
                        changed_output.Add(kv.Key);
                    }
                }
            }
            foreach (var filepath in changed_output)
            {
                bool find_flag = false;
                string xlsx_path = "";
                foreach (var export_cfg in ConfigLoader.appConfig.export)
                {
                    if (!filepath.StartsWith(export_cfg.export_path))
                    {
                        continue;
                    }
                    xlsx_path = filepath.Replace(export_cfg.export_path, export_cfg.excel_path);
                    xlsx_path = xlsx_path.Replace(Path.GetExtension(xlsx_path), ".xlsx");
                    if (ConfigLoader.appConfig.i18n != null && string.Compare(export_cfg.excel_path, ConfigLoader.appConfig.i18n) == 0)
                    {
                        xlsx_path = xlsx_path.Replace('\\', '/');
                        xlsx_path = xlsx_path.Substring(export_cfg.excel_path.Length);
                        xlsx_path = xlsx_path.Substring(xlsx_path.IndexOf('/') + 1);
                        xlsx_path = Path.Combine(export_cfg.excel_path, xlsx_path);
                    }
                    if (File.Exists(xlsx_path))
                    {
                        find_flag = true;
                        break;
                    }
                }
                if (find_flag)
                {
                    if (!OutputChangedXlsx.Contains(xlsx_path))
                    {
                        OutputChangedXlsx.Add(xlsx_path);
                    }
                }
                else
                {
                    logger.Trace($"无法找到 {filepath} 的源文件 {xlsx_path}");
                    changes.output_MD5.Remove(filepath);
                }
            }
        }

        public static void RevertChangedOutput()
        {
            Stopwatch watcher = new Stopwatch();
            watcher.Start();
            //找出本地变化的输出文件,找到其对应的输入,再次生成
            ExcelLoader.FindChangedOutput();
 
            // 2022.05.30 add by zxp 临时检测文件变动数和变动的文件 用来监控生成 lua 文件失败的问题
            logger.Info($"============= 变动文件数，{OutputChangedXlsx.Count()}个 ===========");

            foreach (var changed_file in OutputChangedXlsx)
            {
                ExcelInfo info = CreateExcelInfo(changed_file);

                logger.Info($"============= 变动文件名称，{info.ExcelName} ===========");
                logger.Info($"============= 变动文件路径，{info.ExcelPath} ===========");

                if (info != null)
                {
                    revert_tables.Add(info.ExcelName, info);
                    logger.Trace($"加载文件 {changed_file} 成功");
                }
                else
                {
                    logger.Error($"加载文件 {changed_file} 失败");
                }
            }
            foreach (var cfg in ConfigLoader.appConfig.export)
            {
                Excel2LuaTool.ExportExcelFunc(cfg, ExcelLoader.revert_tables);
            }
            logger.Info($"=============还原变动文件完毕，用时{watcher.ElapsedMilliseconds}毫秒===========");
        }

        public static void SaveMD5(string md5Path)
        {
            string md5file = Path.Combine(md5Path, md5_name);
            if (!ConfigLoader.appConfig.use_md5)
            {
                if (File.Exists(md5file))
                {
                    File.Delete(md5file);
                }
                return;
            }
            if (File.Exists(md5file))
            {
                File.SetAttributes(md5file, FileAttributes.Normal);
            }
            else
            {
                if (JunctionPoint.Exists(md5Path))
                {
                    string replace = JunctionPoint.GetTarget(md5Path);
                    DirectoryInfo parent = Directory.GetParent(md5Path);
                    replace = Path.Combine(parent.Parent.FullName, replace);
                    if (!Directory.Exists(replace))
                    {
                        Directory.CreateDirectory(replace);
                    }
                    md5file = Path.Combine(replace, md5_name);
                    if (File.Exists(md5file))
                    {
                        File.SetAttributes(md5file, FileAttributes.Normal);
                    }
                }
                else if (!Directory.Exists(md5Path))
                {
                    Directory.CreateDirectory(md5Path);
                }
            }
            using (FileStream fs = new FileStream(md5file, FileMode.Create, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    string json = JsonConvert.SerializeObject(changes);
                    sw.WriteLine(json);
                }
            }
        }

        public static bool checkMD5change(string filePath)
        {
            string excelMd5;
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                byte[] retval = md5.ComputeHash(fs);
                StringBuilder sb = new StringBuilder();
                foreach (byte b in retval)
                {
                    sb.Append(b.ToString("x2"));
                }
                excelMd5 = sb.ToString();
            }
            exist_files.Add(filePath);
            if (!changes.input_MD5.ContainsKey(filePath))
            {
                changes.input_MD5.Add(filePath, excelMd5);
            }
            else
            {
                if (changes.input_MD5[filePath] == excelMd5)
                {
                    //如果配置变化或者exe变化全部生成,不检查输出文件
                    if (m_force || m_config_change || m_exe_change)
                    {
                        return false;
                    }
                    foreach (var cfg in ConfigLoader.appConfig.export)
                    {
                        if (!filePath.StartsWith(cfg.excel_path))
                        {
                            continue;
                        }
                        if (ConfigLoader.appConfig.i18n != null && filePath.StartsWith(ConfigLoader.appConfig.i18n))
                        {
                            //多语言文件不加载文件无法确定输出文件目录,不验证输出文件是否齐全
                            continue;
                        }
                        string output_path = filePath.Replace(cfg.excel_path, cfg.export_path);
                        foreach (var outputtype in output_type_list)
                        {
                            if (cfg.format.Contains(outputtype))
                            {
                                output_path = output_path.Replace(Path.GetExtension(output_path), "."+outputtype);
                                output_path = output_path.Replace('\\', '/');
                                if (!changes.output_MD5.ContainsKey(output_path))
                                {
                                    logger.Trace($"{filePath} 缺少输出文件 {output_path} ");
                                    return true;
                                }
                            }
                        }
                    }
                    return false;
                }
                changes.input_MD5[filePath] = excelMd5;
            }
            return true;
        }

        //导入excel数据，兼容xls2lua
        public static bool ImportExcelFileNoSchema(string filePath)
        {
            logger.Trace($"加载文件 {filePath}...");
            string excelName = Path.GetFileNameWithoutExtension(filePath);
            if (tables.ContainsKey(excelName))
            {
                return true;
            }
            if (ConfigLoader.appConfig.use_md5)
            {
                bool file_change = checkMD5change(filePath);
                if (!m_force && !m_config_change && !m_exe_change && !file_change)
                {
                    return true;
                }
            }

            ExcelInfo info = CreateExcelInfo(filePath);
            if (info != null)
            {
                tables.Add(info.ExcelName, info);
                logger.Trace($"加载文件 {filePath} 成功");
                return true;
            }
            else
            {
                logger.Error($"加载文件 {filePath} 失败");
            }
            return false;
        }

        public static ExcelInfo CreateExcelInfo(string filePath)
        {
            IWorkbook hssfworkbook;
            try
            {
                using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    hssfworkbook = new XSSFWorkbook(file);
                }
            }
            catch (Exception e)
            {
                logger.Error($"加载文件 {filePath} 错误, 原因: {e.ToString()}");
                return null;
            }

            ExcelInfo info = new ExcelInfo();
            info.ExcelName = Path.GetFileNameWithoutExtension(filePath);
            info.ExcelPath = filePath;
            info.one_sheet_data_table = new Dictionary<FlagType, Dictionary<string, CommonData>>();
            int current_num = 0;
            int list_num = 1;
            //合并所有sheet到一个datatable
            for (int idx = 0; idx < hssfworkbook.NumberOfSheets; idx++)
            {
                ISheet sheet = hssfworkbook.GetSheetAt(idx);
                string sheet_name = sheet.SheetName;
                if (!HandleSheetName(info, sheet_name))
                {
                    continue;
                }
                int keyRowNum = 0;
                //标识表头和检查规则配置是否正常
                bool trans_flag = true;
                //跳过注释
                IEnumerator rows = sheet.GetRowEnumerator();
                List<string> rule_str_list = new List<string>();
                Dictionary<int, string> rule_str_dic = new Dictionary<int, string>();
                int rule_no = 0;
                while (rows.MoveNext())
                {
                    IRow row = rows.Current as IRow;
                    if (row.GetCell(0).ToString().StartsWith("//"))
                    {
                        keyRowNum++;
                    }
                    else if (row.GetCell(0).ToString().StartsWith("Check"))
                    {
                        rule_no = keyRowNum;
                        for (int i = 0; i < row.LastCellNum; i++)
                        {
                            string ColNo = GetColumnChar(i);
                            ICell cell = row.GetCell(i);
                            if (cell == null || cell.ToString() == "" || cell.ToString().ToLower() == "check")
                            {
                                continue;
                            }
                            else
                            {
                                string cellValue = cell.ToString();
                                cellValue.Trim();
                                cellValue = Regex.Replace(cellValue, @"\s+", "");
                                rule_str_dic.Add(i, cellValue);
                            }
                        }
                        keyRowNum++;
                    }
                    else
                    {
                        break;
                    }
                }

                bool inited = false;
                if (info.FieldIndex.Count > 0)
                {
                    inited = true;
                }
                //解析表头
                IRow title = sheet.GetRow(keyRowNum);
                if (!inited)
                {
                    bool has_index = false;
                    FlagInfo pri_info = null;
                    bool has_devide = false;
                    bool has_devide_link = false;
                    for (int i = 0; i < title.LastCellNum; i++)
                    {
                        ICell cell = title.Cells[i];
                        string ColNo = GetColumnChar(i);
                        if (cell == null)
                        {
                            logger.Error($"文件 {filePath} 表 {sheet.SheetName} 第{keyRowNum + 1}行 第{ColNo} 列为表头,不能为空");
                            trans_flag = false;
                        }
                        string cellstr = title.Cells[i].StringCellValue;
                        if (string.IsNullOrEmpty(cellstr))
                        {
                            break;
                        }
                        string[] fieldstr = cellstr.Split(':');
                        if (fieldstr.Length < 2)
                        {
                            logger.Error($"文件 {filePath} 表 {sheet.SheetName} 第{keyRowNum + 1}行 第{ColNo} 列为表头, {cellstr} 缺少属性分隔符:");
                            trans_flag = false;
                        }
                        string field_name = fieldstr[0];
                        string field_flag = fieldstr[1];
                        if (!inited)
                        {
                            if (info.FieldNames.Contains(field_name))
                            {
                                logger.Error($"文件 {filePath} 表 {sheet.SheetName} 第{keyRowNum + 1}行 第{ColNo} 名称重复");
                                trans_flag = false;
                            }
                            else
                            {
                                info.FieldIndex.Add(field_name, i);
                                info.FieldNames.Add(field_name);
                                FlagInfo fi = Excel2LuaTool.ParseFlag(field_flag);
                                info.FieldFlags.Add(fi);
                                if (fi == null)
                                {
                                    logger.Error($"文件 {filePath} 表 {sheet.SheetName} 第{keyRowNum + 1}行 第{ColNo} 列为表头, {cellstr} 配置错误:");
                                    trans_flag = false;
                                    continue;
                                }
                                if (fi.m_primary)
                                {
                                    info.KeyFieldName = field_name;
                                    pri_info = fi;
                                }
                                if (fi.m_index)
                                {
                                    has_index = true;
                                    info.IndexFieldType.Copy(fi);
                                }
                                if (fi.m_devide)
                                {
                                    has_devide = true;
                                }
                                if (fi.m_devide_link)
                                {
                                    has_devide_link = true;
                                    info.LinkFieldType.Copy(fi);
                                    info.LinkFieldName = field_name;
                                }
                            }
                        }
                    }
                    if (!inited && pri_info == null)
                    {
                        logger.Error($"文件 {filePath} 缺少主键");
                        return null;
                    }
                    //文件需要分表，分表预处理
                    if (info.devideflag)
                    {
                        if (info.devide_num == -1)
                        {
                            if (!has_devide)
                            {
                                logger.Error($"文件 {filePath} 表 {sheet.SheetName} 缺少分表列或分表间隔");
                                trans_flag = false;
                            }
                            else if (!has_devide_link)
                            {
                                logger.Error($"文件 {filePath} 表 {sheet.SheetName} 缺少分表关联列");
                                trans_flag = false;
                            }
                        }
                        if (info.DevideExcelRows == null)
                        {
                            info.DevideExcelRows = new Dictionary<string, Dictionary<string, List<string>>>();
                        }
                    }
                    //文件需要索引键表
                    if (info.indexflag && !has_index)
                    {
                        info.IndexFieldType.Copy(pri_info);
                        if (info.IndexList == null)
                        {
                            info.IndexList = new List<string>();
                        }
                    }
                }
				//使用项目检查工具
                //bool ret_rule = LoadCheckRules(info, rule_str_dic, rule_no);
                //trans_flag = trans_flag && ret_rule;
                //表头及检查规则有错误不解析具体配置
                if (!trans_flag)
                {
                    RemoveErrorFilesMd5(filePath);
                    return null;
                }
                trans_flag = true;
                Dictionary<int, HashSet<string>> UniqueValueDict = new Dictionary<int, HashSet<string>>();
                int lineNO = keyRowNum + 1;

                //实际的配置内容
                while (rows.MoveNext())
                {
                    lineNO++;
                    IRow row = (XSSFRow)rows.Current;
                    //判定最后一行
                    //rows.MoveNext() 有时会出现下一行是空行但仍然读取的问题,但是该行第一列对象为空会导致读取错误
                    if (row.GetCell(0) == null)
                    {
                        break;
                    }

                    var cell0 = row.GetCell(0).ToString();
					//项目不使用///END作为表结尾
                    if (String.IsNullOrWhiteSpace(cell0))
					//if (cell0 == "///END")
                    {
                        break;
                    }
                    //跳过注释行
                    if (cell0.StartsWith("//"))
                    {
                        continue;
                    }

                    List<string> dr = new List<string>();
                    string key = null;
                    string index = null;
                    string group = null;
                    string link = null;
                    for (int i = 0; i < info.FieldNames.Count; i++)
                    {
                        string ColNo = GetColumnChar(i);
                        ICell cell = row.GetCell(i);
                        if (cell == null || cell.ToString() == "")
                        {
                            //判断可空
                            if (!info.FieldFlags[i].m_nullable)
                            {
                                logger.Error($"文件 {filePath} 表 {sheet_name} 第{lineNO}行 第{ColNo}列 配置为空，请指定 'e' 标记 或检查配置");
                                trans_flag = false;
                            }
                            dr.Add(null);
                        }
                        else
                        {
                            //公式转换
                            string cellValue = cell.ToString();
                            if (cell.CellType == CellType.Formula)
                            {
                                switch (info.FieldFlags[i].m_field_type.m_type)
                                {
                                    case eFlagType.INT:
                                        cellValue = ((int)cell.NumericCellValue).ToString();
                                        break;
                                    case eFlagType.FLOAT:
                                        cellValue = ((float)cell.NumericCellValue).ToString();
                                        break;
                                    case eFlagType.STRING:
                                        cellValue = cell.StringCellValue;
                                        break;
                                }
                            }
                            //flag检查
                            if (!Excel2LuaTool.CheckFlag(cellValue, info.FieldFlags[i]))
                            {
                                logger.Error($"文件 {filePath} 表 {sheet_name} 第{lineNO}行 第{ColNo}列配置格式与表头不符");
                                trans_flag = false;
                            }
                            if (info.FieldFlags[i].m_index)
                            {
                                index = cellValue;
                                if (info.IndexList.Contains(index))
                                {
                                    logger.Error($"文件 {filePath} 表 {sheet_name} 第{lineNO}行 第{ColNo}列为索引列,键值{index}重复");
                                    trans_flag = false;
                                }
                            }
                            if (info.FieldFlags[i].m_devide)
                            {
                                group = cellValue;
                            }
                            if (info.FieldFlags[i].m_devide_link)
                            {
                                link = cellValue;
                            }
                            if (info.FieldFlags[i].m_primary)
                            {
                                if (info.FieldFlags[i].m_autoid)
                                {
                                    int index1 = lineNO - keyRowNum - 1;
                                    key = (index1).ToString();
                                    cellValue = key;
                                    info.ArrayParts.Add(index1);
                                }
                                else
                                {
                                    key = cellValue;
                                    if (int.TryParse(key, out int index1))
                                    {
                                        info.ArrayParts.Add(index1);
                                    }
                                }
                                key = cellValue;
                                if (info.ExcelRows.ContainsKey(key))
                                {
                                    logger.Error($"文件 {filePath} 表 {sheet_name} 第{lineNO}行 第{ColNo}列为主键,键值{key}重复");
                                    trans_flag = false;
                                }
                            }
                            if (info.FieldFlags[i].m_unique)
                            {
                                if (!UniqueValueDict.ContainsKey(i))
                                {
                                    UniqueValueDict.Add(i, new HashSet<string>());
                                }
                                var hset = UniqueValueDict[i];
                                if (hset.Contains(cell.ToString()))
                                {
                                    logger.Error($"文件 {filePath} 表{sheet_name} 第{lineNO}行 第{ColNo}列值不许重复,值{cell.ToString()}重复");
                                    trans_flag = false;
                                }
                            }
                            if (info.FieldFlags[i].NeedCheck())
                            {
                                //去除首尾空格，中间空格改单空格
                                //暂时不去掉，多行文本格式会丢失
                                //cellValue = cellValue.Trim();
                                //cellValue = Regex.Replace(cellValue, @"\s+", " ");
                                if (string.IsNullOrWhiteSpace(cellValue)) { }
                                else
                                {
                                    AddDuplicateData(info.one_sheet_data_table, info.FieldFlags[i], cellValue);
                                }
                            }
                            dr.Add(cellValue);
                        }
                    }
                    if (key != null && !info.ExcelRows.ContainsKey(key))
                    {
                        info.ExcelRows.Add(key, dr);
                        if (info.devideflag)
                        {
                            if (info.devide_num != -1)
                            {
                                int list_sum = list_num * info.devide_num;
                                if (!info.DevideExcelRows.ContainsKey(list_sum.ToString()))
                                {
                                    info.DevideExcelRows.Add(list_sum.ToString(), new Dictionary<string, List<string>>());
                                }
                                info.DevideExcelRows[list_sum.ToString()].Add(key, dr);
                                current_num++;
                                if (current_num >= info.devide_num)
                                {
                                    current_num = 0;
                                    list_num++;
                                }
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(group) || string.IsNullOrEmpty(link))
                                {
                                    logger.Error($"文件 {filePath} 表{sheet_name} 第{lineNO}行 分表标识缺失");
                                    trans_flag = false;
                                }
                                else
                                {
                                    if (!info.DevideExcelRows.ContainsKey(group))
                                    {
                                        info.DevideExcelRows.Add(group, new Dictionary<string, List<string>>());
                                    }
                                    info.DevideExcelRows[group].Add(key, dr);
                                    if (!info.LinkRelations.ContainsKey(link))
                                    {
                                        info.LinkRelations.Add(link, new List<string>());
                                    }
                                    if (!info.LinkRelations[link].Contains(group))
                                        info.LinkRelations[link].Add(group);

                                }
                            }
                        }
                        if (!info.indexflag)
                        {
                            continue;
                        }
                        if (index != null && !info.IndexList.Contains(index))
                        {
                            info.IndexList.Add(index);
                        }
                        else if (index == null)
                        {
                            info.IndexList.Add(key);
                        }
                    }
                }
                info.ArrayParts.Sort();
                List<int> parts = new List<int>(info.ArrayParts.Count);
                int k = 0;
                foreach (var id in info.ArrayParts)
                {
                    if (++k == id)
                    {
                        parts.Add(k);
                    }
                    else
                    {
                        break;
                    }
                }
                info.ArrayParts = parts;

                //若出现错误则不存储md5,同时转表报错
                if (!trans_flag)
                {
                    RemoveErrorFilesMd5(filePath);
                    return null;
                }
            }
            return info;
        }

        /// <summary>
        /// excel数字列号转字符列号帮助函数
        /// </summary>
        /// <param name="col">数字列号</param>
        public static string GetColumnChar(int col)
        {
            var a = col / 26;
            var b = col % 26;

            if (a > 0) return GetColumnChar(a - 1) + (char)(b + 65);

            return ((char)(b + 65)).ToString();
        }

        private static void ReconcileOutputFiles()
        {
            foreach (var cfg in ConfigLoader.appConfig.export)
            {
                P4Tool.PerforceReconcile(cfg.excel_path);
                P4Tool.PerforceReconcile(cfg.export_path, cfg.link_path);
            }
        }

        private static void ReconcileOtherFiles(string config_path)
        {
            P4Tool.PerforceReconcile(ConfigLoader.appConfig.global_config_path);
            if (ConfigLoader.appConfig.use_md5)
            {
                if (ExcelLoader.m_config_change)
                {
                    P4Tool.PerforceReconcileFile(config_path);
                }
                if (ExcelLoader.m_exe_change)
                {
                    string exe_path = System.Reflection.Assembly.GetExecutingAssembly().Location;
                    P4Tool.PerforceReconcileFile(exe_path);
                    P4Tool.PerforceReconcileFile(exe_path.Replace(".exe", ".pdb"));
                }
            }
        }

        /// <summary>
        /// 加载Excel列的检测条件
        /// </summary>
        /// <param name="info">Excel 表格数据</param>
        /// <param name="rule_str_dic">规则字符串集合</param>
        private static bool LoadCheckRules(ExcelInfo info, Dictionary<int, string> rule_str_dic, int line_no)
        {
            if (info.FieldCheckRules.Count > 0)
            {
                return true;
            }
            bool all_true = true;
            foreach (var rule_str in rule_str_dic)
            {
                string cellValue = rule_str.Value;
                if (info.FieldFlags[rule_str.Key] == null)
                {
                    continue;
                }
                FlagType flag_type = info.FieldFlags[rule_str.Key].m_field_type;

                List<CheckRule> check_list = new List<CheckRule>();
                if (!CheckFactory.TryCreateRuleList(cellValue, info.ExcelName, rule_str.Key, flag_type, out check_list))
                {
                    all_true = false;
                    string ColNo = GetColumnChar(rule_str.Key);
                    logger.Error($"文件 {info.ExcelPath} 第{line_no + 1}行 第{ColNo}列 为检查规则,配置错误");
                    continue;
                }
                info.FieldCheckRules.Add(rule_str.Key, check_list);
            }
            return all_true;
        }

        /// <summary>
        /// 删除 错误文件的MD5(错误文件不生成MD5数据)
        /// </summary>
        /// <param name="filePath"></param>
        public static void RemoveErrorFilesMd5(string filePath)
        {
            Program.isSuccess = false;

            if (changes.input_MD5.ContainsKey(filePath))
            {
                changes.input_MD5.Remove(filePath);
            }
        }

        public static void AddDuplicateData(CommonDataTable data_table, FlagInfo info, string cell_value)
        {
            bool find_flag = false;
            foreach (var kv in data_table)
            {
                if (kv.Key.IsSameType(info.m_field_type))
                {
                    find_flag = true;
                    if (kv.Value.ContainsKey(cell_value))
                    {
                        kv.Value[cell_value].duplicate = true;
                    }
                    else
                    {
                        kv.Value.Add(cell_value, new CommonData(info));
                    }
                }
            }
            if (!find_flag)
            {
                data_table.Add(info.m_field_type, new Dictionary<string, CommonData>());
                data_table[info.m_field_type].Add(cell_value, new CommonData(info));
            }
        }

        public static void HandleCommonData(CommonDataTable data_table)
        {
            int index = 1;
            foreach (var kv in data_table)
            {
                List<string> del_key = new List<string>();
                foreach (var data in kv.Value)
                {
                    if (!data.Value.duplicate)
                    {
                        del_key.Add(data.Key);
                        continue;
                    }
                    data.Value.index = index;
                    index++;
                }
                foreach (var key in del_key)
                {
                    kv.Value.Remove(key);
                }
            }
        }

        public static bool HandleSheetName(ExcelInfo info, string sheet_name)
        {
            //#开头的才解析
            if (!sheet_name.StartsWith("#"))
            {
                return false;
            }
            string sheet_flag_info = sheet_name.Substring(1);
            Regex flag_reg = new Regex(@"^\s*(?<flag_info>\W(\W|\s|\d)*)(\w|_).*$");
            Match mcom = flag_reg.Match(sheet_flag_info);
            if (!mcom.Success)
            {
                return true;
            }
            sheet_flag_info = mcom.Groups["flag_info"].Value;
            for (int i = 0; i < sheet_flag_info.Length; i++)
            {
                char f = sheet_flag_info[i];
                if (f == '#')
                {
                    info.indexflag = true;
                    continue;
                }
                if (f == '!')
                {
                    info.devideflag = true;
                    Regex devide_reg = new Regex(@"^.*!\((?<devide_num>\d+)\).*$");
                    Match mcom1 = devide_reg.Match(sheet_name);
                    if (mcom1.Success)
                    {
                        string dev_str = mcom1.Groups["devide_num"].Value;
                        info.devide_num = int.Parse(dev_str);
                    }
                    continue;
                }
            }
            return true;
        }

        public static bool ImportCsvFileNoSchema(string filePath)
        {
            logger.Trace("LoadCsv {0}...", filePath);
            string csvName = Path.GetFileNameWithoutExtension(filePath);
            if (tables.ContainsKey(csvName))
            {
                return true;
            }
            //md5
            if (ConfigLoader.appConfig.use_md5)
            {
                if (!checkMD5change(filePath))
                {
                    return true;
                }
            }
            StreamReader csvReader;
            try
            {
                FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                csvReader = new StreamReader(file, Encoding.UTF8);
            }
            catch (Exception e)
            {
                logger.Error("LoadExcel Error: {0}", e.ToString());
                return false;
            }
            ExcelInfo info = new ExcelInfo();
            info.ExcelName = Path.GetFileNameWithoutExtension(filePath);
            info.ExcelPath = filePath;
            string strLine = "";
            //记录每行记录中的各字段内容
            string[] rowStrs = null;
            //标示是否是读取的第一行
            bool IsFirst = true;
            //逐行读取CSV中的数据
            int keyRowNum = 0;
            int lineNO = 0;
            Dictionary<int, HashSet<string>> UniqueValueDict = new Dictionary<int, HashSet<string>>();
            while ((strLine = csvReader.ReadLine()) != null)
            {
                if (strLine.StartsWith("//"))
                {
                    keyRowNum++;
                    continue;
                }
                if (IsFirst == true)
                {
                    rowStrs = strLine.Split(',');
                    IsFirst = false;
                    for (int i = 0; i < rowStrs.Length; ++i)
                    {
                        string[] fieldstr = rowStrs[i].Split(':');
                        if (fieldstr.Length < 2)
                        {
                            logger.Error("Csv {filePath} fieldstr {fieldstr} miss ':' !!");
                            return false;
                        }
                        string field_name = fieldstr[0];
                        string field_flag = fieldstr[1];
                        info.FieldIndex.Add(field_name, i);
                        info.FieldNames.Add(field_name);
                        FlagInfo fi = Excel2LuaTool.ParseFlag(field_flag);
                        if (fi == null)
                        {
                            logger.Error("Csv {filePath}  fieldstr {rowStrs[i]} flag error {field_flag}");
                            return false;
                        }
                        info.FieldFlags.Add(fi);
                        if (fi.m_primary)
                        {
                            info.KeyFieldName = field_name;
                        }
                    }
                    keyRowNum++;
                    lineNO = keyRowNum;
                }
                else
                {
                    lineNO++;
                    rowStrs = strLine.Split(',');
                    List<string> dr = new List<string>();
                    string key = null;
                    for (int i = 0; i < info.FieldNames.Count; i++)
                    {
                        string cellValue = rowStrs[i];
                        if (!Excel2LuaTool.CheckFlag(cellValue, info.FieldFlags[i]))
                        {
                            logger.Error("{0}第{1}行 {2}配置格式错误", filePath, lineNO, info.FieldNames[i]);
                        }

                        if (info.FieldFlags[i].m_primary)
                        {
                            key = cellValue;
                            if (info.ExcelRows.ContainsKey(key))
                            {
                                logger.Error("{0}第{1}行 {2}主键键值{3}重复", filePath, lineNO, info.FieldNames[i], key);
                            }
                        }
                        if (info.FieldFlags[i].m_unique)
                        {
                            if (!UniqueValueDict.ContainsKey(i))
                            {
                                UniqueValueDict.Add(i, new HashSet<string>());
                            }
                            var hset = UniqueValueDict[i];
                            if (hset.Contains(cellValue))
                            {
                                logger.Error("{0}第{1}行 {2}值{3}不允许重复", filePath, lineNO, info.FieldNames[i], key);
                            }
                        }
                        dr.Add(cellValue);
                    }
                    if (key != null && !info.ExcelRows.ContainsKey(key))
                    {
                        info.ExcelRows.Add(key, dr);
                    }
                }
            }
            tables.Add(info.ExcelName, info);
            logger.Trace("LoadCsv {0} OK", filePath);
            return true;
        }
    }
}

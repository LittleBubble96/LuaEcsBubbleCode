using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace SchemaTool
{
    internal enum eFlagType
    {
        INVALID, BOOL, INT, FLOAT, STRING, TIME, TABLE, ARRAY, OBJECT,
    }

    internal class FlagType
    {
        public eFlagType m_type = eFlagType.INVALID;
        public char m_delimiter = ',';
        public FlagType m_inner_type = null;

        public List<eFlagType> m_sub_types = null;
        public bool m_check_sub_type = false;

        public void Copy(FlagType a)
        {
            m_type = a.m_type;
            m_delimiter = a.m_delimiter;
            if (a.m_inner_type != null)
            {
                m_inner_type = new FlagType();
                m_inner_type.Copy(a.m_inner_type);
            }
            else
            {
                m_inner_type = null;
            }
        }

        public bool IsSameType(FlagType ano_type)
        {
            if (ano_type == null)
            {
                return false;
            }
            if (m_type != ano_type.m_type)
            {
                return false;
            }
            if (m_type == eFlagType.OBJECT)
            {
                if (!m_check_sub_type)
                {
                    return m_check_sub_type == ano_type.m_check_sub_type;
                }
                if (!ano_type.m_check_sub_type)
                {
                    return false;
                }
                if (m_sub_types.Count != ano_type.m_sub_types.Count)
                {
                    return false;
                }
                for (int i = 0; i < m_sub_types.Count; ++i)
                {
                    if (m_sub_types[i] != ano_type.m_sub_types[i])
                        return false;
                }
                return true;
            }
            if (m_inner_type == null)
            {
                return ano_type.m_inner_type == null;
            }
            return m_inner_type.IsSameType(ano_type.m_inner_type);
        }
    }

    internal class FlagInfo
    {
        public FlagType m_field_type = new FlagType();
        public bool m_unique = false;
        public bool m_default = false;
        public bool m_ignore = false;
        public bool m_nullable = false;
        public bool m_primary = false;
        public bool m_index = false;
        public bool m_devide = false;
        public bool m_devide_link = false;
        public bool m_client = false;
        public bool m_server = false;
        public bool m_autoid = false;
        public bool NeedCheck()
        {
            return m_field_type.m_type == eFlagType.ARRAY || m_field_type.m_type == eFlagType.TABLE
                || m_field_type.m_type == eFlagType.OBJECT;
        }

        public void Copy(FlagInfo info)
        {
            m_unique = info.m_unique;
            m_default = info.m_default;
            m_ignore = info.m_ignore;
            m_nullable = info.m_nullable;
            m_primary = info.m_primary;
            m_index = info.m_index;
            m_devide = info.m_devide;
            m_devide_link = info.m_devide_link;
            m_client = info.m_client;
            m_server = info.m_server;
            m_field_type.Copy(info.m_field_type);
        }
    }

    internal static class Excel2LuaTool
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        static public List<string> delete_files = new List<string>();
        static public string INDEX_FILE_NAME = "all_tag_table.lua";
        static public string COMMON_DATA_TABLE_PREFIX = "commonData";
        static public string global_prefix = "_G." + COMMON_DATA_TABLE_PREFIX;
        static public bool generate_lua_global = false;

        static string i18n_path = "";
        static int i18n_field = 0;
        static HashSet<string> i18n_name = new HashSet<string>() { "zh", "us", "tw", "kr", "jp" };
        static MD5 md5 = new MD5CryptoServiceProvider();
        public static string datatimestr = DateTime.Now.ToString("yyyyMMddHHmmss/");
        public static bool m_backup = false;

        public static void Export()
        {
            FindNeedDeleteInputFiles();
            DeleteOutputFiles();
            ExportAllChangedExcel();
            ExportIndexTable(ConfigLoader.appConfig.global_config_path);
        }

        public static void FindNeedDeleteInputFiles()
        {
            delete_files = ExcelLoader.lost_files;
            //如果源文件需要处理为需要分表的文件,因为分表规则可能变化所以需要删除原先的所有分表
            if (ExcelLoader.tables.Count > 0)
            {
                foreach (var excel_info in ExcelLoader.tables.Values)
                {
                    if (excel_info.devideflag)
                    {
                        delete_files.Add(excel_info.ExcelPath);
                    }
                }
            }
        }

        public static void ExportAllChangedExcel()
        {
            Stopwatch watcher = new Stopwatch();
            watcher.Start();
            foreach (var cfg in ConfigLoader.appConfig.export)
            {
                ExportExcelFunc(cfg, ExcelLoader.tables);
            }
            logger.Info($"=============Excel导出完毕，用时{watcher.ElapsedMilliseconds}毫秒===========");
        }

        public static void ExportExcel(ExportConfig export_cfg, Dictionary<string, ExcelInfo> excels)
        {
            Stopwatch watcher = new Stopwatch();
            watcher.Start();
            ExportExcelFunc(export_cfg, excels);
            watcher.Stop();

            logger.Info($"=============Excel导出完毕，用时{watcher.ElapsedMilliseconds}毫秒===========");
        }

        public static void ExportExcelFunc(ExportConfig export_cfg, Dictionary<string, ExcelInfo> excels)
        {
            string excelpath = export_cfg.excel_path;
            string exportpath = export_cfg.export_path;
            string format = export_cfg.format;
            string uniquepath = (string.IsNullOrEmpty(export_cfg.unique_path)) ? export_cfg.export_path : export_cfg.unique_path;
            foreach (var info in excels.Values)
            {
                //跳过非本目录下的excel
                if (!info.ExcelPath.StartsWith(excelpath))
                {
                    continue;
                }
                string filedir = Path.GetDirectoryName(info.ExcelPath.Replace(excelpath, exportpath));
                if (ConfigLoader.appConfig.i18n != null && excelpath.StartsWith(ConfigLoader.appConfig.i18n) && !info.ExcelName.Contains("language_list"))
                {
                    for (int i = 1; i < info.FieldNames.Count; i++)
                    {
                        if (info.FieldFlags[i].m_ignore) {
                            continue; // 多语言表忽略的字段不删除配置
                        }
                        filedir = Path.GetDirectoryName(info.ExcelPath.Replace(excelpath, exportpath + info.FieldNames[i] + "/"));
                        i18n_path = filedir;
                        i18n_field = i;
                        if (!Directory.Exists(filedir))
                        {
                            Directory.CreateDirectory(filedir);
                        }
                        if (format.Contains("lua"))
                        {
                            string filepath = Path.Combine(filedir, info.ExcelName + ".lua");
                            generate_lua_global = true;
                            ExportLua(info, filepath, filedir);
                        }
                        if (format.Contains("csv"))
                        {
                            string filepath = Path.Combine(filedir, info.ExcelName + ".csv");
                            ExportCsv(info, filepath);
                        }
                        if (format.Contains("json"))
                        {
                            string filepath = Path.Combine(filedir, info.ExcelName + ".json");
                            ExportJson(info, filepath);
                        }
                    }
                    continue;
                }

                if (!Directory.Exists(filedir))
                {
                    Directory.CreateDirectory(filedir);
                }

                if (format.Contains("lua"))
                {
                    string filepath = Path.Combine(filedir, info.ExcelName + ".lua");
                    if (ConfigLoader.appConfig.duplicate_handle)
                    {
                        ExcelLoader.HandleCommonData(info.one_sheet_data_table);
                    }
                    if (info.devideflag)
                    {
                        ExportDevideIndex(info, filepath, uniquepath);
                        ExportDevideLua(info, filedir, uniquepath);
                    }
                    else
                    {
                        ExportLua(info, filepath, uniquepath);
                    }
                }
                if (format.Contains("csv"))
                {
                    string filepath = Path.Combine(filedir, info.ExcelName + ".csv");
                    ExportCsv(info, filepath);
                }
                if (format.Contains("json"))
                {
                    string filepath = Path.Combine(filedir, info.ExcelName + ".json");
                    ExportJson(info, filepath);
                }
            }
        }
		
        public static void DeleteOutputFiles()
        {
            Stopwatch watcher = new Stopwatch();
            watcher.Start();
            foreach (var cfg in ConfigLoader.appConfig.export)
            {
                DeleteOutputFilesFunc(cfg);
            }
            watcher.Stop();
            logger.Info($"=============多余文件删除完毕，用时{watcher.ElapsedMilliseconds}毫秒===========");
        }

        public static void DeleteOutputFilesFunc(ExportConfig export_cfg)
        {
            //需要删除的文件包含源文件已删除以及源文件需要分表的（分表方式不同可能产生多余文件）
            string excelpath = export_cfg.excel_path;
            string exportpath = export_cfg.export_path;
            string format = export_cfg.format;
            if (delete_files.Count > 0)
            {
                foreach (string info in delete_files)
                {
                    //跳过非本目录下的excel
                    if (!info.StartsWith(excelpath))
                    {
                        continue;
                    }
                    string targetinfo = info.Replace(excelpath, exportpath);
                    string ext = Path.GetExtension(targetinfo);
                    string new_ext = ".lua";
                    if (format.Contains("lua"))
                    {
                        string new_info = targetinfo.Replace(ext, new_ext);
                        //删除分表索引文件
                        if (File.Exists(new_info))
                        {
                            File.SetAttributes(new_info, FileAttributes.Normal);
                            File.Delete(new_info);
                        }
                        new_info = new_info.Replace('\\', '/');
                        ExcelLoader.RemoveOutputFileMD5(new_info);
                        new_info = new_info.Replace(new_ext, "");
                        int last_index = new_info.LastIndexOf('/');
                        string dir = new_info.Substring(0, last_index);
                        string file_name = new_info.Substring(last_index + 1);
                        //删除分表
                        if (Directory.Exists(dir))
                        {
                            string target_file = file_name + "_*.lua";
                            string[] file_list = Directory.GetFiles(dir, target_file);
                            foreach (var file in file_list)
                            {
                                File.SetAttributes(file, FileAttributes.Normal);
                                File.Delete(file);
                                ExcelLoader.RemoveOutputFileMD5(file);
                            }
                        }
                    }
                    if (format.Contains("csv"))
                    {
                        new_ext = ".csv";
                        string new_info = targetinfo.Replace(ext, new_ext);
                        if (File.Exists(new_info))
                        {
                            File.SetAttributes(new_info, FileAttributes.Normal);
                            File.Delete(new_info);
                            ExcelLoader.RemoveOutputFileMD5(new_info);
                        }
                    }
                }
            }
        }

        //导出lua
        public static void ExportLua(ExcelInfo info, string filepath, string unique_path)
        {
            ExportOneLuaFile(info, filepath, unique_path, info.ExcelRows);
        }

        public static void ExportDevideIndex(ExcelInfo info, string filepath, string unique_path)
        {
            if (Directory.Exists(unique_path))
            {
                foreach (string file in Directory.EnumerateFiles(unique_path, Path.GetFileName(filepath), SearchOption.AllDirectories))
                {
                    if (Path.GetExtension(file) == ".lua" && Path.GetFileNameWithoutExtension(file) == Path.GetFileNameWithoutExtension(filepath))
                    {
                        File.SetAttributes(file, FileAttributes.Normal);
                        File.Delete(file);
                    }
                }
            }
            using (FileStream fs = new FileStream(filepath, FileMode.Create, FileAccess.Write))
            {
                StreamWriter sw = new StreamWriter(fs);
                sw.WriteLine("return {");
                if (info.devide_num != -1)
                {
                    foreach (string data in info.DevideExcelRows.Keys)
                    {
                        sw.WriteLine($"\"{info.ExcelName}_{data}\",");
                    }
                    sw.WriteLine("},'filename'");
                }
                else
                {
                    foreach (var data in info.LinkRelations)
                    {
                        string key = FlagTypeToLua(data.Key, info.LinkFieldType.m_field_type, false);
                        sw.Write($"[{key}]={{");
                        foreach (var name in data.Value)
                        {
                            sw.Write($"\"{info.ExcelName}_{name}\",");
                        }
                        sw.WriteLine("},");
                    }
                    sw.WriteLine("}," + "'" + info.LinkFieldName + "'");
                }
                sw.Close();
            }
        }

        public static void ExportDevideLua(ExcelInfo info, string dirpath, string unique_path)
        {
            foreach (var data in info.DevideExcelRows)
            {
                string filepath = Path.Combine(dirpath, info.ExcelName + "_" + data.Key + ".lua");
                ExportOneLuaFile(info, filepath, unique_path, data.Value);
            }
        }

        public static void ExportOneLuaFile(ExcelInfo info, string filepath, string unique_path, Dictionary<string, List<string>> data)
        {
            if (Directory.Exists(unique_path))
            {
                try
                {
                    string fileName = Path.GetFileName(filepath);
                    foreach (string file in Directory.EnumerateFiles(unique_path, fileName,
                        SearchOption.AllDirectories))
                    {
                        if (Path.GetExtension(file) == ".lua" && Path.GetFileNameWithoutExtension(file) ==
                            Path.GetFileNameWithoutExtension(filepath))
                        {
                            File.SetAttributes(file, FileAttributes.Normal);
                            File.Delete(file);
                        }
                    }
                }
                catch
                {
                    logger.Fatal("unique_path:" + unique_path);
                    logger.Fatal("filepath:" + filepath);
                }
            }
            bool is_i18n = i18n_path.Length > 0 && filepath.StartsWith(i18n_path);
            using (FileStream fs = new FileStream(filepath, FileMode.Create, FileAccess.ReadWrite))
            {
                StreamWriter sw = new StreamWriter(fs);
                sw.WriteLine("---this config file was auto-generated by Excel2lua tool, do not modify it!");
                sw.WriteLine("");
                ExportOneLuaContent(sw, data, info, is_i18n);        
                sw.Close();
            }

            if (m_backup)
            {
                ExportBackupFiles(filepath, "client");
            }
            
            using (FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read))
            {
                SaveOutputFileMD5(filepath, fs);
            }
        }

        public static void ExportBackupFiles(string filepath, string stype)
        {
            if (null == filepath || "" == filepath)
            {
                logger.Error("backup fail:" + filepath);
                return ;
            }
            string full_path = Path.GetFullPath(filepath);
            string[] full_path_array = full_path.Split('\\');
            string copyfilepath = "";
            for (int i = 0; i < full_path_array.Length; ++i)
            {
                copyfilepath += full_path_array[i];
                copyfilepath += "/";
                if (full_path_array[i] == "p4")
                {
                    break;
                }
            }
            copyfilepath += "mr/trunk/xls_config/out/";
            copyfilepath += datatimestr;
            copyfilepath += stype;
            copyfilepath += "/";
            //对应配置文件的同级目录
            string[] path_array = filepath.Split('\\');
            int begin_position = 0;
            for (; begin_position < path_array.Length; ++begin_position)
            {
                if (path_array[begin_position] == stype)
                {
                    ++begin_position;
                    break;
                }
            }
            //拼接路径
            for (int i = begin_position; i < path_array.Length - 1; ++i)
            {
                copyfilepath += path_array[i];
                copyfilepath += "/";
            }
            if (!Directory.Exists(copyfilepath))
            {
                Directory.CreateDirectory(copyfilepath);
            }
            copyfilepath += Path.GetFileName(filepath);
            File.Copy(filepath, copyfilepath, false);
        }

        public static void ExportOneLuaContent(StreamWriter sw, Dictionary<string, List<string>> content, ExcelInfo info, bool is_i18n = false)
        {
            StringBuilder config = new StringBuilder();
            config.AppendLine("local config = {");
            var itor = content.GetEnumerator();
            List<string> keys = new List<string>();
            bool first = true;
            List<int> fields = new List<int>();
            List<int> nullables = new List<int>();
            StringBuilder sb = new StringBuilder();
            bool hasEmpty = false;
            int tmp_index = 1;
            while (itor.MoveNext())
            {
                var key = itor.Current.Key;
                var row = itor.Current.Value;
                FlagInfo flag = info.FieldFlags[0];
                key = FlagTypeToLua(key, flag.m_field_type);

                string affix = null;
                if (int.TryParse(key, out int index) && tmp_index == index)
                {
                    tmp_index++;
                    config.Append("{");
                }
                else
                {
                    if (!is_i18n)
                    {
                        config.Append($"[{key}]={{");
                    }
                }

                sb.Clear();
                if (first)
                {
                    foreach (var pair in info.FieldIndex)
                    {
                        var fieldName = pair.Key;
                        var i = pair.Value;
                        flag = info.FieldFlags[i];
                        if (flag.m_ignore || flag.m_devide)
                        {
                            continue;
                        }
                        if (!flag.m_client)
                        {
                            continue;
                        }
                        if (is_i18n && i >= 0)
                        {
                            if (i != i18n_field)
                                continue;
                        }
                        if (flag.m_default || !flag.m_nullable)
                        {
                            fields.Add(i);
                        }
                        else
                        {
                            nullables.Add(i);
                        }
                    }
                    foreach (var i in nullables)
                    {
                        fields.Add(i);
                    }
                }

                foreach (var i in fields)
                {
                    flag = info.FieldFlags[i];
                    if (flag.m_ignore || flag.m_devide)
                    {
                        continue;
                    }
                    if (!flag.m_client)
                    {
                        continue;
                    }
                    string fieldName = info.FieldNames[i];

                    if (is_i18n && i > 0)
                    {
                        if (i != i18n_field)
                            continue;
                        else
                            fieldName = "des";
                    }

                    string v = row[i];
                    if (string.IsNullOrEmpty(v))
                    {
                        if (flag.m_default)
                        {
                            v = FlagTypeDefaultLuaValue(flag.m_field_type, out bool isEmptyTable);
                            if (!hasEmpty && isEmptyTable)
                            {
                                hasEmpty = true;
                            }
                        }
                        else
                        {
                            v = "nil";
                        }
                    }
                    else
                    {
                        if (!is_i18n && flag.NeedCheck())
                        {
                            bool find = false;
                            foreach (var kv in info.one_sheet_data_table)
                            {
                                if (kv.Key.IsSameType(flag.m_field_type))
                                {
                                    if (kv.Value.ContainsKey(v))
                                    {
                                        find = true;
                                        v = COMMON_DATA_TABLE_PREFIX + "[" + kv.Value[v].index + "]";
                                        break;
                                    }
                                }
                            }
                            if (!find)
                            {
                                v = FlagTypeToLua(v, flag.m_field_type);
                            }
                        }
                        else
                        {
                            v = FlagTypeToLua(v, flag.m_field_type);
                        }
                    }
                    //多级字段
                    if (fieldName.Contains('.'))
                    {
                        string[] ss = fieldName.Split('.');
                        if (affix == null)
                        {
                            affix = ss[0];
                            if (first && !keys.Contains(affix))
                            {
                                keys.Add(affix);
                            }
                            sb.Append("{");
                        }
                        else if (affix != ss[0])
                        {
                            sb.Append("}, ");
                            affix = ss[0];
                            if (first && !keys.Contains(affix))
                            {
                                keys.Add(affix);
                            }
                            sb.Append("{");
                        }
                        sb.Append($"{ss[1]}={v}, ");
                    }
                    else
                    {
                        if (is_i18n)
                        {
                            sb.Append($"{v}, ");
                        }
                        else
                        {
                            if (affix != null)
                            {
                                sb.Append("}, ");
                                affix = null;
                            }
                            if (first)
                            {
                                keys.Add(fieldName);
                            }
                            sb.Append($"{v}, ");
                        }

                    }
                }
                if (affix != null)
                {
                    sb.Append("}");
                    affix = null;
                }
                string line = sb.ToString();
                while (line.EndsWith("nil, "))
                {
                    line = line.Substring(0, line.Length - 5);
                }
                if (!is_i18n)
                {
                    config.Append(line);
                    config.AppendLine("},");
                }
                else if (!string.IsNullOrEmpty(line))
                {
                    config.Append($"[{key}]={line}");
                }
                first = false;
            }
            config.Append("}");
            if (!is_i18n)
            {
                StringBuilder keyBuilder = new StringBuilder();
                keyBuilder.Append("local key = {");
                for (int i = 0; i < keys.Count; i++)
                {
                    keyBuilder.Append($"{keys[i]}={i + 1}, ");
                }
                keyBuilder.Append("}");
                sw.WriteLine(keyBuilder.ToString());

                int sum_count = 0;
                foreach (var kv in info.one_sheet_data_table)
                {
                    sum_count += kv.Value.Count;
                }
                if (sum_count > 0)
                {
                    StringBuilder localBuilder = new StringBuilder();
                    localBuilder.Append($"local {COMMON_DATA_TABLE_PREFIX} = {{");
                    foreach (var kv in info.one_sheet_data_table)
                    {
                        foreach (var data in kv.Value)
                        {
                            string one_value = FlagTypeToLua(data.Key, data.Value.flag_info.m_field_type);
                            localBuilder.Append($"{one_value}, ");
                        }
                    }
                    localBuilder.Append("}");
                    sw.WriteLine(localBuilder.ToString());
                }
            }

            if (hasEmpty)
            {
                sw.WriteLine("local empty = {}");
            }

            sw.WriteLine(config.ToString());
            if (is_i18n)
            {
                sw.Write($"return config");
            }
            else
            {
                sw.Write($"return config, '{info.KeyFieldName}', key");
            }
        }

        public static void ExportCsv(ExcelInfo info, string filepath)
        {
            if (File.Exists(filepath))
            {
                File.SetAttributes(filepath, FileAttributes.Normal);
            }
            bool is_i18n = i18n_path.Length > 0 && filepath.StartsWith(i18n_path);

            using (FileStream fs = new FileStream(filepath, FileMode.Create, FileAccess.ReadWrite))
            {
                StreamWriter sw = new StreamWriter(fs);
                string bom = "\uFEFF";
                sw.Write(bom);
                sw.WriteLine("//this config file was auto-generated by Excel2lua tool do not modify it!");
                //表头
                for (int i = 0; i < info.FieldNames.Count; i++)
                {
                    var flag = info.FieldFlags[i];
                    if (flag.m_ignore || flag.m_devide)
                    {
                        continue;
                    }
                    if (!flag.m_server)
                    {
                        continue;
                    }
                    string fieldName = info.FieldNames[i];
                    if (is_i18n && i > 0)
                    {
                        if (i != i18n_field)
                            continue;
                        else
                            fieldName = "des";
                    }
                    sw.Write(fieldName);
                    if (i < info.FieldNames.Count - 1)
                    {
                        sw.Write(",");
                    }
                }
                sw.WriteLine();
                var itor = info.ExcelRows.GetEnumerator();
                while (itor.MoveNext())
                {
                    var row = itor.Current.Value;
                    for (int i = 0; i < row.Count; i++)
                    {
                        var flag = info.FieldFlags[i];
                        if (flag.m_ignore || flag.m_devide)
                        {
                            continue;
                        }
                        if (!flag.m_server)
                        {
                            continue;
                        }
                        if (is_i18n && i > 0 && i != i18n_field)
                        {
                            continue;
                        }
                        string v = row[i];
                        if (string.IsNullOrEmpty(v))
                        {
                            if (flag.m_default)
                            {
                                v = FlagTypeDefaultCsvValue(flag.m_field_type);
                            }
                            else
                            {
                                if (i < row.Count - 1)
                                {
                                    sw.Write(',');
                                }
                                continue;
                            }
                        }
                        v = Regex.Replace(v, "\"", "\"\"");
                        if (v.Contains(",")
                            || v.Contains("\r")
                            || v.Contains("\n")
                            || v.Contains("\"")
                            || v.Contains("\r\n")
                            )
                        {
                            v = $"\"{v}\"";
                        }
                        v = FlagTypeToCsv(v, flag.m_field_type);
                        sw.Write(v);
                        if (i < row.Count - 1)
                        {
                            sw.Write(',');
                        }
                    }
                    sw.WriteLine();
                }
                sw.Close();
            }

            if (m_backup)
            {
                ExportBackupFiles(filepath, "server");
            }

            using (FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read))
            {
                SaveOutputFileMD5(filepath, fs);
            }
        }

        public static void ExportJson(ExcelInfo info, string filepath)
        {
            bool is_i18n = i18n_path.Length > 0 && filepath.StartsWith(i18n_path);
            if (File.Exists(filepath))
            {
                File.SetAttributes(filepath, FileAttributes.Normal);
            }
            using (FileStream fs = new FileStream(filepath, FileMode.Create, FileAccess.Write))
            {
                StreamWriter sw = new StreamWriter(fs);
                ExportJsonLine(sw, "{\r\n");
                ExportJsonImpl(sw, info, is_i18n);
                ExportJsonLine(sw, "}\r\n");
                sw.Close();
            }
            using (FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read))
            {
                SaveOutputFileMD5(filepath, fs);
            }
        }

        public static void ExportJsonImpl(StreamWriter sw, ExcelInfo info, bool is_i18n = false)
        {
            var itor = info.ExcelRows.GetEnumerator();
            itor.MoveNext();
            while (true)
            {
                var key = itor.Current.Key;
                var row = itor.Current.Value;
                FlagInfo flag = info.FieldFlags[0];
                key = FlagTypeToJson(key, flag.m_field_type, true);

                string affix = null;
                ExportJsonLine(sw, $"{key}:{{");
                for (int i = 0; i < row.Count; i++)
                {
                    flag = info.FieldFlags[i];
                    if (flag.m_ignore || flag.m_devide)
                    {
                        continue;
                    }
                    string v = row[i];
                    string fieldName = info.FieldNames[i];
                    if (is_i18n && i > 0)
                    {
                        if (i != i18n_field)
                            continue;
                        else
                            fieldName = "des";
                    }

                    if (string.IsNullOrEmpty(v))
                    {
                        if (flag.m_default)
                        {
                            v = FlagTypeDefaultJsonValue(flag.m_field_type);
                        }
                        else
                        {
                            //continue;
                            v = "null";
                        }
                    }
                    else
                    {
                        //if (!ConfigLoader.appConfig.duplicate_handle)
                        //{
                        //    v = FlagTypeToJson(v, flag.m_field_type);
                        //}
                        //else if (!ConfigLoader.appConfig.global_ref && flag.NeedCheck())
                        //{
                        //    bool find = false;
                        //    foreach (var kv in info.one_sheet_data_table)
                        //    {
                        //        if (kv.Key.IsSameType(flag.m_field_type))
                        //        {
                        //            if (kv.Value.ContainsKey(v))
                        //            {
                        //                find = true;
                        //                v = COMMON_DATA_TABLE_PREFIX + "[\"" + info.ExcelName + "\"]" + "[" + kv.Value[v].index + "]";
                        //            }
                        //        }
                        //    }
                        //    if (!find)
                        //    {
                        //        v = FlagTypeToLua(v, flag.m_field_type);
                        //    }
                        //}
                        //else
                        //{

                        //}
                        v = FlagTypeToJson(v, flag.m_field_type);
                    }
                    //json字符串不允许多余逗号,所以逗号由后一个字段写入
                    bool need_comma = true;
                    bool normal_to_mul = false;
                    bool mul_to_mul = false;
                    //多级字段
                    if (fieldName.Contains('.'))
                    {
                        string[] ss = fieldName.Split('.');
                        //普通字段和多级字段衔接
                        if (affix == null)
                        {
                            affix = ss[0];
                            normal_to_mul = true;
                        }
                        //多级字段1和多级字段2衔接
                        //当两个不同的多级字段连接时,才不写逗号,其余逗号均由下一个字段控制,若下一个字段不存在则不写逗号
                        else if (affix != ss[0])
                        {
                            mul_to_mul = true;
                            need_comma = false;
                            affix = ss[0];
                        }
                        //多级字段1内部衔接
                        //else
                        //{
                        //    sw.Write(", ");
                        //}
                        fieldName = ss[1];
                    }
                    else
                    {
                        //多级字段和普通字段衔接
                        if (affix != null)
                        {
                            ExportJsonLine(sw, "}");
                            affix = null;
                        }
                    }
                    if (i != 0 && need_comma)
                    {
                        ExportJsonLine(sw, ", ");
                    }
                    if (normal_to_mul)
                        ExportJsonLine(sw, $"\"{affix}\":{{");
                    if (mul_to_mul)
                    {
                        ExportJsonLine(sw, $"}}, ");
                        ExportJsonLine(sw, $"\"{affix}\":{{");
                    }
                    ExportJsonLine(sw, $"\"{fieldName}\":{v}");
                }
                if (affix != null)
                {
                    ExportJsonLine(sw, "}");
                    affix = null;
                }
                if (itor.MoveNext())
                {
                    ExportJsonLine(sw, "},\r\n");
                }
                else
                {
                    ExportJsonLine(sw, "}\r\n");
                    break;
                }

            }
        }

        private static void ExportJsonLine(StreamWriter sw, string line_str)
        {
            if (ConfigLoader.appConfig.long_json)
            {
                sw.Write(line_str.Trim().Replace("\r\n", ""));
            }
            else
            {
                sw.Write(line_str);
            }
        }

        public static void ExportIndexTable(string path)
        {
            if (!generate_lua_global)
            {
                return;
            }
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            //读取已有索引键表信息
            string filepath = Path.Combine(path, INDEX_FILE_NAME);
            Dictionary<string, string> index_table = new Dictionary<string, string>();
            if (ConfigLoader.appConfig.use_md5 && File.Exists(filepath))
            {
                File.SetAttributes(filepath, FileAttributes.Normal);
                using (FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read))
                {
                    StreamReader sr = new StreamReader(fs);
                    string line = null;
                    while ((line = sr.ReadLine()) != null)
                    {
                        Regex kv_reg = new Regex(@"^\s*\[""(?<key>(\w|_)*)""\]=\{(?<value>.*)\},\s*$");
                        Match mcom = kv_reg.Match(line);
                        if (mcom.Success)
                        {
                            index_table.Add(mcom.Groups["key"].Value, mcom.Groups["value"].Value);
                        }
                    }
                    sr.Close();
                }
            }
            //修改变化的索引键表
            foreach (ExcelInfo info in ExcelLoader.tables.Values)
            {
                if (!info.indexflag)
                {
                    continue;
                }
                string value = "";
                foreach (string index in info.IndexList)
                {
                    string key = FlagTypeToLua(index, info.IndexFieldType.m_field_type);
                    value = value + key + ",";
                }
                if (index_table.Keys.Contains(info.ExcelName))
                {
                    index_table[info.ExcelName] = value;
                }
                else
                {
                    index_table.Add(info.ExcelName, value);
                }
            }
            //删除变化的索引键表
            if (ExcelLoader.lost_files.Count > 0)
            {
                foreach (var file in ExcelLoader.lost_files)
                {
                    string file_name = file.Replace('\\', '/');
                    Regex tmp_reg = new Regex(@"(?<file_name>(\w|_)*)\.\w*$");
                    Match mcom = tmp_reg.Match(file_name);
                    if (mcom.Success)
                    {
                        file_name = mcom.Groups["file_name"].Value;
                    }
                    index_table.Remove(file_name);
                }
            }
            if (File.Exists(filepath))
            {
                File.SetAttributes(filepath, FileAttributes.Normal);
            }
            using (FileStream fs = new FileStream(filepath, FileMode.Create, FileAccess.Write))
            {
                StreamWriter sw = new StreamWriter(fs);
                sw.WriteLine("return {");
                foreach (var kv in index_table)
                {
                    sw.WriteLine($"[\"{kv.Key}\"]={{{kv.Value}}},");
                }
                sw.WriteLine($"}},'IndexTable'");
                sw.Close();
            }
        }

        public static FlagInfo ParseFlag(string flag)
        {
            FlagInfo info = new FlagInfo();
            FlagType ftype = info.m_field_type;
            for (int i = 0; i < flag.Length; i++)
            {
                char f = flag[i];
                if (f == 'a')
                {
                    ftype.m_type = eFlagType.ARRAY;
                    ftype.m_inner_type = new FlagType();
                    if (ftype != info.m_field_type)
                    {
                        ftype.m_delimiter = ',';
                        if (info.m_field_type.m_delimiter == ',')
                        {
                            info.m_field_type.m_delimiter = '|';
                        }
                    }
                    else if (flag.Contains(','))
                    {
                        ftype.m_delimiter = ',';
                    }
                    else if (flag.Contains('|'))
                    {
                        ftype.m_delimiter = '|';
                    }
                    else if (flag.Contains(';'))
                    {
                        ftype.m_delimiter = ';';
                    }
                    else if (flag.Contains('_'))
                    {
                        ftype.m_delimiter = '_';
                    }
                    else
                    {
                        return null;
                    }
                    ftype = ftype.m_inner_type;
                    flag.Remove(i, 1);
                }
            }

            if (flag.Contains('o'))
            {
                ftype.m_type = eFlagType.OBJECT;
                int sub_str_l = flag.IndexOf('[');
                int sub_str_r = flag.IndexOf(']');
                string sub_str = flag.Substring(sub_str_l + 1, sub_str_r - sub_str_l - 1);
                TransStrToSubType(sub_str, ftype);
                flag = flag.Remove(sub_str_l, sub_str_r - sub_str_l + 1);
            }

            foreach (var f in flag)
            {
                if (f == 'i')
                {
                    ftype.m_type = eFlagType.INT;
                }
                else if (f == 'f')
                {
                    ftype.m_type = eFlagType.FLOAT;
                }
                else if (f == 'b')
                {
                    ftype.m_type = eFlagType.BOOL;
                }
                else if (f == 's')
                {
                    ftype.m_type = eFlagType.STRING;
                }
                else if (f == 't')
                {
                    ftype.m_type = eFlagType.TIME;
                }
                else if (f == 'h')
                {
                    ftype.m_type = eFlagType.TABLE;
                }
                else if (f == '<')
                {
                    info.m_client = true;
                }
                else if (f == '>')
                {
                    info.m_server = true;
                }
                else if (f == '*')//新增忽略功能
                {
                    info.m_ignore = true;
                }
                else if (f == 'p')
                {
                    info.m_primary = true;
                }
                else if (f == 'u')
                {
                    info.m_unique = true;
                }
                else if (f == 'e')
                {
                    info.m_nullable = true;
                }
                else if (f == 'd')
                {
                    info.m_default = true;
                }
                else if (f == 'x')
                {
                    info.m_index = true;
                }
                else if (f == '#')
                {
                    info.m_devide = true;
                }
                else if (f == 'l')
                {
                    info.m_devide_link = true;
                }
            }
            if (info.m_default && info.m_primary)
            {
                info.m_autoid = true;
            }

            if (info.m_client == false && info.m_server == false)
            {
                info.m_client = true;
                info.m_server = true;
            }
            return info;
        }

        public static void TransStrToSubType(string data, FlagType flag_type)
        {
            if (data.Contains('x'))
            {
                flag_type.m_check_sub_type = false;
                return;
            }
            flag_type.m_check_sub_type = true;
            flag_type.m_sub_types = new List<eFlagType>();
            foreach (var f in data)
            {
                if (f == 'i')
                {
                    flag_type.m_sub_types.Add(eFlagType.INT);
                }
                else if (f == 'f')
                {
                    flag_type.m_sub_types.Add(eFlagType.FLOAT);
                }
                else if (f == 'b')
                {
                    flag_type.m_sub_types.Add(eFlagType.BOOL);
                }
                else if (f == 's')
                {
                    flag_type.m_sub_types.Add(eFlagType.STRING);
                }
            }
        }

        public static bool CheckFlag(string data, FlagInfo flag)
        {
            if (flag.m_ignore || flag.m_devide)
            {
                return true;
            }
            if (flag.m_nullable)
            {
                // 空格字符会被判断成正确的 e 标记，修正为空格字符报出log
                if (string.IsNullOrEmpty(data))  //||string.IsNullOrWhiteSpace(data)
                {
                    return true;
                }
            }
            if (!CheckFlagType(data, flag.m_field_type))
            {
                return false;
            }
            return true;
        }

        public static bool CheckFlagType(string data, FlagType type)
        {
            if (type.m_type == eFlagType.BOOL)
            {
                return data.ToLower() == "true" || data.ToLower() == "false" || data == "0" || data == "1";
            }
            if (type.m_type == eFlagType.FLOAT)
            {
                return float.TryParse(data, out float f);
            }
            if (type.m_type == eFlagType.INT)
            {
                if (int.TryParse(data, out int i))
                {
                    return true;
                }
                else
                {
                    return long.TryParse(data, out long li);
                }
            }
            if (type.m_type == eFlagType.TIME)
            {
                return DateTime.TryParse(data, out DateTime dt);
            }
            if (type.m_type == eFlagType.TABLE)
            {
                return true;
            }
            if (type.m_type == eFlagType.STRING)
            {
                return true;
            }
            if (type.m_type == eFlagType.OBJECT)
            {
                if (!type.m_check_sub_type)
                    return true;
                return CheckObjectSubType(data, type);
            }
            if (type.m_type == eFlagType.ARRAY)
            {
                string[] ss = data.Split(type.m_delimiter);
                foreach (string s in ss)
                {
                    if (!CheckFlagType(s, type.m_inner_type))
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        public static bool CheckObjectSubType(string obj_str, FlagType flag_type)
        {
            string[] ss = obj_str.Split(',');

            if (ss.Length > flag_type.m_sub_types.Count)
            {
                return false;
            }
            for (int i = 0; i < ss.Length; ++i)
            {
                string data = ss[i];
                if (flag_type.m_sub_types[i] == eFlagType.BOOL)
                {
                    if (!(data.ToLower() == "true" || data.ToLower() == "false" || data == "0" || data == "1"))
                    {
                        return false;
                    }
                    continue;
                }
                if (flag_type.m_sub_types[i] == eFlagType.FLOAT)
                {
                    if (!float.TryParse(data, out float f))
                    {
                        return false;
                    }
                    continue;
                }
                if (flag_type.m_sub_types[i] == eFlagType.INT)
                {
                    if (int.TryParse(data, out int j))
                    {
                        return true;
                    }
                    else
                    {
                        return long.TryParse(data, out long li);
                    }
                }
                if (flag_type.m_sub_types[i] == eFlagType.STRING)
                {
                    continue;
                }
            }
            return true;
        }

        public static string ToLua(string data, FlagInfo flag)
        {
            return FlagTypeToLua(data, flag.m_field_type);
        }

        public static string FlagTypeToLua(string data, FlagType type, bool is_check = true)
        {
            if (type.m_type == eFlagType.ARRAY)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("{");
                string[] ss = data.Split(type.m_delimiter);
                foreach (string s in ss)
                {
                    string x = FlagTypeToLua(s, type.m_inner_type, is_check);
                    sb.Append($"{x}, ");
                }
                sb.Append("}");
                return sb.ToString();
            }

            if (type.m_type == eFlagType.BOOL)
            {
                if (data.ToLower() == "true" || data == "1")
                {
                    return "true";
                }
                return "false";
            }
            if (type.m_type == eFlagType.STRING)
            {
                if (data.Contains("\n"))
                {
                    data = data.Replace("\n", "\r\n");
                }
                if (data.Contains("\r\n") || data.Contains("\n"))  // || data.Contains("\n")
                {
                    return $"[[{data}]]";
                }
                return $"\"{data.Replace("\"", "\\\"")}\"";
            }
            if (type.m_type == eFlagType.TIME)
            {
                return "{" + data + "}";
            }
            if (type.m_type == eFlagType.TABLE)
            {
                data = Regex.Replace(data, @"\s+", " ");
                return "{" + data + "}";
            }
            if (type.m_type == eFlagType.OBJECT)
            {
                return GenerateObjectLua(data, type);
            }
            return data;
        }

        public static string GenerateObjectLua(string data, FlagType flag_type)
        {
            string[] ss = data.Split(',');
            string rtn_str = "";
            if (!flag_type.m_check_sub_type)
            {
                for (int i = 0; i < ss.Length; ++i)
                {
                    string sub_data = ss[i];
                    if (int.TryParse(sub_data, out int j))
                    {
                        rtn_str = rtn_str + j.ToString() + ",";
                        continue;
                    }
                    if (long.TryParse(sub_data, out long li))
                    {
                        rtn_str = rtn_str + li.ToString() + ",";
                        continue;
                    }
                    if (float.TryParse(sub_data, out float f))
                    {
                        rtn_str = rtn_str + f.ToString() + ",";
                        continue;
                    }
                    if (sub_data.ToLower() == "true" || sub_data.ToLower() == "false")
                    {
                        rtn_str = rtn_str + sub_data.ToLower() + ",";
                        continue;
                    }
                    rtn_str = rtn_str + $"\"{sub_data}\"" + ",";
                }
            }
            else
            {
                for (int i = 0; i < ss.Length; ++i)
                {
                    string sub_data = ss[i];
                    eFlagType sub_type = flag_type.m_sub_types[i];
                    if (sub_type == eFlagType.INT)
                    {
                        int j = int.Parse(sub_data);
                        rtn_str = rtn_str + j + ",";
                        continue;
                    }
                    if (sub_type == eFlagType.FLOAT)
                    {
                        float f = float.Parse(sub_data);
                        rtn_str = rtn_str + f + ",";
                        continue;
                    }
                    if (sub_type == eFlagType.BOOL)
                    {
                        sub_data = (sub_data.ToLower() == "true" || sub_data == "1") ? "true" : "false";
                        rtn_str = rtn_str + sub_data + ",";
                        continue;
                    }
                    if (sub_type == eFlagType.STRING)
                    {
                        rtn_str = rtn_str + $"\"{sub_data}\"" + ",";
                        continue;
                    }
                }
            }
            return "{" + rtn_str + "}";
        }

        public static string FlagTypeDefaultLuaValue(FlagType ftype, out bool isEmptyTable)
        {
            isEmptyTable = false;
            eFlagType type = ftype.m_type;
            if (type == eFlagType.BOOL)
            {
                return "false";
            }
            if (type == eFlagType.STRING)
            {
                return "\"\"";
            }
            if (type == eFlagType.INT || type == eFlagType.FLOAT)
            {
                return "0";
            }
            if (type == eFlagType.ARRAY || type == eFlagType.TABLE || type == eFlagType.OBJECT)
            {
                isEmptyTable = true;
                return "{}";
            }
            return "";
        }

        public static string FlagTypeToCsv(string data, FlagType type)
        {
            if (type.m_type == eFlagType.BOOL)
            {
                if (data.ToLower() == "true" || data == "1")
                {
                    return "1";
                }
                return "0";
            }
            if (type.m_type == eFlagType.TIME)
            {
                return "";
            }
            if (type.m_type == eFlagType.TABLE)
            {
                return "";
            }
            return data;
        }

        public static string FlagTypeDefaultCsvValue(FlagType type)
        {
            if (type.m_type == eFlagType.BOOL)
            {
                return "0";
            }
            if (type.m_type == eFlagType.STRING)
            {
                return "";
            }
            if (type.m_type == eFlagType.INT || type.m_type == eFlagType.FLOAT)
            {
                return "0";
            }
            return "";
        }

        public static string FlagTypeToJson(string data, FlagType type, bool is_key = false)
        {
            if (type.m_type == eFlagType.ARRAY)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("[");
                string[] ss = data.Split(type.m_delimiter);
                for (int i = 0; i < ss.Length; i++)
                {
                    string s = ss[i];
                    string x = FlagTypeToJson(s, type.m_inner_type);
                    sb.Append($"{x}");
                    if (i < ss.Length - 1)
                        sb.Append(",");
                }
                sb.Append("]");
                return sb.ToString();
            }

            if (type.m_type == eFlagType.BOOL)
            {
                if (data.ToLower() == "true" || data == "1")
                {
                    return "true";
                }
                return "false";
            }
            if (type.m_type == eFlagType.STRING)
            {
                return $"\"{data.Replace("\r\n", "\\r\n").Replace("\r", "\\r").Replace("\"", "\\\"")}\"";
            }
            if (type.m_type == eFlagType.TIME)
            {
                return "{" + data + "}";
            }
            if (type.m_type == eFlagType.TABLE)
            {
                data = Regex.Replace(data, @"\s+", " ");
                return "{" + data + "}";
            }
            if (type.m_type == eFlagType.OBJECT)
            {
                return GenerateObjectJson(data, type);
            }
            if (type.m_type == eFlagType.INT && is_key)
            {
                return "\"" + data + "\"";
            }
            return data;
        }

        public static string GenerateObjectJson(string data, FlagType flag_type)
        {
            string[] ss = data.Split(',');
            string rtn_str = "";
            if (!flag_type.m_check_sub_type)
            {
                for (int i = 0; i < ss.Length; ++i)
                {
                    if (i != 0)
                    {
                        rtn_str = rtn_str + ",";
                    }
                    string sub_data = ss[i];
                    int key = i;
                    string key_str = "\"" + key + "\":";
                    rtn_str = rtn_str + key_str;
                    if (int.TryParse(sub_data, out int j))
                    {
                        rtn_str = rtn_str + j.ToString();
                        continue;
                    }
                    if (long.TryParse(sub_data, out long li))
                    {
                        rtn_str = rtn_str + li.ToString();
                        continue;
                    }
                    if (float.TryParse(sub_data, out float f))
                    {
                        rtn_str = rtn_str + f.ToString();
                        continue;
                    }
                    if (sub_data.ToLower() == "true" || sub_data.ToLower() == "false")
                    {
                        rtn_str = rtn_str + sub_data.ToLower();
                        continue;
                    }
                    rtn_str = rtn_str + $"\"{sub_data}\"";
                }
            }
            else
            {
                for (int i = 0; i < ss.Length; ++i)
                {
                    if (i != 0)
                    {
                        rtn_str = rtn_str + ",";
                    }
                    string sub_data = ss[i];
                    int key = i;
                    eFlagType sub_type = flag_type.m_sub_types[i];
                    string key_str = "\"" + key + "\":";
                    rtn_str = rtn_str + key_str;
                    if (sub_type == eFlagType.INT)
                    {
                        int j = int.Parse(sub_data);
                        rtn_str = rtn_str + j;
                        continue;
                    }
                    if (sub_type == eFlagType.FLOAT)
                    {
                        float f = float.Parse(sub_data);
                        rtn_str = rtn_str + f;
                        continue;
                    }
                    if (sub_type == eFlagType.BOOL)
                    {
                        sub_data = (sub_data.ToLower() == "true" || sub_data == "1") ? "true" : "false";
                        rtn_str = rtn_str + sub_data;
                        continue;
                    }
                    if (sub_type == eFlagType.STRING)
                    {
                        rtn_str = rtn_str + $"\"{sub_data}\"";
                        continue;
                    }
                }
            }
            return "{" + rtn_str + "}";
        }

        public static string FlagTypeDefaultJsonValue(FlagType ftype)
        {
            eFlagType type = ftype.m_type;
            if (type == eFlagType.BOOL)
            {
                return "false";
            }
            if (type == eFlagType.STRING)
            {
                return "\"\"";
            }
            if (type == eFlagType.INT || type == eFlagType.FLOAT)
            {
                return "0";
            }
            if (type == eFlagType.ARRAY || type == eFlagType.TABLE)
            {
                return "[]";
            }
            if (type == eFlagType.OBJECT)
            {
                return "{}";
            }
            return "";
        }

        private static void SaveOutputFileMD5(string filepath, FileStream fs)
        {
            string md5_str;
            byte[] retval = md5.ComputeHash(fs);
            StringBuilder md5_sb = new StringBuilder();
            foreach (byte b in retval)
            {
                md5_sb.Append(b.ToString("x2"));
            }
            md5_str = md5_sb.ToString();
            string regular_file = filepath.Replace('\\', '/');
            if (ExcelLoader.changes.output_MD5.ContainsKey(regular_file))
            {
                ExcelLoader.changes.output_MD5[regular_file] = md5_str;
            }
            else
            {
                ExcelLoader.changes.output_MD5.Add(regular_file, md5_str);
            }
        }

        private static void AddSecurityControll2File(string filePath)
        {
            //获取文件信息
            FileInfo fileInfo = new FileInfo(filePath);
            //获得该文件的访问权限
            System.Security.AccessControl.FileSecurity fileSecurity = fileInfo.GetAccessControl();
            //添加ereryone用户组的访问权限规则 完全控制权限
            fileSecurity.AddAccessRule(new FileSystemAccessRule("Everyone", FileSystemRights.FullControl, AccessControlType.Allow));
            //添加Users用户组的访问权限规则 完全控制权限
            fileSecurity.AddAccessRule(new FileSystemAccessRule("Users", FileSystemRights.FullControl, AccessControlType.Allow));
            //设置访问权限
            fileInfo.SetAccessControl(fileSecurity);
        }
    }
}
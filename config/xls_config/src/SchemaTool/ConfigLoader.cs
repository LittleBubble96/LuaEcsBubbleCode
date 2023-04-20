using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SchemaTool
{
    class CheckConfig
    {
        public string[] excel_path;
    }
    class ExportConfig
    {
        public string excel_path;
        public string export_path;
        public string format;
        public string link_path;
        public string unique_path;
    }

    class AppConfig
    {
        public string p4_path;
        public bool use_md5;
        public string md5_path;
        public string schema_path;
        public string global_config_path;
        public bool duplicate_handle=true;//是否执行重复数据转引用
        public bool long_json = true;
        public CheckConfig check;
        public ExportConfig[] export;
        public string i18n;//语言表目录
    }

    static class ConfigLoader
    {
        static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static AppConfig appConfig;
        public static bool LoadConfig(string config)
        {
            try
            {
                string json = File.ReadAllText(config);
                appConfig = JsonConvert.DeserializeObject<AppConfig>(json);
            }
            catch (Exception e)
            {
                logger.Error("LoadConfig {0} error: {1}", config, e.Message);
                return false;
            }
            logger.Trace("LoadConfig {0} OK ", config);
            return true;
        }
    }
}

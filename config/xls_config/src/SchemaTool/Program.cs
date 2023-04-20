using System;
using System.Collections.Generic;
using CommandLine;
using Monitor.Core.Utilities;

namespace SchemaTool
{
    internal class Program
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public static bool isSuccess = true;

        public class Options
        {
            [Option('c', "check", Required = false, HelpText = "检查配置")]
            public bool Check { get; set; }

            [Option('e', "export", Required = false, HelpText = "导出配置")]
            public bool Export { get; set; }

            [Option('f', "false", Required = false, HelpText = "强制重新生成")]
            public bool Force { get; set; }

            [Option('o', "backup", Required = false, HelpText = "备份生成文件")]
            public bool Backup { get; set; }

            [Value(0, Required = true, HelpText = "配置路径")]
            public string ConfigPath { get; set; }
        }

        //-c check -d dump -e export
        private static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                              .WithParsed<Options>(o =>
                              {
                                  string config_path = o.ConfigPath;
                                  if (!ConfigLoader.LoadConfig(config_path))
                                  {
                                      logger.Error("找不到配置：{0}", config_path);
                                      return;
                                  }
                                  //P4Tool.ReadP4Info(ConfigLoader.appConfig.p4_path);
                                  ExcelLoader.m_force = o.Force;
                                  Excel2LuaTool.m_backup = o.Backup;
                                  if (o.Export)
                                  {
                                      string md5path = ConfigLoader.appConfig.md5_path;
                                      ExcelLoader.Init(md5path, config_path, ConfigLoader.appConfig.global_config_path);

                                      foreach (var cfg in ConfigLoader.appConfig.export)
                                      {
                                          ExcelLoader.LoadFile(cfg.excel_path);
                                          if (!VPExcelCheckTool.CheckExcelFieldCondition(cfg.excel_path))
                                          {
                                              isSuccess = false;
                                          }
                                      }
                                      if (isSuccess)
                                      {
                                          ExcelLoader.FindLostFile();
                                          Excel2LuaTool.Export();
                                          ExcelLoader.EndExporting(md5path, config_path);
                                      }
                                  }
                              });
            if (!isSuccess)
            {
                logger.Fatal("有配置错误 请更改！！");
                //Console.ReadLine();
            }
        }
    }
}
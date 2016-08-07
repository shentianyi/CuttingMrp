using Brilliantech.Framwork.Utils.LogUtil;
using CuttingMrpLib;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CuttingMrpDashSvc.Properties;
using System.IO;
using System.Text.RegularExpressions;

namespace CuttingMrpDashSvc.Job
{
    public class AutoStockJob : IJob
    {
        private static Object fileLocker = new Object();

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                LogUtil.Logger.Info("start gen AutoStockJob");

                List<string> files = FileUtility.GetAllFilesFromDirectory(Settings.Default.autoStockFilePath);
//Regex r = new Regex(Settings.Default.autoStockFileRegex);
                 
                LogUtil.Logger.Error(files);

                if (files != null && files.Count > 0)
                {
                    foreach (string file in files)
                    {
                        // if (r.IsMatch(Path.GetFileName(file))){
                        string fileName = Path.GetFileName(file);
                        if (Settings.Default.autoStockFileRegex.Split(';').Contains(fileName))
                        {
                            if (FileUtility.IsFileOpen(file))
                            {
                                string newFile = Process(file);
                                if (!string.IsNullOrWhiteSpace(newFile))
                                {
                                    CalculateSetting setting = new CalculateSetting()
                                    {
                                        TaskType = "AutoStock",
                                        Parameters = newFile //new Dictionary<string, string>()
                                    };
                                    // setting.Parameters.Add("file", newFile);

                                    ICalculateService cs = new CalculateService(Settings.Default.db);
                                    cs.Start(Settings.Default.mrpQueue, setting);
                                }
                            }
                        }
                       // }
                    }
                }
                
                LogUtil.Logger.Info("end gen AutoStockJob");
            }
            catch (Exception ex)
            {
                LogUtil.Logger.Error("AutoStockJob exec error!");
                LogUtil.Logger.Error(ex.Message);
                LogUtil.Logger.Error(ex.StackTrace);

                throw ex;
            }
        }

        public string Process(string file)
        {
            try
            {
                lock (fileLocker)
                {
                    // string currentDir = @"C:\cz\MrpDashSvc";//Directory.GetCurrentDirectory();
                    string currentDir = Settings.Default.autoStockCopyFilePath;

                    string processDir = Path.Combine(currentDir, "Processing", DateTime.Now.ToString("yyyy-MM-dd"));
                    if (!Directory.Exists(processDir))
                    {
                        Directory.CreateDirectory(processDir);
                    }
                    string fileName = Path.Combine(processDir, DateTime.Now.ToString("hh-mm_")+Path.GetFileNameWithoutExtension(file) + "_" + Guid.NewGuid().ToString() + System.IO.Path.GetExtension(file));

                    LogUtil.Logger.Error(fileName);

                    File.Copy(file, fileName, true);
                    LogUtil.Logger.Info("[copy file][from] " + file + " to " + fileName);
                    return fileName;
                }
            }
            catch (Exception ex)
            {
                LogUtil.Logger.Error("AutoStockJob processing error!");
                LogUtil.Logger.Error(ex.Message);
                LogUtil.Logger.Error(ex.StackTrace);
            }
            return null;
        }
    }
}

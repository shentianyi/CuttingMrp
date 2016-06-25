using Brilliantech.Framwork.Utils.LogUtil;
using CuttingMrpDashSvc.Properties;
using CuttingMrpLib;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CuttingMrpDashSvc.Job
{
    public class StockSumRecordJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                DateTime date = DateTime.Now.Date;
                //(DateTime)context.MergedJobDataMap.Get("Date");
                LogUtil.Logger.Info("start gen StockSumRecordJob");
                //IStockSumRecordService ss = new StockSumRecordService(Properties.Settings.Default.db);
                //ss.Generate(date);
             
                CalculateSetting setting = new CalculateSetting()
                {
                    TaskType = "SumStock",
                    Parameters = DateTime.Now.Date.ToString() //new Dictionary<string, string>()
                };
               // setting.Parameters.Add("date", DateTime.Now.Date.ToString());

                ICalculateService cs = new CalculateService(Settings.Default.db);
                cs.Start(Settings.Default.mrpQueue, setting);

                LogUtil.Logger.Info("end gen StockSumRecordJob");
            }
            catch (Exception ex) {
                LogUtil.Logger.Error("StockSumRecordJob exec error!");
                LogUtil.Logger.Error(ex.Message);
                LogUtil.Logger.Error(ex.StackTrace);

                LogUtil.Logger.Info("[Refire] StockSumRecordJob");
                JobExecutionException e2 = new JobExecutionException(ex);
                e2.RefireImmediately = true;
                throw e2;
            }
        }
    }
}

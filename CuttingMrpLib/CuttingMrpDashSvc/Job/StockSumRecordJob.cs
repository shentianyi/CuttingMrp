using Brilliantech.Framwork.Utils.LogUtil;
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
                DateTime date = (DateTime)context.MergedJobDataMap.Get("Date");
                LogUtil.Logger.Info("start run StockSumRecordJob");
                IStockSumRecordService ss = new StockSumRecordService(Properties.Settings.Default.db);
                ss.Generate(date);
                LogUtil.Logger.Info("end run StockSumRecordJob");
            }
            catch (Exception ex) {
                LogUtil.Logger.Error("StockSumRecordJob exec error!");

                LogUtil.Logger.Error(ex.Message);

                LogUtil.Logger.Error(ex.StackTrace);
            }
        }
    }
}

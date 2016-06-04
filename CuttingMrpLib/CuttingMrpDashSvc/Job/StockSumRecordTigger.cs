using Brilliantech.Framwork.Utils.LogUtil;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CuttingMrpDashSvc.Job
{
    public class StockSumRecordTigger
    {

        private static string groupId = "StockSumRecordGroup";

        public StockSumRecordTigger()
        {

            ICronTrigger trigger = (ICronTrigger)TriggerBuilder.Create()
                                                      .WithIdentity("StockSumRecordGroupTrigger-1", groupId)
                                                      .WithCronSchedule(Properties.Settings.Default.stockSumDashCron)
                                                      .Build();

            IJobDetail job = JobBuilder.Create<StockSumRecordJob>()
                .WithIdentity("StockSumRecordGroupJob-1", groupId)
                .Build();

          //  job.JobDataMap.Add("Date", DateTime.Now.Date);

            DashService.Scheduler.ScheduleJob(job, trigger);

            LogUtil.Logger.Info("StockSumRecordTigger: " + trigger.Key + " added");
        }
    }
}

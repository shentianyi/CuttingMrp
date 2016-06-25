using Brilliantech.Framwork.Utils.LogUtil;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CuttingMrpDashSvc.Job
{
    public class AutoStockTigger
    {

        private static string groupId = "AutoStockGroup";

        public AutoStockTigger()
        {

            ICronTrigger trigger = (ICronTrigger)TriggerBuilder.Create()
                                                      .WithIdentity("AutoStockGroupTrigger-1", groupId)
                                                      .WithCronSchedule(Properties.Settings.Default.autoStockCron)
                                                      .Build();

            IJobDetail job = JobBuilder.Create<AutoStockJob>()
                .WithIdentity("AutoStockTiggerGroupJob-1", groupId)
                .Build();


            DashService.Scheduler.ScheduleJob(job, trigger);

            LogUtil.Logger.Info("AutoStockTrigger: " + trigger.Key + " added");
        }
    }
}

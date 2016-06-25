using Brilliantech.Framwork.Utils.LogUtil;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CuttingMrpDashSvc.Job
{
    public class KskBackFlashTigger
    {

        private static string groupId = "KskBackFlashGroup";

        public KskBackFlashTigger()
        {

            ICronTrigger trigger = (ICronTrigger)TriggerBuilder.Create()
                                                      .WithIdentity("KskBackFlashGroupTrigger-1", groupId)
                                                      .WithCronSchedule(Properties.Settings.Default.backFlashCron)
                                                      .Build();

            IJobDetail job = JobBuilder.Create<KskBackFlashJob>()
                .WithIdentity("KskBackFlashGroupJob-1", groupId)
                .Build();

            DashService.Scheduler.ScheduleJob(job, trigger);

            LogUtil.Logger.Info("KskBackFlashTrigger: " + trigger.Key + " added");
        }
    }
}

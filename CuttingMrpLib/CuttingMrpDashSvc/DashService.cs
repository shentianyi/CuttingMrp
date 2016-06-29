using Brilliantech.Framwork.Utils.LogUtil;
using CuttingMrpDashSvc.Job;
using log4net;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace CuttingMrpDashSvc
{
    public partial class DashService : ServiceBase
    {
        private static IScheduler scheduler;

        public DashService()
        {
            InitializeComponent();
           // Common.Logging.LogManager.Adapter = new Common.Logging.Simple.ConsoleOutLoggerFactoryAdapter { Level = Common.Logging.LogLevel.Info };
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                LogUtil.Logger.Info("CZ CuttingMrpDash Starting...");
                ISchedulerFactory sf = new StdSchedulerFactory();
                Scheduler = sf.GetScheduler();

                new StockSumRecordTigger();
                new KskBackFlashTigger();

                new AutoStockTigger();

                Scheduler.Start();

                LogUtil.Logger.Info("CZ CuttingMrpDash Started");
            }
            catch (Exception ex)
            {
                LogUtil.Logger.Error("CZ CuttingMrpDash Start Fail");
                LogUtil.Logger.Error(ex.Message);
                LogUtil.Logger.Error(ex.StackTrace);
                this.Stop();
            }
        }

        protected override void OnStop()
        {
            try
            {
                if (Scheduler != null) {
                    Scheduler.Shutdown();

                    LogUtil.Logger.Info("CuttingMrpDash Cron Task Stoped.");
                }
                LogUtil.Logger.Info("CZ CuttingMrpDash Stoped!");
            }
            catch (Exception ex)
            {
                LogUtil.Logger.Error("CZ CuttingMrpDash Stop Fail");
                LogUtil.Logger.Error(ex.Message);
                LogUtil.Logger.Error(ex.StackTrace);
            }
        }

        public static IScheduler Scheduler { get { return scheduler; } set { scheduler = value; } }
    }
}

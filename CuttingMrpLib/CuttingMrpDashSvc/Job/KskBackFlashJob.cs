using Brilliantech.Framwork.Utils.LogUtil;
using CuttingMrpLib;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CuttingMrpDashSvc.Properties;

namespace CuttingMrpDashSvc.Job
{
    public class KskBackFlashJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                LogUtil.Logger.Info("start run KskBackFlashJob");
                CalculateSetting setting = new CalculateSetting()
                {
                    TaskType="BF"
                };
                ICalculateService cs = new CalculateService(Settings.Default.db);
                cs.Start(Settings.Default.mrpQueue, setting);

                LogUtil.Logger.Info("end run KskBackFlashJob");
            }
            catch (Exception ex)
            {
                LogUtil.Logger.Error("KskBackFlashJob exec error!");
                LogUtil.Logger.Error(ex.Message);
                LogUtil.Logger.Error(ex.StackTrace);
                
                throw ex;
            }
        }
    }
}

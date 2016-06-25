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
                LogUtil.Logger.Info("start gen KskBackFlashJob");
                CalculateSetting setting = new CalculateSetting()
                {
                    TaskType="BF"
                };
                ICalculateService cs = new CalculateService(Settings.Default.db);
                cs.Start(Settings.Default.mrpQueue, setting);

                LogUtil.Logger.Info("end gen KskBackFlashJob");
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

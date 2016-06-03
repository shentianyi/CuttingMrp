using log4net;
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
        ILog log = LogManager.GetLogger("ServiceLog");
        public DashService()
        {
            InitializeComponent();
            log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4.config")));
        }

        protected override void OnStart(string[] args)
        {
            log.Info("CZ CuttingMrpDash Start!");
        }

        protected override void OnStop()
        {
            log.Info("CZ CuttingMrpDash Stop!");
        }
    }
}

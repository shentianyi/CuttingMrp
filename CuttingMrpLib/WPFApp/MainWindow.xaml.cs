using Brilliantech.Framwork.Utils.LogUtil;
using CuttingMrpLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPFApp.Properties;

namespace WPFApp
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        System.Timers.Timer timer;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            timer = new System.Timers.Timer();
            timer.Interval = Settings.Default.interval;
            timer.Elapsed += Timer_Elapsed;
        timer.Enabled = true;
            timer.Start();
            LogUtil.Logger.Info("Server started.......");
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            timer.Stop();
           
                Calculator mrpExe = new CuttingMrpLib.Calculator(Settings.Default.db);
                MessageQueue qu = new System.Messaging.MessageQueue(Settings.Default.queuepath);
                qu.Formatter = new XmlMessageFormatter(new Type[] { typeof(CalculateSetting) });
                Message msg = qu.Receive();
                if (msg != null)
                {
                    CalculateSetting settings = msg.Body as CalculateSetting;
                    if (settings.TaskType == "AutoStock")
                    {
                        mrpExe.ProcessStockImport(settings);

                    }
                }
           
        }

       
    }
}

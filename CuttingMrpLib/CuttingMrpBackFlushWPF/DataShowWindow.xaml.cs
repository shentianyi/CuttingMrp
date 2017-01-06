using CuttingMrpBackFlushWPF.Controllers;
using CuttingMrpBackFlushWPF.Data;
using CuttingMrpBackFlushWPF.Utils;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CuttingMrpBackFlushWPF
{
    /// <summary>
    /// DataShowWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DataShowWindow : Window
    {
        public DataShowWindow()
        {
            InitializeComponent();
            // set default date
            EndDate.SelectedDate = DateTime.Now.AddDays(1.0);
            EndDate.DisplayDateEnd = DateTime.Now.AddDays(1.0);
            StartDate.SelectedDate = DateTime.Now.AddDays(-6.0);
            StartDate.DisplayDateEnd = DateTime.Now;
        }

        /// <summary>
        /// 搜索按钮的点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Search_Btn_Click(object sender, RoutedEventArgs e)
        {
            // 根据条件查询数据
            DateTime startDate = (DateTime) StartDate.SelectedDate;
            DateTime endDate = (DateTime) EndDate.SelectedDate;

            string productNr = productNrTB.Text;
            string partNr = parNrTB.Text;
            ShowData showData = new ShowData();
            List<ProductFinish> list =  showData.SearchData(startDate,endDate,productNr,partNr);
            ResultLB.Content = list.Count().ToString() + "条";
            ((this.FindName("DataShow")) as DataGrid).ItemsSource = list;
        }

        private void ExportBtn_Click(object sender, RoutedEventArgs e)
        {
            if(DataShow.Items.Count>0)
            {
                ExportExcel.ExportDataGridSaveAs(true, this.DataShow,"倒冲入库记录"+ DateTime.Now.ToLongDateString().ToString());

            }else
            {
                MessageBox.Show("当前没有可导出的数据！");
            }
        }
    }
}

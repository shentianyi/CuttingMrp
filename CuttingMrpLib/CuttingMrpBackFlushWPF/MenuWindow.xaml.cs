using CuttingMrpBackFlushWPF.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CuttingMrpBackFlushWPF
{
    /// <summary>
    /// MenuWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MenuWindow : Window
    {
        public MenuWindow()
        {
            InitializeComponent();
            productNrTB.Focus();
        }

        /// <summary>
        /// 界面启动即全屏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // 设置全屏    
            this.WindowState = System.Windows.WindowState.Normal;
            this.WindowStyle = System.Windows.WindowStyle.None;
            this.ResizeMode = System.Windows.ResizeMode.NoResize;
            this.Topmost = true;

            this.Left = 0.0;
            this.Top = 0.0;
            this.Width = System.Windows.SystemParameters.PrimaryScreenWidth;
            this.Height = System.Windows.SystemParameters.PrimaryScreenHeight;
        }

        /// <summary>
        /// 重置按钮，点击清空输入框内容，焦点聚焦到第一输入框！
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void resetBtn_Click(object sender, RoutedEventArgs e)
        {
            productNrTB.Clear();
            parNrTB.Clear();
            productNrTB.Focus();
        }

        /// <summary>
        /// 确定按钮点击事件，验证是否为空，为空焦点聚焦到为空的输入框！
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void submitBtn_Click(object sender, RoutedEventArgs e)
        {
            Commit();
        }

        /// <summary>
        ///  回车键点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void productNrTB_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if ( e.Key == Key.Enter)
            {
                this.parNrTB.Focus();
            }
        }

        private void parNrTB_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                //this.submitBtn.Focus();
                Commit();
            }
        }

        private void Commit()
        {
            CommitInput commitInput = new CommitInput();
            Controllers.Message msg = new Controllers.Message();
            msg = commitInput.CheckInput(productNrTB.Text, parNrTB.Text);
            if (msg.result)
            {
                //DialogResult dr = System.Windows.Forms.MessageBox.Show("确定要提交吗？", "温馨提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                //if (dr == System.Windows.Forms.DialogResult.OK)
                //{
                msg = commitInput.Commit(productNrTB.Text, parNrTB.Text);
                if (msg.result)
                {
                    productNrTB.Clear();
                    parNrTB.Clear();
                    productNrTB.Focus();
                }
                else
                {
                    parNrTB.Focus();
                }
                System.Windows.MessageBox.Show(msg.message);
                //}
            }
            else
            {
                if (msg.message.Equals("唯一码不能为空！") || msg.message.Equals("唯一码不正确！"))
                {
                    productNrTB.Focus();
                }
                else if (msg.message.Equals("配置号不能为空！"))
                {
                    parNrTB.Focus();
                }
                else if (msg.message.Equals("当前唯一码已存在，请确认是否正确！"))
                {
                    parNrTB.Focus();
                }
                System.Windows.MessageBox.Show(msg.message);
            }
        }

        private void history_Btn_Click(object sender, RoutedEventArgs e)
        {
            DataShowWindow dsw = new DataShowWindow();
            dsw.Show();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
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
    /// LoginWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            emailTB.Focus();
        }
        /// <summary>
        /// 重置按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void resetBtn_Click(object sender, RoutedEventArgs e)
        {
            emailTB.Clear();
            pwdTB.Clear();
            emailTB.Focus();
        }
        /// <summary>
        /// 登录按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loginBtn_Click(object sender, RoutedEventArgs e)
        {
            Commit();
        }

        private void Commit()
        {
            if (string.IsNullOrEmpty(emailTB.Text))
            {
                MessageBox.Show("用户名不能为空！");
                emailTB.Focus();
                return;
            }
            else if (string.IsNullOrEmpty(pwdTB.Password))
            {
                MessageBox.Show("密码不能为空！");
                pwdTB.Focus();
                return;
            }

            if (emailTB.Text.Equals(Properties.Settings.Default.username) && pwdTB.Password.Equals(Properties.Settings.Default.password))
            {
                MenuWindow menu = new MenuWindow();
                menu.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("用户名或密码错误！");
                emailTB.Focus();
            }
        }
        private void emailTB_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.pwdTB.Focus();
            }
        }

        private void pwdTB_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.loginBtn.Focus();
                Commit();
            }
        }
    }
}

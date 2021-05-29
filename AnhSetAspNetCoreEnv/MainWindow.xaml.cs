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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AnhSetAspNetCoreEnv
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var ver = IISEnvHelper.AppVersion();
            Title = Tag.ToString().Replace("{ver}", $"{ver}");
            textBox1.Text = $"ASPNETCORE_ENVIRONMENT={IISEnvHelper.GetEnvValue("ASPNETCORE_ENVIRONMENT")}";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            var text = btn.Content.ToString().Trim();
            var section = text.Split(":")[0];
            var env = text.Split(":")[1];
            var envName =env .Split("=")[0];
            var envValue = env.Split("=")[1];

            try
            {
                IISEnvHelper.SetEnv(envName, envValue);
                MessageBox.Show($"设置为：{envName}={envValue}，设置成功", "设置 IIS 环境变量", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"设置为：{envName}={envValue}，设置失败！\r\n\r\n{ex}", "设置 IIS 环境变量", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            var text = btn.Content.ToString().Trim();
            var envName = text.Split(":")[2];

            try
            {
                IISEnvHelper.DelEnv(envName);
                MessageBox.Show($"删除 {envName} 成功", "设置 IIS 环境变量", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"删除 {envName} 失败！\r\n\r\n{ex}", "设置 IIS 环境变量", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            
        }
    }
}

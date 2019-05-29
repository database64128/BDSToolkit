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
using System.Data;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            /*MainWindow1.Width = SystemParameters.VirtualScreenWidth / 2;
            MainWindow1.Height = SystemParameters.VirtualScreenHeight * 3 / 2;*/
            // Loaded += MainWindow_Loaded;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            MinWidth = Width;
            MinHeight = Height;
        }

        private void Log_input_TextChanged(object sender, TextChangedEventArgs e)
        {
            BDSDataHelper data = new BDSDataHelper(log_input.Text);
            connection_log.ItemsSource = data.GetConnections();
            playtime_records.ItemsSource = data.GetPlayers();
        }
    }
}

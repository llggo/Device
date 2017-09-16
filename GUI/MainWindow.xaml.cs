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
using Library;
using System.IO.Ports;
using System.Management;
using FontAwesome.WPF;
using System.Windows.Media.Animation;
using System.Windows.Markup;

namespace GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Library.SerialPort com;
        public MainWindow()
        {
            InitializeComponent();
        }

        ///<summary>
        ///Initialization function
        ///</summary>
        public void Init()
        {
            #region GUI Init
            MainTabItem.Content = new UC.Main();

            #region Status Bar
            var Singal = new Icon(FontAwesomeIcon.Exchange);

            NotificationPanel.Children.Add(Singal.Get());

            Singal.Blink(0.5);
            Singal.Color(Brushes.Green);

            #endregion

            #endregion





            /// Serial Port
            System.IO.Ports.SerialPort port = new System.IO.Ports.SerialPort("COM2");
            com = new Library.SerialPort(port);
            com.StatusChanged += Com_StatusChanged;
        }

        private void Com_StatusChanged(System.IO.Ports.SerialPort serialPort, Library.SerialPort.Status status)
        {

            //labelStatus.Dispatcher.BeginInvoke((Action)(() => labelStatus.Content = status.ToString()));
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Init();

            using (var searcher = new ManagementObjectSearcher
                ("SELECT * FROM WIN32_SerialPort"))
            {
                string[] portnames = System.IO.Ports.SerialPort.GetPortNames();
                var ports = searcher.Get().Cast<ManagementBaseObject>().ToList();
                var tList = (from n in portnames
                             join p in ports on n equals p["DeviceID"].ToString()
                             select n + " - " + p["Caption"]).ToList();

                foreach (string s in tList)
                {
                    Console.WriteLine(s);
                }
            }
        }

        private void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }
    }
}

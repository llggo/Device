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
using System.Diagnostics;
using Device;
using System.Threading;

namespace GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Hubs.Hubs hubs = new Hubs.Hubs();

        ModBus _modBus = new ModBus();

        Icon _iconSingal = new Icon(FontAwesomeIcon.Exchange);
        Icon _iconUsb = new Icon(FontAwesomeIcon.Usb);

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
            NotificationPanel.Children.Add(_iconSingal.Get());
            NotificationPanel.Children.Add(_iconUsb.Get());

            //Singal.Blink(0.5);
            //Singal.Color(Brushes.Green);

            //Animation.Blink(UsbConnect, 0.1);

            #endregion

            #endregion

            hubs.ComStatusChanged += Hubs_ComStatusChanged;
            hubs.KeyboardStatus += Hubs_KeyboardStatus;
            hubs.Start();
        }

        private void Hubs_KeyboardStatus(Communication.Keyboard.ServerStatus status)
        {
            switch (status)
            {
                case Communication.Keyboard.ServerStatus.Connected:
                    Dispatcher.Invoke(() =>
                    {
                        _iconSingal.Blink(null);
                        _iconSingal.Color(Brushes.Green);
                    });
                    break;

                case Communication.Keyboard.ServerStatus.Connecting:
                    Dispatcher.Invoke(() =>
                    {
                        _iconSingal.Blink(1);
                        _iconSingal.Color(Brushes.Black);
                    });
                    break;

                case Communication.Keyboard.ServerStatus.Disconnected:
                    Dispatcher.Invoke(() =>
                    {
                        _iconSingal.Blink(0.1);
                        _iconSingal.Color(Brushes.Red);
                    });
                    break;
            }
        }

        private void Hubs_ComStatusChanged(System.IO.Ports.SerialPort serialPort, Library.SerialPort.Status status)
        {
            Debug.WriteLine(status);

            switch (status)
            {
                case Library.SerialPort.Status.Opened:
                    Dispatcher.Invoke(() =>
                    {
                        _iconUsb.Blink(null);
                        _iconUsb.Color(Brushes.Green);
                    });

                    break;

                case Library.SerialPort.Status.Opening:
                    Dispatcher.Invoke(() =>
                    {
                        _iconUsb.Blink(1);
                        _iconUsb.Color(Brushes.Black);
                    });

                    break;

                case Library.SerialPort.Status.OpenError:
                    Dispatcher.Invoke(() =>
                    {
                        _iconUsb.Blink(0.1);
                        _iconUsb.Color(Brushes.Red);
                    });

                    break;
            }
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

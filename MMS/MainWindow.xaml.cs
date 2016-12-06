using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace MMS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Contains the names of the serial ports in the computer
        /// </summary>
        public List<string> serialPorts;

        public MainWindow()
        {
            InitializeComponent();
            serialPorts = SerialPort.GetPortNames().ToList();
            List<string> itemsToDelete = new List<string>();
            foreach (var item in serialPorts)
            {
                SerialPort port = new SerialPort(item);
                try
                {
                    port.Open();
                    port.Close();
                }
                catch (Exception)
                {
                    itemsToDelete.Add(item);
                }
            }
            foreach (var item in itemsToDelete)
            {
                serialPorts.Remove(item);
            }
            itemsToDelete = null;
        }

        /// <summary>
        /// Checks that the COM ports selected are not used more than once, and checks if the start button can be enabled
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SerialChoose_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            // Selected values for the clients
            object[] clients = new object[]
            {
                Client1Serial.SelectedValue,
                Client2Serial.SelectedValue,
                Client3Serial.SelectedValue
            };

            // Checks if the COM port used in the clients is not used for the master, slave, or IED
            foreach (object item in clients)
            {
                int x = 0;
                foreach (var thing in clients)
                {
                    if (item == thing)
                    {
                        x++;
                    }
                }

                //if ((selected.Contains(item) || x > 1) && item != null)
                //{
                //    WarningClient.Text = $"Warning, {(string)item} cannot be shared";
                //    WarningClient.Visibility = Visibility.Visible;
                //    break;
                //}
                //else
                //{
                //    WarningClient.Visibility = Visibility.Collapsed;
                //}
            }

            // Checks that there are no errors, and whether or not a connection can be initiated
            if (WarningIED.Visibility == Visibility.Collapsed && WarningClient.Visibility == Visibility.Collapsed)
            {
                if (((bool)LiveInput.IsChecked && SerialChooseIED.SelectedValue != null) || ((bool)GenInput.IsChecked &&
                    ((bool)ZeroValues.IsChecked || (bool)P9_99.IsChecked || (bool)N9_99.IsChecked || (bool)AddressIsValue.IsChecked || (bool)MaxValue.IsChecked || (bool)MinValue.IsChecked)))
                {
                    int length = clients.Count(s => s != null);
                    if ((length == 3 && (bool)ThreeClients.IsChecked) || (length == 2 && (bool)TwoClients.IsChecked) || (length == 1 && (bool)OneClient.IsChecked))
                    {
                        Start.IsEnabled = true;
                    }
                    else
                    {
                        Start.IsEnabled = false;
                    }
                }
                else
                {
                    Start.IsEnabled = false;
                }
            }
            else
            {
                Start.IsEnabled = false;
            }
        }

        /// <summary>
        /// Shows the live input options for the user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LiveInput_Checked(object sender, RoutedEventArgs e)
        {
            GenInputSettings.Visibility = Visibility.Collapsed;
            LiveInputSettings.Visibility = Visibility.Visible;
            SerialChooseIED.ItemsSource = serialPorts;
        }

        /// <summary>
        /// Shows the generated input options for the user, and removes user choice for the IED port
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GenInput_Checked(object sender, RoutedEventArgs e)
        {
            LiveInputSettings.Visibility = Visibility.Collapsed;
            GenInputSettings.Visibility = Visibility.Visible;
            SerialChooseIED.SelectedItem = null;
        }

        private void OneClient_Checked(object sender, RoutedEventArgs e)
        {
            clientTabs.Visibility = Visibility.Visible;
            Client1Serial.ItemsSource = serialPorts;
            Client1.Visibility = Visibility.Visible;
            Client2.Visibility = Visibility.Collapsed;
            Client3.Visibility = Visibility.Collapsed;
        }

        private void TwoClients_Checked(object sender, RoutedEventArgs e)
        {
            clientTabs.Visibility = Visibility.Visible;
            Client1Serial.ItemsSource = serialPorts;
            Client2Serial.ItemsSource = serialPorts;
            Client1.Visibility = Visibility.Visible;
            Client2.Visibility = Visibility.Visible;
            Client3.Visibility = Visibility.Collapsed;
        }

        private void ThreeClients_Checked(object sender, RoutedEventArgs e)
        {
            clientTabs.Visibility = Visibility.Visible;
            Client1Serial.ItemsSource = serialPorts;
            Client2Serial.ItemsSource = serialPorts;
            Client1.Visibility = Visibility.Visible;
            Client3Serial.ItemsSource = serialPorts;
            Client2.Visibility = Visibility.Visible;
            Client3.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Shows the user the refresh options
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefreshBool_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)RefreshBool.IsChecked)
            {
                RefreshStackPanel.Visibility = Visibility.Visible;
            }
            else
            {
                RefreshStackPanel.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Starts the test
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Start_Click(object sender, RoutedEventArgs e)
        {
            // Hosted Master Input port
            HostedMB.HMIPort = new SerialPort()
            {
                BaudRate = Convert.ToInt32(BaudRate.Text),
                DataBits = Convert.ToInt32(DataBits.Text),
            };
            switch (ParityComboBox.Text)
            {
                case "None":
                    HostedMB.HMIPort.Parity = Parity.None;
                    break;
                case "Odd":
                    HostedMB.HMIPort.Parity = Parity.Odd;
                    break;
                case "Even":
                    HostedMB.HMIPort.Parity = Parity.Even;
                    break;
            }
            switch (StopBitsComboBox.Text)
            {
                case "1":
                    HostedMB.HMIPort.StopBits = StopBits.One;
                    break;
                case "2":
                    HostedMB.HMIPort.StopBits = StopBits.Two;
                    break;
            }


            if ((bool)LiveInput.IsChecked)
            {
                HostedMB.HMIPort.PortName = (string)SerialChooseIED.SelectedItem;
                HostedMB.TimeOutValue = Convert.ToInt32(Timeout.Text);

                // Hosted Serial Port settings
                HostedMB.HSPort = new SerialPort()
                {
                    PortName = (string)Client1Serial.SelectedValue,
                    BaudRate = Convert.ToInt32(Client1BaudRate.Text),
                    DataBits = Convert.ToInt32(Client1DataBits.Text)
                };
                switch (Client1ParityComboBox.Text)
                {
                    case "None":
                        HostedMB.HMIPort.Parity = Parity.None;
                        break;
                    case "Odd":
                        HostedMB.HMIPort.Parity = Parity.Odd;
                        break;
                    case "Even":
                        HostedMB.HMIPort.Parity = Parity.Even;
                        break;
                }
                switch (Client1StopBitsComboBox.Text)
                {
                    case "1":
                        HostedMB.HMIPort.StopBits = StopBits.One;
                        break;
                    case "2":
                        HostedMB.HMIPort.StopBits = StopBits.Two;
                        break;
                }

                new Thread(HostedMB.Live).Start(); // Starts static class in new thread
            }
            else
            {

            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
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
            SerialChooseMaster.ItemsSource = serialPorts;
            SerialChooseSlave.ItemsSource = serialPorts;
        }

        /// <summary>
        /// Checks that the COM ports selected are not used more than once, and checks if the start button can be enabled
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SerialChoose_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Selected values for master, slave, and IED
            object[] selected = new object[] 
            {
                SerialChooseSlave.SelectedValue,
                SerialChooseMaster.SelectedValue,
                SerialChooseIED.SelectedValue
            };

            // Selected values for the clients
            object[] clients = new object[]
            {
                Client1Serial.SelectedValue,
                Client2Serial.SelectedValue,
                Client3Serial.SelectedValue
            };

            // Checks if the master and the slave have different COM ports, and that they're not null
            if (SerialChooseMaster.SelectedValue == SerialChooseSlave.SelectedValue && SerialChooseMaster.SelectedValue != null)
            {
                WarningMasterSlave.Visibility = Visibility.Visible;
            }
            else
            {
                WarningMasterSlave.Visibility = Visibility.Collapsed;
            }

            // Checks if the master and the slave have different COM ports to the IED
            if ((SerialChooseIED.SelectedValue == SerialChooseMaster.SelectedValue || SerialChooseIED.SelectedValue == SerialChooseSlave.SelectedValue) && SerialChooseIED.SelectedValue != null)
            {
                WarningIED.Visibility = Visibility.Visible;
            }
            else
            {
                WarningIED.Visibility = Visibility.Collapsed;
            }

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

                if ((selected.Contains(item) || x > 1) && item != null)
                {
                    WarningClient.Text = $"Warning, {(string)item} cannot be shared";
                    WarningClient.Visibility = Visibility.Visible;
                    break;
                }
                else
                {
                    WarningClient.Visibility = Visibility.Collapsed;
                }
            }

            // Checks that there are no errors, and whether or not a connection can be initiated
            if (WarningMasterSlave.Visibility == Visibility.Collapsed && WarningIED.Visibility == Visibility.Collapsed && WarningClient.Visibility == Visibility.Collapsed)
            {
                if (selected[0] != null && selected[1] != null)
                {
                    if (((bool)LiveInput.IsChecked && selected[2] != null) || ((bool)GenInput.IsChecked && 
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
            else
            {
                Start.IsEnabled = false;
            }
        }

        /// <summary>
        /// Redirects the user to the com0com download link
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
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
    }
}

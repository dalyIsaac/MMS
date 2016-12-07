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
            GeneratedItemsCheckbox = new CheckBox[] { ZeroValues, P9_99, N9_99, AddressIsValue, MaxValue, MinValue };
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
        /// Checks if the start button can be enabled
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PIMConnected_Checked(object sender, RoutedEventArgs e)
        {
            SerialChoose_SelectionChanged();
        }

        /// <summary>
        /// Checks that the COM ports selected are not used more than once, and checks if the start button can be enabled
        /// </summary>
        private void SerialChoose_SelectionChanged()
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

                if ((SerialChooseIED.SelectedValue == item || x > 1) && item != null)
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
            if (WarningIED.Visibility == Visibility.Collapsed && WarningClient.Visibility == Visibility.Collapsed)
            {
                if ((bool)PIMConnected.IsChecked)
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
            else
            {
                Start.IsEnabled = false;
            }
        }

        /// <summary>
        /// Checks that the COM ports selected are not used more than once, and checks if the start button can be enabled
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SerialChoose_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SerialChoose_SelectionChanged();
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

        /// <summary>
        /// Shows only a single client for the user to set
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OneClient_Checked(object sender, RoutedEventArgs e)
        {
            clientTabs.Visibility = Visibility.Visible;
            Client1Serial.ItemsSource = serialPorts;
            Client1.Visibility = Visibility.Visible;
            Client2.Visibility = Visibility.Collapsed;
            Client3.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Shows two clients for the user to set
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TwoClients_Checked(object sender, RoutedEventArgs e)
        {
            clientTabs.Visibility = Visibility.Visible;
            Client1Serial.ItemsSource = serialPorts;
            Client2Serial.ItemsSource = serialPorts;
            Client1.Visibility = Visibility.Visible;
            Client2.Visibility = Visibility.Visible;
            Client3.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Shows three clients for the user to set
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// Contains a list of all the checkbox items
        /// </summary>
        private CheckBox[] GeneratedItemsCheckbox;

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
                if (!(bool)RefreshBool.IsChecked)
                {
                    foreach (var item in GeneratedItemsCheckbox)
                    {
                        item.IsChecked = false;
                    }
                }
            }
        }

        /// <summary>
        /// Ensures that only one checkbox can be checked if the input is not refreshing, for generated settings
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Checkbox_Checked(object sender, RoutedEventArgs e)
        {
            if (!(bool)RefreshBool.IsChecked)
            {
                var checkedItem = new CheckBox();
                foreach (var item in GeneratedItemsCheckbox)
                {
                    if ((bool)item.IsChecked)
                    {
                        checkedItem = item;
                    }
                    else
                    {
                        item.IsEnabled = false;
                    }
                }
            }
        }

        /// <summary>
        /// Enables all the checkboxes if the checked one is unchecked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Checkbox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!(bool)RefreshBool.IsChecked)
            {
                foreach (var item in GeneratedItemsCheckbox)
                {
                    item.IsEnabled = true;
                }
            }
        }

        /// <summary>
        /// Starts or kills the test
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Start_Click(object sender, RoutedEventArgs e)
        {
            if ((string)Start.Content == "Start")
            {
                Start.Content = "Stop";
                Start.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ff5252"));

                #region Client 1 settings
                HostedMB.Client1Port = new SerialPort()
                {
                    PortName = (string)Client1Serial.SelectedValue,
                    BaudRate = Convert.ToInt32(Client1BaudRate.Text),
                    DataBits = Convert.ToInt32(Client1DataBits.Text)
                };
                switch (Client1ParityComboBox.Text)
                {
                    case "None":
                        HostedMB.Client1Port.Parity = Parity.None;
                        break;
                    case "Odd":
                        HostedMB.Client1Port.Parity = Parity.Odd;
                        break;
                    case "Even":
                        HostedMB.Client1Port.Parity = Parity.Even;
                        break;
                }
                switch (Client1StopBitsComboBox.Text)
                {
                    case "1":
                        HostedMB.Client1Port.StopBits = StopBits.One;
                        break;
                    case "2":
                        HostedMB.Client1Port.StopBits = StopBits.Two;
                        break;
                }
                #endregion

                // Specifies the creation of slaves for clients
                // All the slaves are created on the same thread
                if ((bool)OneClient.IsChecked)
                {
                    new Thread(HostedMB.CreateHostedSlave1).Start();
                }
                else if ((bool)TwoClients.IsChecked)
                {
                    Client2Settings();
                    new Thread(HostedMB.CreateHostedSlave1And2).Start();
                }
                else
                {
                    Client2Settings();
                    #region Client 3 settings
                    HostedMB.Client3Port = new SerialPort()
                    {
                        PortName = (string)Client3Serial.SelectedValue,
                        BaudRate = Convert.ToInt32(Client3BaudRate.Text),
                        DataBits = Convert.ToInt32(Client3DataBits.Text)
                    };
                    switch (Client3ParityComboBox.Text)
                    {
                        case "None":
                            HostedMB.Client3Port.Parity = Parity.None;
                            break;
                        case "Odd":
                            HostedMB.Client3Port.Parity = Parity.Odd;
                            break;
                        case "Even":
                            HostedMB.Client3Port.Parity = Parity.Even;
                            break;
                    }
                    switch (Client3StopBitsComboBox.Text)
                    {
                        case "1":
                            HostedMB.Client3Port.StopBits = StopBits.One;
                            break;
                        case "2":
                            HostedMB.Client3Port.StopBits = StopBits.Two;
                            break;
                    }
                    #endregion
                    new Thread(HostedMB.CreateHostedSlave1And2And3).Start();
                }

                HostedMB.RefreshData = (bool)RefreshBool.IsChecked;
                if (HostedMB.RefreshData)
                {
                    HostedMB.RefreshFreq = Convert.ToInt32(RefreshRate.Text);
                }

                #region Live vs. Generated
                if ((bool)LiveInput.IsChecked)
                {
                    #region Hosted Master Port settings
                    HostedMB.HostedMasterPort = new SerialPort()
                    {
                        PortName = (string)SerialChooseIED.SelectedItem,
                        BaudRate = Convert.ToInt32(BaudRate.Text),
                        DataBits = Convert.ToInt32(DataBits.Text),
                    };
                    switch (ParityComboBox.Text)
                    {
                        case "None":
                            HostedMB.HostedMasterPort.Parity = Parity.None;
                            break;
                        case "Odd":
                            HostedMB.HostedMasterPort.Parity = Parity.Odd;
                            break;
                        case "Even":
                            HostedMB.HostedMasterPort.Parity = Parity.Even;
                            break;
                    }
                    switch (StopBitsComboBox.Text)
                    {
                        case "1":
                            HostedMB.HostedMasterPort.StopBits = StopBits.One;
                            break;
                        case "2":
                            HostedMB.HostedMasterPort.StopBits = StopBits.Two;
                            break;
                    }
                    #endregion
                    HostedMB.TimeOutValue = Convert.ToInt32(Timeout.Text);

                    new Thread(HostedMB.Live).Start(); // Starts static class in new thread
                }
                else
                {
                    if (!HostedMB.RefreshData)
                    {
                        foreach (var item in GeneratedItemsCheckbox)
                        {

                        }
                    }
                }
                #endregion
            }
            else
            {
                KillThreads();
                Start.Content = "Start";
                Start.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#33cc57"));
            }
        }

        private (float outputValue, bool AddressIsValueBool) GenInputValue(CheckBox checkbox)
        {
            switch (checkbox.Name)
            {
                case "+9.99":
                    return (9.99F, false);
                case "-9.99":
                    return (-9.99F, false);
                case "AddressIsValue":
                    return (0, true);
                case "MaxValue":
                    return (170141163178059628080016879768630000000F, false);
                case "MinValue":
                    return (-170141163178059628080016879768630000000F, false);
                case "0":
                default:
                    return (0, false);
            }
        }

        /// <summary>
        /// Kills currently running background processes for clients
        /// </summary>
        public static void KillThreads()
        {
            if (HostedMB.HostedMasterPort != null)
            {
                HostedMB.HostedMasterPort.Close();
            }
            if (HostedMB.Client1Port != null)
            {
                HostedMB.Client1Port.Close();
            }
            if (HostedMB.Client2Port != null)
            {
                HostedMB.Client2Port.Close();
            }
            if (HostedMB.Client3Port != null)
            {
                HostedMB.Client3Port.Close();
            }
        }

        /// <summary>
        /// Sets the settings for client 2
        /// </summary>
        private void Client2Settings()
        {
            HostedMB.Client2Port = new SerialPort()
            {
                PortName = (string)Client2Serial.SelectedValue,
                BaudRate = Convert.ToInt32(Client2BaudRate.Text),
                DataBits = Convert.ToInt32(Client2DataBits.Text)
            };
            switch (Client2ParityComboBox.Text)
            {
                case "None":
                    HostedMB.Client2Port.Parity = Parity.None;
                    break;
                case "Odd":
                    HostedMB.Client2Port.Parity = Parity.Odd;
                    break;
                case "Even":
                    HostedMB.Client2Port.Parity = Parity.Even;
                    break;
            }
            switch (Client2StopBitsComboBox.Text)
            {
                case "1":
                    HostedMB.Client2Port.StopBits = StopBits.One;
                    break;
                case "2":
                    HostedMB.Client2Port.StopBits = StopBits.Two;
                    break;
            }
        }

        /// <summary>
        /// Checks that the refresh rate is available
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefreshRate_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                int test = Convert.ToInt32(RefreshRate.Text);
                if (test <= 0)
                {
                    WarningRefreshRate.Visibility = Visibility.Visible;
                }
                else
                {
                    WarningRefreshRate.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception)
            {
                if (WarningRefreshRate != null)
                {
                    WarningRefreshRate.Visibility = Visibility.Visible;
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            KillThreads();
        }

        /// <summary>
        /// Shows the user the com0com settings or downloads it for them in a browser
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
                e.Handled = true;
            }
            catch (Exception)
            {
                Process.Start(new ProcessStartInfo("https://storage.googleapis.com/google-code-archive-downloads/v2/code.google.com/powersdr-iq/setup_com0com_W7_x64_signed.exe"));
                e.Handled = true;
            }
        }


    }
}

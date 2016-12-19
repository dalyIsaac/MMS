﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;

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

        /// <summary>
        /// Window constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            GeneratedItemsCheckbox = new CheckBox[] { ZeroValues, P9_99, N9_99, AddressIsValue, MaxValue, MinValue }; // Loads the GeneratedItems checkboxes into a list
            TextBoxArray = new TextBox[] { BaudRate, DataBits, Timeout, RefreshRate, Client1BaudRate, Client1DataBits, Client2BaudRate, Client2DataBits, Client3BaudRate, Client3DataBits, Hours, Minutes };
            CheckSerial();
        }

        /// <summary>
        /// Checks which serial ports can be open
        /// </summary>
        private void CheckSerial()
        {
            serialPorts = SerialPort.GetPortNames().ToList();
            List<string> itemsToDelete = new List<string>();
            foreach (var item in serialPorts) // Deletes ports which cannot be used by the application
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
            if (WarningIED.Visibility == Visibility.Collapsed && WarningClient.Visibility == Visibility.Collapsed && WarningTextBlock.Visibility == Visibility.Collapsed)
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
            SerialChoose_SelectionChanged();
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
            SerialChoose_SelectionChanged();
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
            clientTabs.SelectedIndex = 0;
            Client1Serial.ItemsSource = serialPorts;
            Client2Serial.SelectedItem = null;
            Client3Serial.SelectedItem = null;
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
            clientTabs.SelectedIndex = 1;
            Client1Serial.ItemsSource = serialPorts;
            Client2Serial.ItemsSource = serialPorts;
            Client3Serial.SelectedItem = null;
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
            clientTabs.SelectedIndex = 2;
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
            foreach (var item in GeneratedItemsCheckbox)
            {
                item.IsEnabled = true;
            }
            RefreshStackPanel.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Shows the user the refresh options
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefreshBool_Unchecked(object sender, RoutedEventArgs e)
        {
            RefreshStackPanel.Visibility = Visibility.Collapsed;
            if (GeneratedItemsCheckbox.Count(x => (bool)x.IsChecked) != 1)
            {
                foreach (var item in GeneratedItemsCheckbox)
                {
                    item.IsChecked = false;
                }
            }
            else
            {
                foreach (var item in GeneratedItemsCheckbox)
                {
                    if (!(bool)item.IsChecked)
                    {
                        item.IsEnabled = false;
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
            SerialChoose_SelectionChanged();
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
            SerialChoose_SelectionChanged();
        }

        /// <summary>
        /// Notifies the user that the currently selected COM port is not available
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private bool TestSerialPort(string name)
        {
            SerialPort port = new SerialPort(name);
            try
            {
                port.Open();
                port.Close();
                return true;
            }
            catch (Exception)
            {
                string text = $"{name} is currently in use. Please select another COM port";
                string caption = "COM port not available";
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBoxImage icon = MessageBoxImage.Warning;
                MessageBox.Show(text, caption, button, icon);
                CheckSerial();
                EnableDisableSettings();
                SerialChooseIED.ItemsSource = serialPorts;
                Client1Serial.ItemsSource = serialPorts;
                Client2Serial.ItemsSource = serialPorts;
                Client3Serial.ItemsSource = serialPorts;
                return false;
            }
        }

        /// <summary>
        /// Contains the time of the start of the test
        /// </summary>
        public static DateTime EndTime = new DateTime();

        private static void CheckTime()
        {
            while (true)
            {
                if (DateTime.Now >= EndTime)
                {
                    GenContinue = false;
                    return;
                }
            }
        }

        public static void SetTime()
        {
            EndTime = DateTime.Now;
            EndTime.AddHours(HoursToAdd);
            EndTime.AddMinutes(MinsToAdd);
            new Thread(CheckTime).Start();
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
                EnableDisableSettings();
                #region Client 1 settings

                // Checks if the COM port selected is being used
                if (!TestSerialPort((string)Client1Serial.SelectedValue))
                {
                }
                else
                {
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
                        if (!TestSerialPort((string)Client2Serial.SelectedValue))
                        {
                            return;
                        }
                        Client2Settings();
                        new Thread(HostedMB.CreateHostedSlave1And2).Start();
                    }
                    else
                    {
                        if (!TestSerialPort((string)Client2Serial.SelectedValue))
                        {
                            return;
                        }
                        Client2Settings();
                        if (!TestSerialPort((string)Client3Serial.SelectedValue))
                        {
                            return;
                        }
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

                    HostedMB.HostedSlave1RequestNum = 0;
                    HostedMB.HostedSlave2RequestNum = 0;
                    HostedMB.HostedSlave3RequestNum = 0;

                    // Ensures that data input does not begin before the data store is created
                    while (!HostedMB.CreateHostedSlave1Status)
                    {

                    }

                    HostedMB.RefreshData = (bool)RefreshBool.IsChecked;
                    if (HostedMB.RefreshData)
                    {
                        HostedMB.RefreshFreq = Convert.ToInt32(RefreshRate.Text);
                    }

                    Start.Content = "Stop";
                    Start.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ff5252"));

                    #region Live vs. Generated
                    if ((bool)LiveInput.IsChecked)
                    {
                        if (!TestSerialPort((string)SerialChooseIED.SelectedValue))
                        {
                            return;
                        }
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
                        List<string> GenItemsString = new List<string>();
                        GenContinue = true;
                        foreach (var item in GeneratedItemsCheckbox)
                        {
                            if ((bool)item.IsChecked)
                            {
                                GenItemsString.Add(item.Name);
                            }
                        }
                        new Thread(() => HostedMB.Generated(GenItemsString)).Start();
                    }
                    #endregion
                }
            }
            else
            {
                EnableDisableSettings();
                KillThreads();
                string text;
                string caption;
                MessageBoxButton button;
                MessageBoxImage icon;

                if ((bool)OneClient.IsChecked)
                {
                    text = $"Client 1 ({HostedMB.Client1Port.PortName}): {HostedMB.HostedSlave1RequestNum} requests";
                }
                else if ((bool)TwoClients.IsChecked)
                {
                    text = $"Client 1 ({HostedMB.Client1Port.PortName}): {HostedMB.HostedSlave1RequestNum} requests\nClient 2 ({HostedMB.Client2Port.PortName}): {HostedMB.HostedSlave2RequestNum} requests";
                }
                else
                {
                    text = $"Client 1 ({HostedMB.Client1Port.PortName}): {HostedMB.HostedSlave1RequestNum} requests\nClient 2 ({HostedMB.Client2Port.PortName}): {HostedMB.HostedSlave2RequestNum} requests\nClient 3 ({HostedMB.Client3Port.PortName}): {HostedMB.HostedSlave3RequestNum} requests";
                }
                caption = "Number of client requests received";
                button = MessageBoxButton.OK;
                icon = MessageBoxImage.Information;
                MessageBox.Show(text, caption, button, icon);
                GenContinue = false;
                Start.Content = "Start";
                Start.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#33cc57"));
            }
        }

        /// <summary>
        /// Disables the settings if the MMS is running - enables them if it is not
        /// </summary>
        private void EnableDisableSettings()
        {
            InputSettingsRow1.IsEnabled = !InputSettingsRow1.IsEnabled;
            InputSettingsRow2.IsEnabled = !InputSettingsRow2.IsEnabled;
            RefreshSettings.IsEnabled = !RefreshSettings.IsEnabled;
            ClientSettingsRow1.IsEnabled = !ClientSettingsRow1.IsEnabled;
            ClientSettingsRow2.IsEnabled = !ClientSettingsRow2.IsEnabled;
            PIMConnected.IsEnabled = !PIMConnected.IsEnabled;
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
        /// Contains all the textboxes in the program
        /// </summary>
        private TextBox[] TextBoxArray;

        private bool TextBoxCheck(TextBox item)
        {
            try
            {
                if (item.Name == "Hours" || item.Name == "Minutes")
                {
                    TextBox[] testArray = new TextBox[] { Hours, Minutes };
                    if (testArray[0].Text == "" && testArray[1].Text == "")
                    {
                        return true;
                    }
                    List<int> testArrayValues = new List<int>();
                    foreach (var thing in testArray)
                    {
                        try
                        {
                            testArrayValues.Add(Convert.ToInt32(thing.Text));
                        }
                        catch (Exception)
                        {
                            testArrayValues.Add(-1);
                        }
                    }
                    if (testArrayValues[0] <= 0 && testArrayValues[1] <= 0)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }

                int test = Convert.ToInt32(item.Text);
                if (test <= 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }

            }
            catch (Exception)
            {
                if (item.Text != "")
                {
                    return false;
                }
                return true;
            }
        }
        /// <summary>
        /// Checks that the refresh rate is available
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            List<bool> StatusBool = new List<bool>(); // True means pass
            if (TextBoxArray != null)
            {
                foreach (var item in TextBoxArray)
                {
                    StatusBool.Add(TextBoxCheck(item));
                    try
                    {
                        SerialChoose_SelectionChanged();
                    }
                    catch (Exception)
                    {
                    }
                }
                if (StatusBool.Contains(false))
                {
                    WarningTextBlock.Visibility = Visibility.Visible;
                }
                else
                {
                    WarningTextBlock.Visibility = Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// True: Continue. False: Stop current process
        /// </summary>
        public static bool GenContinue = true;

        /// <summary>
        /// Kills all the threads and the program
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            KillThreads();
            GenContinue = false;
            Application.Current.Shutdown();
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

        private void SelectFileNameLocation_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// True: Logging is on. False: Logging if off
        /// </summary>
        public static bool LogBoolChecked;

        /// <summary>
        /// Shows LoggerInfo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LogBool_Checked(object sender, RoutedEventArgs e)
        {
            LogBoolChecked = true;
            LoggerInfo.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Hides LoggerInfo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LogBool_Unchecked(object sender, RoutedEventArgs e)
        {
            LogBoolChecked = false;
            LoggerInfo.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Hours to add to the current time
        /// </summary>
        private static int HoursToAdd;

        /// <summary>
        /// Minutes to add to the current time
        /// </summary>
        private static int MinsToAdd;

        /// <summary>
        /// Checks if SelectFileNameLocation can be enabled
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Time_TextChanged(object sender, TextChangedEventArgs e)
        {
            List<bool> StatusBoolList = new List<bool>();
            if (Hours != null && Minutes != null)
            {
                TextBox[] TextBoxArray = new TextBox[] { Hours, Minutes };

                foreach (var item in TextBoxArray)
                {
                    if (item.Text == "")
                    {
                        StatusBoolList.Add(false);
                    }
                    else
                    {
                        StatusBoolList.Add(TextBoxCheck(item));
                    }
                }
                if (StatusBoolList.Contains(false))
                {
                    SelectFileNameLocation.IsEnabled = false;
                }
                else
                {
                    SelectFileNameLocation.IsEnabled = true;
                    HoursToAdd = Convert.ToInt32(Hours.Text);
                    MinsToAdd = Convert.ToInt32(Minutes.Text);
                }
                TextBox_TextChanged(sender, e);
            }
        }
    }
}

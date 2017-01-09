﻿using Modbus.Device;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MMS
{
    public static class HostedMB
    {
        /// <summary>
        /// References CreateHostedSlave1 and CreateHostedSlave2
        /// </summary>
        public static void CreateHostedSlave1And2()
        {
            CreateHostedSlave1();
            CreateHostedSlave2();
        }

        /// <summary>
        /// References CreateHostedSlave1, CreateHostedSlave2, and CreateHostedSlave3
        /// </summary>
        public static void CreateHostedSlave1And2And3()
        {
            CreateHostedSlave1();
            CreateHostedSlave2();
            CreateHostedSlave3();
        }

        public static bool[] SlavesCreated;

        #region Hosted Slave 1
        /// <summary>
        /// Hosted Slave 1
        /// </summary>
        private static ModbusSerialSlave HostedSlave1;

        /// <summary>
        /// Creates Hosted Slave 1
        /// </summary>
        public static void CreateHostedSlave1()
        {
            Client1Port.Open();
            HostedSlave1 = ModbusSerialSlave.CreateRtu(1, Client1Port);
            HostedSlave1.DataStore = Modbus.Data.DataStoreFactory.CreateDefaultDataStore(0, 0, 0, 65535);
            HostedSlave1.ModbusSlaveRequestReceived += new EventHandler<ModbusSlaveRequestEventArgs>(HostedSlave1_Request_Event);
            Task Listen1 = new Task(HostedSlave1.Listen);
            Listen1.Start();
            SlavesCreated[0] = true;
        }

        /// <summary>
        /// Event for incrementing HostedSlave1RequestNum
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void HostedSlave1_Request_Event(object sender, ModbusSlaveRequestEventArgs e)
        {
            HostedSlave1RequestNum++;
        }

        /// <summary>
        /// Stores the number of requests that HostedSlave1 has received
        /// </summary>
        public static ulong HostedSlave1RequestNum;
        #endregion

        #region Hosted Slave 2
        /// <summary>
        /// Hosted Slave 2
        /// </summary>
        private static ModbusSerialSlave HostedSlave2;

        /// <summary>
        /// Creates hosted slave 2
        /// </summary>
        private static void CreateHostedSlave2()
        {
            Client2Port.Open();
            HostedSlave2 = ModbusSerialSlave.CreateRtu(1, Client2Port);
            HostedSlave2.DataStore = HostedSlave1.DataStore;
            HostedSlave2.ModbusSlaveRequestReceived += new EventHandler<ModbusSlaveRequestEventArgs>(HostedSlave2_Request_Event);
            Task Listen2 = new Task(HostedSlave2.Listen);
            Listen2.Start();
            SlavesCreated[1] = true;
        }

        /// <summary>
        /// Event for incrementing HostedSlave2RequestNum
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void HostedSlave2_Request_Event(object sender, ModbusSlaveRequestEventArgs e)
        {
            HostedSlave2RequestNum++;
        }

        /// <summary>
        /// Stores the number of requests that HostedSlave2 has received
        /// </summary>
        public static ulong HostedSlave2RequestNum;
        #endregion

        #region Hosted Slave 3
        /// <summary>
        /// Hosted Slave 3
        /// </summary>
        private static ModbusSerialSlave HostedSlave3;

        /// <summary>
        /// Creates hosted slave 3
        /// </summary>
        private static void CreateHostedSlave3()
        {
            Client3Port.Open();
            HostedSlave3 = ModbusSerialSlave.CreateRtu(1, Client3Port);
            HostedSlave3.DataStore = HostedSlave1.DataStore;
            HostedSlave3.ModbusSlaveRequestReceived += new EventHandler<ModbusSlaveRequestEventArgs>(HostedSlave3_Request_Event);
            Task Listen3 = new Task(HostedSlave3.Listen);
            Listen3.Start();
            SlavesCreated[2] = true;
        }

        /// <summary>
        /// Event for incrementing HostedSlave3RequestNum
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void HostedSlave3_Request_Event(object sender, ModbusSlaveRequestEventArgs e)
        {
            HostedSlave3RequestNum++;
        }

        /// <summary>
        /// Stores the number of requests that HostedSlave3 has received
        /// </summary>
        public static ulong HostedSlave3RequestNum;
        #endregion

        #region Clients
        /// <summary>
        /// Client 1 port
        /// </summary>
        public static SerialPort Client1Port;

        /// <summary>
        /// Client 2 port
        /// </summary>
        public static SerialPort Client2Port;

        /// <summary>
        /// Client 3 port
        /// </summary>
        public static SerialPort Client3Port;
        #endregion

        /// <summary>
        /// Hosted Master Input port
        /// </summary>
        public static SerialPort HostedMasterPort;

        #region Live
        /// <summary>
        /// Contains the user's settings on the frequency of the data input refreshing
        /// </summary>
        public static int RefreshFreq;

        /// <summary>
        /// Timeout value for the hosted master receiving data
        /// </summary>
        public static int TimeOutValue;

        /// <summary>
        /// Executes with live input
        /// </summary>
        public static void Live(CancellationToken ct)
        {
            HostedMasterPort.Open();
            IModbusSerialMaster master = ModbusSerialMaster.CreateRtu(HostedMasterPort);
            master.Transport.ReadTimeout = TimeOutValue;
            while (!ct.IsCancellationRequested)
            {
                ScanIn(master);
                Thread.Sleep(RefreshFreq);
            }
            ClosePorts();
            // Write CSV here
        }


        /// <summary>
        /// Scans in data from the IED and writes it
        /// </summary>
        /// <param name="master"></param>
        /// <returns></returns>
        private static void ScanIn(IModbusMaster master)
        {
            for (int i = 0; i < 272; i += 2)
            {
                ushort[] items = master.ReadInputRegisters(1, (ushort)i, 2);
                //float itemsFloat = BinaryToFloat(items); // Checks that the data is correct
                WriteValues(items, i);
            }
        }
        #endregion

        #region Generated
        /// <summary>
        /// Generated values to be written to the DataStore
        /// </summary>
        public static void Generated(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                foreach (var item in MainWindow.GenItemsString)
                {
                    switch (item)
                    {
                        case "ZeroValues":
                            for (int i = 0; i < 272; i += 2)
                            {
                                WriteValues(new ushort[] { 0, 0 }, i);
                            }
                            break;
                        case "P9_99":
                            for (int i = 0; i < 272; i += 2)
                            {
                                WriteValues(FloatToBinary(9.99F), i);
                            }
                            break;
                        case "N9_99":
                            for (int i = 0; i < 272; i += 2)
                            {
                                WriteValues(FloatToBinary(-9.99F), i);
                            }
                            break;
                        case "AddressIsValue":
                            for (int i = 0; i < 272; i += 2)
                            {
                                WriteValues(FloatToBinary(30001 + i), i);
                            }
                            break;
                        case "MaxValue":
                            for (int i = 0; i < 272; i += 2)
                            {
                                WriteValues(FloatToBinary(170141163178059628080016879768630000000F), i);
                            }
                            break;
                        case "MinValue":
                            for (int i = 0; i < 272; i += 2)
                            {
                                WriteValues(FloatToBinary(-170141163178059628080016879768630000000F), i);
                            }
                            break;
                    }
                        Thread.Sleep(RefreshFreq);
                }
            }
            // Write CSV here
            ClosePorts();
        }

        /// <summary>
        /// Converts float to ushort values for input into a register
        /// Code source: https://www.codeproject.com/Questions/484209/Convertplusfloatplustoplusbinary
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static ushort[] FloatToBinary(float value)
        {
            bool negative = false;
            if (value < 0)
            {
                negative = true;
            }

            int bitCount = sizeof(float) * 8; // never rely on your knowledge of the size
            char[] result = new char[bitCount]; // better not use string, to avoid ineffective string concatenation repeated in a loop

            // now, most important thing: (int)value would be "semantic" cast of the same
            // mathematical value (with possible rounding), something we don't want; so:
            int intValue = System.BitConverter.ToInt32(BitConverter.GetBytes(value), 0);

            for (int bit = 0; bit < bitCount; ++bit)
            {
                int maskedValue = intValue & (1 << bit); // this is how shift and mask is done.
                if (maskedValue > 0)
                    maskedValue = 1;
                // at this point, masked value is either int 0 or 1
                result[bitCount - bit - 1] = maskedValue.ToString()[0]; // bits go right-to-left in usual Western Arabic-based notation
            }

            string stringResult = new string(result);

            // Changes the binary value to a negative number (according to IEEE 754)
            if (negative)
            {
                stringResult = '1' + stringResult.Substring(1);
            }

            string firstHalf = stringResult.Substring(0, (stringResult.Length / 2));
            string secondHalf = stringResult.Substring((stringResult.Length / 2), (stringResult.Length / 2));

            return new ushort[] { Convert.ToUInt16(firstHalf, 2), Convert.ToUInt16(secondHalf, 2) };
        }
        #endregion

        /// <summary>
        /// Writes values to the DataStore
        /// </summary>
        private static void WriteValues(ushort[] items, int i)
        {
            HostedSlave1.DataStore.InputRegisters[i + 1] = items[0];
            HostedSlave1.DataStore.InputRegisters[i + 2] = items[1];
        }

        /// <summary>
        /// Closes the ports
        /// </summary>
        private static void ClosePorts()
        {
            if (HostedMasterPort != null)
            {
                HostedMasterPort.Close();
            }
            if (Client1Port != null)
            {
                Client1Port.Close();
            }
            if (Client2Port != null)
            {
                Client2Port.Close();
            }
            if (Client3Port != null)
            {
                Client3Port.Close();
            }
        }
    }
}

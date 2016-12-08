using Modbus.Device;
using System;
using System.Collections.Generic;
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
        /// Hosted Master Input port
        /// </summary>
        public static SerialPort HostedMasterPort;

        /// <summary>
        /// Hosted Slave 1
        /// </summary>
        private static ModbusSerialSlave HostedSlave1;

        /// <summary>
        /// Hosted Slave 2
        /// </summary>
        private static ModbusSerialSlave HostedSlave2;

        /// <summary>
        /// Hosted Slave 3
        /// </summary>
        private static ModbusSerialSlave HostedSlave3;

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

        /// <summary>
        /// Timeout value for the hosted master receiving data
        /// </summary>
        public static int TimeOutValue;

        /// <summary>
        /// Contains the user's setting whether the data should be continually refreshed
        /// </summary>
        public static bool RefreshData;

        /// <summary>
        /// Contains the user's settings on the frequency of the data input refreshing
        /// </summary>
        public static int RefreshFreq;

        /// <summary>
        /// Executes with live input
        /// </summary>
        public static void Live()
        {
            HostedMasterPort.Open();
            IModbusSerialMaster master = ModbusSerialMaster.CreateRtu(HostedMasterPort);
            master.Transport.ReadTimeout = TimeOutValue;
            if (RefreshData)
            {
                while (true)
                {
                    ScanIn(master);
                    Thread.Sleep(RefreshFreq);
                }
            }
            else
            {
                ScanIn(master);
            }
        }

        public static void Generated(List<string> input)
        {
            bool status = true;
            while (status)
            {
                foreach (var item in input)
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
                    if (!RefreshData)
                    {
                        status = false;
                    }
                    else
                    {
                        Thread.Sleep(RefreshFreq);
                    }
                }
                status = MainWindow.GenContinue;
            }
        }

        /// <summary>
        /// Writes the generated values into the slave data store
        /// </summary>
        private static void WriteValues(ushort[] items, int i)
        {
            HostedSlave1.DataStore.InputRegisters[i + 1] = items[0];
            HostedSlave1.DataStore.InputRegisters[i + 2] = items[1];
        }

        /// <summary>
        /// Set to true when the datastore has been completed
        /// </summary>
        public static bool CreateHostedSlave1Status = false;

        /// <summary>
        /// Creates hosted slave 1
        /// </summary>
        public static void CreateHostedSlave1()
        {
            Client1Port.Open();
            HostedSlave1 = ModbusSerialSlave.CreateRtu(1, Client1Port);
            HostedSlave1.DataStore = Modbus.Data.DataStoreFactory.CreateDefaultDataStore(0, 0, 0, 65535);
            CreateHostedSlave1Status = true;
            new Thread(HostedSlave1.Listen).Start();
        }

        /// <summary>
        /// References CreateHostedSlave1 and CreateHostedSlave2
        /// </summary>
        public static void CreateHostedSlave1And2()
        {
            CreateHostedSlave1();
            CreateHostedSlave2();
        }

        /// <summary>
        /// Creates hosted slave 2
        /// </summary>
        private static void CreateHostedSlave2()
        {
            Client2Port.Open();
            HostedSlave2 = ModbusSerialSlave.CreateRtu(1, Client2Port);
            HostedSlave2.DataStore = HostedSlave1.DataStore;
            new Thread(HostedSlave2.Listen).Start();
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

        /// <summary>
        /// Creates hosted slave 3
        /// </summary>
        private static void CreateHostedSlave3()
        {
            Client3Port.Open();
            HostedSlave3 = ModbusSerialSlave.CreateRtu(1, Client3Port);
            HostedSlave3.DataStore = HostedSlave1.DataStore;
            new Thread(HostedSlave3.Listen).Start();
        }

        /// <summary>
        /// Scanns in data from the IED and writes it
        /// </summary>
        /// <param name="master"></param>
        /// <param name="index"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        private static void ScanIn(IModbusMaster master)
        {
            try
            {
                for (int i = 0; i < 272; i += 2)
                {
                    ushort[] items = master.ReadInputRegisters(1, (ushort)i, 2);
                    //float itemsFloat = BinaryToFloat(items); // Checks that the data is correct
                    WriteValues(items, i);
                }
            }
            catch (Exception)
            {
                Thread.CurrentThread.Abort();
            }

        }

        /// <summary>
        /// Converts the input from the two registers to a single floating point value
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        private static float BinaryToFloat(ushort[] values)
        {
            byte[] asByte = new byte[] {
                (byte)(values[1] % 256),
                (byte)(values[1] / 256),
                (byte)(values[0] % 256),
                (byte)(values[0] / 256),
            };
            float result = BitConverter.ToSingle(asByte, 0);

            return result;
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
            if (negative)
            {
                stringResult = '1' + stringResult.Substring(1);
            }

            string firstHalf = stringResult.Substring(0, (stringResult.Length / 2));
            string secondHalf = stringResult.Substring((stringResult.Length / 2), (stringResult.Length / 2));

            return new ushort[] { Convert.ToUInt16(firstHalf, 2), Convert.ToUInt16(secondHalf, 2) };
        }
    }
}

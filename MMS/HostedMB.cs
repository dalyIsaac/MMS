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
                    ScanIn(master, 0, 272);
                    Thread.Sleep(RefreshFreq);
                }
            }
            else
            {
                ScanIn(master, 0, 272);
            }
        }

        /// <summary>
        /// Creates hosted slave 1
        /// </summary>
        public static void CreateHostedSlave1()
        {
            Client1Port.Open();
            HostedSlave1 = ModbusSerialSlave.CreateRtu(1, Client1Port);
            HostedSlave1.DataStore = Modbus.Data.DataStoreFactory.CreateDefaultDataStore(0, 0, 0, 65535);
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
        private static void ScanIn(IModbusMaster master, int index, int max)
        {
            try
            {
                for (int i = index; i < max; i += 2)
                {
                    ushort[] items = master.ReadInputRegisters(1, (ushort)i, 2);
                    float itemsFloat = ToFloat(items); // Checks that the data is correct
                    HostedSlave1.DataStore.InputRegisters[i + 1] = items[0]; // Consider moving to another module
                    HostedSlave1.DataStore.InputRegisters[i + 2] = items[1];
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
        private static float ToFloat(ushort[] values)
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
    }
}

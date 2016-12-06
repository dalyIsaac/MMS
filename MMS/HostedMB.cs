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
        public static SerialPort HMIPort;

        /// <summary>
        /// Hosted Master Output port
        /// </summary>
        public static SerialPort HMOPort;

        /// <summary>
        /// Hosted Slave port
        /// </summary>
        public static SerialPort HSPort;

        /// <summary>
        /// Timeout value for the hosted master receiving data
        /// </summary>
        public static int TimeOutValue;

        /// <summary>
        /// Hosted Master Output master
        /// </summary>
        private static IModbusSerialMaster HMOMaster;

        /// <summary>
        /// Hosted Slave Input slave
        /// </summary>
        private static ModbusSerialSlave HSIslave;

        /// <summary>
        /// Executes with live input
        /// </summary>
        public static void Live()
        {
            new Thread(CreateHostedSlave).Start(); // Starts method in new thread
            Thread.CurrentThread.Name = "Live";
            HMIPort.Open();
            IModbusSerialMaster master = ModbusSerialMaster.CreateRtu(HMIPort);
            master.Transport.ReadTimeout = TimeOutValue;
            CreateHostedMaster();
            int counter = 0;
            counter = ScanIn(master, counter, 0, 54);
            counter = ScanIn(master, counter, 56, 108);
            counter = ScanIn(master, counter, 200, 272);
        }

        private static void CreateHostedMaster()
        {
            HMOPort.Open();
            HMOMaster = ModbusSerialMaster.CreateRtu(HMOPort);
        }

        private static void CreateHostedSlave()
        {
            Thread.CurrentThread.Name = "CHS";
            HSPort.Open();
            HSIslave = ModbusSerialSlave.CreateRtu(1, HSPort);
            HSIslave.DataStore = Modbus.Data.DataStoreFactory.CreateDefaultDataStore(0, 0, 65535,65535);
            HSIslave.Listen();
            HSIslave.ModbusSlaveRequestReceived += new EventHandler<ModbusSlaveRequestEventArgs>(TestEvent);
        }

        private static void TestEvent(object sender, ModbusSlaveRequestEventArgs e)
        {
            Console.WriteLine("Hello, World!");
        }

        private static int ScanIn(IModbusMaster master, int counter, int index, int max)
        {
            for (int i = index; i < max; i++)
            {
                counter++;

                // Write to slave here
                ushort[] items = master.ReadInputRegisters(1, (ushort)i, 2);
                HMOMaster.WriteMultipleRegisters(1, (ushort)i, items);
            }
            return counter;
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

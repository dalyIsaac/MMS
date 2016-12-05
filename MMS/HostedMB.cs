using Modbus.Device;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMS
{
    public static class HostedMB
    {
        /// <summary>
        /// New port for the hosted master
        /// </summary>
        public static SerialPort port;

        /// <summary>
        /// Timeout value for the hosted master receiving data
        /// </summary>
        public static int TimeOutValue;

        /// <summary>
        /// Executes with live input
        /// </summary>
        public static void Live()
        {
            port.Open();
            IModbusSerialMaster master = ModbusSerialMaster.CreateRtu(port);
            master.Transport.ReadTimeout = TimeOutValue;
            int counter = 0;
            counter = ScanIn(master, counter, 0, 54);
            counter = ScanIn(master, counter, 56, 108);
            counter = ScanIn(master, counter, 200, 272);
        }

        private static Dictionary<int, float> collectedValues = new Dictionary<int, float>();

        private static int ScanIn(IModbusMaster master, int counter, int index, int max)
        {
            for (int i = index; i < max; i += 2)
            {
                counter++;
                collectedValues.Add((30001 + i), ToFloat(master.ReadInputRegisters(1, (ushort)i, 2)));
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

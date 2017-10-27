using Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Vibrate_CLI
{
    class Program
    {
        static SerialPort serialPort;
        static void Main(string[] args)
        {
            if (args.Length >= 1)
            {
                var comName = args[0];

                if (!String.IsNullOrEmpty(comName))
                {
                    int n = 0;

                    var com = comName;

                    if (int.TryParse(com, out n))
                    {
                        com = "COM" + n;
                    }

                    com.ToUpper();


                    serialPort = new SerialPort(new System.IO.Ports.SerialPort(com, 9600, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One));

                    serialPort.StatusChanged += SerialPort_StatusChanged;

                    serialPort.DataReceived += SerialPort_ModBusMessageParsed;

                    serialPort.Open();

                }
            }

            while (true)
            {
                var readLine = Console.ReadLine();
                Regex regex = new Regex(@"\w+|""[\w\s]*""");
                var commands = regex.Matches(readLine).Cast<Match>().Select(m => m.Value.Trim(' ')).ToArray();

                Console.WriteLine(String.Join(" ", commands));

                bool vibrate = false;

                if (commands.Length >=1)
                {
                    if (commands[0].Equals("vibrate"))
                    {
                        vibrate = true;
                    }
                }

                if (vibrate)
                {
                    if(commands.Length >= 2)
                    {
                        if (commands[1].Equals("vibrate"))
                        {
                            serialPort.Send(SyscallCommandVibrate(commands[2]));
                        }

                        if (commands[1].Equals("set"))
                        {
                            serialPort.Send(SyscallCommandSetAddress(commands[2]));
                        }
                    }
                }
            }
        }

        private static void SerialPort_ModBusMessageParsed(byte[] data)
        {
            Console.WriteLine("Received [{0}]", String.Join(" ", data));
        }

        private static void SerialPort_StatusChanged(System.IO.Ports.SerialPort serialPort, SerialPort.Status status)
        {
            Console.WriteLine(status);
        }

        private static byte[] SyscallCommandVibrate(string Address)
        {
            var _d = new byte[6];

            var _t = Encoding.ASCII.GetBytes(Address);

            _d[0] = 0x01;
            _d[5] = 0x03;

            Buffer.BlockCopy(_t, 0, _d, 5 - _t.Length, _t.Length);

            //return new byte[] { 0x01, 0x30, 0x30, 0x30, 0x31, 0x03 };
            return _d;
        }

        private static byte[] SyscallCommandSetAddress(string Address)
        {
            var _d = new byte[6];

            var _t = Encoding.ASCII.GetBytes(Address);

            _d[0] = 0x53;
            _d[5] = 0x03;

            Buffer.BlockCopy(_t, 0, _d, 5 - _t.Length, _t.Length);

            //return new byte[] { 0x53, 0x30, 0x30, 0x30, 0x31, 0x03 };
            return _d;
        }
    }
}

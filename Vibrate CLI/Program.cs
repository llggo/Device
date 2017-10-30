using System;
using System.Collections.Generic;
using System.IO.Ports;
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


                    serialPort = new SerialPort(com, 9600, Parity.None, 8, StopBits.One);

                    serialPort.DataReceived += SerialPort_DataReceived;

                    try
                    {
                        Console.WriteLine("{0} opening", com);

                        serialPort.Open();

                        Console.WriteLine("{0} opened", com);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("{0} error: {1}", com, ex);
                    }
                    

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
                            var d = SyscallCommandVibrate(commands[2]);

                            serialPort.Write(d, 0, d.Length);

                            Console.WriteLine("Send: [{0}] - L: {1}", String.Join(" ", d), d.Length);
                        }

                        if (commands[1].Equals("set"))
                        {
                            var d = SyscallCommandSetAddress(commands[2]);

                            serialPort.Write(d, 0, d.Length);

                            Console.WriteLine("Send: [{0}] - L: {1}", String.Join(" ", d), d.Length);
                        }
                    }
                }
            }
        }

        private static void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Console.WriteLine("Received event");
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

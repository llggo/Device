using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandLine;
using CommandLine.Text;
using Library;
using System.Threading;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace CLI
{
    class Program :IDisposable
    {
        static SerialPort _serialPort;
        static ModBus _modBus = new ModBus();
        static string _text = "";
        static int _command = 0;
        static int _address = -1;
        static int _device = 1;
        static void Main(string[] args)
        {

            using (var opt = new StartApplicationOptions())
            {
                if(args.Length >= 1)
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
                        if (opt.Verbose) Console.WriteLine("Args: {0}", com);



                        _serialPort = new SerialPort(new System.IO.Ports.SerialPort(com, 19200));

                        _serialPort.StatusChanged += _port_StatusChanged;

                        _serialPort.DataReceived += _serialPort_DataReceived;

                        _serialPort.Open();

                    }
                }
                
            }



            while (true)
            {
                var _readLine = Console.ReadLine();

                Regex regex = new Regex(@"\w+|""[\w\s]*""");

                var commands = regex.Matches(_readLine).Cast<Match>().Select(m => m.Value.Trim(' ')).ToArray();


                if(commands.Length >= 1)
                {
                    string device = commands[0];
                    if (device.Equals("led"))
                    {
                        _device = 1;
                    }
                }

                if(commands.Length >= 2)
                {
                    string _action = commands[1];

                    switch (_action)
                    {
                        case "on":
                            _command = 4;
                            break;

                        case "off":
                            _command = 3;
                            break;

                        case "stop":
                            _command = 8;
                            break;

                        case "ping":
                            _command = 1;
                            break;

                        default:
                            _command = 2;
                            break;
                    }
                }

                if(_command == 2)
                {
                    if (commands.Length >= 3)
                    {
                        _text = commands[2];

                        if (_text.Length > 4)
                        {
                            _command = 7;
                        }
                    }

                    if (commands.Length >= 4)
                    {
                        string addr = commands[3];

                        try
                        {
                            _address = int.Parse(addr);
                        }
                        catch
                        {
                            _address = -1;
                        }
                    }
                }
                else
                {
                    if (commands.Length >= 3)
                    {
                        string addr = commands[2];

                        try
                        {
                            _address = int.Parse(addr);
                        }
                        catch
                        {
                            _address = -1;
                        }
                    }
                }

                

                

                
                        

                _serialPort.Send(_modBus.BuildText(_device, _address, _command, _text));

                Console.WriteLine("Send: {0} -> Led: {1} \nData: {2}", _text, _address, string.Join(", ", _modBus.BuildText(_device, _address, _command, _text)));

            }
        }

        private static void _serialPort_DataReceived(byte[] data)
        {
            Console.WriteLine("Message Received: {0}", String.Join(", ", data));
        }

        private static void _port_StatusChanged(System.IO.Ports.SerialPort serialPort, SerialPort.Status status)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("{0} Status: ", _serialPort.PortName);

            switch (status)
            {
                case SerialPort.Status.OpenError:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case SerialPort.Status.CloseError:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case SerialPort.Status.Opened:
                    Console.ForegroundColor = ConsoleColor.Green;

                    _serialPort.Send(_modBus.BuildText(1, 0, 4, ""));

                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
            }
            
            Console.WriteLine("{0}", status);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }

    class StartApplicationOptions :IDisposable
    {
        [Option('c', "com",
          HelpText = "Input com number ex: COM1.")]
        public string ComNumber { get; set; }

        [Option('n', "name",
          HelpText = "Input com name ex: CHS341.")]
        public string ComName { get; set; }

        [Option('b', "baud ",
          HelpText = "Input com name ex: CHS341.")]
        public string BaudRate { get; set; }

        [Option('a', "auto",
          HelpText = "Auto Detect COM Port")]
        public string Auto { get; set; }

        [Option('v', "verbose",
          HelpText = "Prints all messages to standard output.")]
        public bool Verbose { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        public void Dispose()
        {
            
        }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this,
              (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }

    class Device : IDisposable
    {
        [Option('l', "led",
          HelpText = "ex: -l | --led")]
        public bool Led { get; set; }


        [Option('v', "verbose",
          HelpText = "Prints all messages to standard output.")]
        public bool Verbose { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        public void Dispose()
        {

        }
    }

    class LedOption: IDisposable
    {
        [Option('s', "show",
          HelpText = "ex: -s | --show")]
        public bool Show { get; set; }

        [Option('r', "run",
          HelpText = "ex: -r | --run")]
        public bool Run { get; set; }

        [Option('a', "address",
          HelpText = "ex: -a | --address")]
        public int Address { get; set; }

        [Option('t', "text",
          HelpText = "ex: -a | --text")]
        public string Text { get; set; }

        [Option('o', "on",
          HelpText = "ex: -a | --text")]
        public bool On { get; set; }

        [Option('f', "off",
          HelpText = "ex: -a | --text")]
        public bool Off { get; set; }


        [Option('v', "verbose",
          HelpText = "Prints all messages to standard output.")]
        public bool Verbose { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        public void Dispose()
        {

        }
    }
}

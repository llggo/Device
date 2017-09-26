using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandLine;
using CommandLine.Text;
using Library;
using System.Threading;
using System.Diagnostics;

namespace CLI
{
    class Program :IDisposable
    {
        static SerialPort _serialPort;
        static ModBus _modBus = new ModBus();
        static string _text = "";
        static int _address, _command;
        static void Main(string[] args)
        {
            using(var opt = new StartApplicationOptions())
            {
                if (CommandLine.Parser.Default.ParseArguments(args, opt))
                {
                    if(!String.IsNullOrEmpty(opt.ComNumber))
                    {
                        int n = 0;
                        var com = opt.ComNumber;
                        if (int.TryParse(com, out n))
                        {
                            com = "COM" + n;
                            
                        }

                        com.ToUpper();
                        if (opt.Verbose) Console.WriteLine("Args: {0}", com);



                        _serialPort = new SerialPort(new System.IO.Ports.SerialPort(com, 19200));

                        _serialPort.StatusChanged += _port_StatusChanged;

                        _serialPort.ModBusMessageParsed += _serialPort_DataReceived;

                        _serialPort.Open();

                    }
                }
            }



            while (true)
            {
                var command = Console.ReadLine();
                string[] commands = command.Split(' ');

                using (var opt = new Options())
                {
                    if (CommandLine.Parser.Default.ParseArguments(commands, opt))
                    {
                        if (opt.Exit)
                        {
                            Console.WriteLine("CLI Exiting");
                            Thread.Sleep(1000);
                            return;
                        }
                        else
                        {
                            var _send = true;

                            if (String.IsNullOrEmpty(opt.Address)){
                                Debug.WriteLine("Address is null");
                                _send = false;
                            }
                            else
                            {
                                if (!int.TryParse(opt.Address, out _address))
                                {
                                    Debug.WriteLine("Parse address fail: " + opt.Address);
                                    _send = false;
                                }
                            }

                            

                            if (opt.Show)
                            {
                                _command = 2;

                                if (opt.Run)
                                {
                                    _command = 7;
                                }

                                if (!String.IsNullOrEmpty(opt.Text))
                                {
                                    _text = opt.Text;
                                }
                                else
                                {
                                    Debug.WriteLine("Text is null");
                                    _send = false;
                                }
                            }else if(opt.Off)
                            {
                                _command = 3;
                            }
                            else if(opt.On)
                            {
                                _command = 4;
                            }
                            else
                            {
                                _send = false;
                            }


                            if (_send)
                            {
                                var data = _modBus.BuildText(1, _address, _command, _text);
                                _serialPort.Send(data);
                               
                                Console.WriteLine("Queue [{0}] to led [{1}] as command [{2}]", opt.Text, _address, _command);
                                Console.WriteLine("Data [{0}] - Length {1}", String.Join(", ", data), data.Length);
                                
                            }
                            else
                            {
                                Console.WriteLine("Error: Command line");
                            }

                        }

                        
                    }
                }
                
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

    class Options :IDisposable
    {
        [Option('o', "on",
          HelpText = "ex: -s | --show")]
        public bool On { get; set; }

        [Option('f', "off",
          HelpText = "ex: -s | --show")]
        public bool Off { get; set; }

        [Option('s', "show",
          HelpText = "ex: -s | --show")]

        public bool Show { get; set; }

        [Option('a', "address")]
        public string Address { get; set; }

        [Option('t', "text")]
        public string Text { get; set; }

        [Option('r', "run")]
        public bool Run { get; set; }

        [Option('e', "exit", 
          HelpText = "Exit CLI")]
        public bool Exit { get; set; }

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
            var _helpText = new HelpText();
            _helpText.Heading = "-s | --show";
            _helpText.AddPostOptionsLine("Option: -a | --address value -t | --text value");
            _helpText.AddPostOptionsLine("Example: -s -a 1 -t welcome | --show --address 1 --text welcome");
            _helpText.AddPostOptionsLine("");

            return _helpText;
        }
    }
}

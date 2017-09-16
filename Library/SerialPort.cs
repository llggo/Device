using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;

namespace Library
{
    public class SerialPort
    {
        Mutex _mutex = new Mutex();
        bool _lock;

        Queue<byte[]> _sendQueue = new Queue<byte[]>();
        public enum Status
        {
            Opened,
            Closed,
            Opening,
            Closing,
            CloseError,
            OpenError
        }

        private System.IO.Ports.SerialPort _serialPort;

        public delegate void StatusEventHandler(System.IO.Ports.SerialPort serialPort, Status status);

        public delegate void ReceivedEventHandler(byte[] data);

        public event StatusEventHandler StatusChanged;

        public event ReceivedEventHandler ModBusMessageParsed;

        private BackgroundWorker _comControlWorker, _comReadWorker, _comSendWorker;

        Parser _parser;

        public SerialPort(System.IO.Ports.SerialPort serialPort)
        {
            Init(serialPort);
        }

        public SerialPort(string Port)
        {
            Init(new System.IO.Ports.SerialPort(Port));
        }

        private void Init(System.IO.Ports.SerialPort serialPort)
        {
            _serialPort = serialPort;
            _serialPort.DataReceived += _serialPort_DataReceived;

            _comControlWorker = new BackgroundWorker();
            _comControlWorker.WorkerSupportsCancellation = true;
            _comControlWorker.DoWork += _comControlWorker_DoWork;

            _comReadWorker = new BackgroundWorker();
            _comReadWorker.WorkerSupportsCancellation = true;
            _comReadWorker.DoWork += _comReadWorker_DoWork;

            _comSendWorker = new BackgroundWorker();
            _comSendWorker.WorkerSupportsCancellation = true;
            _comSendWorker.DoWork += _comSendWorker_DoWork;
            

            _parser = new Parser();
            _parser.MessageParsed += _parser_MessageParsed;
        }

        

        private void _parser_MessageParsed(byte[] data)
        {
            ModBusMessageParsed(data);
        }

        private void _serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (!_comReadWorker.IsBusy)
            {
                _comReadWorker.RunWorkerAsync();
            }
        }

        private byte[] ReadByteInPort()
        {
            byte[] r = new byte[_serialPort.BytesToRead];
            try
            {
                _serialPort.Read(r, 0, _serialPort.BytesToRead);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            finally
            {
                if (_serialPort.BytesToRead != 0) _serialPort.DiscardInBuffer();
            }
            return r;
        }

        public void Close()
        {
            if (_comControlWorker.IsBusy)
            {
                _comControlWorker.CancelAsync();
            }
        }

        public void Send(byte[] data)
        {
            _sendQueue.Enqueue(data);

            if (!_comSendWorker.IsBusy)
            {
                _comSendWorker.RunWorkerAsync();
            }
           
        }

        public void Open()
        {
            if (!_comControlWorker.IsBusy)
            {
                _comControlWorker.RunWorkerAsync();
            }
        }

        public string PortName { get => _serialPort.PortName; }


        private void _comControlWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            int loop = 0;
            while (!_comControlWorker.CancellationPending)
            {
                if (!_serialPort.IsOpen)
                {
                    try
                    {
                        StatusChanged(_serialPort, Status.Opening);

                        _serialPort.Open();

                        if (!_comReadWorker.IsBusy)
                        {
                            _comReadWorker.RunWorkerAsync();
                        }

                        if (!_comSendWorker.IsBusy)
                        {
                            _comSendWorker.RunWorkerAsync();
                        }

                        StatusChanged(_serialPort, Status.Opened);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);

                        if(loop >= 5)
                        {
                            StatusChanged(_serialPort, Status.OpenError);
                            loop = 0;
                        }
                        loop++;
                    }
                }
                else
                {
                    if (_comReadWorker.IsBusy)
                    {
                        _comReadWorker.CancelAsync();
                    }

                    if (_comSendWorker.IsBusy)
                    {
                        _comSendWorker.CancelAsync();
                    }
                }
                Thread.Sleep(1000);
            }

            if (_serialPort != null)
            {
                if (_serialPort.IsOpen)
                {
                    try
                    {
                        StatusChanged(_serialPort, Status.Closing);
                        _serialPort.Close();
                        StatusChanged(_serialPort, Status.Closed);
                    }
                    catch
                    {
                        StatusChanged(_serialPort, Status.CloseError);
                    }
                }
            }
        }

        private void _comSendWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!_comSendWorker.CancellationPending && _sendQueue.Count != 0)
            {
                var d = _sendQueue.Dequeue();

                if (_serialPort != null && _serialPort.IsOpen)
                {
                    try
                    {
                        if (!_lock)
                        {
                            _mutex.WaitOne();
                            _lock = true;
                        }

                        _serialPort.Write(d, 0, d.Length);

                        if (_lock)
                        {
                            _mutex.ReleaseMutex();
                            _lock = false;
                        }
                       
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Send To COM Error: " + ex);
                    }
                }
            }
        }
        private void _comReadWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            
            while (!_comReadWorker.CancellationPending)
            {
                _parser.Process(ReadByteInPort());

                switch (_parser.State)
                {
                    case Parser.Status.Start:
                        //code lock com read
                        if (!_lock)
                        {
                            _mutex.WaitOne();
                            _lock = true;
                        }
                        break;
                    case Parser.Status.Stop:
                        //code unlock com read
                        if (_lock)
                        {
                            _mutex.ReleaseMutex();
                            _lock = false;
                        }
                        break;
                }
            }
        }
    }
}

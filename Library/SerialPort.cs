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

        private Queue<byte[]> _sendQueue = new Queue<byte[]>();

        public bool ModBusParser = false;

        public enum Status
        {
            Opened,
            Closed,
            Opening,
            Closing,
            CloseError,
            OpenError,
            SendError,
            ReceivedEror
        }

        private System.IO.Ports.SerialPort _serialPort;

        public delegate void StatusEventHandler(System.IO.Ports.SerialPort serialPort, Status status);

        public delegate void ReceivedEventHandler(byte[] data);

        public event StatusEventHandler StatusChanged;

        public event ReceivedEventHandler DataReceived;

        private BackgroundWorker _comControlWorker, _comReadWorker, _comSendWorker;

        private Parser _parser;

        private bool _isRead = false;

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
            _comReadWorker.RunWorkerCompleted += _comReadWorker_RunWorkerCompleted;

            _comSendWorker = new BackgroundWorker();
            _comSendWorker.WorkerSupportsCancellation = true;
            _comSendWorker.DoWork += _comSendWorker_DoWork;
            

            _parser = new Parser();
            _parser.MessageParsed += _parser_MessageParsed;

        }

        private void _comReadWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _isRead = false;
            Debug.WriteLine("Woker read com run completed");
        }

        private void _parser_MessageParsed(byte[] data)
        {
            DataReceived(data);
        }

        private void _serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Debug.WriteLine("Serial port data received");

            if (!_comReadWorker.IsBusy && ModBusParser)
            {
                _isRead = true;
                _comReadWorker.RunWorkerAsync();
            }
            else
            {
                Thread.Sleep(100);
                DataReceived(ReadByteInPort());
            }
        }

        private byte[] ReadByteInPort()
        {
            //Thread.Sleep(1000);

            byte[] r = new byte[_serialPort.BytesToRead];
            try
            {
                _serialPort.Read(r, 0, _serialPort.BytesToRead);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception read byte in port: " + ex.ToString());
            }
            finally
            {
                if (_serialPort.BytesToRead != 0) _serialPort.DiscardInBuffer();

                Debug.WriteLine("Read byte in port finally");
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

                        if (!_comSendWorker.IsBusy)
                        {
                            _comSendWorker.RunWorkerAsync();
                        }

                        StatusChanged(_serialPort, Status.Opened);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Exception com control do work: " + ex);

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
                        Debug.WriteLine("Exception com control do work: ");
                        StatusChanged(_serialPort, Status.CloseError);
                    }
                }
            }
        }

        private void _comSendWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!_comSendWorker.CancellationPending && _sendQueue.Count != 0)
            {
                if (!_isRead)
                {
                    var d = _sendQueue.Dequeue();

                    if (_serialPort != null && _serialPort.IsOpen)
                    {
                        try
                        {
                            var s = DateTime.Now.Millisecond;
                            Debug.WriteLine("--- Serial port send --- \n Data: {0} \n Length: {1}", String.Join(" ", d), d.Count());
                            _serialPort.Write(d, 0, d.Length);
                            Debug.WriteLine(" E: {0}ms \n--- End ---", DateTime.Now.Millisecond - s);
                        }
                        catch (Exception ex)
                        {
                            StatusChanged(_serialPort, Status.SendError);
                            Debug.WriteLine("Send to com error: " + ex);
                        }
                    }
                }

            }
        }
        private void _comReadWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Debug.WriteLine("Worker read com start");

            while (!_comReadWorker.CancellationPending)
            {
                _parser.Process(ReadByteInPort());

                switch (_parser.State)
                {
                    case Parser.Status.Start:
                        //code lock com read
                        Debug.WriteLine("Parser start");

                        break;
                    case Parser.Status.Stop:
                        //code unlock com read
                        Debug.WriteLine("Parser stop");


                        if (_comReadWorker.IsBusy)
                        {
                            _comReadWorker.CancelAsync();
                        }
                        break;
                }
            }
        }
    }
}

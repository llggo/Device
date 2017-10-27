using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Library;
using System.Diagnostics;
using WebSocket4Net;

namespace Vibrate
{
    public partial class Main : Form
    {
        SerialPort serialPort;

        WebSocket webSocket;
        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            serialPort = new SerialPort(new System.IO.Ports.SerialPort("COM8", 9600, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One));
            serialPort.StatusChanged += SerialPort_StatusChanged;
            serialPort.DataReceived += SerialPort_ModBusMessageParsed;

            serialPort.Open();
        }

        private void Initial()
        {
            webSocket = new WebSocket("ws://mqserver:3000/room/outter/join");
            webSocket.Opened += WebSocket_Opened;
            webSocket.Closed += WebSocket_Closed;
            webSocket.Error += WebSocket_Error;
            webSocket.DataReceived += WebSocket_DataReceived;
            webSocket.Open();
        }

        private void WebSocket_DataReceived(object sender, WebSocket4Net.DataReceivedEventArgs e)
        {
            Debug.WriteLine(e.Data);
        }

        private void WebSocket_Error(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
        {
            Debug.WriteLine(e.Exception);
        }

        private void WebSocket_Closed(object sender, EventArgs e)
        {
            Debug.WriteLine("Closed");
        }

        private void WebSocket_Opened(object sender, EventArgs e)
        {
            Debug.WriteLine("Open");
        }

        private void SerialPort_ModBusMessageParsed(byte[] data)
        {
            //throw new NotImplementedException();
        }

        private void SerialPort_StatusChanged(System.IO.Ports.SerialPort serialPort, SerialPort.Status status)
        {
            //throw new NotImplementedException();
            Debug.WriteLine(status);
        }

        private void buttonSet_Click(object sender, EventArgs e)
        {
            serialPort.Send(SyscallCommandSetAddress(textBoxAddress.Text)); //Gui Rung
        }

        private void buttonVibrate_Click(object sender, EventArgs e)
        {
            serialPort.Send(SyscallCommandVibrate(textBoxAddress.Text)); //Gui Rung
        }

        private byte[] SyscallCommandVibrate(string Address)
        {
            var _d = new byte[6];

            var _t = Encoding.ASCII.GetBytes(Address);

            _d[0] = 0x01;
            _d[5] = 0x03;

            Buffer.BlockCopy(_t, 0, _d, 5 - _t.Length, _t.Length);

            //return new byte[] { 0x01, 0x30, 0x30, 0x30, 0x31, 0x03 };
            return _d;
        }

        private byte[] SyscallCommandSetAddress(string Address)
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

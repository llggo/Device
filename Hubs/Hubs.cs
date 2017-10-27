using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Communication;
using static Communication.Keyboard;

namespace Hubs
{
    public class Hubs
    {
        public Hubs()
        {
            
        }

        public void Start()
        {
            serialPort = new Library.SerialPort(new System.IO.Ports.SerialPort("COM1", 19200));
            serialPort.StatusChanged += SerialPort_StatusChanged;
            serialPort.DataReceived += SerialPort_ModBusMessageParsed;

            serialPort.Open();

            keyboard.DataSend += keyboard_DataSend;
            keyboard.WebSocketStatus += Keyboard_WebSocketStatus;
            keyboard.Start();
        }

        private void Keyboard_WebSocketStatus(ServerStatus status)
        {
            KeyboardStatus(status);
        }

        private void keyboard_DataSend(byte[] data)
        {
            serialPort.Send(data);
        }

        private void SerialPort_ModBusMessageParsed(byte[] data)
        {
            keyboard.Received(data);
        }

        private void SerialPort_StatusChanged(System.IO.Ports.SerialPort serialPort, Library.SerialPort.Status status)
        {
            ComStatusChanged(serialPort, status);
        }

        #region Init

        public delegate void ComEventHandler(System.IO.Ports.SerialPort serialPort, Library.SerialPort.Status status);
        public event ComEventHandler ComStatusChanged;

        public delegate void KeyboardStatusEventHandler(ServerStatus status);
        public event KeyboardStatusEventHandler KeyboardStatus;

        #region Serial Port
        public Library.SerialPort serialPort;
        #endregion

        Keyboard keyboard = new Keyboard();

        #endregion
    }
}

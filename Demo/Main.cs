using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Library;

namespace Demo
{
    public partial class Main : Form
    {
        Queue<int> _queue = new Queue<int>();

        SerialPort serialPort;
        public Main()
        {
            InitializeComponent();
        }

        private void textBoxIdInput_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == 13)
            {
                var id = int.Parse(textBoxIdInput.Text);
                _queue.Enqueue(id);
                textBoxIdInput.Text = "";
                PrintRichTextBox();
            }
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            if(_queue.Count > 0)
            {
                var id = _queue.Dequeue();
                Debug.WriteLine(id);
                PrintRichTextBox();
            }
            else
            {
                MessageBox.Show("Queue is Empty");
            }
           
        }

        private void PrintRichTextBox()
        {
            int[] _temp = new int[_queue.Count];

            _queue.CopyTo(_temp, 0);


            richTextBoxQueue.Clear();

            foreach (var d in _temp)
            {
                richTextBoxQueue.Text += d + "\n";
            }
        }

        private void buttonVir_Click(object sender, EventArgs e)
        {
            //serialPort.Send(new byte[] { 0x53, 0x30, 0x30, 0x30, 0x31, 0x03 }); //Set dia chi
            serialPort.Send(new byte[] { 0x01, 0x30, 0x30, 0x30, 0x31, 0x03 }); //Gui Rung
        }

        private void Main_Load(object sender, EventArgs e)
        {
            serialPort = new SerialPort(new System.IO.Ports.SerialPort("COM7", 9600, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One));
            serialPort.StatusChanged += SerialPort_StatusChanged;
            serialPort.DataReceived += SerialPort_ModBusMessageParsed;

            serialPort.Open();
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
    }
}

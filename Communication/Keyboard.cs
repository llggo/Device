using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Net.WebSockets;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.ComponentModel;
using System.Xml.Linq;
using System.IO;
using System.Json;
using Newtonsoft.Json;
using System.Windows.Threading;
using Library;

namespace Communication
{
    public class Keyboard
    {
        public Keyboard()
        {

        }

        public void Start()
        {
            sendWorker = new BackgroundWorker();
            sendWorker.WorkerSupportsCancellation = true;
            sendWorker.DoWork += SendWorker_DoWork;
            sendWorker.RunWorkerCompleted += SendWorker_RunWorkerCompleted;
            sendWorker.RunWorkerAsync();

            webSocketWorker = new BackgroundWorker();
            webSocketWorker.WorkerSupportsCancellation = true;
            webSocketWorker.DoWork += WebSocketWorker_DoWork;
            webSocketWorker.RunWorkerAsync();
        }

        private void SendWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Debug.WriteLine("Send worker completed");
        }

        private void SendWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!sendWorker.CancellationPending)
            {
                Debug.WriteLine("While: ");

                if(send.Count > 0)
                {
                    //var s = send.Peek();
                    var s = send.Dequeue();

                    DataSend(s);

                    Thread.Sleep(1000);

                    if(received.Count > 0)
                    {
                        var r = received.Dequeue();

                        var sMID = ModBus.GetMessageId(s);
                        var rMID = ModBus.GetMessageId(r);

                        if (sMID.Equals(rMID))
                        {
                            send.Dequeue();
                        }
                    }

                }
                else
                {
                    if (sendWorker.IsBusy)
                    {
                        sendWorker.CancelAsync();
                    }
                }
            }
        }

        private void WebSocketWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Connect().Wait();
            Debug.WriteLine("WebSocketWorker_DoWork Exited");
        }

        private async Task Connect()
        {
            string uri = "ws://mqserver:3000/room/actor/join?branch_code=AMAST02&actor_type=counter&counter_code=AMAST02_C1&user_id=usr_YtC6nWz3gK8PFl4BJuXX&reconnect_count=0";

            try
            {
                WebSocketStatus(ServerStatus.Connecting);

                clientWebSocket = new ClientWebSocket();
                await clientWebSocket.ConnectAsync(new Uri(uri), CancellationToken.None);

                WebSocketStatus(ServerStatus.Connected);

                await Task.WhenAll(Receive(clientWebSocket), Send(clientWebSocket));

                
            }

            catch (Exception ex)
            {
                Debug.WriteLine("ConnectAsync Exception: {0}", ex);

                WebSocketStatus(ServerStatus.ConnectError);
            }

            finally
            {
                if (clientWebSocket != null)
                    clientWebSocket.Dispose();



                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("WebSocket closed.");
                Console.ResetColor();

                WebSocketStatus(ServerStatus.Disconnected);

            }

        }

        private async Task Send(ClientWebSocket webSocket)
        {
            //byte[] buffer = encoder.GetBytes("{\"op\":\"blocks_sub\"}"); //"{\"op\":\"unconfirmed_sub\"}");
            //byte[] buffer = System.Text.Encoder.GetBytes("{\"op\":\"unconfirmed_sub\"}");
            //await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);

            //while (webSocket.State == WebSocketState.Open)
            //{
            //    LogStatus(false, buffer, buffer.Length);
            //    await Task.Delay(delay);
            //}
        }

        private async Task Receive(ClientWebSocket webSocket)
        {
            ArraySegment<Byte> buffer = new ArraySegment<byte>(new Byte[8192]);


            while (webSocket.State == WebSocketState.Open)
            {
                Debug.WriteLine("webSocket: " + webSocket.State);

                WebSocketReceiveResult result = null;

                using (var ms = new MemoryStream())
                {
                    do
                    {
                        Debug.WriteLine("do result: ");

                        result = await webSocket.ReceiveAsync(buffer, CancellationToken.None);
                        ms.Write(buffer.Array, buffer.Offset, result.Count);
                    }

                    while (!result.EndOfMessage);

                    ms.Seek(0, SeekOrigin.Begin);

                    Debug.WriteLine("read");

                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        using (var reader = new StreamReader(ms, Encoding.UTF8))
                        {
                            // do stuff
                            var str = reader.ReadToEnd();

                            var handle = str.Split(' ')[0].Trim('/');

                            var data = str.Remove(0, str.IndexOf(' '));

                            Debug.WriteLine("handle " + handle);

                            switch (handle)
                            {
                                case "initial":
                                    _initial = JsonConvert.DeserializeObject<Models.Initial>(data);


                                    InitKeyboard();

                                    break;
                            }

                            Debug.WriteLine("Initial " + _initial.Counters);
                        }
                    }

                }

            }

            Debug.WriteLine("webSocket out while: " + webSocket.State);
        }

        private void Send(byte[] data)
        {
            send.Enqueue(data);

            if (!sendWorker.IsBusy)
            {
                sendWorker.RunWorkerAsync();
            }
        }

        public void InitKeyboard()
        {
            //Init Service
            
            List<string> _listService = new List<string>();
            _listService.Add("A");
            _listService.Add("AC");

            Send(keyboard.SetService(247, _listService));

            //Thread.Sleep(2000);
            //Send(keyboard.SetService(247, _listService));
            //Thread.Sleep(2000);
            //Send(keyboard.SetService(247, _listService));
            //Thread.Sleep(2000);
            //Send(keyboard.SetService(247, _listService));

            //Init Counter
            //List<string> _listCounter = new List<string>();

            //_listCounter.Add("100");

            //foreach (var c in _initial.Counters)
            //{
            //    _listCounter.Add(c.CNum);
            //}

            //Thread.Sleep(100);

            //Send(keyboard.SetCounter(247, "Counter", _listCounter));
        }

        public void Received(byte[] data)
        {
            received.Enqueue(data);
            Debug.WriteLine("Keyboard received: " + String.Join(" ", data));
        }

        public enum ServerStatus{
            Connected,
            Disconnected,
            Connecting,
            Disconecting,
            ConnectError,
            Sync,
            Sending,
            Sended,
            Received,
            receiving
        }

        Device.Keyboard keyboard = new Device.Keyboard();

        Queue<byte[]> received = new Queue<byte[]>();
        Queue<byte[]> send = new Queue<byte[]>();

        BackgroundWorker sendWorker;

        public delegate void SendEventHandler(byte[] data);
        public event SendEventHandler DataSend;

        public delegate void WebSocketEventHandler(ServerStatus status);
        public event WebSocketEventHandler WebSocketStatus;

        ClientWebSocket clientWebSocket;

        BackgroundWorker webSocketWorker;

        //Var
        Models.Initial _initial = null;
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlTypes;
using InTheHand.Net.Sockets;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Sockets;
using System.Net;

using myBluetoothProject.Helpers;
using myBluetoothProject.Services;
using myBluetoothProject.Messenger;

namespace myBluetoothProject
{
    public partial class CentralServerForm : Form
    {
        #region Static region

        #endregion

        #region Events & Event methods

        private void startButton_Click(object sender, EventArgs e)
        {
            startButton.Enabled = false;
            UpdateUI("Market server is started");
            Thread tcpServerThread = new Thread(new ThreadStart(AsynchronousSocketListener.StartListening));
            tcpServerThread.Start();
        }

        #region UnhandledEvents;

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void activityBox_TextChanged(object sender, EventArgs e) { }

        #endregion;

        #endregion

        #region Fields

        private CentralServer server;
        private IList<Device> devices;
        private ISenderBluetoothService service;
        private bool ServerRunning = false;

        // Thread signal for tcp server
        //public static ManualResetEvent allDone = new ManualResetEvent(false);

        #endregion

        #region Properties

        #endregion

        #region Constructors

        public CentralServerForm(CentralServer server)
        {
            InitializeComponent();
            this.server = server;
            service = new SenderBluetoothService();
            devices = new List<Device>();
            Messenger<Msg>.Default.AddHandler<string>(Msg.AppLog,UpdateUI);
        }

        public CentralServerForm()
        {
            InitializeComponent();
            service = new SenderBluetoothService();
            devices = new List<Device>();
            Messenger<Msg>.Default.AddHandler<string>(Msg.AppLog, UpdateUI);
        }

        #endregion

        #region Methods

        // update esl info
        private async void UpdateProductInfo(UpdateEventArgs args)
        {
            activityBox.Clear();
            string dataType = "-1";

            switch (args.Type)
            {
                case UpdateType.Price:
                    dataType = "0";
                    break;
                case UpdateType.Image:
                    dataType = "1";
                    break;
                case UpdateType.Info:
                    dataType = "2";
                    break;
            }

            try
            {
                string sendDataString = args.Esl + Definitions.OnRequestGUID + dataType + Definitions.OnRequestGUID + args.NewValue + Definitions.OnRequestGUID;
                byte[] sendDataByte = System.Text.Encoding.UTF8.GetBytes(sendDataString);

                // get authanticated local servers 
                devices = await this.service.GetDevices();

                // check if device count non zero
                if (devices.Count == 0)
                {
                    UpdateUI("There is no avaliable local server in range , Try Again");
                    return;
                }

                var candidateDevices = devices.Where(device => device.DeviceName == args.LocalServer);

                if (candidateDevices.Count() <= 0)
                {
                    UpdateUI(String.Format("There is no local server named: {0}, try again!", args.LocalServer));
                    return;
                }
                else if (candidateDevices.Count() > 1)
                {
                    UpdateUI("There were too many devices have same name, check it again");
                    return;
                }

                Device targetDevice = candidateDevices.First();
                UpdateUI(String.Format("Local server of product : {0} -> " + args.LocalServer, args.Esl));

                // Local server is found and connection will begin
                UpdateUI("Info is updating ..." + Environment.NewLine);

                SenderBluetoothService service = new SenderBluetoothService();
                var wasSent = await service.Send(targetDevice, sendDataByte);

                // if data was sent to local server successfully , listen from response from local server
                if (wasSent)
                {
                    if (!ServerRunning)
                    {
                        ParameterizedThreadStart start = new ParameterizedThreadStart(ServerConnectThread);
                        Thread ServerThread = new Thread(start);
                        ServerThread.Start(args);
                    }
                }

                // if data was not sent to local server
                else
                {
                    UpdateUI(string.Format("Info can not updated , Local server is not ready ,TRY AGAIN!" + Environment.NewLine));
                }
            }
            catch (Exception ex)
            {
                UpdateUI("A problem occured connecting to local server. Details: " + ex.Message);
            }
        }

        private void UpdateUI(string Message)
        {
            Func<int> del = delegate()
            {
                activityBox.AppendText(Message + Environment.NewLine);
                return 0;
            };

            Invoke(del);
        }

        private void Initialize()
        {
            UpdateUI("Market server is started");
            UpdateEventArgs newEventArgs = new UpdateEventArgs(UpdateType.Info, "3.55", "CILEK", "Cilek");
            UpdateProductInfo(newEventArgs);
        }

        // waiting response thread from local server
        private void ServerConnectThread(object eventArgs)
        {
            UpdateEventArgs args = (UpdateEventArgs)eventArgs;

            Guid responseGuid = new Guid(Definitions.OnResponseGUID);

            BluetoothListener BTListener = new BluetoothListener(responseGuid);
            BTListener.Start();

            ServerRunning = true;
            BluetoothClient conn = BTListener.AcceptBluetoothClient();
            Stream mStream = conn.GetStream();

            while (ServerRunning)
            {
                try
                {
                    int bytesRead = -1;

                    byte[] received = new byte[1024];
                    bytesRead = mStream.Read(received, 0, received.Length);

                    // if received any reply
                    if (bytesRead != -1)
                    {
                        string result = System.Text.Encoding.UTF8.GetString(received, 0, bytesRead);

                        // if send to to digital tag from local server successful
                        if (result.Equals("1"))
                        {
                            if (args.Type == UpdateType.Price)
                            {
                                UpdateUI("Price is updated");
                            }

                            else if (args.Type == UpdateType.Image)
                            {
                                UpdateUI("Image is updated");
                            }

                            else if (args.Type == UpdateType.Info)
                            {
                                UpdateUI("Info is updated");
                            }
                        }

                        else
                        {
                            if (args.Type == UpdateType.Price)
                            {
                                UpdateUI(string.Format("Price can not updated, ESL is not ready") + Environment.NewLine);
                            }

                            else if (args.Type == UpdateType.Image)
                            {
                                UpdateUI(string.Format("Image can not updated, ESL is not ready") + Environment.NewLine);
                            }

                            else if (args.Type == UpdateType.Info)
                            {
                                UpdateUI(string.Format("Information can not updated, ESL is not ready") + Environment.NewLine);
                            }
                        }

                        ServerRunning = false;
                        conn.Close();
                        BTListener.Stop();
                    }
                }
                catch (Exception ex)
                {
                    ServerRunning = false;
                    conn.Close();
                    BTListener.Stop();
                    UpdateUI("A problem occured, Details: " + ex.Message);
                }
            }
        }

        // tcp server-client region
        //private void StartListening()
        //{
        //    // Data buffer for incoming data.
        //    byte[] bytes = new Byte[1024];

        //    // Establish the local endpoint for the socket.
        //    // The DNS name of the computer
        //    // running the listener is "host.contoso.com".
        //    IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
        //    IPAddress ipAddress = ipHostInfo.AddressList[0];
        //    IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

        //    // Create a TCP/IP socket.
        //    Socket listener = new Socket(AddressFamily.InterNetwork,
        //        SocketType.Stream, ProtocolType.Tcp);

        //    // Bind the socket to the local endpoint and listen for incoming connections.
        //    try
        //    {
        //        listener.Bind(localEndPoint);
        //        listener.Listen(100);

        //        while (true)
        //        {
        //            // Set the event to nonsignaled state.
        //            allDone.Reset();

        //            // Start an asynchronous socket to listen for connections.
        //            Msg.AppLog.Publish("Waiting from price center...");
        //            listener.BeginAccept(
        //                new AsyncCallback(AcceptCallback),
        //                listener);

        //            // Wait until a connection is made before continuing.
        //            allDone.WaitOne();
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Msg.AppLog.Publish(e.ToString());
        //    }
        //}

        //private void AcceptCallback(IAsyncResult ar)
        //{
        //    // Signal the main thread to continue.
        //    allDone.Set();

        //    // Get the socket that handles the client request.
        //    Socket listener = (Socket)ar.AsyncState;
        //    Socket handler = listener.EndAccept(ar);

        //    // Create the state object.
        //    StateObject state = new StateObject();
        //    state.workSocket = handler;
        //    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
        //        new AsyncCallback(ReadCallback), state);
        //}

        //private void ReadCallback(IAsyncResult ar)
        //{
        //    String content = String.Empty;

        //    // Retrieve the state object and the handler socket
        //    // from the asynchronous state object.
        //    StateObject state = (StateObject)ar.AsyncState;
        //    Socket handler = state.workSocket;

        //    // Read data from the client socket. 
        //    int bytesRead = handler.EndReceive(ar);

        //    if (bytesRead > 0)
        //    {
        //        // There  might be more data, so store the data received so far.
        //        state.sb.Append(Encoding.ASCII.GetString(
        //            state.buffer, 0, bytesRead));

        //        // Check for end-of-file tag. If it is not there, read 
        //        // more data.
        //        content = state.sb.ToString();
        //        if (content.IndexOf("<EOF>") > -1)
        //        {
        //            // All the data has been read from the 
        //            // client. Display it on the console.
        //            Msg.AppLog.Publish(String.Format("Read {0} bytes from socket. \n Data : {1}",
        //                content.Length, content));

        //            // Update Request received from price center , connection will begin with local server

        //            Msg.AppLog.Publish();

        //            // parse content and create UpdateEventArgs

        //            string requestResult = "failed";

        //            //Send(handler, requestResult);
        //        }
        //        //else
        //        //{
        //        //    // Not all data received. Get more.
        //        //    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
        //        //    new AsyncCallback(ReadCallback), state);
        //        //}
        //    }
        //}

        //private void Send(Socket handler, String data)
        //{
        //    // Convert the string data to byte data using ASCII encoding.
        //    byte[] byteData = Encoding.ASCII.GetBytes(data);

        //    // Begin sending the data to the remote device.
        //    handler.BeginSend(byteData, 0, byteData.Length, 0,
        //        new AsyncCallback(SendCallback), handler);
        //}

        //private void SendCallback(IAsyncResult ar)
        //{
        //    try
        //    {
        //        // Retrieve the socket from the state object.
        //        Socket handler = (Socket)ar.AsyncState;

        //        // Complete sending the data to the remote device.
        //        int bytesSent = handler.EndSend(ar);
        //        Msg.AppLog.Publish(string.Format("Sent {0} bytes to client.", bytesSent));

        //        handler.Shutdown(SocketShutdown.Both);
        //        handler.Close();

        //    }
        //    catch (Exception e)
        //    {
        //        Msg.AppLog.Publish(e.ToString());
        //    }
        //}

        #endregion
    }
}

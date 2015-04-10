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
            Thread tcpServerThread = new Thread(new ThreadStart(socketListener.StartListening));
            tcpServerThread.Start();
        }

        #endregion

        #region Fields

        private CentralServer server;
        private IList<Device> devices;
        private ISenderBluetoothService service;
        private bool ServerRunning = false;
        private AsynchronousSocketListener socketListener;

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
            socketListener = new AsynchronousSocketListener();
            Messenger<Msg>.Default.AddHandler<string>(Msg.AppLog,UpdateUI);
            Messenger<Msg>.Default.AddHandler(Msg.AppClean, CleanLogs);
            this.FormClosed += CloseServer;     // TO-DO: Update : kill asyncrenous socket listener thread
        }

        public CentralServerForm()
        {
            InitializeComponent();
            service = new SenderBluetoothService();
            socketListener = new AsynchronousSocketListener();
            devices = new List<Device>();
            Messenger<Msg>.Default.AddHandler<string>(Msg.AppLog, UpdateUI);
            this.FormClosed += CloseServer;    // TO-DO: Update : kill asyncrenous socket listener thread
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
                string sendDataString = args.Esl + Definitions.OnMarketBranchRequestGUID + dataType + Definitions.OnMarketBranchRequestGUID + args.NewValue + Definitions.OnMarketBranchRequestGUID;
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

        private void CleanLogs()
        {
            activityBox.Clear();
        }

        // waiting response thread from local server
        private void ServerConnectThread(object eventArgs)
        {
            UpdateEventArgs args = (UpdateEventArgs)eventArgs;

            Guid responseGuid = new Guid(Definitions.OnESLResponseGUID);

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

        public void CloseServer(object sender,EventArgs args)
        {
            socketListener.CloseSocketConnection();
        }

        #endregion

        #region UnhandledEvents;

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void activityBox_TextChanged(object sender, EventArgs e) { }

        #endregion;
    }
}

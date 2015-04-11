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
        private AsynchronousSocketListener socketListener;

        #endregion

        #region Properties

        #endregion

        #region Constructors

        public CentralServerForm(CentralServer server)
        {
            InitializeComponent();
            this.server = server;
            socketListener = new AsynchronousSocketListener();
            Messenger<Msg>.Default.AddHandler<string>(Msg.AppLog,UpdateUI);
            Messenger<Msg>.Default.AddHandler(Msg.AppClean, CleanLogs);
            this.FormClosed += CloseServer;     // TO-DO: Update : kill asyncrenous socket listener thread
        }

        public CentralServerForm()
        {
            InitializeComponent();
            socketListener = new AsynchronousSocketListener();
            Messenger<Msg>.Default.AddHandler<string>(Msg.AppLog, UpdateUI);
            this.FormClosed += CloseServer;    // TO-DO: Update : kill asyncrenous socket listener thread
        }

        #endregion

        #region Methods

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
            Func<int> del = delegate()
            {
                activityBox.Clear();
                return 0;
            };

            Invoke(del);
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

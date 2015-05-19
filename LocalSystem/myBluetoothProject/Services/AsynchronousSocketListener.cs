using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

using InTheHand.Net.Sockets;

using myBluetoothProject.Messenger;
using myBluetoothProject.Helpers;

namespace myBluetoothProject.Services
{
    public class AsynchronousSocketListener
    {
        // Thread signal for tcp server
        private ManualResetEvent allDone = new ManualResetEvent(false);

        private Socket listener;

        #region BluetoothServices

        private IList<Device> devices;
        private ISenderBluetoothService service;
        private bool ServerRunning = false;

        #endregion

        public AsynchronousSocketListener()
        {
            service = new SenderBluetoothService();
            devices = new List<Device>();
        }

        private async void UpdateProductInfo(UpdateEventArgs args)
        {
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
                    Msg.AppLog.Publish("There is no avaliable local server in range , Try Again");
                    Send(args.TcpSocket, "FAILED");
                    return;
                }

                var candidateDevices = devices.Where(device => device.DeviceName == args.LocalServer);

                if (candidateDevices.Count() <= 0)
                {
                    Msg.AppLog.Publish(String.Format("There is no local server named: {0}, try again!", args.LocalServer));
                    Send(args.TcpSocket, "FAILED");
                    return;
                }
                else if (candidateDevices.Count() > 1)
                {
                    Msg.AppLog.Publish("There were too many devices have same name, check it again");
                    Send(args.TcpSocket, "FAILED");
                    return;
                }

                Device targetDevice = candidateDevices.First();
                Msg.AppLog.Publish(String.Format("Local server of product : {0} -> " + args.LocalServer, args.Esl));

                // Local server is found and connection will begin
                Msg.AppLog.Publish("Info is updating ..." + Environment.NewLine);

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
                    Msg.AppLog.Publish(string.Format("Info can not updated , Local server is not ready ,TRY AGAIN!" + Environment.NewLine));
                    Send(args.TcpSocket, "FAILED");
                    return;
                }
            }
            catch (Exception ex)
            {
                Msg.AppLog.Publish("A problem occured connecting to local server. Details: " + ex.Message);
                Send(args.TcpSocket, "FAILED");
                return;
            }
        }

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
                                Send(args.TcpSocket, "SUCCESSED");
                                Msg.AppLog.Publish("Price is updated");
                            }

                            else if (args.Type == UpdateType.Image)
                            {
                                Send(args.TcpSocket, "SUCCESSED");
                                Msg.AppLog.Publish("Image is updated");
                            }

                            else if (args.Type == UpdateType.Info)
                            {
                                Send(args.TcpSocket, "SUCCESSED");
                                Msg.AppLog.Publish("Info is updated");
                            }
                        }

                        else
                        {
                            if (args.Type == UpdateType.Price)
                            {
                                Send(args.TcpSocket, "FAILED");
                                Msg.AppLog.Publish(string.Format("Price can not updated, ESL is not ready") + Environment.NewLine);
                            }

                            else if (args.Type == UpdateType.Image)
                            {
                                Send(args.TcpSocket, "FAILED");
                                Msg.AppLog.Publish(string.Format("Image can not updated, ESL is not ready") + Environment.NewLine);
                            }

                            else if (args.Type == UpdateType.Info)
                            {
                                Send(args.TcpSocket, "FAILED");
                                Msg.AppLog.Publish(string.Format("Information can not updated, ESL is not ready") + Environment.NewLine);
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
                    Send(args.TcpSocket, "FAILED");
                    Msg.AppLog.Publish("A problem occured, Details: " + ex.Message);
                }
            }
        }

        public void StartListening()
        {
            try
            {
                // Data buffer for incoming data.
                byte[] bytes = new Byte[1024];

                // Establish the remote endpoint for the socket.
               
                IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, Definitions.ServerSocketNo);

                // Create a TCP/IP socket.
                listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                LingerOption lo = new LingerOption(false, 0);
                listener.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Linger, lo);

                // Bind the socket to the local endpoint and listen for incoming connections.
                listener.Bind(remoteEndPoint);
                listener.Listen(100);

                while (true)
                {
                    // Set the event to nonsignaled state.
                    allDone.Reset();

                    // Start an asynchronous socket to listen for connections.
                    Msg.AppLog.Publish("Waiting from price center...");
                    listener.BeginAccept(
                        new AsyncCallback(AcceptCallback),
                        listener);

                    // Wait until a connection is made before continuing.
                    allDone.WaitOne();
                }
            }
            catch (Exception e)
            {
                Msg.AppLog.Publish(e.ToString());
            }
        }

        public void CloseSocketConnection()
        {
            try
            {
                listener.Shutdown(SocketShutdown.Both);
                listener.Disconnect(true);
            }
            catch (Exception ex)
            {

            }
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.
            allDone.Set();

            // Get the socket that handles the client request.
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            // Create the state object.
            StateObject state = new StateObject();
            state.workSocket = handler;
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);
        }

        private void ReadCallback(IAsyncResult ar)
        {
            String content = String.Empty;

            // Retrieve the state object and the handler socket
            // from the asynchronous state object.
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            // Read data from the client socket. 
            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0)
            {
                // There  might be more data, so store the data received so far.
                state.sb.Append(Encoding.ASCII.GetString(
                    state.buffer, 0, bytesRead));

                // Check for end-of-file tag. If it is not there, read 
                // more data.
                content = state.sb.ToString();
                if (content.IndexOf("<EOF>") > -1)
                {
                    UpdateEventArgs args = CreateUpdatePackage(content,Definitions.OnPriceCenterRequestGUID);
                    UpdateEventArgs argsWithSocket = new UpdateEventArgs(args, handler);

                    Msg.AppClean.Publish();
                    Msg.AppLog.Publish("Request is received from Price Center");
                    Msg.AppLog.Publish("Request details : \n");
                    Msg.AppLog.Publish(string.Format("Local server : {0}\n",args.LocalServer));
                    Msg.AppLog.Publish(string.Format("Product : {0}\n", args.Esl));
                    Msg.AppLog.Publish(string.Format("New info : {0}\n", args.NewValue));

                    // try connect with local server and get response
                    UpdateProductInfo(argsWithSocket);
                }
                else
                {
                    // Not all data received. Get more.
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReadCallback), state);
                }
            }
        }

        private void Send(Socket handler, String data)
        {
            // Convert the string data to byte data using ASCII encoding.
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.
            handler.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), handler);
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.
                int bytesSent = handler.EndSend(ar);
                Msg.AppLog.Publish(string.Format("Response sent to Price Center."));

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();

            }
            catch (Exception e)
            {
                Msg.AppLog.Publish(e.ToString());
            }
        }

        private UpdateEventArgs CreateUpdatePackage(string content,string key)
        {
            string[] seperator = new string[] { key };
            string[] allMessage = content.Split(seperator, StringSplitOptions.None);

            var localServer = allMessage[0];
            var infoType = allMessage[1];
            var eslName = allMessage[2];
            var newInfo = allMessage[3];

            UpdateType type;
            if (infoType == "0")
                type = UpdateType.Price;
            else if (infoType == "1")
                type = UpdateType.Image;
            else if (infoType == "2")
                type = UpdateType.Info;
            else
                type = UpdateType.Unknown;

            return new UpdateEventArgs(type, newInfo, localServer, eslName);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using myBluetoothProject.Messenger;
using myBluetoothProject.Helpers;

namespace myBluetoothProject.Services
{
    public class AsynchronousSocketListener
    {
        // Thread signal for tcp server
        private ManualResetEvent allDone = new ManualResetEvent(false);

        private Socket listener;

        public AsynchronousSocketListener()
        {
        }

        public void StartListening()
        {
            try
            {
                // Data buffer for incoming data.
                byte[] bytes = new Byte[1024];

                // Establish the local endpoint for the socket.
                // The DNS name of the computer
                // running the listener is "host.contoso.com".
                IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

                // Create a TCP/IP socket.
                listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                LingerOption lo = new LingerOption(false, 0);
                listener.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Linger, lo);

                // Bind the socket to the local endpoint and listen for incoming connections.
                listener.Bind(localEndPoint);
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
                    // All the data has been read from the 
                    // client. Display it on the console.
                    Msg.AppLog.Publish(String.Format("Read {0} bytes from socket. \n ", content.Length));

                    UpdateEventArgs args = CreateUpdatePackage(content,Definitions.OnPriceCenterRequestGUID);

                    // try connect with local server and get response



                    // send response to price center
                    string requestResult = "FAILED";

                    Send(handler, requestResult);
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
                Msg.AppLog.Publish(string.Format("Sent {0} bytes to client.", bytesSent));

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
            string[] allMessage = content.Split(key.ToCharArray());

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

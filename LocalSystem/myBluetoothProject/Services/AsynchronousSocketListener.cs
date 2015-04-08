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
            Msg.AppClean.Publish();

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
                    Msg.AppLog.Publish(String.Format("Read {0} bytes from socket. \n Data : {1}",
                        content.Length, content));

                    // Update Request received from price center , connection will begin with local server
                    string[] infoDetails = new string[4];
                    try
                    {
                        infoDetails = Regex.Split(content, Definitions.OnRequestGUID);
                        if(infoDetails.Count() != 4)
                            throw new Exception();
                    }
                    catch (Exception ex)
                    {
                        Msg.AppLog.Publish(ex.Message);
                    }

                    UpdateType type;
                    if (infoDetails[0] == "0")
                        type = UpdateType.Price;
                    else if (infoDetails[0] == "1")
                        type = UpdateType.Image;
                    else if (infoDetails[0] == "2")
                        type = UpdateType.Info;
                    else
                        type = UpdateType.Unknown;

                    UpdateEventArgs newEvent = new UpdateEventArgs(type,infoDetails[1],infoDetails[2],infoDetails[3]);
                    

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
    }
}

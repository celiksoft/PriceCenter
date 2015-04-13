using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using ChainedMarketDijitalTag.Helpers;
using ChainedMarketDijitalTag.Messenger;

namespace ChainedMarketDijitalTag.Services
{
    public class AsynchronousClient
    {
        private MarketBranchServerInfo serverInfo;
        private UpdateEventArgs updateArgs;

        // ManualResetEvent instances signal completion.
        private ManualResetEvent connectDone = new ManualResetEvent(false);
        private ManualResetEvent sendDone = new ManualResetEvent(false);
        private ManualResetEvent receiveDone = new ManualResetEvent(false);

        // The response from the remote device.
        private String response = String.Empty;

        public AsynchronousClient(MarketBranchServerInfo serverInfo,UpdateEventArgs args)
        {
            this.serverInfo = serverInfo;
            updateArgs = args;
        }

        public void StartClient()
        {
            // Connect to a remote device.
            try
            {
                // Establish the remote endpoint for the socket.
                // The name of the 
                // remote device is "host.contoso.com".

                //IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
                //IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(serverInfo.Ip, serverInfo.SocketNo);

                // Create a TCP/IP socket.
                Socket client = new Socket(AddressFamily.InterNetwork,
                    SocketType.Stream, ProtocolType.Tcp);

                // Connect to the remote endpoint.
                client.BeginConnect(remoteEP,
                    new AsyncCallback(ConnectCallback), client);
                connectDone.WaitOne();

                var serverMessage = CreateMessage(updateArgs);

                // Send test data to the remote device.
                Send(client, serverMessage);
                sendDone.WaitOne();

                // Receive the response from the remote device.
                Receive(client);
                receiveDone.WaitOne();

                // Write the response to the console.
                Msg.UpdateInfoLog.Publish(string.Format("Response received : {0}", response));

                // Release the socket.
                client.Shutdown(SocketShutdown.Both);
                client.Close();
            }
            catch (Exception e)
            {
                Msg.UpdateInfoLog.Publish("ERROR : A problem occured while connecting market server");
            }
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket client = (Socket)ar.AsyncState;

                // Complete the connection.
                client.EndConnect(ar);

                Msg.UpdateInfoLog.Publish(string.Format("Socket connected to {0}",
                    client.RemoteEndPoint.ToString()));

                // Signal that the connection has been made.
                connectDone.Set();
            }
            catch (Exception e)
            {
                Msg.UpdateInfoLog.Publish("ERROR : A problem occured while connecting market server");
            }
        }

        private void Receive(Socket client)
        {
            try
            {
                // Create the state object.
                StateObject state = new StateObject();
                state.workSocket = client;

                // Begin receiving the data from the remote device.
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception e)
            {
                Msg.UpdateInfoLog.Publish("ERROR : A problem occured while connecting market server");
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the state object and the client socket 
                // from the asynchronous state object.
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.workSocket;

                // Read data from the remote device.
                int bytesRead = client.EndReceive(ar);

                if (bytesRead > 0)
                {
                    // There might be more data, so store the data received so far.
                    state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                    response = state.sb.ToString();

                    receiveDone.Set();
                }
            }
            catch (Exception e)
            {
                Msg.UpdateInfoLog.Publish("ERROR : A problem occured while connecting market server");
            }
        }

        private void Send(Socket client, String data)
        {
            // Convert the string data to byte data using ASCII encoding.
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.
            client.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), client);
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket client = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.
                int bytesSent = client.EndSend(ar);
                Msg.UpdateInfoLog.Publish("User update request sent to market server, response is being waited");

                // Signal that all bytes have been sent.
                sendDone.Set();
            }
            catch (Exception e)
            {
                Msg.UpdateInfoLog.Publish("ERROR : A problem occured while connecting market server");
            }
        }

        private string CreateMessage(UpdateEventArgs args)
        {
            string updateType;

            if (args.Type == UpdateType.Price)
                updateType = "0";
            else if (args.Type == UpdateType.Image)
                updateType = "1";
            else if (args.Type == UpdateType.Info)
                updateType = "2";
            else
                updateType = "-1";

            string message = args.LocalServer + Definitions.OnPriceCenterRequestGUID + updateType + Definitions.OnPriceCenterRequestGUID +
                             args.Esl + Definitions.OnPriceCenterRequestGUID + args.NewValue + Definitions.OnPriceCenterRequestGUID + "<EOF>";
            return message;
        }

    }
}

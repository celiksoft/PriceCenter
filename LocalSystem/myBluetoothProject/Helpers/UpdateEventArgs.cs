using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace myBluetoothProject.Helpers
{
    public class UpdateEventArgs
    {
        public UpdateEventArgs(UpdateType type, string value, string localServer, string eslName)
        {
            Type = type;
            NewValue = value;
            LocalServer = localServer;
            Esl = eslName;
        }

        public UpdateEventArgs(UpdateEventArgs other, Socket tcpSocket)
        {
            Type = other.Type;
            NewValue = other.NewValue;
            LocalServer = other.LocalServer;
            Esl = other.Esl;
            TcpSocket = tcpSocket;
        }

        public UpdateType Type
        {
            get;
            private set;
        }

        public string NewValue
        {
            get;
            private set;
        }

        public string LocalServer
        {
            get;
            private set;
        }

        public string Esl
        {
            get;
            private set;
        }
        public Socket TcpSocket
        {
            get;
            private set;
        }
    }
}

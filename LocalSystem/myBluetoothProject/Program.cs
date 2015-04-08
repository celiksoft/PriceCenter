using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data;
using myBluetoothProject.Services;
using InTheHand.Net;
using InTheHand.Net.Sockets;

namespace myBluetoothProject
{
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // create a central server instance
            CentralServer server = new CentralServer();
            Application.Run(new CentralServerForm(server));
        }
    }
}
